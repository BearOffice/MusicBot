using System;
using System.Threading.Tasks;

namespace MusicBear.Core
{
    public class MessageHandler
    {
        public static MessageHandler Service { get; }
        public event Func<MessageEventArgs, Task> Message;

        static MessageHandler()
        {
            Service = new MessageHandler();
        }

        private MessageHandler() { }

        public void AddMessage(string source, string message) => Message?.Invoke(new MessageEventArgs(source, message));
    }

    public class MessageEventArgs
    {
        public string Source { get; private set; }
        public string Message { get; private set; }

        public MessageEventArgs(string source, string message)
        {
            this.Source = source;
            this.Message = message;
        }
    }
}
