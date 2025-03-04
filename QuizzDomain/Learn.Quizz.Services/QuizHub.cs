using Learn.Core.Shared.Services;
using Learn.Quizz.Models.Question;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        public async Task StartGame(string gameCode)
        {
            await messagePublisher.PublishGameStartingAsync(gameCode);

            var gameInfo = await quizService.GetGameAsync(gameCode, CancellationToken.None);
            if (!gameInfo.Success || gameInfo.Data is null)
            {
                return;
            }

            var gameStartResult = await quizService.StartGameAsync(gameInfo.Data.Id, CancellationToken.None);
            var game = await quizService.GetFullGameAsync(gameInfo.Data.Id, CancellationToken.None);

            foreach(var question in game.Data?.Questions ?? [])
            {
                var outQuest = new QuestionReference
                {
                    Category = question.Category.Name,
                    QuestionText = question.QuestionText,
                    Options = question.Options?.Select(s => new QuestionOptionReference
                    {
                        Id = s.Id,
                        Text = s.Text
                    }).ToList() ?? []
                };

                await messagePublisher.PublishGameQuestionAsync(gameCode, JsonConvert.SerializeObject(outQuest));

                await Task.Delay(30000);

                await messagePublisher.PublishQuestionResult(gameCode, question.Explanation);
            }

            await Task.Delay(3000);
            await messagePublisher.PublishGameEnded(gameCode);

        }


    }
}
