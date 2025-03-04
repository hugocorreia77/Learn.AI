using Learn.Core.Shared.Models.User;
using System.Text.Json.Serialization;

namespace Learn.Quizz.Repository.Models
{
    public class Attempt
    {
        [JsonPropertyOrder(1)] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyOrder(2)] public required UserReference User { get; set; }
        [JsonPropertyOrder(3)] public QuestionOption? Answer { get; set; }
    }
}
