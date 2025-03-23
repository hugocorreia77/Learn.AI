using Learn.Core.Shared.Repository;
using Learn.Users.Models;
using System.Text.Json.Serialization;

namespace Learn.Learning.Repository.Models
{
    public class Question : IAuditableAbstractModel
    {
        [JsonPropertyOrder(1)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyOrder(2)]
        public required Language Language { get; set; }
        [JsonPropertyOrder(3)]
        public required QuestionCategory Category { get; set; }
        [JsonPropertyOrder(4)]
        public required string QuestionText { get; set; }
        [JsonPropertyOrder(5)]
        public QuestionDifficulty Difficulty { get; set; }
        [JsonPropertyOrder(6)]
        public QuestionType QuestionType { get; set; }
        [JsonPropertyOrder(7)]
        public List<QuestionOption>? Options { get; set; }
        [JsonPropertyOrder(8)]
        public string? CorrectAnswer { get; set; }
        [JsonPropertyOrder(9)]
        public string? Explanation { get; set; }
        [JsonPropertyOrder(10)]
        public List<string> Tags { get; set; } = [];
        [JsonPropertyOrder(11)]
        public List<Attachment> Attachments { get; set; } = [];
        [JsonPropertyOrder(12)]
        public required UserReference CreatedBy { get; set; }
        [JsonPropertyOrder(13)]
        public required DateTime CreatedOn { get; set; }
        [JsonPropertyOrder(14)]
        public UserReference? UpdatedBy { get; set; }
        [JsonPropertyOrder(15)]
        public DateTime? UpdatedOn { get; set; }
        [JsonPropertyOrder(16)]
        public UserReference? DeletedBy { get; set; }
        [JsonPropertyOrder(17)]
        public DateTime? DeletedOn { get; set; }
        [JsonPropertyOrder(18)]
        public bool IsDeleted { get; set; }
    }
}
