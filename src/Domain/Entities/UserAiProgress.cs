using Domain.Shared;

namespace Domain.Entities;

public class UserAiProgress : BaseEntity
{
    public string Content { get; set; }
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}