using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using BearMLLib;
using BearMLLib.Serialization.Conversion;

namespace MusicBear.Core
{
    public static class Config
    {
        public static string Token { get; }
        public static char Prefix { get; }
        public static string Game { get; set; }
        public static bool SonginStatus { get; }
        public static UserStatus Status { get; }
        public static string HelpText { get; }

        static Config()
        {
            try
            {
                var provider = new ConversionProvider(typeof(UserStatus),
                    str => str.ToLower() switch
                    {
                        "online" => UserStatus.Online,
                        "idle" => UserStatus.Idle,
                        "donotdisturb" => UserStatus.DoNotDisturb,
                        "invisible" => UserStatus.Invisible,
                        _ => throw new Exception("Incorrect status format.")
                    },
                    null);

                var reader = new BearML("config.txt", new[] { provider });
                
                Token = reader.GetContent<string>("token");
                Prefix = reader.GetContent<char>("prefix");
                Game = reader.GetContent<string>("game");
                SonginStatus = reader.GetContent<bool>("songinstatus");
                Status = reader.GetContent<UserStatus>("status");
                HelpText = reader.GetContent<string>("help");
                if (string.IsNullOrEmpty(HelpText)) HelpText = "Help message is empty.";
            }
            catch (Exception ex)
            {
                MessageHandler.Service.Add("Message", ex.Message);
                AppControl.Exit();
            }
        }
    }
}
