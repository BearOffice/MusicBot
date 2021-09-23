using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicBear.AudioAssistant
{
    public class QueueManager
    {
        private readonly List<string> _playlist = new List<string>();  // Contains music files' paths
        public bool IsPlaying { get => !(NowPlayingPath == ""); }
        public int RemainingNumber { get => _playlist.Count(); }
        public string NowPlayingPath { get; private set; } = "";
        public string NowPlaying { get; private set; } = "";
        public bool HasRest
        {
            get
            {
                if (_playlist.Count > 0) return true;
                return false;
            }
        }

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
            for (int i = 0; i < _playlist.Count; i++)  //Linq cannot be used because _playlist is readonly
            {
                string temp = _playlist[i];
                int random = new Random().Next(0, _playlist.Count);
                _playlist[i] = _playlist[random];
                _playlist[random] = temp;
            }
            return true;
        }

        public bool Delete(int pos)
        {
            if (!IsValid(pos)) return false;
            _playlist.RemoveAt(pos);
            return true;
        }

        public bool DeleteAll()
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
            else if (NowPlayingPath != "")
            {
                NowPlayingPath = "";
                NowPlaying = "";
            }
        }

        public void StartPlay()
        {
            if (NowPlayingPath == "")
                PlayNext();
        }

        public string GetRestQueue()
        {
            var queue = "";
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
            if (0 <= pos && pos < _playlist.Count) return true;
            return false;
        }

        private string GetTitle(string path)
        {
            var tagfile = TagLib.File.Create(path);   // Get audio file's title
            var title = tagfile.Tag.Title;
            if (title == "" || title == null)
                title = Path.GetFileName(path);
            return title;
        }
    }
}
