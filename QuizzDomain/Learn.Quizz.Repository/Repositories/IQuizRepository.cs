using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Models.User;
using Learn.Quizz.Repository.Models;

namespace Learn.Quizz.Repository.Repositories
{
    public interface IQuizRepository
    {
        Task<BaseContentResponse<QuizzGame>> CreateQuizAsync(QuizzGame quiz, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> GetQuizAsync(string quizCode, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> GetQuizAsync(Guid quizId, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> JoinQuizAsync(string quizCode, UserReference user, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> UpdateQuizStatusAsync(Guid quizId, GameStatus status, UserReference user, CancellationToken cancellationToken);
    }
}
