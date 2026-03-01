using Application.Contracts.Repositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> FindUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> FindUserByDeviceIdAsync(string deviceId)
        => await _context.Users.FirstOrDefaultAsync(x => x.DeviceId == deviceId);
    

    public async Task<User> CreateNewGuestAsync(string deviceId)
    {
        var checkIfDeviceIdExists = await _context.Users.AnyAsync(x => x.DeviceId == deviceId);

        if (checkIfDeviceIdExists)
            throw new ApplicationException("This user already exists");

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "User");
        
        if(role is null)
            throw new ApplicationException("This role does not exist");
        
        var newUser = new User()
        {
            DeviceId = deviceId,
            Id = Guid.NewGuid(),
            Email = null,
            RoleId = role.Id,
        };
        
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        
        return newUser;
    }
}