using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace Api;

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
            Console.WriteLine("Поймано исключение");//Залогировать потом нормально с параметрами

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = ex switch
        {
            InvalidCredentialsException =>
                (HttpStatusCode.Unauthorized, ex.Message),

            UserNotFoundException =>
                (HttpStatusCode.NotFound, ex.Message),

            _ =>
                (HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<string>.Fail(message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
