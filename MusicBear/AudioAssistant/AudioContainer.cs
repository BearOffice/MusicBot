using System.Threading;
using Discord.Audio;
using NAudio.Wave;

namespace MusicBear.AudioAssistant
{
    public class AudioContainer
    {
        public IAudioClient AudioClient { get; set; }
        public AudioOutStream AudioOutStream { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public QueueManager QueueManager { get; set; }
        public ResamplerDmoStream ResamplerDmoStream { get; set; }
    }
}
