using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicBear.AudioAssistants
{
    public class QueueManager
    {
        private readonly List<string> _playlist = new();  // Contains music files' paths
        public bool IsPlaying { get => !string.IsNullOrEmpty(NowPlayingPath); }
        public int RemainingNumber { get => _playlist.Count; }
        public string NowPlayingPath { get; private set; } = string.Empty;
        public string NowPlaying { get; private set; } = string.Empty;
        public bool HasRest { get => _playlist.Count > 0; }

        public void Add(string name)
        {
            _playlist.Add(name);
        }

        public bool AddTo(string name, int pos)
        {
            if (!IsValid(pos)) return false;
            _playlist.Insert(pos, name);
            return true;
        }

        public bool MoveToTop(int pos)
        {
            if (!IsValid(pos)) return false;
            var moveitem = _playlist[pos];
            _playlist.RemoveAt(pos);
            _playlist.Insert(0, moveitem);
            return true;
        }

        public bool Shuffle()
        {
            if (!HasRest) return false;
            for (int i = 0; i < _playlist.Count; i++)
            {
                string temp = _playlist[i];
                int random = new Random().Next(0, _playlist.Count);
                _playlist[i] = _playlist[random];
                _playlist[random] = temp;
            }
            return true;
        }

        public bool Remove(int pos)
        {
            if (!IsValid(pos)) return false;
            _playlist.RemoveAt(pos);
            return true;
        }

        public bool RemoveAll()
        {
            if (!HasRest) return false;
            _playlist.Clear();
            return true;
        }

        public void PlayNext()
        {
            if (HasRest)
            {
                var playitem = _playlist[0];
                NowPlayingPath = playitem;
                NowPlaying = GetTitle(playitem);
                _playlist.RemoveAt(0);
            }
            else if (!string.IsNullOrEmpty(NowPlayingPath))
            {
                NowPlayingPath = string.Empty;
                NowPlaying = string.Empty;
            }
        }

        public void StartPlay()
        {
            if (string.IsNullOrEmpty(NowPlayingPath)) PlayNext();
        }

        public string GetRestQueue()
        {
            var queue = string.Empty;
            if (HasRest)
            {
                for (int i = 0; i < _playlist.Count; i++)
                {
                    queue += $"{i}.".PadRight(4) + $"{GetTitle(_playlist[i])}\n";
                    if (i == 19)  // 20 elements totally
                    {
                        queue += "...\n";
                        break;
                    }
                }
            }
            return queue;
        }

        private bool IsValid(int pos)
        {
            return 0 <= pos && pos < _playlist.Count;
        }

        private static string GetTitle(string path)
        {
            var tagfile = TagLib.File.Create(path);   // Get audio file's title
            var title = tagfile.Tag.Title;
            if (string.IsNullOrEmpty(title))
                title = Path.GetFileName(path);
            return title;
        }
    }
}
