using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errorResponse) = CreateErrorResponse(exception);
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private (int StatusCode, object Response) CreateErrorResponse(Exception exception)
    {
        return exception switch
        {
            NotFoundException nf => (404, new
            {
                Code = 404,
                Message = nf.Message
            }),

            ConflictException cf => (409, new
            {
                Code = 409,
                Message = cf.Message,
                Details = cf.Details
            }),

            ValidationException ve => (400, new
            {
                Code = 400,
                Message = "Validation failed",
                Errors = ve.Errors
            }),

            BadRequestException br => (400, new
            {
                Code = 400,
                Message = br.Message
            }),
            _ => (500, new
            {
                Code = 500,
                Message = "An unexpected error occurred.",
                // NEW: Grab the InnerException message if it exists
                Details = exception.InnerException?.Message ?? exception.Message 
            })

        };
    }
}