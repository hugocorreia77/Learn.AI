using Learn.Core.Shared.Models.Response;
using Learn.Quizz.Models.Quiz.Input;
using Learn.Quizz.Models.Quiz.Result;

namespace Learn.Quizz.Services.Interfaces
{
    public interface IQuizService
    {
        Task<BaseContentResponse<QuizGameResult>> CreateGameAsync(CreateQuizInput input, CancellationToken cancellationToken);
    }
}
