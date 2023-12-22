namespace SharedMessages.SharedData;

public interface IMongoRepository <T> where T : BaseEntity
{
    
    public Task<List<T>> GetAsync();

    public  Task<T?> GetAsync(string id);

    public  Task CreateAsync(T newEntitiy);

    public  Task UpdateAsync(string id, T entitiy);

    public  Task RemoveAsync(string id);

}