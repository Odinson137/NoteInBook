using AutoMapper;
using Note2Book.Dto;
using Note2Book.Models;

namespace Note2Book.Data;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<Genre, GenreDto>();
        CreateMap<Author, AuthorDto>();
    }
}