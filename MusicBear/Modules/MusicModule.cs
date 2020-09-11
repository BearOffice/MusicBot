using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MusicBear.Services;


namespace MusicBear.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        public AudioServices AudioServices { get; set; }
        private const string _errMsg = "This command must be ran from within a server";

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("join", RunMode = RunMode.Async)]
        [Alias("j")]
        public async Task JoinAsync()
            => await AudioServices.JoinAsync((Context.User as IGuildUser)?.VoiceChannel, Context.Channel);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        public async Task PlayAsync([Remainder] string path)
            => await AudioServices.AddAsync(Context.Guild, Context.Channel, path, isNext: false);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("playnext", RunMode = RunMode.Async)]
        [Alias("pn")]
        public async Task PlayNextAsync([Remainder] string path)
            => await AudioServices.AddAsync(Context.Guild, Context.Channel, path, isNext: true);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("playlist", RunMode = RunMode.Async)]
        [Alias("pl")]
        public async Task PlaylistAsync([Remainder] string playlistName)
            => await AudioServices.AddAsync(Context.Guild, Context.Channel, playlistName);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("movetonext")]
        [Alias("mvn")]
        public Task MoveToNextAsync(int pos)
            => AudioServices.QueueOp(Context.Guild, Context.Channel, AudioServices.OpType.MoveToNext, hasPos: true, pos);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("nowplaying")]
        [Alias("np")]
        public Task NowPlayingAsync()
            => AudioServices.GetNowPlaying(Context.Guild, Context.Channel);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("skip")]
        [Alias("s")]
        public Task SkipAsync()
            => AudioServices.SKip(Context.Guild, Context.Channel);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("queue")]
        [Alias("q")]
        public Task QueueAsync()
            => AudioServices.GetQueue(Context.Guild, Context.Channel);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("shuffle")]
        [Alias("sf")]
        public Task ShuffleAsync()
            => AudioServices.QueueOp(Context.Guild, Context.Channel, AudioServices.OpType.Shuffle, hasPos: false);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("remove")]
        [Alias("rm")]
        public Task RemoveAsync(int pos)
            => AudioServices.QueueOp(Context.Guild, Context.Channel, AudioServices.OpType.Remove, hasPos: true, pos);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("removeall")]
        [Alias("rma")]
        public Task RemoveAllAsync()
            => AudioServices.QueueOp(Context.Guild, Context.Channel, AudioServices.OpType.RemoveAll, hasPos: false);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("stop")]
        [Alias("sp")]
        public Task StopAsync()
            => AudioServices.Stop(Context.Guild, Context.Channel);

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("leave")]
        [Alias("l")]
        public async Task LeaveAsync()
            => await AudioServices.LeaveAsync(Context.Guild, Context.Channel);
    }
}
