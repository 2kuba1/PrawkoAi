using Domain.Shared;

namespace Domain;

public class Category : BaseEntity
{
    public required string Name { get; set; }

    public List<Question> Questions { get; set; } = new();
}