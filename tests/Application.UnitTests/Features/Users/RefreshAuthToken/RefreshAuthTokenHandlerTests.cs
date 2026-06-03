using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Features.Users.RefreshAuthToken;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Application.UnitTests.Features.Users.RefreshAuthToken;

public class RefreshAuthTokenHandlerTests
{
    [Fact]
    public async Task Handle_WhenTokenDoesNotExist_ShouldThrowRefreshTokenExpiredException()
    {
        // Arrange
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var authService = Substitute.For<IAuthService>();
        var configuration = CreateConfiguration();

        refreshTokenRepository.GetUsersRefreshToken("old-token").Returns((RefreshToken?)null);

        var handler = new RefreshAuthTokenHandler(
            refreshTokenRepository,
            authService,
            configuration);

        // Act
        var act = async () => await handler.Handle(
            new Application.Features.Users.RefreshAuthToken.RefreshAuthToken("old-token"),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<RefreshTokenExpiredException>();

        authService.DidNotReceive()
            .CreateToken(Arg.Any<User>());

        await refreshTokenRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<RefreshToken>());
    }

    [Fact]
    public async Task Handle_WhenTokenExpired_ShouldThrowRefreshTokenExpiredException()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "old-token",
            UserId = Guid.NewGuid(),
            User = new User { Id = Guid.NewGuid(), DeviceId = "device-123" },
            ExpiresOnUtc = DateTime.UtcNow.AddMinutes(-1)
        };

        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var authService = Substitute.For<IAuthService>();
        var configuration = CreateConfiguration();

        refreshTokenRepository.GetUsersRefreshToken(refreshToken.Token).Returns(refreshToken);

        var handler = new RefreshAuthTokenHandler(
            refreshTokenRepository,
            authService,
            configuration);

        // Act
        var act = async () => await handler.Handle(
            new Application.Features.Users.RefreshAuthToken.RefreshAuthToken(refreshToken.Token),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<RefreshTokenExpiredException>();

        authService.DidNotReceive()
            .CreateToken(Arg.Any<User>());

        await refreshTokenRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<RefreshToken>());
    }

    [Fact]
    public async Task Handle_WhenTokenIsValid_ShouldRotateRefreshToken()
    {
        // Arrange
        var oldToken = "old-token";
        var newRefreshToken = "new-refresh-token";
        var newAccessToken = "new-access-token";
        var user = new User { Id = Guid.NewGuid(), DeviceId = "device-123" };
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = oldToken,
            UserId = user.Id,
            User = user,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
        };

        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var authService = Substitute.For<IAuthService>();
        var configuration = CreateConfiguration();

        refreshTokenRepository.GetUsersRefreshToken(oldToken).Returns(refreshToken);
        authService.CreateToken(user).Returns(newAccessToken);
        authService.GenerateRefreshToken().Returns(newRefreshToken);

        var handler = new RefreshAuthTokenHandler(
            refreshTokenRepository,
            authService,
            configuration);

        // Act
        var result = await handler.Handle(new Application.Features.Users.RefreshAuthToken.RefreshAuthToken(oldToken), CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be(newAccessToken);
        result.RefreshToken.Should().Be(newRefreshToken);

        await refreshTokenRepository.Received(1)
            .UpdateAsync(Arg.Is<RefreshToken>(x =>
                x.Id == refreshToken.Id &&
                x.Token == newRefreshToken &&
                x.ExpiresOnUtc > DateTime.UtcNow));
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:RefreshTokenExpirationTimeInDays"] = "30"
            })
            .Build();
    }
}
