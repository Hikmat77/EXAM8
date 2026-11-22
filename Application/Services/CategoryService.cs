using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    private ICategoryRepository repository = categoryRepository;

    public async Task<ApiResponse<List<CategoryDto>>> GetAllAsync()
    {
        var categories = await repository.GetAllAsync();
        var result = categories.Select(x => new CategoryDto()
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
        return new ApiResponse<List<CategoryDto>>(HttpStatusCode.OK, "List of categories.", result);
    }
}