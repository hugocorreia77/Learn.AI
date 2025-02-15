namespace Learn.AI.Repository.Domain.Models
{
    public class QuestionCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}
