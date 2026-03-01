using System.Text.Json.Serialization;
using Domain.Shared;

namespace Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }

    [JsonIgnore]
    public List<Question> Questions { get; set; } = new();
}