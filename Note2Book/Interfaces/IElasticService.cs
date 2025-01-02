using Note2Book.Dto;

namespace Note2Book.Interfaces;

public interface IElasticService
{
    public Task AddIndexAsync<T>();

    Task<bool> AddOrUpdateEntityAsync<T>(T entity) where T : BaseDto;

    Task<bool> AddOrUpdateEntitiesAsync<T>(IEnumerable<T> entities) where T : BaseDto;

    Task<T?> GetAsync<T>(string key);

    Task<ICollection<T>> SearchAsync<T>() where T : BaseDto;

    Task<bool> RemoveAsync<T>(string key) where T : BaseDto;
}