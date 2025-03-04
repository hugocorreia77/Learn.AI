using Learn.Quizz.Models.Hub;
using Learn.Quizz.Models.Messages;
using Learn.Quizz.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Learn.Quizz.Services
{
    public class QuizPublisherService : IQuizPublisherService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<QuizPublisherService> _logger;

        public QuizPublisherService(ILogger<QuizPublisherService> logger, IConnectionMultiplexer redis)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task PublishGameEnded(string gameCode)
        {
            var sub = _redis.GetSubscriber();
            string msg = JsonConvert.SerializeObject(new GameEnded
            {
                GameCode = gameCode,
                Message = "The game is ended! Play again!"
            });
            _logger.LogTrace("Publishing {Message}", msg);
            var clientsReceivedCount = await sub.PublishAsync(new RedisChannel(HubChannels.GameEnded, RedisChannel.PatternMode.Literal), msg);
            _logger.LogTrace("Message received by {TotalClients}", clientsReceivedCount);
        }

        public async Task PublishGameQuestionAsync(string gameCode, string message)
        {
            var sub = _redis.GetSubscriber();
            string msg = JsonConvert.SerializeObject(new SendQuestion
            {
                GameCode = gameCode,
                Question = message
            });
            _logger.LogTrace("Publishing {Message}", msg);
            var clientsReceivedCount = await sub.PublishAsync(new RedisChannel(HubChannels.Question, RedisChannel.PatternMode.Literal), msg);
            _logger.LogTrace("Message received by {TotalClients}", clientsReceivedCount);
        }

        public async Task PublishGameStartingAsync(string gameCode)
        {
            var sub = _redis.GetSubscriber();
            string msg = JsonConvert.SerializeObject(new StartingGame
            {
                GameCode = gameCode,
                Message = "Game is starting! Get ready!"
            });
            _logger.LogTrace("Publishing {Message}", msg);
            var clientsReceivedCount = await sub.PublishAsync(new RedisChannel(HubChannels.GameStarting, RedisChannel.PatternMode.Literal), msg);
            _logger.LogTrace("Message received by {TotalClients}", clientsReceivedCount);
        }

        public async Task PublishPlayerJoinedAsync(string gameCode, string message)
        {
            var sub = _redis.GetSubscriber();
            string msg = JsonConvert.SerializeObject(new PlayerJoined
            {
                GameCode = gameCode,
                Message = message
            });

            _logger.LogTrace("Publishing {Message}", msg);
            var clientsReceivedCount = await sub.PublishAsync(new RedisChannel(HubChannels.PlayerJoined, RedisChannel.PatternMode.Literal), msg);
            _logger.LogTrace("Message received by {TotalClients}", clientsReceivedCount);
        }

        public async Task PublishQuestionResult(string gameCode, string message)
        {
            var sub = _redis.GetSubscriber();
            string msg = JsonConvert.SerializeObject(new QuestionResult
            {
                GameCode = gameCode,
                Solution = message
            });

            _logger.LogTrace("Publishing {Message}", msg);
            var clientsReceivedCount = await sub.PublishAsync(new RedisChannel(HubChannels.QuestionResult, RedisChannel.PatternMode.Literal), msg);
            _logger.LogTrace("Message received by {TotalClients}", clientsReceivedCount);
        }
    }
}
