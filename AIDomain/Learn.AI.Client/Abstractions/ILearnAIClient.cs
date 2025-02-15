using Learn.AI.Models.Questions.Result;
using Learn.Core.Shared.Models.Response;

namespace Learn.AI.Client.Abstractions
{
    public interface ILearnAIClient
    {
        Task<BaseContentResponse<QuestionResult>?> GetNewQuestionAsync(string category, string language, CancellationToken cancellationToken);
    }
}
