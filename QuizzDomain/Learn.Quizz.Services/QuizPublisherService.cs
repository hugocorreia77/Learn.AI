using Learn.Quizz.Models.Hub;
using Learn.Quizz.Models.Messages;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
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

    }
}
