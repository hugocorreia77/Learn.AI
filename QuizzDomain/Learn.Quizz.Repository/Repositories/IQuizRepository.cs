using Learn.Core.Shared.Models.Response;
using Learn.Quizz.Repository.Models;
using Learn.Users.Models;

namespace Learn.Quizz.Repository.Repositories
{
    public interface IQuizRepository
    {
        Task EnsureCollection();
        Task<BaseContentResponse<QuizzGame>> CreateQuizAsync(QuizzGame quiz, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> GetQuizAsync(string quizCode, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> GetQuizAsync(Guid quizId, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> JoinQuizAsync(string quizCode, UserReference user, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> UpdateQuizStatusAsync(Guid quizId, GameStatus status, UserReference user, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> SetQuestionAttemptAsync(Guid quizId, Guid questionId, Attempt attempt, UserReference user, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> StartQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> AddScoreAsync(Guid gameId, UserReference user, int score, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> CloseQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken);
    }
}
