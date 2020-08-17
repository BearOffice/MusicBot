using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicBear.Assistor
{
    public class QueueManager
    {
        // _playlist[0] is reserved by nowplaying
        // "Real" queue start from _playlist[1]
        private readonly List<string> _playlist = new List<string>();
        public bool IfRemaining { get => !(NowPlaying == ""); }
        public string NowPlaying
        {
            get
            {
                if (_playlist.Count == 0)
                    return "";
                else
                    return _playlist[0];
            }
        }
        public int RestNumber
        {
            get
            {
                if (IsValid())
                    return _playlist.Count() - 1;
                return 0;
            }
        }

        public void Add(string name) => _playlist.Add(name);

        public bool AddTo(string name, int pos)
        {
            if (!IsValid(pos)) { return false; }
            _playlist.Insert(pos, name);
            return true;
        }

        public bool MoveToTop(int pos)
        {
            if (!IsValid(pos)) { return false; }
            var moveitem = _playlist[pos];
            _playlist.RemoveAt(pos);
            _playlist.Insert(1, moveitem);
            return true;
        }

        public bool Shuffle()
        {
            if (!IsValid()) { return false; }
            for (int i = 1; i < _playlist.Count; i++)  //Linq cannot be used because _playlist is readonly
            {
                string temp = _playlist[i];
                int random = new Random().Next(1, _playlist.Count);
                _playlist[i] = _playlist[random];
                _playlist[random] = temp;
            }
            return true;
        }

        public bool Delete(int pos)
        {
            if (!IsValid(pos)) { return false; }
            _playlist.RemoveAt(pos);
            return true;
        }

        public bool DeleteAll()
        {
            if (!IsValid()) { return false; }
            var nowplaying = _playlist[0];
            _playlist.Clear();
            _playlist.Add(nowplaying);
            return true;
        }

        public void UpdatePlaying()
        {
            if (_playlist.Count != 0)
            {
                _playlist.RemoveAt(0);
            }
        }

        public string GetRestQueue()
        {
            var queue = "";
            if (IsValid())
            {
                for (int i = 1; i < _playlist.Count; i++)
                {
                    queue += $"{i}.".PadRight(4) + $"{_playlist[i]}\n";
                    if (i == 20)
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
            if (1 <= pos && pos < _playlist.Count) { return true; }
            return false;
        }

        private bool IsValid()
        {
            if (_playlist.Count > 1) { return true; }
            return false;
        }
    }
}
