
namespace mstsc.Aries.Antis
{
sealed class SysInternals
    {
        public SysInternals()
        {
            if (new CheckProcess().IsProcessRunning("Sysinternals: www.sysinternals.com"))
            {
                new AntiFound();
            }
            //System.GC.Collect();
        }
    }
}
