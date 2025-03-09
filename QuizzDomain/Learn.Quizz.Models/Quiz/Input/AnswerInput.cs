namespace Learn.Quizz.Models.Quiz.Input
{
    public class AnswerInput
    {
        public Guid QuizId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid AttemptId { get; set; }
    }
}
