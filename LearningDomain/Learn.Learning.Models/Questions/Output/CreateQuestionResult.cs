namespace Learn.Learning.Models.Questions.Output
{
    public class CreateQuestionResult
    {
        public required Guid Id { get; set; }
        public required Language Language { get; set; }
        public required string QuestionText { get; set; }
        public required string Category { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<QuestionOption>? Options { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
    }
}
