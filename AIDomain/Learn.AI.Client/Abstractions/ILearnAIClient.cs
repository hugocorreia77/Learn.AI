using Learn.AI.Models.Questions.Result;
using Learn.Core.Shared.Models.Response;

namespace Learn.AI.Client.Abstractions
{
    public interface ILearnAIClient
    {
        Task<BaseContentResponse<QuestionResult>?> GetQuestionAsync(string category, CancellationToken cancellationToken);
        Task<BaseContentResponse<List<QuestionResult>>> GetQuestionsAsync(int numberOfQuestions, List<string> categories, CancellationToken cancellationToken);
    }
}
