using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using Note2Book.Data;
using Note2Book.Dto;
using Note2Book.Interfaces;

namespace Note2Book.Services;

public class BookElasticService : IBookElasticService
{
    private readonly ElasticsearchClient _elasticClient;

    public BookElasticService(IOptions<ElasticSearchSettings> elasticSearchSettings)
    {
        _elasticClient = new ElasticsearchClient(new Uri(elasticSearchSettings.Value.Url));
    }

    public async Task<List<int>> SearchBooksAsync(string query)
    {
        var response = await _elasticClient.SearchAsync<BookDto>(nameof(BookDto).ToLowerInvariant(), s =>
            s.Query(
                q => q.MultiMatch(m => 
                    m.Query(query)))
                    .Fields(f => f
                        .Field(b => b.Title)
                        .Field(b => b.Description)
                        .Field(b => b.Author!.Name)
                        .Field(b => b.Genres.Select(c => c.Title))
                        ));
        
        return response.Documents.Select(c => c.Id).ToList();
    }
}