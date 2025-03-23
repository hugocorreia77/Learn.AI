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
using Microsoft.Extensions.Logging;

namespace Learn.Quizz.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUserContextService _userContextService;
        private readonly ILearnAIClient _aiClient;
        private readonly ILogger<QuizService> _logger;

        public QuizService(ILogger<QuizService> logger
            , IQuizRepository quizRepository
            , IUserContextService userContextService
            , ILearnAIClient aiClient
            , IQuizPublisherService quizPublisher)
        {
            _quizRepository = quizRepository;
            _userContextService = userContextService;
            _aiClient = aiClient;
            _logger = logger;
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
                Type = input.QuizType.ToDbModel(),
                GameScore = []
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
                    Owner = new Users.Models.UserReference
                    {
                        Name = quizResult.Data.CreatedBy.Name,
                        Username = quizResult.Data.CreatedBy.Name,
                        Id = quizResult.Data.CreatedBy.Id
                    },
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
                        //new Users.Models.UserReference
                        //{
                        //    Name = quizGame.CreatedBy.Name,
                        //    Username = quizGame.CreatedBy.Name,
                        //    Id = quizGame.CreatedBy.Id
                        //},
                        Status = quizGame.Status.ToSharedModel(),
                        NumberOfQuestions = quizGame.Questions.Count,
                        CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                    }
                }.SetSucceeded();
            }

            if (quizGame.Status == GameStatus.InProgress)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Não pode juntar-se ao jogo, o jogo já está em progresso.");
            }
            else if (quizGame.Status == GameStatus.Finished)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("O jogo já terminou.");
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
                    Owner = new Users.Models.UserReference
                    {
                        Name = joinedGame.CreatedBy.Name,
                        Username = joinedGame.CreatedBy.Name,
                        Id = joinedGame.CreatedBy.Id
                    },
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
                    Owner = new Users.Models.UserReference
                    {
                        Name = quizGame.CreatedBy.Name,
                        Username = quizGame.CreatedBy.Name,
                        Id = quizGame.CreatedBy.Id
                    },
                    NumberOfQuestions = quizGame.Questions.Count,
                    Id = quizGame.Id,
                    CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                }
            };
        }
        #endregion

        #region public async Task<BaseContentResponse<QuizGameResult>> GetGameAsync(string gameCode, CancellationToken cancellationToken)
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
                    Owner = new Users.Models.UserReference
                    {
                        Name = quizGame.CreatedBy.Name,
                        Username = quizGame.CreatedBy.Name,
                        Id = quizGame.CreatedBy.Id
                    },
                    NumberOfQuestions = quizGame.Questions.Count,
                    Id = quizGame.Id,
                    CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                }
            };
        }
        #endregion

        public Task<BaseContentResponse<QuizzGame>> GetFullGameAsync(Guid quizId, CancellationToken cancellationToken)
            => _quizRepository.GetQuizAsync(quizId, cancellationToken);

        public async Task<BaseContentResponse<QuizGameResult>> StartGameAsync(Guid quizId, CancellationToken cancellationToken)
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

            if (quizGame.Status == GameStatus.InProgress)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("O jogo já está em progresso.");
            }
            else if (quizGame.Status == GameStatus.Finished)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("O jogo já terminou.");
            }

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
            quizGame = gameResult.Data;

            return new BaseContentResponse<QuizGameResult>
            {
                Data = new QuizGameResult
                {
                    Code = quizGame.Code,
                    Status = quizGame.Status.ToSharedModel(),
                    Name = quizGame.Name,
                    Owner = new Users.Models.UserReference
                    {
                        Name = quizGame.CreatedBy.Name,
                        Username = quizGame.CreatedBy.Name,
                        Id = quizGame.CreatedBy.Id
                    },
                    NumberOfQuestions = quizGame.Questions.Count,
                    Id = quizGame.Id,
                    CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                }
            }.SetSucceeded();
        }

        public async Task<BaseContentResponse<QuizzGame>> SetAttemptAsync(Guid quizId, Guid questionId, Guid optionId, CancellationToken cancellationToken)
        {            
            var gameInfo = await _quizRepository.GetQuizAsync(quizId, cancellationToken);
            if (!gameInfo.Success || gameInfo.Data is null)
            {
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError("Quiz not found.");
            }
            var game = gameInfo.Data;

            var user = _userContextService.GetUser();

            if(!game.Users.Exists(s => s.Id == user.Id))
            {
                //User do not belongs to this game
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError("User does not belong to the game.");
            }

            var question = game.Questions.SingleOrDefault(q => q.Id == questionId);
            if (question is null)
            {
                //Question does not exists
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError("Question not found.");
            }
            if (question.Status == QuestionStatus.Closed)
            {
                //Question is closed
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError("Question can not be answered anymore.");
            }

            if (question.Attempts is not null && question.Attempts.Exists(q => q.User.Id == user.Id))
            {
                //Already has an answer
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError("An attempt was already submitted");
            }

            if (question.Options is null || !question.Options.Exists(q => q.Id == optionId))
            {
                // Response option does not exists
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError("Attempt is not valid");
            }
            var option = question.Options.First(q => q.Id == optionId);

            var gameResponse = await _quizRepository.SetQuestionAttemptAsync(quizId, questionId, new Attempt { User = user, Answer = option}, user, cancellationToken);

            return gameResponse;
        }

        public async Task<BaseContentResponse<QuizGameResult>> EndGameAsync(Guid quizId, CancellationToken cancellationToken)
        {
            var gameResult = await _quizRepository.UpdateQuizStatusAsync(quizId, GameStatus.Finished,
                _userContextService.GetUser(), cancellationToken);

            if (!gameResult.Success)
            {
                return new BaseContentResponse<QuizGameResult>().SetFailed().AddError("Não foi possível iniciar o jogo.");
            }
            if (gameResult.Data is null)
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
                    Owner = new Users.Models.UserReference
                    {
                        Name = quizGame.CreatedBy.Name,
                        Username = quizGame.CreatedBy.Name,
                        Id = quizGame.CreatedBy.Id
                    },
                    NumberOfQuestions = quizGame.Questions.Count,
                    Id = quizGame.Id,
                    CurrentQuestion = GetQuestionIndex(quizGame.Questions, GetCurrentQuestion(quizGame.Questions))
                }
            }.SetSucceeded();
        }

        public Task<BaseContentResponse<QuizzGame>> StartQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken)
            => _quizRepository.StartQuestionAsync(quizId, questionId, cancellationToken);

        public Task<BaseContentResponse<QuizzGame>> CloseQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken)
            => _quizRepository.CloseQuestionAsync(quizId, questionId, cancellationToken);


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
                _logger.LogError("Não obteve resposta da API de AI");
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

        public Task<BaseContentResponse<QuizzGame>> AddScoreAsync(Guid gameId, int score, CancellationToken cancellationToken)
            => _quizRepository.AddScoreAsync(gameId, _userContextService.GetUser(), score, cancellationToken);
        #endregion

        #endregion
    }
}
