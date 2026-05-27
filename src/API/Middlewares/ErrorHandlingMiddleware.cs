using System.Net;
using System.Text.Json;
using Domain.Shared;

namespace API.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BaseException ex)
        {
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(
                context, 
                HttpStatusCode.InternalServerError, 
                "Internal server error has occured, try again later."
            );
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}