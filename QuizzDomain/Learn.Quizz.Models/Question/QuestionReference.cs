namespace Learn.Quizz.Models.Question
{
    public class QuestionReference
    {
        public required Guid Id { get; set; }
        public required string Category { get; set; }
        public required string QuestionText { get; set; }
        public List<QuestionOptionReference>? Options { get; set; }
    }
}
