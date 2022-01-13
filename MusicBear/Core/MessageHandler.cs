using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicBear.Core
{
    public class MessageHandler
    {
        public static MessageHandler Service { get; } = new MessageHandler();
        public event Func<MessageEventArgs, Task> Message;

        private MessageHandler() { }

        public void Add(string source, string message) => Message?.Invoke(new MessageEventArgs(source, message));
    }

    public class MessageEventArgs
    {
        public string Source { get; }
        public string Message { get; }

        public MessageEventArgs(string source, string message)
        {
            Source = source;
            Message = message;
        }
    }
}
