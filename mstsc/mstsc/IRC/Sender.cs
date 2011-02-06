
namespace mstsc.IRC
{
sealed class Sender
    {
        public string Host;
        public string Nick;
        public Sender(string Nick, string Host)
        {
            this.Host = Host;
            this.Nick = Nick;
        }
    }
}
