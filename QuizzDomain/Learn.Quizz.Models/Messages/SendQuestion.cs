namespace Learn.Quizz.Models.Messages
{
    public class SendQuestion
    {
        public string GameCode { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }
}
