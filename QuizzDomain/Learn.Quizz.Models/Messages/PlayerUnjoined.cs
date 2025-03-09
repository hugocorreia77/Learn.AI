namespace Learn.Quizz.Models.Messages
{
    public class PlayerUnjoined
    {
        public string GameCode { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}
