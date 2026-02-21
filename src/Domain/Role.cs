using Domain.Shared;

namespace Domain;

public class Role : BaseEntity
{
    public required string Name { get; set; }

    public List<User> Users { get; set; } = new();
}