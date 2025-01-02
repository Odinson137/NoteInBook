using Note2Book.Dto;

namespace Note2Book.Interfaces;

public interface IBookElasticService
{
    Task<List<int>> SearchBooksAsync(string query);
}