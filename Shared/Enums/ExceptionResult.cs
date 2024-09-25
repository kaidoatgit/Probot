namespace Probot.Shared.Enums;

public enum ExceptionResult
{
    InternalServerError500,
    NotModified304,

    ProductNotFound404,
    ProductOptionNotFound404,

    UserNotFound404,
    UserConflict409,
    UserAddressConflict409,
    
    OrderBadRequest400,
    OrderNotFound404,
    
    InvoiceBadRequest400,
    InvoiceNotFound404,

    ProductKeyBadRequest400,
    ProductKeyNotFound404,
    ProductKeyConflict409,

    SubscriptionNotFound404,
    SubscriptionConflict409,

    FailedDependency424,

    SolanaRpcBadGateway502,

    ProductSettingBadRequest400,
    ProductSettingNotFound404,
    ProductSettingConflict409,

    ProRaffleSettingConflict409,
    ProRaffleSettingNotFound404,
    Unauthorized401,
    Forbidden403,
}
