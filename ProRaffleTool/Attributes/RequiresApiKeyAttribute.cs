using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Probot.ProRaffleTool.Options;
using Probot.Shared.Dtos;
using Probot.Shared.Enums;

namespace Probot.ProRaffleTool.Attributes;

[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
internal class RequiresApiKeyAttribute : Attribute, IAuthorizationFilter
{ 
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        if(IsAllowAnonymous(httpContext.GetEndpoint()))
        {
            return;
        }

        var proRaffleApiSettings = httpContext.RequestServices.GetRequiredService<IOptions<ProRaffleApiSettings>>().Value;
        if (!httpContext.Request.Headers.TryGetValue(proRaffleApiSettings.ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 401,
                Content = JsonSerializer.Serialize(new ErrorResponse
                {
                    ExceptionResult = ExceptionResult.Unauthorized401,
                    ErrorMessage = "API Key was not provided."
                }),
                ContentType = "application/json"
            };
            return;
        }

        if (!proRaffleApiSettings.ApiKey.Equals(extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 401,
                Content = JsonSerializer.Serialize(new ErrorResponse
                {
                    ExceptionResult = ExceptionResult.Unauthorized401,
                    ErrorMessage = "Unauthorized access."
                }),
                ContentType = "application/json"
            };
            return;
        }
    }
    private static bool IsAllowAnonymous(Endpoint? endpoint)
    {
        return endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;
    }

}
