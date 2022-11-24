using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;

var builder = WebApplication.CreateBuilder(args);
ServiceSettings servcieSetting = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
// Add services to the container.

builder.Services.AddControllers(opt=>
{
    opt.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(provider=>{
    var mongodbSetting = builder.Configuration.GetSection(nameof(MongoSetting)).Get<MongoSetting>();
    var mongodbClient = new MongoClient(mongodbSetting.ConnectionString);

    return mongodbClient.GetDatabase(servcieSetting.ServiceName);
});

builder.Services.AddScoped<IItemRepository,ItemRepository>();

BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
