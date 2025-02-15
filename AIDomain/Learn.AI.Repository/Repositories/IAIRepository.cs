using Learn.AI.Repository.Domain.Models;
using Learn.Core.Shared.Models.Response;

namespace Learn.AI.Repository.Repositories
{
    public interface IAIRepository
    {
        Task<BaseContentResponse<List<Question>>> GetQuestionsAsync(string category, CancellationToken cancellationToken);
        Task<BaseContentResponse<Question>> CreateQuestionAsync(Question question, CancellationToken cancellationToken);
    }
}
