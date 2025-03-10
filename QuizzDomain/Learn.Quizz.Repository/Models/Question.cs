using System.Text.Json.Serialization;

namespace Learn.Quizz.Repository.Models
{
    public class Question
    {
        [JsonPropertyOrder(1)] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyOrder(2)] public QuestionStatus Status { get; set; }
        [JsonPropertyOrder(3)] public required Language Language { get; set; }
        [JsonPropertyOrder(4)] public required QuestionCategory Category { get; set; }
        [JsonPropertyOrder(5)] public required string QuestionText { get; set; }
        [JsonPropertyOrder(6)] public List<QuestionOption>? Options { get; set; }
        [JsonPropertyOrder(7)] public string? CorrectAnswer { get; set; }
        [JsonPropertyOrder(8)] public string? Explanation { get; set; }
        [JsonPropertyOrder(9)] public List<string> Tags { get; set; } = [];
        [JsonPropertyOrder(10)] public List<Attachment> Attachments { get; set; } = [];
        [JsonPropertyOrder(11)] public List<Attempt> Attempts { get; set; } = [];
        [JsonPropertyOrder(12)] public DateTime? StartedAt { get; set; }
        [JsonPropertyOrder(13)] public DateTime? EndedAt { get; set; }
    }
}
