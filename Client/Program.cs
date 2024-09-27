using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Probot.Client.Clients.SubscriptionApi;
using Probot.Client.Mappers;
using Probot.Client.Managers;
using Newtonsoft.Json;
using JsonSubTypes;
using Probot.Shared.Dtos.ProductSetting.Request;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Dtos.ProductSetting.Response;
using Probot.Shared.Dtos.ProRaffleSetting.Response;
using Probot.Client.Clients.ProRaffleApi;
using Probot.Client.Options;
using Microsoft.Extensions.Options;

namespace Probot.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var services = ConfigureServices();
                var bot = services.GetRequiredService<Probot>();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    bot.Stop();
                };
                await bot.RunAsync(services);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

            var services = new ServiceCollection();
            services.Configure<ProbotSettings>(configuration.GetSection("ProbotSettings"));
            services.AddHttpClient<UserClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/users/"); });
            services.AddHttpClient<ProductClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/products/"); });
            services.AddHttpClient<OrderClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/orders/"); });
            services.AddHttpClient<ProductKeyClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/product_keys/"); });
            services.AddHttpClient<SubscriptionClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/subscriptions/"); });
            services.AddHttpClient<ProRaffleSettingClient>((serviceProvider, opt) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<ProbotSettings>>().Value;
                opt.BaseAddress = new Uri("http://localhost:7091/api/pro_raffle_settings/");
                opt.DefaultRequestHeaders.Add("X-API-KEY", settings.ProRaffleApiKey);
            });
            services.AddHttpClient<OAuthClient>((serviceProvider, opt) => 
            {                 
                var settings = serviceProvider.GetRequiredService<IOptions<ProbotSettings>>().Value;
                opt.BaseAddress = new Uri("http://localhost:7091/api/oauth2/discord/");
                opt.DefaultRequestHeaders.Add("X-API-KEY", settings.ProRaffleApiKey);
            });
            services.AddSingleton<CancelationTokenManager>();
            services.AddSingleton<OrderManager>();
            services.AddSingleton<ProductManager>();
            services.AddSingleton<UserManager>();
            services.AddSingleton<CartManager>();
            services.AddSingleton<ProRaffleSettingManager>();
            services.AddSingleton<Mapper>();
            services.AddSingleton<HubManager>();
            services.AddSingleton(provider =>
            {
                var settings = new JsonSerializerSettings();
                
                settings.Converters.Add(JsonSubtypesConverterBuilder
                    .Of<ProductSettingRequest>("Discriminator")
                    .RegisterSubtype<ProRaffleSettingRequest>(nameof(ProRaffleSettingRequest))
                    .SerializeDiscriminatorProperty()
                    .Build());

                settings.Converters.Add(JsonSubtypesConverterBuilder
                    .Of<ProductSettingResponse>("Discriminator")
                    .RegisterSubtype<ProRaffleSettingResponse>(nameof(ProRaffleSettingResponse))
                    .SerializeDiscriminatorProperty()
                    .Build());

                return settings;
            });
            services.AddSingleton<Probot>();
            // services.AddLogging(configure => configure.AddConsole().AddDebug());
            return services.BuildServiceProvider();
        }
    }
}