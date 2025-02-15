using Learn.AI.Models.Questions.Result;
using Learn.Core.Shared.Models.Response;
using Learn.Learning.Models.Questions.Input;
using Learn.Learning.Models.Questions.Output;
using Learn.Learning.Repository.Models;

namespace Learn.Learning.Services.Interfaces
{
    public interface ILearningService
    {
        Task<BaseContentResponse<CreateQuestionResult>> CreateQuestion(CreateQuestionInput createQuestion, CancellationToken cancellationToken);
    }
}
