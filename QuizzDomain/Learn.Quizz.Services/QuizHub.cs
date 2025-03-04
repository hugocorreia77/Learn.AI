using Learn.Core.Shared.Services;
using Learn.Quizz.Models.Question;
using Learn.Quizz.Repository.Models;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Learn.Quizz.Services
{
    public class QuizHub(ILogger<QuizHub> logger
        //, IQuizService quizService
        //, IQuizPublisherService messagePublisher
        , IQuizEngine quizEngine
        ) : Hub
    {

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity is not null && !Context.User.Identity.IsAuthenticated)
            {
                logger.LogWarning("Someone tried to connect without authentication.");
                Context.Abort();
            }

            await base.OnConnectedAsync();
        }

        public async Task JoinGame(string gameCode)
        {
            await quizEngine.JoinGame(Context.ConnectionId, gameCode);
        }

        public async Task StartGame(string gameCode)
        {
            await quizEngine.StartGame(Context.ConnectionId, gameCode);
        }

        public async Task SetAttempt(Guid quizId, Guid questionId, Guid optionId)
        {
            await quizEngine.SetAttempt(Context.ConnectionId, quizId, questionId, optionId);
        }

    }
}
