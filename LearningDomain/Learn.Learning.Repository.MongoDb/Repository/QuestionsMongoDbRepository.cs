using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Repository.Configurations;
using Learn.Learning.Repository.Models;
using Learn.Learning.Repository.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Learn.Learning.Repository.MongoDb.Repository
{
    public class QuestionsMongoDbRepository : IQuestionsRepository
    {
        #region Properties
        protected ILogger<QuestionsMongoDbRepository> _logger;
        private const string COLLECTION_NAME = "Questions";
        private readonly IMongoCollection<Question> Questions;
        private readonly InsertOneOptions InsertOneOptions = new()
        {
            BypassDocumentValidation = false
        };
        #endregion

        #region Constructor
        public QuestionsMongoDbRepository(ILogger<QuestionsMongoDbRepository> logger, IOptionsSnapshot<RepositoryConfiguration> settings)
        {
            _logger = logger;
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            EnsureCollection(database).Wait();
            Questions = database.GetCollection<Question>(COLLECTION_NAME);
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

        #region Create question
        public async Task<BaseContentResponse<Question>> CreateQuestionAsync(Question question, CancellationToken cancellationToken)
        {
            try
            {
                await Questions.InsertOneAsync(question, InsertOneOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while trying to insert one question into collection.");
                return new BaseContentResponse<Question>().SetFailed().AddError(ex.Message);
            }

            return new BaseContentResponse<Question>
            {
                Data = question
            };
        }
        #endregion

        #region Get questions by category
        public async Task<BaseContentResponse<List<Question>>> GetQuestionsAsync(string category, CancellationToken cancellationToken)
        {
            try
            {
                var searchResult = await Questions.FindAsync(d => d.Category.Name == category, cancellationToken: cancellationToken);
                return new BaseContentResponse<List<Question>>
                {
                    Data = searchResult.ToList(cancellationToken: cancellationToken)
                }.SetSucceeded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while trying to find questions from collection.");
                return new BaseContentResponse<List<Question>>().SetFailed().AddError(ex.Message);
            }
        }
        #endregion

    }
}
