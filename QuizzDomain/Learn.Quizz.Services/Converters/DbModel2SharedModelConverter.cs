namespace Learn.Quizz.Services.Converters
{
    public static class DbModel2SharedModelConverter
    {
        public static Models.Enums.QuizzType ToSharedModel(this Repository.Models.QuizzType quizType)
            => quizType switch
            {
                Repository.Models.QuizzType.SinglePlayer => Models.Enums.QuizzType.SinglePlayer,
                Repository.Models.QuizzType.OnlineMultiplayer => Models.Enums.QuizzType.OnlineMultiplayer,
                _ => throw new Exception("Quizz type conversion not supported"),
            };

        public static Models.Enums.GameStatus ToSharedModel(this Repository.Models.GameStatus gameStatus)
            => gameStatus switch
            {
                Repository.Models.GameStatus.Finished => Models.Enums.GameStatus.Finished,
                Repository.Models.GameStatus.InProgress => Models.Enums.GameStatus.InProgress,
                Repository.Models.GameStatus.WaitingPlayersToJoin => Models.Enums.GameStatus.WaitingPlayersToJoin,
                _ => throw new Exception("GameStatus type conversion not supported"),
            };
    }
}
