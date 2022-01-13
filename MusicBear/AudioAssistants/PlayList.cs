using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicBear.AudioAssistants
{
    public static class PlayList
    {
        public static Dictionary<string, List<string>> Info
        {
            get 
            {
                var dic = new Dictionary<string, List<string>>();

                var path = Path.Combine(Environment.CurrentDirectory, "playlist");

                if (Directory.Exists(path))
                {
                    var dirinfo = new DirectoryInfo(path);
                    var filesinfo = dirinfo.EnumerateFiles("*.txt", SearchOption.AllDirectories);

                    foreach (var fileinfo in filesinfo)
                    {
                        dic.Add(fileinfo.Name, File.ReadLines(fileinfo.FullName).ToList());
                    }
                }

                return dic;
            }
        }
    }
}
