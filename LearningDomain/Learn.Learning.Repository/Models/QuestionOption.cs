using System.Text.Json.Serialization;

namespace Learn.Learning.Repository.Models
{
    public class QuestionOption
    {
        [JsonPropertyOrder(1)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyOrder(2)]
        public required string Text { get; set; }
        [JsonPropertyOrder(3)]
        public bool IsCorrect { get; set; }
    }
}
