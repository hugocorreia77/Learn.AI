﻿using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Repository.Configurations;
using Learn.Quizz.Repository.Models;
using Learn.Quizz.Repository.Repositories;
using Learn.Users.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Learn.Quizz.Repository.MongoDb.Repository
{
    public class QuizMongoDbRepository : IQuizRepository
    {
        #region Properties
        protected ILogger<QuizMongoDbRepository> _logger;
        private const string COLLECTION_NAME = "Quiz";
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<QuizzGame> QuizzGames;
        private readonly InsertOneOptions InsertOneOptions = new()
        {
            BypassDocumentValidation = false
        };
        #endregion

        #region Constructor
        public QuizMongoDbRepository(ILogger<QuizMongoDbRepository> logger, IOptions<RepositoryConfiguration> settings)
        {
            _logger = logger;
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
            QuizzGames = _database.GetCollection<QuizzGame>(COLLECTION_NAME);
        }
        #endregion

        #region private async Task EnsureCollection()
        public async Task EnsureCollection()
        {
            var collections = await _database.ListCollectionNamesAsync();
            var collectionList = await collections.ToListAsync();

            if (!collectionList.Contains(COLLECTION_NAME))
            {
                await _database.CreateCollectionAsync(COLLECTION_NAME);
                _logger.LogInformation($"Collection '{COLLECTION_NAME}' created on MongoDB.");
            }
            else
            {
                _logger.LogInformation($"Collection '{COLLECTION_NAME}' exists.");
            }

            // criar indice
            //var collection = database.GetCollection<Dominio>(collectionName);
            //var indexKeys = Builders<Dominio>.IndexKeys.Ascending(d => d.Nome);
            //var indexOptions = new CreateIndexOptions { Unique = true };
            //var indexModel = new CreateIndexModel<Dominio>(indexKeys, indexOptions);

            //await collection.Indexes.CreateOneAsync(indexModel);
            //Console.WriteLine("Índice único criado para o campo 'Nome'.");
        }
        #endregion

        public async Task<BaseContentResponse<QuizzGame>> CreateQuizAsync(QuizzGame quiz, CancellationToken cancellationToken)
        {
            try
            {
                await QuizzGames.InsertOneAsync(quiz, InsertOneOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while trying to insert one question into collection.");
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError(ex.Message);
            }

            return new BaseContentResponse<QuizzGame>
            {
                Data = quiz
            };
        }

        public async Task<BaseContentResponse<QuizzGame>> GetQuizAsync(string quizCode, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<QuizzGame>.Filter;
                var filter = builder.Eq(q => q.Code, quizCode);
                var result = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
                return new BaseContentResponse<QuizzGame>
                {
                    Data = result
                }.SetSucceeded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while trying to find the game.");
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError(ex.Message);
            }
        }

        public async Task<BaseContentResponse<QuizzGame>> JoinQuizAsync(string quizCode, UserReference user, CancellationToken cancellationToken)
        {
            var builder = Builders<QuizzGame>.Filter;
            var filter = builder.Eq(q => q.Code, quizCode);
            var quizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);

            if (quizGame.Users.Exists(e => e.Id == user.Id))
            {
                return new BaseContentResponse<QuizzGame> { Data = quizGame }.SetSucceeded();
            }

            var update = Builders<QuizzGame>.Update.Push(nameof(QuizzGame.Users), user);

            _ = await QuizzGames.UpdateOneAsync(filter, update);

            var updatedQuizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
            return new BaseContentResponse<QuizzGame>
            {
                Data = updatedQuizGame
            }.SetSucceeded();
        }

        public async Task<BaseContentResponse<QuizzGame>> GetQuizAsync(Guid quizId, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<QuizzGame>.Filter;
                var filter = builder.Eq(q => q.Id, quizId);
                var result = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
                return new BaseContentResponse<QuizzGame>
                {
                    Data = result
                }.SetSucceeded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while trying to find the game.");
                return new BaseContentResponse<QuizzGame>().SetFailed().AddError(ex.Message);
            }
        }

        public async Task<BaseContentResponse<QuizzGame>> UpdateQuizStatusAsync(Guid quizId, GameStatus status, UserReference user, CancellationToken cancellationToken)
        {
            var builder = Builders<QuizzGame>.Filter;
            var filter = builder.Eq(q => q.Id, quizId);
            var quizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
            var update = Builders<QuizzGame>.Update.Set(s => s.Status, status);

            _ = await QuizzGames.UpdateOneAsync(filter, update);

            var updatedQuizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
            return new BaseContentResponse<QuizzGame>
            {
                Data = updatedQuizGame
            }.SetSucceeded();
        }

        public async Task<BaseContentResponse<QuizzGame>> SetQuestionAttemptAsync(Guid quizId, Guid questionId, Attempt attempt, UserReference user, CancellationToken cancellationToken)
        {
            var builder = Builders<QuizzGame>.Filter;
            
            var filter = Builders<QuizzGame>.Filter.And(
                   Builders<QuizzGame>.Filter.Eq(q => q.Id, quizId),
                   Builders<QuizzGame>.Filter.Eq($"{nameof(QuizzGame.Questions)}.{nameof(Question.Id)}", questionId)
            );

            var update = Builders<QuizzGame>.Update
                 .Push($"{nameof(QuizzGame.Questions)}.$.{nameof(Question.Attempts)}", attempt);

            _ = await QuizzGames.UpdateOneAsync(filter, update);

            var updatedQuizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
            return new BaseContentResponse<QuizzGame>
            {
                Data = updatedQuizGame
            }.SetSucceeded();
        }

        public async Task<BaseContentResponse<QuizzGame>> StartQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken)
        {
            var builder = Builders<QuizzGame>.Filter;

            var filter = Builders<QuizzGame>.Filter.And(
                   Builders<QuizzGame>.Filter.Eq(q => q.Id, quizId),
                   Builders<QuizzGame>.Filter.Eq($"{nameof(QuizzGame.Questions)}.{nameof(Question.Id)}", questionId)
            );

            var update = Builders<QuizzGame>.Update
                 .Set($"{nameof(QuizzGame.Questions)}.$.{nameof(Question.StartedAt)}", DateTime.Now)
                 .Set($"{nameof(QuizzGame.Questions)}.$.{nameof(Question.Status)}", QuestionStatus.InProgress)
                 ;

            _ = await QuizzGames.UpdateOneAsync(filter, update);

            var updatedQuizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
            return new BaseContentResponse<QuizzGame>
            {
                Data = updatedQuizGame
            }.SetSucceeded();
        }

        public async Task<BaseContentResponse<QuizzGame>> AddScoreAsync(Guid gameId, UserReference user, int score, CancellationToken cancellationToken)
        {
            var filter = Builders<QuizzGame>.Filter.And(
                   Builders<QuizzGame>.Filter.Eq(g => g.Id, gameId),
                   Builders<QuizzGame>.Filter.ElemMatch(g => g.GameScore, p => p.User == user)
               );

            var update = Builders<QuizzGame>.Update.Inc($"{nameof(QuizzGame.GameScore)}.$.{nameof(UserScore.Score)}", score);

            var updateResult = await QuizzGames.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            if (updateResult.ModifiedCount == 0)
            {
                var addPlayerFilter = Builders<QuizzGame>.Filter.Eq(g => g.Id, gameId);
                var addPlayerUpdate = Builders<QuizzGame>.Update.Push(g => g.GameScore, 
                    new UserScore
                    {
                        User = user,
                        Score = score
                    });

                await QuizzGames.UpdateOneAsync(addPlayerFilter, addPlayerUpdate, cancellationToken: cancellationToken);
            }

            var updatedQuizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
            return new BaseContentResponse<QuizzGame>
            {
                Data = updatedQuizGame
            };
        }

        public async Task<BaseContentResponse<QuizzGame>> CloseQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken)
        {
            var filter = Builders<QuizzGame>.Filter.And(
                   Builders<QuizzGame>.Filter.Eq(g => g.Id, quizId),
                   Builders<QuizzGame>.Filter.ElemMatch(g => g.Questions, p => p.Id == questionId)
               );

            var update = Builders<QuizzGame>.Update.Set($"{nameof(QuizzGame.Questions)}.$.{nameof(Question.Status)}", QuestionStatus.Closed);

            await QuizzGames.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            var updatedQuizGame = await QuizzGames.Find(filter).SingleOrDefaultAsync(cancellationToken);
            return new BaseContentResponse<QuizzGame>
            {
                Data = updatedQuizGame
            };
        }
    }
}
