using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using Discord;
using MusicBear.Assistor;

namespace MusicBear.Core
{
    static class Config
    {
        public static string Token { get; }
        public static char Prefix { get; }
        public static string Game { get; set; }
        public static bool SonginStatus { get; }
        public static string HelpText { get; }
        public static UserStatus Status { get; }

        static Config()
        {
            try
            {
                HelpText = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Help.txt"));
                new Playlist();

                var xdoc = XDocument.Load(Path.Combine(Environment.CurrentDirectory, "AppConfig.xml"));
                var pairs = xdoc.Root.Elements()
                    .Select(x => new
                    {
                        Key = x.Name.LocalName,
                        Value = x.Value,
                    });
                var dict = pairs.ToDictionary(x => x.Key, x => x.Value);

                Token = dict["token"];

                if (dict["prefix"].Length == 1)
                    Prefix = Convert.ToChar(dict["prefix"]);
                else
                    throw new Exception("Prefix setting in \"AppCofig.xml\" should be only one character");

                Game = dict["game"];

                var songinstatus = dict["songinstatus"];
                if (String.Compare(songinstatus, "True", ignoreCase: true) == 0)
                    SonginStatus = true;
                else if (String.Compare(songinstatus, "False", ignoreCase: true) == 0)
                    SonginStatus = false;
                else
                    throw new Exception("SonginStatus setting in \"AppCofig.xml\" isn't correct");

                var status = dict["status"];

                Status = status.ToLower() switch
                {
                    "online" => UserStatus.Online,
                    "idle" => UserStatus.Idle,
                    "donotdisturb" => UserStatus.DoNotDisturb,
                    "invisible" => UserStatus.Invisible,
                    _ => throw new Exception("Status setting in \"AppCofig.xml\" isn't correct")
                };      
            }
            catch (Exception ex)
            {
                MessageHandler.Service.AddMessage("Message", ex.Message);
                AppControl.Exit();
            }
        }
    }
}
