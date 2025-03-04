using Learn.Quizz.Models.Hub;
using Learn.Quizz.Models.Messages;
using Learn.Quizz.Models.Question;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Learn.Quizz.Services
{
    public class QuizConsumerService : IQuizConsumerService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IHubContext<QuizHub> _hubContext;
        private readonly ILogger<QuizPublisherService> _logger;

        public QuizConsumerService(ILogger<QuizPublisherService> logger, IConnectionMultiplexer redis, IHubContext<QuizHub> hubContext)
        {
            _redis = redis;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task RegisterSubscribers()
        {
            var sub = _redis.GetSubscriber();
            sub.Subscribe(
                    new RedisChannel(HubChannels.PlayerJoined, RedisChannel.PatternMode.Literal)
                ,   async(channel, message) => await ConsumePlayerJoinMessage(channel, message)
                );
            sub.Subscribe(
                    new RedisChannel(HubChannels.Question, RedisChannel.PatternMode.Literal)
                , async (channel, message) => await ConsumeQuestionMessage(channel, message)
                );
            sub.Subscribe(
                    new RedisChannel(HubChannels.QuestionResult, RedisChannel.PatternMode.Literal)
                , async (channel, message) => await ConsumeQuestionResultMessage(channel, message)
                );
            sub.Subscribe(
                    new RedisChannel(HubChannels.GameStarting, RedisChannel.PatternMode.Literal)
                , async (channel, message) => await ConsumeGameStartingMessage(channel, message)
                );
            sub.Subscribe(
                    new RedisChannel(HubChannels.GameEnded, RedisChannel.PatternMode.Literal)
                , async (channel, message) => await ConsumeGameEndedMessage(channel, message)
                );

            await Task.CompletedTask;
        }

        private async Task ConsumePlayerJoinMessage(RedisChannel channel, RedisValue message)
        {
            try
            {
                PlayerJoined playerJoined = JsonConvert.DeserializeObject<PlayerJoined>(message);
                Console.WriteLine("Player Joined: {0} {1}", playerJoined.GameCode, playerJoined.Message);

                await _hubContext
                            .Clients
                            .Group(playerJoined.GameCode)
                            .SendAsync("PlayerJoined", playerJoined.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send message via SignalR. Message: {Message}", message);
            }
        }

        private async Task ConsumeQuestionMessage(RedisChannel channel, RedisValue message)
        {
            try
            {
                SendQuestion sendQuestion = JsonConvert.DeserializeObject<SendQuestion>(message);
                Console.WriteLine("Sending question: {0} {1}", sendQuestion.GameCode, sendQuestion.Question);

                QuestionReference qst = JsonConvert.DeserializeObject<QuestionReference>(sendQuestion.Question);

                await _hubContext
                            .Clients
                            .Group(sendQuestion.GameCode)
                            .SendAsync("QuestionSent", qst);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send message via SignalR. Message: {Message}", message);
            }
        }
        
        private async Task ConsumeQuestionResultMessage(RedisChannel channel, RedisValue message)
        {
            try
            {
                QuestionResult questionResult = JsonConvert.DeserializeObject<QuestionResult>(message);
                Console.WriteLine("Sending question solution: {0} {1}", questionResult.GameCode, questionResult.Solution);

                await _hubContext
                            .Clients
                            .Group(questionResult.GameCode)
                            .SendAsync("QuestionSolutionSent", questionResult.Solution);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send message via SignalR. Message: {Message}", message);
            }
        }
        
        private async Task ConsumeGameStartingMessage(RedisChannel channel, RedisValue message)
        {
            try
            {
                StartingGame startingGame = JsonConvert.DeserializeObject<StartingGame>(message);
                Console.WriteLine("Sending starting game message: {0} {1}", startingGame.GameCode, startingGame.Message);

                await _hubContext
                            .Clients
                            .Group(startingGame.GameCode)
                            .SendAsync("GameStarting", startingGame.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send message via SignalR. Message: {Message}", message);
            }
        }

        private async Task ConsumeGameEndedMessage(RedisChannel channel, RedisValue message)
        {
            try
            {
                GameEnded startingGame = JsonConvert.DeserializeObject<GameEnded>(message);
                Console.WriteLine("Sending game ended message: {0} {1}", startingGame.GameCode, startingGame.Message);

                await _hubContext
                            .Clients
                            .Group(startingGame.GameCode)
                            .SendAsync("GameEnded", startingGame.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send message via SignalR. Message: {Message}", message);
            }
        }
    }
}
