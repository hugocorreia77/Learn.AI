using Learn.Core.Shared.Services;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Learn.Quizz.Services
{
    public class QuizHub(ILogger<QuizHub> logger
        , IQuizService quizService
        , IQuizPublisherService messagePublisher) : Hub
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

        public async Task JoinGame(string gameId)
        {
            var user = Context.GetUser();

            var joinGameResult = await quizService.JoinGameAsync(new Models.Quiz.Input.JoinQuizInput
            {
                QuizCode = gameId,
            }, CancellationToken.None);

            if (!joinGameResult.Success)
            {
                logger.LogError("User {User} couldn't join to game {QuizCode}", user.Username, gameId);
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await messagePublisher.PublishPlayerJoinedAsync(gameId, $"Player {user.Username} joined the game!");
        }

    }
}
