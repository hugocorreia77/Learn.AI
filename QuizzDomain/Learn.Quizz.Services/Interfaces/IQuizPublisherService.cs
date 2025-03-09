namespace Learn.Quizz.Services.Interfaces
{
    public interface IQuizPublisherService
    {
        Task PublishPlayerJoinedAsync(string gameCode, string message);
        Task PublishPlayerUnjoinedAsync(string gameCode, string message);
        Task PublishGameStartingAsync(Guid quizId, string gameCode);
        Task PublishGameQuestionAsync(string gameCode, string message);
        Task PublishQuestionResult(string gameCode, string message);
        Task PublishGameEnded(string gameCode);
    }
}
