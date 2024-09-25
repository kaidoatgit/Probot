using Probot.Shared.Dtos;
using System.Text.Json;
using Probot.Shared.Enums;
using Probot.ProRaffleTool.Exceptions;
using Probot.Shared.Helpers;

namespace Probot.ProRaffleTool.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();
            switch (exception)
            {
                case ProRaffleException ex:
                {
                    _logger.LogInformation(exception.Message);

                    response.ExceptionResult = ex.ExceptionResult;
                    response.ErrorMessage = ex.Message;

                    context.Response.StatusCode = ex.ExceptionResult.MapToStatusCode();
                    break;
                }
                default:
                {
                    _logger.LogError(exception, exception.Message);
                    
                    response.ExceptionResult = ExceptionResult.InternalServerError500;
                    response.ErrorMessage = "Internal Server Error";
                    
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
                }
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
