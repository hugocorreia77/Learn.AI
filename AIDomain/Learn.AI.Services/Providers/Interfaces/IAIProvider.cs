using Learn.AI.Models.Questions;
using Learn.AI.Models.Questions.Result;
using Learn.Core.Shared.Models.Response;

namespace Learn.AI.Services.Providers.Interfaces
{
    public interface IAIProvider
    {
        Task<BaseContentResponse<QuestionResult>> GenerateQuestion(string category, QuestionDifficulty difficulty = QuestionDifficulty.Medium, string language = "pt-PT", CancellationToken cancellationToken = default);
        Task<BaseContentResponse<List<QuestionResult>>> GenerateQuestions(int numberOfQuestions, List<string> categories, QuestionDifficulty difficulty = QuestionDifficulty.Medium, string language = "pt-PT", CancellationToken cancellationToken = default);
    }
}
