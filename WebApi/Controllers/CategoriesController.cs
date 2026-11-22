using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("categories")]
public class CategoriesController(
    ICategoryService categoryService,
    ILogger<CategoriesController> logger) : ControllerBase
{
    private ICategoryService service = categoryService;
    private ILogger<CategoriesController> _logger = logger;

    [HttpGet]
    public async Task<ApiResponse<List<CategoryDto>>> GetAll()
    {
        _logger.LogInformation("GET categories {date}", DateTime.Now);
        return await service.GetAllAsync();
    }
}
