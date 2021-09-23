using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicBear.AudioAssistant
{
    public static class Playlist
    {
        private static Dictionary<string, List<string>> _dic;

        public static Dictionary<string, List<string>> Info
        {
            get 
            {
                Update();
                return _dic;
            }
        }

        private static void Update()
        {
            _dic = new Dictionary<string, List<string>>();

            var path = Path.Combine(Environment.CurrentDirectory, "Playlist");

            if (Directory.Exists(path))
            {
                var dirinfo = new DirectoryInfo(path);
                var filesinfo = dirinfo.EnumerateFiles("*.txt", SearchOption.AllDirectories);
                
                foreach(var fileinfo in filesinfo)
                {
                    _dic.Add(fileinfo.Name, File.ReadLines(fileinfo.FullName).ToList());
                }
            }
        }
    }
}
