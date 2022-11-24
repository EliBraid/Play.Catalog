namespace Play.Catalog.Service.Settings{
    public class MongoSetting
    {
        public string Host { get; init; }

        public int Port { get; init; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}