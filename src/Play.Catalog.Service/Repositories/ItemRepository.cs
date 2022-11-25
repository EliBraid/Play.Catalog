using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{

    public class MongoRepository<T> : IRepository<T> where T: IEntity
    {
        private readonly IMongoCollection<T> dbColleciton;
        private readonly FilterDefinitionBuilder<T> filter = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database,string collectionName)
        {
            dbColleciton = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbColleciton.Find(filter.Empty).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filte = filter.Eq(entity => entity.Id, id);

            return await dbColleciton.Find(filte).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity));
            }

            await dbColleciton.InsertOneAsync(entity);


        }
        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity));
            }
            FilterDefinition<T> filte = filter.Eq(existingEntity => existingEntity.Id, entity.Id);

            await dbColleciton.ReplaceOneAsync(filte, entity);

        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filte = filter.Eq(entity => entity.Id, id);
            await dbColleciton.DeleteOneAsync(filte);
        }
    }
}