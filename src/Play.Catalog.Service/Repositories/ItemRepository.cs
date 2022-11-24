using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class ItemRepository{
        private const string collectionName = "items";

        private readonly IMongoCollection<Item> dbColleciton;
        private readonly FilterDefinitionBuilder<Item> filter = Builders<Item>.Filter;

        public ItemRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");

            var database = mongoClient.GetDatabase("Catalog");

            dbColleciton = database.GetCollection<Item>(collectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await dbColleciton.Find(filter.Empty).ToListAsync();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            FilterDefinition<Item> filte = filter.Eq(entity => entity.Id,id);

            return await dbColleciton.Find(filte).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Item entity)
        {
            if(entity == null){
                throw new ArgumentException(nameof(entity));
            }

            await dbColleciton.InsertOneAsync(entity);
            
            
            }
        public async Task UpdateAsync(Item entity)
        {
            if(entity == null){
                throw new ArgumentException(nameof(entity));
            }
            FilterDefinition<Item> filte = filter.Eq(existingEntity =>existingEntity.Id,entity.Id);

            await dbColleciton.ReplaceOneAsync(filte,entity);
            
        }

        public async Task RemoveAsync(Guid id){
            FilterDefinition<Item> filte = filter.Eq(entity => entity.Id,id);
            await dbColleciton.DeleteOneAsync(filte);
        }
    }
}