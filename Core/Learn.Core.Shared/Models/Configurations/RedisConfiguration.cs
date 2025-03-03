namespace Learn.Core.Shared.Models.Configurations
{
    public class RedisConfiguration
    {
        public string Address { get; set; } = string.Empty;
        public string ChannelPrefix { get; set; } = string.Empty;
    }
}
