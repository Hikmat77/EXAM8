using Application.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class ApiResponseStatusCodeFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is IApiResponse response)
        {
            objectResult.StatusCode = (int)response.StatusCode;
            context.HttpContext.Response.StatusCode = (int)response.StatusCode;
        }

        await next();
    }
}
