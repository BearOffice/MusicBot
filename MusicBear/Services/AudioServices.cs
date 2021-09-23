using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NAudio.Wave;
using MusicBear.Core;
using MusicBear.AudioAssistant;

namespace MusicBear.Services
{
    public class AudioServices
    {
        private readonly ConcurrentDictionary<ulong, AudioContainer> _container; // Allow bots to play music in multiple threads
        private readonly DiscordSocketClient _discord;

        public AudioServices(DiscordSocketClient discord)
        {
            _container = new ConcurrentDictionary<ulong, AudioContainer>();
            _discord = discord;
        }

        public async Task<bool> JoinAsync(IVoiceChannel voiceChannel, IMessageChannel channel)
        {
            if (voiceChannel == null)
            {
                await channel.SendMessageAsync("<Mention> __User must be in a voice channel__");
                return false;
            }
            var guildId = voiceChannel.Guild.Id;
            if (_container.TryGetValue(guildId, out _)) return false;

            var Container = new AudioContainer
            {
                AudioClient = await voiceChannel.ConnectAsync(),
                CancellationTokenSource = new CancellationTokenSource(),
                QueueManager = new QueueManager(),
            };
            Container.AudioOutStream = Container.AudioClient.CreatePCMStream(AudioApplication.Music, bitrate: 128000);
            _container.TryAdd(guildId, Container);

            return true;
        }

        public async Task AddAsync(IGuild guild, IVoiceChannel voiceChannel, IMessageChannel channel, string item, bool isNext)
        {
            if (!_container.TryGetValue(guild.Id, out _))   // may out null
            {
                if (!await JoinAsync(voiceChannel, channel)) return;
            }

            _container.TryGetValue(guild.Id, out AudioContainer container);

            if (Playlist.Info.TryGetValue($"{item}.txt", out _))   // Check if the item is playlist
                await AddListAsync(guild, channel, item, isNext);
            else if (File.Exists(item))
                await AddSingleAsync(guild, channel, item, isNext);
            else
                await channel.SendMessageAsync($"<Exception> __Cannot find the music file or playlist specified__\n" +
                    $"Check if the music file's path is correct " +
                    $"or use command {Config.Prefix}playlist to confirm the avaliable playlists");

            var queue = container.QueueManager;
            if (!queue.IsPlaying)
            {
                queue.StartPlay();
                await SendingLoopAsync(guild, channel);
            }
        }

        // Add single song
        private async Task AddSingleAsync(IGuild guild, IMessageChannel channel, string path, bool isNext)
        {
            _container.TryGetValue(guild.Id, out AudioContainer container);
            var queue = container.QueueManager;

            if (isNext)
                queue.AddTo(path, 0);     // Add the song to the top of the queue
            else
                queue.Add(path);

            await channel.SendMessageAsync($"`{path} added`");
        }

        // Add Playlist
        private async Task AddListAsync(IGuild guild, IMessageChannel channel, string playlistName, bool isNext)
        {
            Playlist.Info.TryGetValue($"{playlistName}.txt", out var paths);
            _container.TryGetValue(guild.Id, out AudioContainer container);
            var queue = container.QueueManager;
            var ex = 0;        // Count the files' amount that do not exist

            if (isNext)
            {
                paths.Reverse();
                paths.ForEach(path =>
                {
                    if (File.Exists(path)) queue.AddTo(path, 0);
                    else ex++;
                });
            }
            else
            {
                paths.ForEach(path =>
                {
                    if (File.Exists(path)) queue.Add(path);
                    else ex++;
                });
            }

            if (ex == paths.Count)
            {
                await channel.SendMessageAsync($"<Exception> __Cannot find any music files specified__\n" +
                    $"Check out the music files paths");
                return;
            }
            else if (ex > 0)
            {
                await channel.SendMessageAsync($"<Mention> __Some songs({ex}) have been skipped " +
                    $"because these music files cannot be specified__");
            }

            await channel.SendMessageAsync($"`{playlistName} added`");
        }

        private async Task SendingLoopAsync(IGuild guild, IMessageChannel channel)
        {
            _container.TryGetValue(guild.Id, out AudioContainer container);
            var queue = container.QueueManager;
            while (queue.IsPlaying)
            {
                await channel.SendMessageAsync($"`Now playing  {queue.NowPlaying}`");
                if (Config.SonginStatus && _container.Count == 1)  // Prevent collisions when the bot connects to more than one voice channel
                    await _discord.SetGameAsync(queue.NowPlaying);

                await SendAsync(guild, queue.NowPlayingPath);

                Thread.Sleep(2000);
                queue.PlayNext();
            }
            await _discord.SetGameAsync(Config.Game);
        }

        private async Task SendAsync(IGuild guild, string path)
        {
            _container.TryGetValue(guild.Id, out AudioContainer container);
            var audioOutStream = container.AudioOutStream;
            var token = container.CancellationTokenSource.Token;

            var format = new WaveFormat(48000, 16, 2);
            using var reader = new MediaFoundationReader(path);
            using var resamplerDmo = new ResamplerDmoStream(reader, format);
            try
            {
                container.ResamplerDmoStream = resamplerDmo;
                await resamplerDmo.CopyToAsync(audioOutStream, token)
                   .ContinueWith(t => { return; });
            }
            finally
            {
                await audioOutStream.FlushAsync();
                container.CancellationTokenSource = new CancellationTokenSource();
            }
        }

        public async Task SkipAsync(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                container.CancellationTokenSource.Cancel();
                await channel.SendMessageAsync($"<Mention> __Music skipped__");
            }
            else
            {
                await channel.SendMessageAsync($"<Mention> __Nothing is playing now__");
            }
        }

        public async Task GetNowPlayingAsync(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                if (container.QueueManager.IsPlaying && container.ResamplerDmoStream != null)
                {
                    var current = container.ResamplerDmoStream.CurrentTime;
                    var total = container.ResamplerDmoStream.TotalTime;
                    var diff = (int)(current.TotalSeconds / total.TotalSeconds * 15);   // Bar's length
                    var bar = ":arrow_forward: ";
                    for (int i = 0; i < 15; i++)
                    {
                        if (i == diff) bar += ":radio_button:";
                        else bar += "▬";
                    }
                    bar += $" [{current.ToString(@"mm\:ss")}/{total.ToString(@"mm\:ss")}]";

                    await channel.SendMessageAsync($"`Now Playing  {container.QueueManager.NowPlaying}`\n{bar}");
                }
            }
            else
                await channel.SendMessageAsync($"<Mention> __Nothing is playing now__");
        }

        public async Task GetQueueAsync(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                var contents = container.QueueManager.GetRestQueue();
                if (contents == "")
                    await channel.SendMessageAsync($"<Mention> __Queue is empty__");
                else
                    await channel.SendMessageAsync($">>> Music Queue\n```{contents}```");
            }
            else
                await channel.SendMessageAsync($"<Mention> __Queue is empty__");
        }

        public async Task QMoveToTopAsync(IGuild guild, IMessageChannel channel, int pos)
            => await QueueOpAsync(guild, channel, OpType.MoveToNext, hasPos: true, pos: pos);

        public async Task QShuffleAsync(IGuild guild, IMessageChannel channel)
            => await QueueOpAsync(guild, channel, OpType.Shuffle, hasPos: false);

        public async Task QRemoveAsync(IGuild guild, IMessageChannel channel, int pos)
            => await QueueOpAsync(guild, channel, OpType.Remove, hasPos: true, pos: pos);

        public async Task QRemoveAllAsync(IGuild guild, IMessageChannel channel)
            => await QueueOpAsync(guild, channel, OpType.RemoveAll, hasPos: false);

        private async Task<bool> IsQueueEmpty(IGuild guild, IMessageChannel channel)
        {
            if (!_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                await channel.SendMessageAsync($"<Mention> __Queue is empty__");
                return true;
            }

            return false;
        }

        private async Task QueueOpAsync(IGuild guild, IMessageChannel channel, OpType opType, bool hasPos, int pos = 0)
        {
            if (!_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                await channel.SendMessageAsync($"<Mention> __Queue is empty__");
                return;
            }

            var isSuccess = opType switch
            {
                OpType.MoveToNext => container.QueueManager.MoveToTop(pos),
                OpType.Shuffle => container.QueueManager.Shuffle(),
                OpType.Remove => container.QueueManager.Delete(pos),
                OpType.RemoveAll => container.QueueManager.DeleteAll(),
                _ => false,
            };

            if (!isSuccess)
            {
                if (hasPos)
                    await channel.SendMessageAsync($"<Mention> __The operation {opType} failed__");
                else
                    await channel.SendMessageAsync($"<Mention> __Queue is empty__");
            }
            else
            {
                await channel.SendMessageAsync($"<Mention> __{opType} succeeded__");
            }
        }

        private enum OpType
        {
            MoveToNext,
            Shuffle,
            Remove,
            RemoveAll,
        }

        public async Task StopAsync(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                container.CancellationTokenSource.Cancel();
                container.QueueManager.DeleteAll();
                await channel.SendMessageAsync($"<Mention> __Music stopped__");
            }
            else
            {
                await channel.SendMessageAsync($"<Mention> __Nothing is playing now__");
            }
        }

        public async Task LeaveAsync(IGuild guild, IMessageChannel channel)
        {
            if (!_container.TryRemove(guild.Id, out AudioContainer container))
            {
                await channel.SendMessageAsync($"<Mention> __Bot has not joined to audio channel yet__");
                return;
            }
            await channel.SendMessageAsync($"<Mention> __Bot disconnected from audio channel__");
            await container.AudioClient.StopAsync();
        }

        public async Task LeaveAllAsync()
        {
            foreach (var item in _container)
            {
                await item.Value.AudioClient.StopAsync();
            }
        }
    }
}
