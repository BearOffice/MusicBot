using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MusicBear.Core;

namespace MusicBear.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);  // DI
        }

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage is not SocketUserMessage message) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasCharPrefix(Config.Prefix, ref argPos)
                || message.HasMentionPrefix(_discord.CurrentUser, ref argPos)
                || message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_discord, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> command, 
            ICommandContext context, IResult result)
        {
            if (!command.IsSpecified) return;

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync($"<{result.Error}> __{result.ErrorReason}__");
        }
    }
}
