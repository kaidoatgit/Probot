using ProPayments.Service.Dtos;
using ProPayments.Service.Exceptions;
using System.Text.Json;

namespace ProPayments.Service.Middleware
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

            var response = new MetaData();
            switch (exception)
            {
                case ServiceException ex:
                    _logger.LogInformation(exception.Message);
                    context.Response.StatusCode = ex.StatusCode;
                    response.StatusCode = ex.StatusCode;
                    response.ErrorMessage = ex.Message;
                    break;
                default:
                    _logger.LogError(exception, exception.Message);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.ErrorMessage = "Internal Server Error";
                    break;
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
