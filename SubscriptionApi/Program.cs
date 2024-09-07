using Microsoft.OpenApi.Models;
using Probot.SubscriptionApi.Clients;
using Probot.SubscriptionApi.Clients.IClients;
using Probot.SubscriptionApi.Options;
using Probot.Data;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Middleware;
using Probot.SubscriptionApi.Services.BackgroundServices;
using Probot.SubscriptionApi.Services.BackgroundServices.Helpers;
using Probot.SubscriptionApi.Services.BackgroundServices.IServices;
using Probot.SubscriptionApi.Services.Hubs;
using Probot.SubscriptionApi.Services.Services;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.SubscriptionApi.Services.Shared;
using Probot.SubscriptionApi.Services.Shared.IShared;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSubTypes;
using Probot.Shared.Dtos.ProductSetting.Request;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Dtos.ProductSetting.Response;
using Probot.Shared.Dtos.ProRaffleSetting.Response;

var builder = WebApplication.CreateBuilder(args);
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole(); // Adds Console logging
// builder.Logging.AddDebug(); // Adds Debug logging

builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings"));
builder.Services.AddProbotContext(builder.Configuration);
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };
});
builder.Services.AddMemoryCache();

#region external services
builder.Services.AddHttpClient<ICoingeckoClient, CoingeckoClient>(opt =>
{
    opt.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
});
builder.Services.AddHttpClient<ISolanaRpcClient, SolanaRpcClient>(opt =>
{
    opt.BaseAddress = new Uri("https://api.mainnet-beta.solana.com/");
});
#endregion

#region background services
builder.Services.AddHostedService<SubscriptionCheckService>();
builder.Services.AddScoped<IOrderMonitorService, OrderService>();
builder.Services.AddSingleton<IOrderQueueService, OrderQueueService>();
builder.Services.AddSingleton<IMonitorService, MonitorService>();
builder.Services.AddHostedService<OrderMonitorService>();
builder.Services.AddHostedService<TransactionMonitorService>();
#endregion

#region subscriptions service
builder.Services.AddSingleton<Mapper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IProductKeyService, ProductKeyService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProRaffleSettingService, ProRaffleSettingService>();
#endregion

builder.Services.AddControllers(options =>
{
    // Prevent trimming of the "Async" suffix from action names
    options.SuppressAsyncSuffixInActionNames = false;
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(JsonSubtypesConverterBuilder
        .Of<ProductSettingRequest>("Discriminator")
        .RegisterSubtype<ProRaffleSettingRequest>(nameof(ProRaffleSettingRequest))
        .SerializeDiscriminatorProperty()
        .Build());
    options.SerializerSettings.Converters.Add(JsonSubtypesConverterBuilder
        .Of<ProductSettingResponse>("Discriminator")
        .RegisterSubtype<ProRaffleSettingResponse>(nameof(ProRaffleSettingResponse))
        .SerializeDiscriminatorProperty()
        .Build());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SubscriptionService", Version = "v1" });
});


var app = builder.Build();
await app.SeedDatabaseAsync();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SubscriptionService v1"));
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationhub");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();
