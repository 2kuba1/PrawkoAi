using Domain.Shared;

namespace Domain.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }

    public List<User> Users { get; set; } = new();
}