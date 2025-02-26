using System.Text.Json.Serialization;

namespace Learn.Quizz.Repository.Models
{
    public class Attempt
    {
        [JsonPropertyOrder(1)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyOrder(1)]
        public QuestionOption? Answer { get; set; }

    }
}
