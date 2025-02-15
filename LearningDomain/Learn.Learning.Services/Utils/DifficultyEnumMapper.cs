namespace Learn.Learning.Services.Utils
{
    public static class DifficultyEnumMapper
    {
        public static Repository.Models.QuestionDifficulty MapToModel(this AI.Models.Questions.QuestionDifficulty difficulty)
            =>
            difficulty switch
            {
                AI.Models.Questions.QuestionDifficulty.Easy => Repository.Models.QuestionDifficulty.Easy,
                AI.Models.Questions.QuestionDifficulty.Medium => Repository.Models.QuestionDifficulty.Medium,
                AI.Models.Questions.QuestionDifficulty.Hard => Repository.Models.QuestionDifficulty.Hard,
                _ => throw new Exception("Question Difficulty can not be mapped."),
            };

        public static Models.Questions.Output.QuestionDifficulty MapToResult(this Repository.Models.QuestionDifficulty difficulty)
            =>
            difficulty switch
            {
                Repository.Models.QuestionDifficulty.Easy => Models.Questions.Output.QuestionDifficulty.Easy,
                Repository.Models.QuestionDifficulty.Medium => Models.Questions.Output.QuestionDifficulty.Medium,
                Repository.Models.QuestionDifficulty.Hard => Models.Questions.Output.QuestionDifficulty.Hard,
                _ => throw new Exception("Question Difficulty can not be mapped."),
            };




    }
}
