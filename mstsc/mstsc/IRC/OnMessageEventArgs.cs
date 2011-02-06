
namespace mstsc.IRC
{
sealed class OnMessageEventArgs
    {
        public Sender Sender;
        public string Message;

        public OnMessageEventArgs(string Message, Sender Sender)
        {
            this.Message = Message;
            this.Sender = Sender;
        }
    }
}
