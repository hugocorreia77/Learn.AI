using Learn.Core.Shared.Models.Response;
using Learn.Learning.Repository.Models;

namespace Learn.Learning.Repository.Repositories
{
    public interface IQuestionsRepository
    {
        Task<BaseContentResponse<List<Question>>> GetQuestionsAsync(string category, CancellationToken cancellationToken);
        Task<BaseContentResponse<Question>> CreateQuestionAsync(Question question, CancellationToken cancellationToken);
    }
}
