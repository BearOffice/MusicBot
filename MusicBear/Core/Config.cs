using System;
using System.IO;
using Discord;
using ConfReaderLib;

namespace MusicBear.Core
{
    public static class Config
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
                // load help message
                if(File.Exists("Help.txt"))
                    HelpText = File.ReadAllText("Help.txt");
                else
                    HelpText = "Help message does not exist";

                // load config
                var reader = new ConfReader("AppConfig.conf");

                // token
                Token = reader.GetValue("token");

                // prefix
                var prefix = reader.GetValue("prefix");
                if (prefix.Length == 1)
                    Prefix = Convert.ToChar(prefix);
                else
                    throw new Exception("Prefix setting in \"AppConfig.conf\" should be only one character");

                // game
                Game = reader.GetValue("game");

                // songinstatus
                var songinstatus = reader.GetValue("songinstatus");
                if (string.Compare(songinstatus, "True", ignoreCase: true) == 0)
                    SonginStatus = true;
                else if (string.Compare(songinstatus, "False", ignoreCase: true) == 0)
                    SonginStatus = false;
                else
                    throw new Exception("SonginStatus setting in \"AppCofig.xml\" isn't correct");

                // status
                var status = reader.GetValue("status");
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
