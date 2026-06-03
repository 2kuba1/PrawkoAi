using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Features.Users.GuestLogin;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Users.GuestLogin;

public class GuestLoginHandlerTests
{
    [Fact]
    public async Task Handle_WhenGuestDoesNotExist_ShouldCreateGuestAndRefreshToken()
    {
        // Arrange
        var deviceId = "device-123";
        var accessToken = "access-token";
        var refreshTokenValue = "refresh-token";
        var guest = new User { Id = Guid.NewGuid(), DeviceId = deviceId };

        var authService = Substitute.For<IAuthService>();
        var userRepository = Substitute.For<IUserRepository>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

        userRepository.FindUserByDeviceIdAsync(deviceId).Returns((User?)null);
        userRepository.CreateNewGuestAsync(deviceId).Returns(guest);
        authService.CreateToken(guest).Returns(accessToken);
        authService.GenerateRefreshToken().Returns(refreshTokenValue);

        var handler = new GuestLoginHandler(
            authService,
            userRepository,
            refreshTokenRepository);

        // Act
        var result = await handler.Handle(new Application.Features.Users.GuestLogin.GuestLogin(deviceId), CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshTokenValue);

        await userRepository.Received(1)
            .CreateNewGuestAsync(deviceId);

        await refreshTokenRepository.Received(1)
            .CreateAsync(Arg.Is<RefreshToken>(x =>
                x.UserId == guest.Id &&
                x.Token == refreshTokenValue &&
                x.ExpiresOnUtc > DateTime.UtcNow));
    }

    [Fact]
    public async Task Handle_WhenGuestExists_ShouldReuseGuestAndCreateRefreshToken()
    {
        // Arrange
        var deviceId = "device-123";
        var accessToken = "access-token";
        var refreshTokenValue = "refresh-token";
        var guest = new User { Id = Guid.NewGuid(), DeviceId = deviceId };

        var authService = Substitute.For<IAuthService>();
        var userRepository = Substitute.For<IUserRepository>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

        userRepository.FindUserByDeviceIdAsync(deviceId).Returns(guest);
        authService.CreateToken(guest).Returns(accessToken);
        authService.GenerateRefreshToken().Returns(refreshTokenValue);

        var handler = new GuestLoginHandler(
            authService,
            userRepository,
            refreshTokenRepository);

        // Act
        var result = await handler.Handle(new Application.Features.Users.GuestLogin.GuestLogin(deviceId), CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshTokenValue);

        await userRepository.DidNotReceive()
            .CreateNewGuestAsync(Arg.Any<string>());

        await refreshTokenRepository.Received(1)
            .CreateAsync(Arg.Is<RefreshToken>(x =>
                x.UserId == guest.Id &&
                x.Token == refreshTokenValue));
    }
}
