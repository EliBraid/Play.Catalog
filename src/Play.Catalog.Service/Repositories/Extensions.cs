using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service.Repositories
{
    public static class Extensions{
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));
            
            services.AddSingleton(provider=>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                ServiceSettings servcieSetting = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongodbSetting = configuration.GetSection(nameof(MongoSetting)).Get<MongoSetting>();
                var mongodbClient = new MongoClient(mongodbSetting.ConnectionString);
                return mongodbClient.GetDatabase(servcieSetting.ServiceName);
            });
            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services,string collectionName) where T : IEntity
        {
            services.AddScoped<IRepository<Item>>(prvider=>
            {
                var database = prvider.GetService<IMongoDatabase>();
                return new MongoRepository<Item>(database,collectionName);
            });
            return services;
        }
    }
}