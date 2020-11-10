using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicBear.Assistor
{
    public static class PlaylistInfo
    {
        private static Dictionary<string, List<string>> _list;
        public static Dictionary<string, List<string>> List 
        {
            get
            {
                Refresh();
                return _list;
            }
        }

        private static void Refresh()
        {
            _list = new Dictionary<string, List<string>>();
            var path = Path.Combine(Environment.CurrentDirectory, "Playlist");
            if (Directory.Exists(path))
            {
                var di = new DirectoryInfo(path);
                di.EnumerateFiles("*.txt", SearchOption.AllDirectories)
                    .ToList().ForEach(name => _list.Add(name.Name, File.ReadLines(name.FullName).ToList()));
            }
        }
    }
}
