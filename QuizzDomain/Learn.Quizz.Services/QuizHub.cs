using Learn.Quizz.Models.Quiz.Input;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Learn.Quizz.Services
{
    public class QuizHub(ILogger<QuizHub> logger
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

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //await quizEngine.UnjoinGame(Context.ConnectionId,) ??

            base.OnDisconnectedAsync(exception);
        }

        public async Task<string> JoinGame(string gameCode)
        {
            var result = await quizEngine.JoinGameAsync(Context.ConnectionId, gameCode, CancellationToken.None);
        
            if(!result.Success)
            {
                return result.Errors.FirstOrDefault()?.Message ?? "Não foi possível juntar-se ao jogo.";
            }

            return "Juntou-se ao jogo!";
        }

        public async Task<string> StartGame(string gameCode)
        {
            var result = await quizEngine.StartGameAsync(Context.ConnectionId, gameCode, CancellationToken.None);
        
            if(!result.Success)
            {
                return result.Errors.FirstOrDefault()?.Message ?? "Não foi possível iniciar o jogo";
            }

            return "Jogo iniciado!";
        }

        public async Task AnswerOption(AnswerInput answer)
        {
            await quizEngine.SetAttemptAsync(answer.QuizId, answer.QuestionId, answer.AttemptId, CancellationToken.None);
        }


    }
}
