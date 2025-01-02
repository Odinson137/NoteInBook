using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using Note2Book.Data;
using Note2Book.Dto;
using Note2Book.Interfaces;

namespace Note2Book.Services;

public class ElasticService : IElasticService
{
    private readonly ElasticsearchClient _elasticClient;
    public ElasticService(IOptions<ElasticSearchSettings> elasticSearchSettings)
    {
        _elasticClient = new ElasticsearchClient(new Uri(elasticSearchSettings.Value.Url));
    }
    
    public async Task AddIndexAsync<T>()
    {
        var isExist = await _elasticClient.Indices.ExistsAsync(typeof(T).Name.ToLowerInvariant());
        if (isExist.Exists) return;
        
        await _elasticClient.Indices.CreateAsync<T>(typeof(T).Name.ToLowerInvariant());
    }
    
    public async Task<bool> AddOrUpdateEntityAsync<T>(T entity) where T : BaseDto
    {
        var response = await _elasticClient.IndexAsync(entity, 
            x => x.Index(typeof(T).Name.ToLowerInvariant())
                .OpType(OpType.Index));
        return response.IsValidResponse;
    }
    
    public async Task<bool> AddOrUpdateEntitiesAsync<T>(IEnumerable<T> entities) where T : BaseDto
    {
        var response = await _elasticClient.BulkAsync(x => x.Index(typeof(T).Name.ToLowerInvariant())
                .UpdateMany(entities, (b, e) => b.Doc(e).DocAsUpsert()));
        return response.IsValidResponse;
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var response = await _elasticClient.GetAsync<T>(key, 
            g => g.Index(typeof(T).Name.ToLowerInvariant()));
        return response.Source;
    }
    
    public async Task<ICollection<T>> SearchAsync<T>() where T : BaseDto
    {
        var response = await _elasticClient.SearchAsync<T>(
            g => g.Index(typeof(T).Name.ToLowerInvariant()));
        return response.Documents.ToArray();
    }
    
    public async Task<bool> RemoveAsync<T>(string key) where T : BaseDto
    {
        var response = await _elasticClient.DeleteAsync<T>(key,
            g => g.Index(typeof(T).Name.ToLowerInvariant()));
        return response.IsValidResponse;
    }
}