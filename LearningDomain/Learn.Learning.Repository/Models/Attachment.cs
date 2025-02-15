namespace Learn.Learning.Repository.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
