using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Persistence.Repositories;

namespace Persistence.IntegrationTests.Repositories;

[Collection(IntegrationTestCollection.Name)]
public class UserRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public UserRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateNewGuestAsync_WhenDeviceIdIsNew_ShouldCreateUserWithUserRole()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var role = new Role { Id = Guid.NewGuid(), Name = "User" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.CreateNewGuestAsync("device-123");

        // Assert
        result.DeviceId.Should().Be("device-123");
        result.RoleId.Should().Be(role.Id);

        var savedUser = await context.Users.AsNoTracking().SingleAsync();
        savedUser.DeviceId.Should().Be("device-123");
    }

    [Fact]
    public async Task CreateNewGuestAsync_WhenDeviceIdExists_ShouldThrowUserAlreadyExistsException()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var role = new Role { Id = Guid.NewGuid(), Name = "User" };
        context.Roles.Add(role);
        context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            DeviceId = "device-123",
            RoleId = role.Id,
            Role = role
        });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        // Act
        var act = async () => await repository.CreateNewGuestAsync("device-123");

        // Assert
        await act.Should().ThrowAsync<UserAlreadyExistsException>();
    }

    [Fact]
    public async Task UpdateStreak_WhenLastStreakWasYesterday_ShouldIncrementStreakAndClearDashboardCache()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var role = new Role { Id = Guid.NewGuid(), Name = "User" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            DeviceId = "device-123",
            RoleId = role.Id,
            Role = role,
            Streak = 2,
            LastStreakDate = DateTime.UtcNow.Date.AddDays(-1)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var cache = new RecordingDistributedCache();
        var repository = new UserRepository(context);

        // Act
        await repository.UpdateStreak(user.Id, cache, "B");

        // Assert
        var updatedUser = await context.Users.AsNoTracking().SingleAsync();
        updatedUser.Streak.Should().Be(3);
        updatedUser.LastStreakDate.Should().Be(DateTime.UtcNow.Date);

        cache.RemovedKeys.Should().Contain($"dashboard_data_{user.Id}_category_B");
    }

    private sealed class RecordingDistributedCache : IDistributedCache
    {
        public List<string> RemovedKeys { get; } = [];

        public byte[]? Get(string key) => null;

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
            => Task.FromResult<byte[]?>(null);

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
            => Task.CompletedTask;

        public void Remove(string key)
        {
            RemovedKeys.Add(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            RemovedKeys.Add(key);
            return Task.CompletedTask;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
        }

        public Task SetAsync(
            string key,
            byte[] value,
            DistributedCacheEntryOptions options,
            CancellationToken token = default)
            => Task.CompletedTask;
    }
}
