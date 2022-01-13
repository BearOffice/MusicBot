using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicBear.Core
{
    public static class AppControl
    {
        public static void Exit()
        {
            Task.Delay(1000).Wait();
            MessageHandler.Service.Add("Message", "This application will be closed automatically in 15sec.");
            Task.Delay(15000).Wait();
            Environment.Exit(0);
        }
    }
}
