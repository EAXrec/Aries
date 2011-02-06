using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Aries.IconMethods
{
    //Icon extractor by - unknown
    public enum IconSize
    {
        Small,
        Large
    }

    sealed class IconMethod2
    {
        private const int SHGFI_ICON = 0x100;
        private const int SHGFI_SMALLICON = 1;
        private const int SHGFI_LARGEICON = 0;

        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            int iIcon;
            int dwAttributes;

            [VBFixedString(260), MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [VBFixedString(80), MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32", EntryPoint = "SHGetFileInfoA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SHGetFileInfo([MarshalAs(UnmanagedType.VBByRefStr)] ref string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int ByValcbFileInfo, int uFlags);

        private Bitmap Extract(string File, IconSize Size)
        {
            SHFILEINFO aSHFileInfo = new SHFILEINFO();
            int cbFileInfo;
            int uflags = SHGFI_ICON;
            Bitmap Icon;

            if (Size == IconSize.Small)
            {
                uflags = SHGFI_ICON | SHGFI_LARGEICON;
            }
            else if (Size == IconSize.Large)
            {
                uflags = SHGFI_ICON | SHGFI_SMALLICON;
            }
            cbFileInfo = Marshal.SizeOf(aSHFileInfo);
            SHGetFileInfo(ref File, 0, ref aSHFileInfo, cbFileInfo, uflags);

            Icon = Bitmap.FromHicon(aSHFileInfo.hIcon);

            return Icon;
        }

        public Bitmap ExtractIcon(string File)
        {
            return Extract(File, IconSize.Small);
        }
    }
}
