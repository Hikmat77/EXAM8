using System.Net;

namespace Application.Responses;

public interface IApiResponse
{
    HttpStatusCode StatusCode { get; }
}

public class ApiResponse<T> : IApiResponse
{
    public HttpStatusCode StatusCode {get; set;}
    public string? Message {get; set;}
    public T? Data {get; set;}

    public ApiResponse(HttpStatusCode statusCode, string message, T data)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }
    public ApiResponse(HttpStatusCode statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }
}