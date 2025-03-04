using Learn.Core.Shared.Models.Response;
using Learn.Quizz.Models.Quiz.Input;
using Learn.Quizz.Models.Quiz.Result;
using Learn.Quizz.Repository.Models;

namespace Learn.Quizz.Services.Interfaces
{
    public interface IQuizService
    {
        Task<BaseContentResponse<QuizGameResult>> CreateGameAsync(CreateQuizInput input, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizGameResult>> JoinGameAsync(JoinQuizInput input, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizGameResult>> GetGameAsync(Guid quizId, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizzGame>> GetFullGameAsync(Guid quizId, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizGameResult>> GetGameAsync(string gameCode, CancellationToken cancellationToken);
        Task<BaseContentResponse<QuizGameResult>> StartGameAsync(Guid quizId, CancellationToken cancellationToken);
    }
}
