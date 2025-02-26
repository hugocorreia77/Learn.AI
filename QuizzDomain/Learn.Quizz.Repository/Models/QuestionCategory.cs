namespace Learn.Quizz.Repository.Models
{
    public class QuestionCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}
