namespace Learn.Quizz.Services.Interfaces
{
    public interface IQuizPublisherService
    {
        Task PublishPlayerJoinedAsync(string gameCode, string message);
    }
}
