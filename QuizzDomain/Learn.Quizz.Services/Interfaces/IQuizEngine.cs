namespace Learn.Quizz.Services.Interfaces
{
    public interface IQuizEngine
    {
        Task JoinGame(string connectionId, string gameCode);
        Task UnjoinGame(string connectionId, string gameCode);
        Task StartGame(string connectionId, string gameCode);
        Task SetAttempt(Guid gameId, Guid questionId, Guid optionId);
    }
}
