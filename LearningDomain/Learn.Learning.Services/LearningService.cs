using Learn.AI.Client.Abstractions;
using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Services.Abstractions;
using Learn.Learning.Models.Questions.Input;
using Learn.Learning.Models.Questions.Output;
using Learn.Learning.Repository.Models;
using Learn.Learning.Repository.Repositories;
using Learn.Learning.Services.Interfaces;
using Learn.Learning.Services.Utils;

namespace Learn.Learning.Services
{
    public class LearningService(IUserContextService userContextService, IQuestionsRepository questionsRepository, ILearnAIClient aiClient) 
        : ILearningService
    {
        public async Task<BaseContentResponse<CreateQuestionResult>> CreateQuestion(CreateQuestionInput createQuestion, CancellationToken cancellationToken)
        {
            var question = await aiClient.GetQuestionAsync(createQuestion.Category, cancellationToken);

            if (question is not null 
                && question.Success 
                && question.Data is not null)
            {
                var qst = new Question
                {
                    Language = new Language
                    {
                        AcceptLanguage = question.Data.Language.AcceptLanguage,
                        LanguageName = question.Data.Language.LanguageName
                    },
                    Category = new QuestionCategory
                    {
                        Name = question.Data.Category
                    },
                    CreatedBy = userContextService.GetUser(),
                    CreatedOn = DateTime.Now,
                    QuestionText = question.Data.QuestionText,
                    CorrectAnswer = question.Data.CorrectAnswer,
                    Difficulty = question.Data.Difficulty.MapToModel(),
                    Explanation = question.Data.Explanation,
                    Options = question.Data.Options?.Select(s => new Repository.Models.QuestionOption
                    {
                        Text = s.Text,
                        IsCorrect = s.IsCorrect
                    }).ToList() ?? [],
                    QuestionType = question.Data.QuestionType.MapToModel(),
                    Tags = [createQuestion.Category]
                };

                var result = await questionsRepository.CreateQuestionAsync(qst, cancellationToken);

                if (result.Success && result.Data is not null)
                {
                    return new BaseContentResponse<CreateQuestionResult>
                    {
                        Data = new CreateQuestionResult
                        {
                            Id = result.Data.Id,
                            Category = result.Data.Category.Name,
                            Language = new Models.Language
                            {
                                AcceptLanguage = result.Data.Language.AcceptLanguage,
                                LanguageName = result.Data.Language.LanguageName
                            },
                            CorrectAnswer = result.Data.CorrectAnswer,
                            QuestionText = result.Data.QuestionText,
                            Difficulty = result.Data.Difficulty.MapToResult(),
                            Explanation = result.Data.Explanation,
                            Options = result.Data.Options?.Select(s => new Models.Questions.Output.QuestionOption { Text = s.Text, IsCorrect = s.IsCorrect }).ToList(),
                            QuestionType = result.Data.QuestionType.MapToResult()
                        }
                    };
                }

                return new BaseContentResponse<CreateQuestionResult>
                {
                    Errors = result.Errors,
                    Warnings = result.Warnings
                }.SetFailed();
            }

            return new BaseContentResponse<CreateQuestionResult>
            {
                Errors = question?.Errors ?? [],
                Warnings = question?.Warnings ?? [],                
            }.SetFailed();
        }
    }
}
