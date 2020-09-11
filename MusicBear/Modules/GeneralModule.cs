using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MusicBear.Services;
using MusicBear.Core;
using MusicBear.Assistor;

namespace MusicBear.Modules
{
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        public AudioServices AudioServices { get; set; }

        [Command("setgame")]
        public async Task GameAsync([Remainder] string setgame)  // [Remainder] takes all arguments as one
        {
            Config.Game = setgame;
            await Context.Client.SetGameAsync(setgame);
            await ReplyAsync("<Mention> __Operation succeeded__");
        }

        [Command("setstatus")]
        public async Task StatusAsync(string setstatus)
        {
            switch (setstatus.ToLower())
            {
                case "online":
                    await Context.Client.SetStatusAsync(UserStatus.Online);
                    break;
                case "idle":
                    await Context.Client.SetStatusAsync(UserStatus.Idle);
                    break;
                case "donotdisturb":
                    await Context.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    break;
                case "invisible":
                    await Context.Client.SetStatusAsync(UserStatus.Invisible);
                    break;
                default:
                    await ReplyAsync("<Mention> __The value is not valid__\n Only 'Online' 'Idle' 'DoNotDisturb' 'Invisible' can be set");
                    return;
            }
            await ReplyAsync("<Mention> __Operation succeeded__");
        }

        [Command("help")]
        public Task HelpAsync()
            => ReplyAsync($">>> Help Message ```{Config.HelpText}```");

        [Command("ping")]
        public Task PingAsync()
            => ReplyAsync($"Current Ping  {Context.Client.Latency}ms");

        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;
            await ReplyAsync(user.ToString());
        }

        [Command("showplaylists")]
        [Alias("spl")]
        public async Task ShowPlaylistsAsync()
        {
            if (Playlist.List.Count == 0)
                await ReplyAsync("<Mention> __Playlist is empty__");
            else
            {
                var pl = String.Join(", ", Playlist.List.Keys.ToArray()).Replace(".txt", ""); // Remove file extension ".txt"
                await ReplyAsync($">>> Playlists\n```{pl}```");
            }
        }

        [Command("updateplaylists")]
        [Alias("upl")]
        public async Task UpdatePlaylistsAsync()
        {
            new Playlist();
            await ReplyAsync("<Mention> __Playlists updated__");
        }

        [RequireOwner]
        [Command("shutdown", RunMode = RunMode.Async)]
        [Alias("exit", "disconnect")]
        public async Task ExitAsync()
        {
            await AudioServices.LeaveAllAsync();
            await ReplyAsync("See you next time.");
            await Context.Client.StopAsync();
            AppControl.Exit();
        }
    }
}
