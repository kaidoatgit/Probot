using Probot.Shared.Enums;

namespace Probot.Shared.Helpers;

public static class ExceptionHelper
{
    public static int MapToStatusCode(this ExceptionResult exceptionResult)
    {
        int statusCode = 200;
        switch(exceptionResult)
        {
            case ExceptionResult.NotModified304:
                statusCode = 304;
                break;
            case ExceptionResult.OrderBadRequest400:
            case ExceptionResult.InvoiceBadRequest400:
            case ExceptionResult.ProductKeyBadRequest400:
            case ExceptionResult.ProductSettingBadRequest400:
                statusCode = 400;
                break;
            case ExceptionResult.Unauthorized401:
                statusCode = 401;
                break;
            case ExceptionResult.Forbidden403:
                statusCode = 403;
                break;
            case ExceptionResult.ProductNotFound404:
            case ExceptionResult.ProductOptionNotFound404:
            case ExceptionResult.UserNotFound404:
            case ExceptionResult.OrderNotFound404:
            case ExceptionResult.InvoiceNotFound404:
            case ExceptionResult.ProductKeyNotFound404:
            case ExceptionResult.SubscriptionNotFound404:
            case ExceptionResult.ProductSettingNotFound404:
            case ExceptionResult.ProRaffleSettingNotFound404:
                statusCode = 404;
                break;
            case ExceptionResult.UserConflict409:
            case ExceptionResult.UserAddressConflict409:
            case ExceptionResult.ProductKeyConflict409:
            case ExceptionResult.SubscriptionConflict409:
            case ExceptionResult.ProductSettingConflict409:
            case ExceptionResult.ProRaffleSettingConflict409:
                statusCode = 409;
                break;
            case ExceptionResult.FailedDependency424:
                statusCode = 424;
                break;
            case ExceptionResult.InternalServerError500:
                statusCode = 500;
                break;
            case ExceptionResult.SolanaRpcBadGateway502:
                statusCode = 502;
                break;
        };
        return statusCode;
    }
}
