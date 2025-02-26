namespace Learn.Quizz.Services.Converters
{
    public static class SharedModel2DbModelConverter
    {

        public static Repository.Models.QuizzType ToDbModel(this Models.Enums.QuizzType quizType)
            => quizType switch
            {
                Models.Enums.QuizzType.SinglePlayer => Repository.Models.QuizzType.SinglePlayer,
                Models.Enums.QuizzType.OnlineMultiplayer => Repository.Models.QuizzType.OnlineMultiplayer,
                _ => throw new Exception("Quizz type conversion not supported"),
            };

        public static Repository.Models.GameStatus ToDbModel(this Models.Enums.GameStatus gameStatus)
            => gameStatus switch
            {
                Models.Enums.GameStatus.Finished => Repository.Models.GameStatus.Finished,
                Models.Enums.GameStatus.InProgress => Repository.Models.GameStatus.InProgress,
                Models.Enums.GameStatus.WaitingPlayersToJoin => Repository.Models.GameStatus.WaitingPlayersToJoin,
                _ => throw new Exception("GameStatus type conversion not supported"),
            };

    }
}
