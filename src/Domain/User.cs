using Domain.Shared;

namespace Domain;

public class User : BaseEntity
{
    public required string DeviceId { get; set; }
    public required string Email { get; set; }
    public Guid RoleId { get; set; }

    public List<UserAnswer> UserAnswers { get; set; } = new();
    public Role Role { get; set; }
}