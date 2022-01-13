using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.Commands;
using MusicBear.Services;

namespace MusicBear.CommandModules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        public AudioServices AudioServices { get; set; }
        private const string _err = "This command must be ran from within a server";

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("join", RunMode = RunMode.Async)]
        [Alias("j")]
        public async Task JoinAsync()
        {
            await AudioServices.JoinAsync((Context.User as IGuildUser)?.VoiceChannel, Context.Channel);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        public async Task PlayAsync([Remainder] string item)
        {
            await AudioServices.AddAsync(Context.Guild, (Context.User as IGuildUser)?.VoiceChannel,
                           Context.Channel, item, isNext: false);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("playnext", RunMode = RunMode.Async)]
        [Alias("pn")]
        public async Task PlayNextAsync([Remainder] string item)
        {
            await AudioServices.AddAsync(Context.Guild, (Context.User as IGuildUser)?.VoiceChannel,
                           Context.Channel, item, isNext: true);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("movetonext")]
        [Alias("mn")]
        public async Task MoveToNextAsync(int pos)
        {
            await AudioServices.QMoveToTopAsync(Context.Guild, Context.Channel, pos);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("nowplaying")]
        [Alias("np")]
        public async Task NowPlayingAsync()
        {
            await AudioServices.GetNowPlayingAsync(Context.Guild, Context.Channel);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("skip")]
        [Alias("s")]
        public async Task SkipAsync()
        {
            await AudioServices.SkipAsync(Context.Guild, Context.Channel);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("queue")]
        [Alias("q")]
        public async Task QueueAsync()
        {
            await AudioServices.GetQueueAsync(Context.Guild, Context.Channel);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("shuffle")]
        [Alias("sf")]
        public async Task ShuffleAsync()
        {
            await AudioServices.QShuffleAsync(Context.Guild, Context.Channel);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("remove")]
        [Alias("rm")]
        public async Task RemoveAsync(int pos)
        {
            await AudioServices.QRemoveAsync(Context.Guild, Context.Channel, pos);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("removeall")]
        [Alias("rma")]
        public async Task RemoveAllAsync()
        {
            await AudioServices.QRemoveAllAsync(Context.Guild, Context.Channel);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("stop", RunMode = RunMode.Async)]
        public async Task StopAsync()
        {
            await AudioServices.StopAsync(Context.Guild, Context.Channel);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _err)]
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveAsync()
        {
            await AudioServices.LeaveAsync(Context.Guild, Context.Channel);
        }
    }
}
