using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Configs;
using ProPayments.Client.Mappers;
using ProPayments.Client.Services.Managers;

namespace ProPayments.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var services = ConfigureServices();
                var bot = services.GetRequiredService<ProPayments>();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    bot.Stop();
                };
                await bot.RunAsync(services);

                //just to read the message
                // Console.ReadKey();
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
            .AddEnvironmentVariables()
            .Build();

            var services = new ServiceCollection();
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.AddHttpClient<UserClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/users/"); });
            services.AddHttpClient<ProductClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/products/"); });
            services.AddHttpClient<OrderClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/orders/"); });
            services.AddHttpClient<ProRaffleClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/pro_raffles/"); });
            services.AddHttpClient<SubscriptionClient>(opt => { opt.BaseAddress = new Uri("https://localhost:7240/api/subscriptions/"); });
            services.AddSingleton<CancelationTokenManager>();
            services.AddSingleton<DiscordManager>();
            services.AddSingleton<OrderManager>();
            services.AddSingleton<ProductManager>();
            services.AddSingleton<UserManager>();
            services.AddSingleton<CartManager>();
            services.AddSingleton<Mapper>();
            services.AddSingleton<HubManager>();
            //services.AddSingleton(provider =>
            //{
            //    var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;
            //    return new DiscordClient(new DiscordConfiguration
            //    {
            //        Token = appSettings.Token,
            //        TokenType = TokenType.Bot,
            //        Intents = DiscordIntents.All,
            //        MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Error,
            //        AutoReconnect = false
            //    });
            //});
            services.AddSingleton<ProPayments>();
            //services.AddLogging(configure => configure.AddConsole());
            return services.BuildServiceProvider();
        }
    }
}