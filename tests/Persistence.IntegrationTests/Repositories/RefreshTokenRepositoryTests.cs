using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.IntegrationTests.Repositories;

[Collection(IntegrationTestCollection.Name)]
public class RefreshTokenRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public RefreshTokenRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetUsersRefreshToken_WhenTokenExists_ShouldReturnTokenWithUser()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var user = await AddUserAsync(context);
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "refresh-token-1",
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        var repository = new RefreshTokenRepository(context);

        // Act
        var result = await repository.GetUsersRefreshToken("refresh-token-1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(refreshToken.Id);
        result.User.Should().NotBeNull();
        result.User.DeviceId.Should().Be(user.DeviceId);
    }

    [Fact]
    public async Task RemoveUserRefreshTokens_ShouldDeleteOnlyTokensForRequestedUser()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var firstUser = await AddUserAsync(context, "device-1");
        var secondUser = await AddUserAsync(context, "device-2");

        context.RefreshTokens.AddRange(
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = firstUser.Id,
                Token = "first-token",
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
            },
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = secondUser.Id,
                Token = "second-token",
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
            });

        await context.SaveChangesAsync();

        var repository = new RefreshTokenRepository(context);

        // Act
        await repository.RemoveUserRefreshTokens(firstUser.Id);

        // Assert
        var remainingTokens = await context.RefreshTokens
            .AsNoTracking()
            .Select(x => x.Token)
            .ToListAsync();

        remainingTokens.Should().Equal("second-token");
    }

    private static async Task<User> AddUserAsync(
        Persistence.Database.AppDbContext context,
        string deviceId = "device-123")
    {
        var role = new Role { Id = Guid.NewGuid(), Name = $"User-{Guid.NewGuid()}" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            DeviceId = deviceId,
            RoleId = role.Id,
            Role = role
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }
}
