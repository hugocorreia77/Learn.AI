namespace Learn.Quizz.Services.Interfaces
{
    public interface IQuizEngine
    {
        Task JoinGame(string connectionId, string gameId);
        Task StartGame(string connectionId, string gameCode);
        Task SetAttempt(string connectionId, Guid gameId, Guid questionId, Guid optionId);
    }
}
