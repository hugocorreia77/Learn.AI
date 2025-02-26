using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Repository.Configurations;
using Learn.Quizz.Repository.Models;
using Learn.Quizz.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IMongoCollection<QuizzGame> QuizzGames;
        private readonly InsertOneOptions InsertOneOptions = new()
        {
            BypassDocumentValidation = false
        };
        #endregion

        #region Constructor
        public QuizMongoDbRepository(ILogger<QuizMongoDbRepository> logger, IOptionsSnapshot<RepositoryConfiguration> settings)
        {
            _logger = logger;
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            EnsureCollection(database).Wait();
            QuizzGames = database.GetCollection<QuizzGame>(COLLECTION_NAME);
        }
        #endregion

        #region private async Task EnsureCollection(IMongoDatabase database)
        private async Task EnsureCollection(IMongoDatabase database)
        {
            var collections = await database.ListCollectionNamesAsync();
            var collectionList = await collections.ToListAsync();

            if (!collectionList.Contains(COLLECTION_NAME))
            {
                await database.CreateCollectionAsync(COLLECTION_NAME);
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

    }
}
