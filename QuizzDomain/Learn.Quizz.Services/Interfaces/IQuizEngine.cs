using Learn.Core.Shared.Models.Response;
using Learn.Quizz.Models.Quiz.Result;

namespace Learn.Quizz.Services.Interfaces
{
    public interface IQuizEngine
    {
        Task<BaseContentResponse<QuizGameResult>> JoinGameAsync(string connectionId, string gameCode, CancellationToken cancellationToken);
        Task UnjoinGameAsync(string connectionId, string gameCode, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizGameResult>> StartGameAsync(string connectionId, string gameCode, CancellationToken cancellationToken);
        Task SetAttemptAsync(Guid gameId, Guid questionId, Guid optionId, CancellationToken cancellationToken);
        int CalculateScore(DateTime questionStartTime, DateTime questionSubmitTime);
    }
}
