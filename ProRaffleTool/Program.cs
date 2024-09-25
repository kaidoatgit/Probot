using Probot.Data;
using Probot.ProRaffleTool.Clients;
using Probot.ProRaffleTool.Clients.Abstractions;
using Probot.ProRaffleTool.Mappers;
using Probot.ProRaffleTool.Options;
using Probot.ProRaffleTool.Services.BackgroundServices;
using Probot.ProRaffleTool.Services.Services;
using Probot.ProRaffleTool.Services.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProRaffleApiSettings>(builder.Configuration.GetSection("ProRaffleApiSettings"));
builder.Services.Configure<AlphabotSettings>(builder.Configuration.GetSection("AlphabotSettings"));
builder.Services.Configure<OAuthSettings>(builder.Configuration.GetSection("OAuthSettings"));
builder.Services.Configure<BackgroundServicesSettings>(builder.Configuration.GetSection("BackgroundServicesSettings"));
builder.Services.AddProbotContext(builder.Configuration);
builder.Services.AddMemoryCache();

#region External Services
builder.Services.AddHttpClient<AlphabotClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.alphabot.app/v1/");
});

builder.Services.AddHttpClient<IDiscordClient, DiscordClient>(opt =>
{
    opt.BaseAddress = new Uri("https://discord.com/api/v10/");
});
#endregion

builder.Services.AddSingleton<Mapper>();
builder.Services.AddScoped<IProRaffleSettingService, ProRaffleSettingService>();
builder.Services.AddHostedService<RaffleRegistrationService>();
builder.Services.AddSingleton<IRaffleRateLimiterService, RaffleRateLimiterService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProRaffleTool v1"));
// }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
