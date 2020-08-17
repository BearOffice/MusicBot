using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NAudio.Wave;
using MusicBear.Core;
using MusicBear.Assistor;

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

        public async Task JoinAsync(IVoiceChannel voiceChannel, IMessageChannel channel)
        {
            if (voiceChannel == null)
            {
                await channel.SendMessageAsync("<Mention> __User must be in a voice channel__");
                return;
            }
            var guildId = voiceChannel.Guild.Id;
            if (_container.TryGetValue(guildId, out _)) { return; }

            var Container = new AudioContainer
            {
                AudioClient = await voiceChannel.ConnectAsync(),
                CancellationTokenSource = new CancellationTokenSource(),
                QueueManager = new QueueManager(),
            };
            Container.AudioOutStream = Container.AudioClient.CreatePCMStream(AudioApplication.Music, bitrate: 128000);
            _container.TryAdd(guildId, Container);
        }

        public async Task AddAsync(IGuild guild, IMessageChannel channel, string path, bool isNext)  // Add single song
        {
            if (!_container.TryGetValue(guild.Id, out _))
            {
                await channel.SendMessageAsync($"<Mention> __Bot has not joined to audio channel yet__\nUse command {Config.Prefix}join first");
                return;
            }
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync($"<Exception> __Cannot find the file specified__");
                return;
            }

            _container.TryGetValue(guild.Id, out AudioContainer container);
            var queue = container.QueueManager;
            var remaining = queue.IfRemaining;

            if (isNext) queue.AddTo(path, 1);     // Add the song to the top of the queue
            else queue.Add(path);

            if (!remaining)
                await LoopAsync(guild, channel);
            else
                await channel.SendMessageAsync($"`Added  {path}`");
        }

        public async Task AddAsync(IGuild guild, IMessageChannel channel, string playlistName)   // Add Playlist
        {
            if (!_container.TryGetValue(guild.Id, out _))
            {
                await channel.SendMessageAsync($"<Mention> __Bot has not joined to audio channel yet__\nUse command {Config.Prefix}join first");
                return;
            }
            if (!Playlist.List.TryGetValue($"{playlistName}.txt", out List<string> paths))   // Add file extension ".txt"
            {
                await channel.SendMessageAsync($"<Exception> __Cannot find the playlist specified__\nUse command {Config.Prefix}showplaylists to confirm the avaliable playlists");
                return;
            }

            _container.TryGetValue(guild.Id, out AudioContainer container);
            var queue = container.QueueManager;
            var remaining = queue.IfRemaining;
            var ex = 0;        // Return while no file exists

            foreach (var path in paths)
            {
                if (File.Exists(path)) queue.Add(path);
                else ex++;
            }
            if (ex == paths.Count)
            {
                await channel.SendMessageAsync($"<Exception> __Cannot find the files specified__");
                return;
            }
            else if (ex > 0)
            {
                await channel.SendMessageAsync($"<Mention> __Some files have been skipped because these files cannot be specified__");
            }

            await channel.SendMessageAsync($"`Added  {playlistName}`");

            if (!remaining) 
                await LoopAsync(guild, channel);
        }

        private async Task LoopAsync(IGuild guild, IMessageChannel channel)
        {
            _container.TryGetValue(guild.Id, out AudioContainer container);
            var queue = container.QueueManager;
            while (queue.IfRemaining)
            {
                var tagfile = TagLib.File.Create(queue.NowPlaying);   // Get the audio file's title
                var title = tagfile.Tag.Title;
                await channel.SendMessageAsync($"`Now playing  {title}`");
                if (Config.SonginStatus)
                    if (_container.Count == 1) await _discord.SetGameAsync(title); // Prevent collisions when the bot connects to more than one voice channel

                await SendAsync(guild, queue.NowPlaying);

                Thread.Sleep(2000);
                queue.UpdatePlaying();
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

        public Task SKip(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                container.CancellationTokenSource.Cancel();
                channel.SendMessageAsync($"<Mention> __Music skipped__");
            }
            else
            {
                channel.SendMessageAsync($"<Mention> __Nothing is playing now__");
            }
            return Task.CompletedTask;
        }

        public Task GetNowPlaying(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                if (container.QueueManager.IfRemaining && container.ResamplerDmoStream != null)
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

                    var tagfile = TagLib.File.Create(container.QueueManager.NowPlaying);   // Get the audio file's title
                    var title = tagfile.Tag.Title;

                    channel.SendMessageAsync($"`Now Playing  {title}`\n{bar}");
                }
            }
            else
                channel.SendMessageAsync($"<Mention> __Nothing is playing now__");
            return Task.CompletedTask;
        }

        public Task GetQueue(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                var contents = container.QueueManager.GetRestQueue();
                if (contents == "")
                    channel.SendMessageAsync($"<Mention> __Queue is empty__");
                else
                    channel.SendMessageAsync($">>> Music Queue\n```{contents}```");
            }
            else
                channel.SendMessageAsync($"<Mention> __Queue is empty__");
            return Task.CompletedTask;
        }

        public Task QueueOp(IGuild guild, IMessageChannel channel, OpType opType, bool hasPos, int pos = 0)
        {
            if (!_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                channel.SendMessageAsync($"<Mention> __Queue is empty__");
                return Task.CompletedTask;
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
                    channel.SendMessageAsync($"<Mention> __Operation failed__");
                else
                    channel.SendMessageAsync($"<Mention> __Queue is empty__");
            }
            else
            {
                channel.SendMessageAsync($"<Mention> __Operation succeeded__");
            }
            return Task.CompletedTask;
        }

        public enum OpType
        {
            MoveToNext,
            Shuffle,
            Remove,
            RemoveAll,
        }

        public Task Stop(IGuild guild, IMessageChannel channel)
        {
            if (_container.TryGetValue(guild.Id, out AudioContainer container))
            {
                container.CancellationTokenSource.Cancel();
                container.QueueManager.DeleteAll();
                channel.SendMessageAsync($"<Mention> __Music stopped__");
            }
            else
            {
                channel.SendMessageAsync($"<Mention> __Nothing is playing now__");
            }
            return Task.CompletedTask;
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


