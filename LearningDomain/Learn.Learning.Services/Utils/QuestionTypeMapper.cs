namespace Learn.Learning.Services.Utils
{
    public static class QuestionTypeMapper
    {
        public static Repository.Models.QuestionType MapToModel(this AI.Models.Questions.QuestionType type)
            =>
            type switch
            {
                AI.Models.Questions.QuestionType.TrueFalse => Repository.Models.QuestionType.TrueFalse,
                AI.Models.Questions.QuestionType.MultipleChoice => Repository.Models.QuestionType.MultipleChoice,
                AI.Models.Questions.QuestionType.OpenText => Repository.Models.QuestionType.OpenText,
                _ => throw new Exception("Question Type can not be mapped."),
            };

        public static Models.Questions.Output.QuestionType MapToResult(this Repository.Models.QuestionType type)
            =>
            type switch
            {
                Repository.Models.QuestionType.TrueFalse => Models.Questions.Output.QuestionType.TrueFalse,
                Repository.Models.QuestionType.MultipleChoice => Models.Questions.Output.QuestionType.MultipleChoice,
                Repository.Models.QuestionType.OpenText => Models.Questions.Output.QuestionType.OpenText,
                _ => throw new Exception("Question Type can not be mapped."),
            };

    }
}
