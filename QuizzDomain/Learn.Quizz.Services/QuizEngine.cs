using Learn.Core.Shared.Services.Abstractions;
using Learn.Quizz.Models.Question;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Learn.Quizz.Services
{
    public class QuizEngine
    (
         ILogger<QuizEngine> logger
       , IQuizPublisherService messagePublisher
       , IUserContextService userContextService 
       , IQuizService quizService
       , IHubContext<QuizHub> hubContext
        ) : IQuizEngine
    {

        public async Task JoinGame(string connectionId, string gameCode)
        {
            var user = userContextService.GetUser();

            var joinGameResult = await quizService.JoinGameAsync(new Models.Quiz.Input.JoinQuizInput
            {
                QuizCode = gameCode,
            }, CancellationToken.None);

            if (!joinGameResult.Success)
            {
                logger.LogError("User {User} couldn't join to game {QuizCode}", user.Username, gameCode);
                return;
            }

            await hubContext.Groups.AddToGroupAsync(connectionId, gameCode);
            await messagePublisher.PublishPlayerJoinedAsync(gameCode, $"Player {user.Username} joined the game!");
        }

        public async Task UnjoinGame(string connectionId, string gameCode)
        {
            var user = userContextService.GetUser();
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, gameCode);
            await messagePublisher.PublishPlayerJoinedAsync(gameCode, $"Player {user.Username} joined the game!");

        }

        public async Task StartGame(string connectionId, string gameCode)
        {

            var gameInfo = await quizService.GetGameAsync(gameCode, CancellationToken.None);
            if (!gameInfo.Success || gameInfo.Data is null)
            {
                return;
            }
            await messagePublisher.PublishGameStartingAsync(gameInfo.Data.Id, gameCode);

            var gameStartResult = await quizService.StartGameAsync(gameInfo.Data.Id, CancellationToken.None);
            var game = await quizService.GetFullGameAsync(gameInfo.Data.Id, CancellationToken.None);

            foreach (var question in game.Data?.Questions ?? [])
            {
                var outQuest = new QuestionReference
                {
                    Id = question.Id,
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

                if(!string.IsNullOrEmpty(question.Explanation))
                {
                    await messagePublisher.PublishQuestionResult(gameCode, question.Explanation);
                }
            }

            await Task.Delay(3000);
            await messagePublisher.PublishGameEnded(gameCode);
        }
    
        public async Task SetAttempt(Guid gameId, Guid questionId, Guid optionId)
        {
            await quizService.SetAttemptAsync(gameId, questionId, optionId, CancellationToken.None);
        }

    }
}
