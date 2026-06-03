using Application.Contracts.Repositories;
using Application.Features.Users.RevokeRefreshTokens;
using Application.UnitTests.TestHelpers;
using Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Users.RevokeRefreshTokens;

public class RevokeRefreshTokensHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsAuthorized_ShouldRemoveRefreshTokensAndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var httpContextAccessor = HttpContextHelper.WithUserId(userId);

        var handler = new RevokeRefreshTokensHandler(
            refreshTokenRepository,
            httpContextAccessor);

        // Act
        var result = await handler.Handle(new Application.Features.Users.RevokeRefreshTokens.RevokeRefreshTokens(userId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        await refreshTokenRepository.Received(1)
            .RemoveUserRefreshTokens(userId);
    }

    [Fact]
    public async Task Handle_WhenUserIsUnauthorized_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var httpContextAccessor = HttpContextHelper.WithUserId(Guid.NewGuid());

        var handler = new RevokeRefreshTokensHandler(
            refreshTokenRepository,
            httpContextAccessor);

        // Act
        var act = async () => await handler.Handle(
            new Application.Features.Users.RevokeRefreshTokens.RevokeRefreshTokens(userId),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();

        await refreshTokenRepository.DidNotReceive()
            .RemoveUserRefreshTokens(Arg.Any<Guid>());
    }
}
