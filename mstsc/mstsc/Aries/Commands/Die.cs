using System;

namespace mstsc.Aries.Commands
{
sealed class Die
    {
        public Die()
        {
            Main.Instance.IRCClient.MessageCurrentChannel("{Die} Master requested me to die");
            Main.Instance.IRCClient.Quit(" ");
            Main.Instance.IRCClient.TCPStream.Close();
            Main.Instance.IRCClient.Writer.Close();
            Main.Instance.IRCClient.Reader.Close();
            Environment.Exit(-1);
        }
    }
}
