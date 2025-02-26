using Learn.Core.Shared.Models.Response;
using Learn.Quizz.Repository.Models;

namespace Learn.Quizz.Repository.Repositories
{
    public interface IQuizRepository
    {
        Task<BaseContentResponse<QuizzGame>> CreateQuizAsync(QuizzGame quiz, CancellationToken cancellationToken);
    }
}
