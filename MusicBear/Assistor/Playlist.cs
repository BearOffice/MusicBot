using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicBear.Assistor
{
    public class Playlist
    {
        public static Dictionary<string, List<string>> List { get; }

        static Playlist()
        {
            List = new Dictionary<string, List<string>>();
        }

        public Playlist()
        {
            List.Clear();
            var path = Path.Combine(Environment.CurrentDirectory, "Playlist");
            if (Directory.Exists(path))
            {
                var di = new DirectoryInfo(path);
                di.EnumerateFiles("*.txt", SearchOption.AllDirectories)
                    .ToList().ForEach(name => List.Add(name.Name, File.ReadLines(name.FullName).ToList()));
            }
        }
    }
}
