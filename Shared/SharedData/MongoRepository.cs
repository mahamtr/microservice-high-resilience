using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace SharedMessages.SharedData;

public class MongoRepository <T> : IMongoRepository<T>  where T : BaseEntity 
{
    private readonly IMongoCollection<T> _collection;
    
    public MongoRepository(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<T>(
            databaseSettings.Value.CollectionName);
    }
    
    public async Task<List<T>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<T?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(T newBook) =>
        await _collection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, T updatedBook) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);

}