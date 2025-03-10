using Learn.Core.Shared.Models.User;
using Learn.Core.Shared.Repository;
using System.Text.Json.Serialization;

namespace Learn.Quizz.Repository.Models
{
    public class QuizzGame : IAuditableAbstractModel
    {
        [JsonPropertyOrder(1)] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyOrder(2)] public required string Name { get; set; }
        [JsonPropertyOrder(3)] public required string Code { get; set; }
        [JsonPropertyOrder(4)] public required QuizzType Type { get; set; }
        [JsonPropertyOrder(5)] public required GameStatus Status { get; set; }
        [JsonPropertyOrder(7)] public required List<UserReference> Users { get; set; }
        [JsonPropertyOrder(8)] public UserReference? Winner { get; set; }
        [JsonPropertyOrder(9)] public required List<Question> Questions { get; set; }
        [JsonPropertyOrder(10)] public required List<UserScore> GameScore { get; set; } = [];
        [JsonPropertyOrder(11)] public required UserReference CreatedBy { get; set; }
        [JsonPropertyOrder(12)] public required DateTime CreatedOn { get; set; }
        [JsonPropertyOrder(13)] public UserReference? UpdatedBy { get; set; }
        [JsonPropertyOrder(14)] public DateTime? UpdatedOn { get; set; }
        [JsonPropertyOrder(15)] public UserReference? DeletedBy { get; set; }
        [JsonPropertyOrder(16)] public DateTime? DeletedOn { get; set; }
        [JsonPropertyOrder(17)] public bool IsDeleted { get; set; }
    }
}
