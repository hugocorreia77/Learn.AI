using Learn.Core.Shared.Models.User;
using Learn.Core.Shared.Repository;
using System.Text.Json.Serialization;

namespace Learn.Users.Repository.Models
{
    public class User : IAuditableAbstractModel
    {
        [JsonPropertyOrder(1)] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyOrder(2)] public required string Username { get; set; }
        [JsonPropertyOrder(3)] public required string Password { get; set; }
        [JsonPropertyOrder(4)] public required string Name { get; set; }
        [JsonPropertyOrder(5)] public string CountryCode { get; set; } = string.Empty;
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        [JsonPropertyOrder(6)] public UserReference? CreatedBy { get; set; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        [JsonPropertyOrder(7)] public DateTime CreatedOn { get; set; }
        [JsonPropertyOrder(8)] public UserReference? UpdatedBy { get; set; }
        [JsonPropertyOrder(9)] public DateTime? UpdatedOn { get; set; }
        [JsonPropertyOrder(10)] public UserReference? DeletedBy { get; set; }
        [JsonPropertyOrder(11)] public DateTime? DeletedOn { get; set; }
        [JsonPropertyOrder(12)] public bool IsDeleted { get; set; }
    }
}
