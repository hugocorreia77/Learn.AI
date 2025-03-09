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

        public async Task JoinGame(string gameCode)
        {
            await quizEngine.JoinGame(Context.ConnectionId, gameCode);
        }

        public async Task StartGame(string gameCode)
        {
            // Este processamento tem de ser feito numa thread ou processo diferente
            // usar direto o StartGame, bloqueia a thread no servidor e não permite processar outros pedidos
            _ = Task.Run(async () => await quizEngine.StartGame(Context.ConnectionId, gameCode));
        }

        public async Task RespondeOption(string quizId, string questionId, string optionId)
        {
            await quizEngine.SetAttempt(Guid.Parse(quizId), Guid.Parse(questionId), Guid.Parse(optionId));
        }

        public async Task RespondeCena(AnswerInput quizanswer)
        {
        }
        public async Task Tau(int valor)
        {
        }

        public async Task<string> Xau(string valor)
        {
            var x = valor;

            return "xau!!";
        }

    }
}
