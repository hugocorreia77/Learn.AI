using Learn.Quizz.Models.Hub;
using Learn.Quizz.Models.Messages;
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

        public void ListenForPlayerJoinMessage()
        {
            var sub = _redis.GetSubscriber();
            sub.Subscribe(
                new RedisChannel(HubChannels.PlayerJoined, RedisChannel.PatternMode.Literal)
                , async (channel, message) =>            
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
                });
        }
    }
}
