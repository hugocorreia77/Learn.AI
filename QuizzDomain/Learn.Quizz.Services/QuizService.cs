using Learn.AI.Client.Abstractions;
using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Services.Abstractions;
using Learn.Quizz.Models.Quiz.Input;
using Learn.Quizz.Models.Quiz.Result;
using Learn.Quizz.Repository.Models;
using Learn.Quizz.Repository.Repositories;
using Learn.Quizz.Services.Converters;
using Learn.Quizz.Services.Interfaces;

namespace Learn.Quizz.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUserContextService _userContextService;
        private readonly ILearnAIClient _aiClient;

        public QuizService(IQuizRepository quizRepository, IUserContextService userContextService, ILearnAIClient aiClient)
        {
            _quizRepository = quizRepository;
            _userContextService = userContextService;
            _aiClient = aiClient;
        }

        public async Task<BaseContentResponse<QuizGameResult>> CreateGameAsync(CreateQuizInput input, CancellationToken cancellationToken)
        {
            var code = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6).ToString();

            var quiz = new QuizzGame
            {
                Code = code,
                Name = input.Name,
                CreatedBy = _userContextService.GetUser(),
                CreatedOn = DateTime.Now,
                Users = [],
                Questions = await GetQuestionsForQuizAsync(input.Categories, input.NumberOfQuestions, cancellationToken),
                Status = GameStatus.WaitingPlayersToJoin,
                Type = input.QuizType.ToDbModel()
            };

            var quizResult = await _quizRepository.CreateQuizAsync(quiz, cancellationToken);

            if (!quizResult.Success)
            {
                return new BaseContentResponse<QuizGameResult>
                {
                    Errors = quizResult.Errors,
                    Warnings = quizResult.Warnings
                }.SetFailed();
            }
            if(quizResult.Data is null)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Quiz game could not be created");
            }

            return new BaseContentResponse<QuizGameResult>
            {
                Data = new QuizGameResult
                {
                    Code = quizResult.Data.Code,
                    Name = quizResult.Data.Name,
                    Owner = quizResult.Data.CreatedBy,
                    Status = quizResult.Data.Status.ToSharedModel(),
                    NumberOfQuestions = quiz.Questions.Count,
                    CurrentQuestion = GetQuestionIndex(quizResult.Data.Questions, GetCurrentQuestion(quizResult.Data.Questions))                    
                }
            }.SetSucceeded();
        }

        private static Question? GetCurrentQuestion(List<Question> questions)
            => questions.Find(f => f.Status == QuestionStatus.Waiting);

        private static int GetQuestionIndex(List<Question> questions, Question? question)
        {
            if(question is null)
            {
                return 0;
            }

            return questions.IndexOf(question);
        }

        private async Task<List<Question>> GetQuestionsForQuizAsync(List<string> categories, int totalQuestions, CancellationToken cancellationToken)
        {
            List<Question> questions = [];
            var language = _userContextService.GetSelectedLanguage();
            var rnd = new Random();

            // Get random index
            for (int i=0; i< totalQuestions; i++)
            {
                int index = rnd.Next(categories.Count);
                var category = categories[index];

                var questionResponse = await _aiClient.GetNewQuestionAsync(category, language, cancellationToken);
                if(questionResponse is null || !questionResponse.Success || questionResponse.Data is null)
                {
                    continue;
                }

                questions.Add(new Question
                {
                    Category = new QuestionCategory
                    {
                        Name = category,
                    },
                    Language = new Language
                    {
                        AcceptLanguage = questionResponse.Data.Language.AcceptLanguage,
                        LanguageName = questionResponse.Data.Language.LanguageName
                    },
                    QuestionText = questionResponse.Data.QuestionText,
                    CorrectAnswer = questionResponse.Data.CorrectAnswer,
                    Explanation = questionResponse.Data.Explanation,
                    Status = QuestionStatus.Waiting,
                    Options = questionResponse.Data.Options?.Select(s => new QuestionOption
                    {
                        IsCorrect = s.IsCorrect,
                        Text = s.Text,
                    }).ToList() ?? [],
                });
            }

            return questions;
        }

    }
}
