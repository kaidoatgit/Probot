using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProPayments.Service.Clients;
using ProPayments.Service.Clients.IClients;
using ProPayments.Service.Config;
using ProPayments.Service.Data;
using ProPayments.Service.Extensions;
using ProPayments.Service.Mappers;
using ProPayments.Service.Middleware;
using ProPayments.Service.Services.BackgroundServices;
using ProPayments.Service.Services.BackgroundServices.Helpers;
using ProPayments.Service.Services.BackgroundServices.IServices;
using ProPayments.Service.Services.Hubs;
using ProPayments.Service.Services.Services;
using ProPayments.Service.Services.Services.IServices;
using ProPayments.Service.Services.Shared;
using ProPayments.Service.Services.Shared.IShared;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services.Configure<ServiceConfiguration>(builder.Configuration.GetSection("ServiceConfiguration"));
builder.Services.AddDbContext<SubscriptionContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SubscriptionDatabase")));
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true,
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

#region pro payments service
builder.Services.AddSingleton<Mapper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISubscriptionService, ProPayments.Service.Services.Services.SubscriptionService>();
builder.Services.AddScoped<IProductKeyService, ProductKeyService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProRaffleService, ProRaffleService>();
builder.Services.AddScoped<IUserSettingService, UserSettingService>();
#endregion

builder.Services.AddControllers(options =>
{
    // Prevent trimming of the "Async" suffix from action names
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SubscriptionService", Version = "v1" });
});


var app = builder.Build();
await app.SeedDatabaseAsync();
app.UseMiddleware<ExceptionHandlingMiddleware>();
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
app.Run();
