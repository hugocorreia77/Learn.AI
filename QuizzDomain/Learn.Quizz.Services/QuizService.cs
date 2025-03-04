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
using StackExchange.Redis;

namespace Learn.Quizz.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUserContextService _userContextService;
        private readonly ILearnAIClient _aiClient;

        public QuizService(IQuizRepository quizRepository
            , IUserContextService userContextService
            , ILearnAIClient aiClient
            , IQuizPublisherService quizPublisher)
        {
            _quizRepository = quizRepository;
            _userContextService = userContextService;
            _aiClient = aiClient;
        }

        #region public async Task<BaseContentResponse<QuizGameResult>> CreateGameAsync(CreateQuizInput input, CancellationToken cancellationToken)
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
                    Id = quizResult.Data.Id,
                    Code = quizResult.Data.Code,
                    Name = quizResult.Data.Name,
                    Owner = quizResult.Data.CreatedBy,
                    Status = quizResult.Data.Status.ToSharedModel(),
                    NumberOfQuestions = quiz.Questions.Count,
                    CurrentQuestion = GetQuestionIndex(quizResult.Data.Questions, GetCurrentQuestion(quizResult.Data.Questions))                    
                }
            }.SetSucceeded();
        }
        #endregion

        #region public async Task<BaseContentResponse<QuizGameResult>> JoinGameAsync(JoinQuizInput input, CancellationToken cancellationToken)
        public async Task<BaseContentResponse<QuizGameResult>> JoinGameAsync(JoinQuizInput input, CancellationToken cancellationToken)
        {
            var game = await _quizRepository.GetQuizAsync(input.QuizCode, cancellationToken);
            if (!game.Success)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Not found");
            }
            if(game.Data is null)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Not found");
            }
            var quizGame = game.Data;

            var user = _userContextService.GetUser();
            if(quizGame.Users.Exists(us => us.Id == user.Id))
            {
                return new BaseContentResponse<QuizGameResult>
                {
                    Data = new QuizGameResult
                    {
                        Id = quizGame.Id,
                        Code = quizGame.Code,
                        Name = quizGame.Name,
                        Owner = quizGame.CreatedBy,
                        Status = quizGame.Status.ToSharedModel(),
                        NumberOfQuestions = quizGame.Questions.Count,
                        CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                    }
                }.SetSucceeded();
            }

            var quizGameJoined = await _quizRepository.JoinQuizAsync(input.QuizCode, user, cancellationToken);
            if (!quizGameJoined.Success)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Não foi possível juntar-se ao jogo.");
            }
            if (quizGameJoined.Data is null)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Não foi possível juntar-se ao jogo.");
            }
            var joinedGame = quizGameJoined.Data;

            return new BaseContentResponse<QuizGameResult>
            {
                Data = new QuizGameResult
                {
                    Code = joinedGame.Code,
                    Status = joinedGame.Status.ToSharedModel(),
                    Name = joinedGame.Name,
                    Owner = joinedGame.CreatedBy,
                    NumberOfQuestions = joinedGame.Questions.Count,
                    Id = joinedGame.Id,
                    CurrentQuestion = GetQuestionIndex(joinedGame.Questions, GetCurrentQuestion(joinedGame.Questions))                    
                }
            };
        }
        #endregion

        #region public async Task<BaseContentResponse<QuizGameResult>> GetGameAsync(Guid quizId, CancellationToken cancellationToken)
        public async Task<BaseContentResponse<QuizGameResult>> GetGameAsync(Guid quizId, CancellationToken cancellationToken)
        {
            var game = await _quizRepository.GetQuizAsync(quizId, cancellationToken);
            if (!game.Success)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Not found");
            }
            if (game.Data is null)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Not found");
            }
            var quizGame = game.Data;

            return new BaseContentResponse<QuizGameResult>
            {
                Data = new QuizGameResult
                {
                    Code = quizGame.Code,
                    Status = quizGame.Status.ToSharedModel(),
                    Name = quizGame.Name,
                    Owner = quizGame.CreatedBy,
                    NumberOfQuestions = quizGame.Questions.Count,
                    Id = quizGame.Id,
                    CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                }
            };
        }
        #endregion

        public async Task<BaseContentResponse<QuizGameResult>> GetGameAsync(string gameCode, CancellationToken cancellationToken)
        {
            var game = await _quizRepository.GetQuizAsync(gameCode, cancellationToken);
            if (!game.Success)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Not found");
            }
            if (game.Data is null)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Not found");
            }
            var quizGame = game.Data;

            return new BaseContentResponse<QuizGameResult>
            {
                Data = new QuizGameResult
                {
                    Code = quizGame.Code,
                    Status = quizGame.Status.ToSharedModel(),
                    Name = quizGame.Name,
                    Owner = quizGame.CreatedBy,
                    NumberOfQuestions = quizGame.Questions.Count,
                    Id = quizGame.Id,
                    CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                }
            };
        }


        public Task<BaseContentResponse<QuizzGame>> GetFullGameAsync(Guid quizId, CancellationToken cancellationToken)
            => _quizRepository.GetQuizAsync(quizId, cancellationToken);

        public async Task<BaseContentResponse<QuizGameResult>> StartGameAsync(Guid quizId, CancellationToken cancellationToken)
        {
            var gameResult = await _quizRepository.UpdateQuizStatusAsync(quizId, GameStatus.InProgress,
                _userContextService.GetUser(), cancellationToken);

            if (!gameResult.Success)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Não foi possível iniciar o jogo.");
            }
            if(gameResult.Data is null)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Não foi possível iniciar o jogo.");
            }
            var quizGame = gameResult.Data;

            return new BaseContentResponse<QuizGameResult>
            {
                Data = new QuizGameResult
                {
                    Code = quizGame.Code,
                    Status = quizGame.Status.ToSharedModel(),
                    Name = quizGame.Name,
                    Owner = quizGame.CreatedBy,
                    NumberOfQuestions = quizGame.Questions.Count,
                    Id = quizGame.Id,
                    CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                }
            }.SetSucceeded();
        }


        #region Private

        #region private static Question? GetCurrentQuestion(List<Question> questions)
        private static Question? GetCurrentQuestion(List<Question> questions)
            => questions.Find(f => f.Status == QuestionStatus.Waiting);
        #endregion

        #region private static int GetQuestionIndex(List<Question> questions, Question? question)
        private static int GetQuestionIndex(List<Question> questions, Question? question)
        {
            if(question is null)
            {
                return 0;
            }

            return questions.IndexOf(question);
        }
        #endregion

        #region private async Task<List<Question>> GetQuestionsForQuizAsync(List<string> categories, int totalQuestions, CancellationToken cancellationToken)
        private async Task<List<Question>> GetQuestionsForQuizAsync(List<string> categories, int totalQuestions, CancellationToken cancellationToken)
        {
            List<Question> questions = [];
            var language = _userContextService.GetSelectedLanguage();
            var rnd = new Random();

            var questionsResult = await _aiClient.GetQuestionsAsync(totalQuestions, categories, cancellationToken);
            if (!questionsResult.Success)
            {
                return [];
            }

            return questionsResult.Data?.Select(s => new Question
            {
                Category = new QuestionCategory
                {
                    Name = s.Category,
                },
                Language = new Language
                {
                    AcceptLanguage = s.Language.AcceptLanguage,
                    LanguageName = s.Language.LanguageName
                },
                QuestionText = s.QuestionText,
                CorrectAnswer = s.CorrectAnswer,
                Explanation = s.Explanation,
                Status = QuestionStatus.Waiting,
                Options = s.Options?.Select(s => new QuestionOption
                {
                    IsCorrect = s.IsCorrect,
                    Text = s.Text,
                }).ToList() ?? [],
            }).ToList() ?? [];
        }

        #endregion

        #endregion
    }
}
