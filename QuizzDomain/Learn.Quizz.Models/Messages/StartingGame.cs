namespace Learn.Quizz.Models.Messages
{
    public class StartingGame
    {
        public required Guid QuizId { get; set; }
        public string GameCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
