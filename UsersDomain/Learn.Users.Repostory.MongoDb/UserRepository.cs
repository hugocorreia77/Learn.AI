using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Repository.Configurations;
using Learn.Users.Repository.Models;
using Learn.Users.Repository.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Learn.Core.Shared.Extensions;

namespace Learn.Users.Repository.MongoDb
{
    public class UserRepository : IUserRepository
    {
        #region Properties
        protected ILogger<UserRepository> _logger;
        private const string COLLECTION_NAME = "Users";
        private readonly IMongoCollection<User> Users;
        private readonly InsertOneOptions InsertOneOptions = new()
        {
            BypassDocumentValidation = false
        };
        #endregion

        #region Constructor
        public UserRepository(ILogger<UserRepository> logger, IOptionsSnapshot<RepositoryConfiguration> settings)
        {
            _logger = logger;
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            EnsureCollection(database).GetAwaiter().GetResult();
            Users = database.GetCollection<User>(COLLECTION_NAME);
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

        #region public async Task<BaseContentResponse<User>> CreateUserAsync(User model, CancellationToken cancellationToken)
        public async Task<BaseContentResponse<User>> CreateUserAsync(User model, CancellationToken cancellationToken)
        {
            try
            {
                if (await UsernameExists(model.Username, cancellationToken))
                {
                    return new BaseContentResponse<User>().SetFailed().AddError("User already exists.");
                }
                await Users.InsertOneAsync(model, InsertOneOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while trying to insert one question into collection.");
                return new BaseContentResponse<User>().SetFailed().AddError(ex.Message);
            }

            return new BaseContentResponse<User>
            {
                Data = model
            };
        }
        #endregion

        #region private async Task<bool> UsernameExists(string username, CancellationToken cancellationToken)
        private async Task<bool> UsernameExists(string username, CancellationToken cancellationToken)
        {
            FilterDefinitionBuilder<User> filterDefinitionBuilder = new FilterDefinitionBuilder<User>();
            var filterDefinition = filterDefinitionBuilder.Eq(nameof(User.Username), username);
            var results = await Users.FindAsync<User>(filterDefinition, cancellationToken: cancellationToken);
            return await results.AnyAsync(cancellationToken);
        }
        #endregion

        #region public async Task<BaseContentResponse<User>> GetUserAsync(string username, string password, CancellationToken cancellationToken)
        public async Task<BaseContentResponse<User>> GetUserAsync(string username, string password, CancellationToken cancellationToken)
        {
            FilterDefinitionBuilder<User> filterDefinitionBuilder = new();
            var filterDefinition = filterDefinitionBuilder
                .And(
                    filterDefinitionBuilder.Eq(nameof(User.Username), username),
                    filterDefinitionBuilder.Eq(nameof(User.Password), password)
                );
                
            var results = await Users.FindAsync<User>(filterDefinition, cancellationToken: cancellationToken);
            var users = await results.ToListAsync(cancellationToken);
            
            if(users.Count == 0)
            {
                return new BaseContentResponse<User>().SetFailed().AddError("Not found.");
            }
            if (users.Count > 1)
            {
                _logger.LogCritical("Critical error occurred! More than one user was found with the same username and password. Please, check: {username}", username);
                return new BaseContentResponse<User>().SetFailed().AddError("It was not possible to determine the current user.");
            }

            return new BaseContentResponse<User>
            {
                Data = users.First()
            }.SetSucceeded();
        }
        #endregion

        public async Task<BaseContentResponse<User>> GetUserAsync(string username, CancellationToken cancellationToken)
        {
            FilterDefinitionBuilder<User> filterDefinitionBuilder = new();
            var filterDefinition = filterDefinitionBuilder.Eq(nameof(User.Username), username);
            var results = await Users.FindAsync<User>(filterDefinition, cancellationToken: cancellationToken);
            var users = await results.ToListAsync(cancellationToken);

            if (users.Count == 0)
            {
                return new BaseContentResponse<User>().SetFailed().AddError("Not found.");
            }
            if (users.Count > 1)
            {
                _logger.LogCritical("Critical error occurred! More than one user was found with the same username and password. Please, check: {username}", username);
                return new BaseContentResponse<User>().SetFailed().AddError("It was not possible to determine the current user.");
            }

            return new BaseContentResponse<User>
            {
                Data = users.First()
            }.SetSucceeded();
        }

    }
}
