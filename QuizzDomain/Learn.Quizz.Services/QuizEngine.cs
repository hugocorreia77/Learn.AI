using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Services.Abstractions;
using Learn.Quizz.Models.Question;
using Learn.Quizz.Models.Quiz.Result;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;

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

        public async Task<BaseContentResponse<QuizGameResult>> JoinGameAsync(string connectionId, string gameCode, CancellationToken cancellationToken)
        {
            var user = userContextService.GetUser();

            var joinGameResult = await quizService.JoinGameAsync(new Models.Quiz.Input.JoinQuizInput
            {
                QuizCode = gameCode,
            }, cancellationToken);

            if (!joinGameResult.Success)
            {
                logger.LogError("User {User} couldn't join to game {QuizCode}", user.Username, gameCode);
                return joinGameResult;
            }

            await hubContext.Groups.AddToGroupAsync(connectionId, gameCode);
            await messagePublisher.PublishPlayerJoinedAsync(gameCode, $"Player {user.Username} joined the game!");
            return joinGameResult;
        }

        public async Task UnjoinGameAsync(string connectionId, string gameCode, CancellationToken cancellationToken)
        {
            var user = userContextService.GetUser();
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, gameCode);
            await messagePublisher.PublishPlayerJoinedAsync(gameCode, $"Player {user.Username} joined the game!");

        }

        public async Task<BaseContentResponse<QuizGameResult>> StartGameAsync(string connectionId, string gameCode, CancellationToken cancellationToken)
        {
            var gameInfo = await quizService.GetGameAsync(gameCode, cancellationToken);
            if (!gameInfo.Success || gameInfo.Data is null)
            {
                return gameInfo;
            }

            var gameStartResult = await quizService.StartGameAsync(gameInfo.Data.Id, cancellationToken);
            if (!gameStartResult.Success)
            {
                return gameStartResult;
            }

            // do not await
            Start(gameInfo.Data, gameCode, cancellationToken);
            
            return gameStartResult;
        }

        private async Task Start(QuizGameResult gameInfo, string gameCode, CancellationToken cancellationToken)
        {
            var game = await quizService.GetFullGameAsync(gameInfo.Id, cancellationToken);

            await messagePublisher.PublishGameStartingAsync(gameInfo.Id, gameCode);

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
                await quizService.StartQuestionAsync(gameInfo.Id, question.Id, cancellationToken);

                await Task.Delay(30000);
                await quizService.CloseQuestionAsync(gameInfo.Id, question.Id, cancellationToken);

                if (!string.IsNullOrEmpty(question.Explanation))
                {
                    await messagePublisher.PublishQuestionResult(gameCode, question.Explanation);
                }
            }

            await quizService.EndGameAsync(gameInfo.Id, cancellationToken);
            await messagePublisher.PublishGameEnded(gameCode);
        }

        public async Task SetAttemptAsync(Guid gameId, Guid questionId, Guid optionId, CancellationToken cancellationToken)
        {
            var responseTime = DateTime.Now;
            var qzGame = await quizService.SetAttemptAsync(gameId, questionId, optionId, cancellationToken);
            var question = qzGame.Data?.Questions.Find(q => q.Id == questionId);
            
            if (question is not null && question.StartedAt.HasValue)
            {
                var option = question.Options?.SingleOrDefault(k => k.IsCorrect);

                if(option is not null && option.Id == optionId)
                {
                    var questScore = CalculateScore(question.StartedAt.Value, responseTime);
                    await quizService.AddScoreAsync(gameId, questScore, cancellationToken);
                }
            }

        }

        public virtual int CalculateScore(DateTime questionStartTime, DateTime questionSubmitTime)
        {
            double responseTime = (questionSubmitTime - questionStartTime).TotalSeconds;
            double timeLeft = Math.Max(0, 30 - responseTime);
            return (int)(10 + (timeLeft * 90 / 30)); // Calcula os pontos com base no tempo
        }
    }
}
