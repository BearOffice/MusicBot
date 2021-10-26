using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using MusicBear.Services;

namespace MusicBear.Core
{
    public class EntryPoint
    {
        public async Task EntryAsync()
        {
            using var services = ConfigureServices();

            MessageHandler.Service.Message += MessageAsync;
            MessageHandler.Service.AddMessage("Message", "Initializing");

            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += MessageAsync;
            services.GetRequiredService<CommandService>().Log += MessageAsync;

            try
            {
                await client.LoginAsync(TokenType.Bot, Config.Token);
                await client.StartAsync();
            }
            catch (Exception ex)
            {
                MessageHandler.Service.AddMessage("Discord", ex.Message);
                AppControl.Exit();
            }

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            // initialize game and status
            await client.SetGameAsync(Config.Game);
            await client.SetStatusAsync(Config.Status);

            await Task.Delay(Timeout.Infinite);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<AudioServices>()
                .BuildServiceProvider();
        }

        private Task MessageAsync(LogMessage msg) =>
            Task.Run(() => MessageWritter(msg.Source, msg.Message));

        private Task MessageAsync(MessageEventArgs msg) =>
            Task.Run(() => MessageWritter(msg.Source, msg.Message));

        private void MessageWritter(string source, string message)
        {
            var messagenow = string.Format("{0}  {1}", DateTime.Now.ToString("T"), source);
            Console.WriteLine($"{messagenow,-20}{message}");
        }
    }
}
