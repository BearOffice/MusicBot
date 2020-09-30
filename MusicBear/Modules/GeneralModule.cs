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
            await ReplyAsync("<Mention> __Set game succeeded__");
        }

        [Command("setstatus")]
        public async Task StatusAsync(string status)
        {
            try
            {
                UserStatus userstatus = status switch
                {
                    "online" => UserStatus.Online,
                    "idle" => UserStatus.Idle,
                    "donotdisturb" => UserStatus.DoNotDisturb,
                    "invisible" => UserStatus.Invisible,
                    _ => throw new Exception("<Mention> __The value is not valid__\n Only 'Online' 'Idle' 'DoNotDisturb' 'Invisible' can be set"),
                };
                await ReplyAsync("<Mention> __Set status succeeded__");
            }
            catch(Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
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
            await ReplyAsync("See you next time");
            await Context.Client.StopAsync();
            AppControl.Exit();
        }
    }
}
