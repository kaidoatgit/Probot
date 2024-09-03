using Probot.Data;
using Probot.ProRaffleTool.Clients;
using Probot.ProRaffleTool.Options;
using Probot.ProRaffleTool.Services.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProRaffleSettings>(builder.Configuration.GetSection("ProRaffleSettings"));
builder.Services.AddProbotContext(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<AlphabotClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.alphabot.app/v1/");
});
builder.Services.AddHostedService<RaffleRegistrationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
