using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace Aries.IconMethods
{
    public sealed class IcDvImg : IDisposable
    {
        // Fields
        private const int BI_RGB = 0;
        private const int BI_RLE4 = 2;
        private const int BI_RLE8 = 1;
        private ColorDepth colorDepth;
        private byte[] data;
        private const int DIB_PAL_COLORS = 1;
        private const int DIB_PAL_INDICES = 2;
        private const int DIB_PAL_LOGINDICES = 4;
        private const int DIB_PAL_PHYSINDICES = 2;
        private const int DIB_RGB_COLORS = 0;
        private IntPtr hIcon;
        private const short IMAGE_ICON = 1;
        private Size size;

        // Methods
        internal IcDvImg(byte[] b)
        {
            this.colorDepth = ColorDepth.Depth4Bit;
            this.hIcon = IntPtr.Zero;
            this.data = new byte[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                this.data[i] = b[i];
            }
            BITMAPINFOHEADER bitmapinfoheader = new BITMAPINFOHEADER(this.data);
            this.size.Width = bitmapinfoheader.biWidth;
            this.size.Height = bitmapinfoheader.biHeight / 2;
            switch (bitmapinfoheader.biBitCount)
            {
                case 1:
                case 4:
                    this.colorDepth = ColorDepth.Depth4Bit;
                    break;

                case 8:
                    this.colorDepth = ColorDepth.Depth8Bit;
                    break;

                case 0x10:
                    this.colorDepth = ColorDepth.Depth16Bit;
                    break;

                case 0x18:
                    this.colorDepth = ColorDepth.Depth24Bit;
                    break;

                case 0x20:
                    this.colorDepth = ColorDepth.Depth32Bit;
                    break;
            }
            this.createIcon();
        }

        public IcDvImg(Icon icon)
        {
            this.colorDepth = ColorDepth.Depth4Bit;
            this.hIcon = IntPtr.Zero;
        }

        public IcDvImg(Size size, ColorDepth colorDepth)
        {
            this.colorDepth = ColorDepth.Depth4Bit;
            this.hIcon = IntPtr.Zero;
            this.setDeviceImage(size, colorDepth);
            this.createIcon();
        }

        [DllImport("gdi32")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);
        [DllImport("gdi32")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32", CharSet = CharSet.Auto)]
        private static extern IntPtr CreateDC([MarshalAs(UnmanagedType.LPTStr)] string lpDriverName, IntPtr lpDeviceName, IntPtr lpOutput, IntPtr lpInitData);
        private void createIcon()
        {
            if (this.hIcon != IntPtr.Zero)
            {
                DestroyIcon(this.hIcon);
                this.hIcon = IntPtr.Zero;
            }
            ICONINFO piconInfo = new ICONINFO();
            piconInfo.fIcon = 1;
            this.getIconBitmap(false, true, ref piconInfo.hBmColor);
            this.getIconBitmap(true, true, ref piconInfo.hBmMask);
            this.hIcon = CreateIconIndirect(ref piconInfo);
            DeleteObject(piconInfo.hBmColor);
            DeleteObject(piconInfo.hBmMask);
        }

        [DllImport("user32")]
        private static extern IntPtr CreateIconIndirect(ref ICONINFO piconInfo);
        [DllImport("gdi32")]
        private static extern int DeleteDC(IntPtr hdc);
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr hObject);
        [DllImport("user32")]
        private static extern int DestroyIcon(IntPtr hIcon);
        private int dibNumColors(BITMAPINFOHEADER bmInfoHeader)
        {
            if (bmInfoHeader.biClrUsed != 0)
            {
                return bmInfoHeader.biClrUsed;
            }
            switch (bmInfoHeader.biBitCount)
            {
                case 1:
                    return 2;

                case 4:
                    return 0x10;

                case 8:
                    return 0x100;
            }
            return 0;
        }

        public void Dispose()
        {
            if (this.hIcon != IntPtr.Zero)
            {
                DestroyIcon(this.hIcon);
                this.hIcon = IntPtr.Zero;
            }
        }

        [DllImport("gdi32")]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hBitmap, int nStartScan, int nNumScans, IntPtr Bits, IntPtr BitsInfo, int wUsage);
        private Bitmap getIconBitmap(bool mask, bool returnHandle, ref IntPtr hBmp)
        {
            Bitmap bitmap = null;
            BITMAPINFOHEADER structure = new BITMAPINFOHEADER(this.data);
            if (mask)
            {
                IntPtr hdc = CreateCompatibleDC(IntPtr.Zero);
                hBmp = CreateCompatibleBitmap(hdc, structure.biWidth, structure.biHeight / 2);
                IntPtr hObject = SelectObject(hdc, hBmp);
                RGBQUAD rgbquad = new RGBQUAD();
                int cb = structure.biSize + (Marshal.SizeOf(rgbquad) * 2);
                IntPtr ptr = Marshal.AllocCoTaskMem(cb);
                Marshal.WriteInt32(ptr, Marshal.SizeOf(structure));
                Marshal.WriteInt32(ptr, 4, structure.biWidth);
                Marshal.WriteInt32(ptr, 8, structure.biHeight / 2);
                Marshal.WriteInt16(ptr, 12, (short)1);
                Marshal.WriteInt16(ptr, 14, (short)1);
                Marshal.WriteInt32(ptr, 0x10, 0);
                Marshal.WriteInt32(ptr, 20, 0);
                Marshal.WriteInt32(ptr, 0x18, 0);
                Marshal.WriteInt32(ptr, 0x1c, 0);
                Marshal.WriteInt32(ptr, 0x20, 0);
                Marshal.WriteInt32(ptr, 0x24, 0);
                Marshal.WriteInt32(ptr, 40, 0);
                Marshal.WriteByte(ptr, 0x2c, 0xff);
                Marshal.WriteByte(ptr, 0x2d, 0xff);
                Marshal.WriteByte(ptr, 0x2e, 0xff);
                Marshal.WriteByte(ptr, 0x2f, 0);
                int num2 = this.MaskImageSize(structure);
                IntPtr destination = Marshal.AllocCoTaskMem(num2);
                Marshal.Copy(this.data, this.MaskImageIndex(structure), destination, num2);
                SetDIBitsToDevice(hdc, 0, 0, structure.biWidth, structure.biHeight / 2, 0, 0, 0, structure.biHeight / 2, destination, ptr, 0);
                Marshal.FreeCoTaskMem(destination);
                Marshal.FreeCoTaskMem(ptr);
                SelectObject(hdc, hObject);
                DeleteObject(hdc);
            }
            else
            {
                IntPtr ptr5 = CreateDC("DISPLAY", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                IntPtr ptr6 = CreateCompatibleDC(ptr5);
                hBmp = CreateCompatibleBitmap(ptr5, structure.biWidth, structure.biHeight / 2);
                DeleteDC(ptr5);
                IntPtr ptr7 = SelectObject(ptr6, hBmp);
                int num3 = this.XorImageIndex(structure);
                int num4 = this.XorImageSize(structure);
                IntPtr ptr8 = Marshal.AllocCoTaskMem(num3);
                Marshal.Copy(this.data, 0, ptr8, num3);
                Marshal.WriteInt32(ptr8, 8, structure.biHeight / 2);
                IntPtr ptr9 = Marshal.AllocCoTaskMem(num4);
                Marshal.Copy(this.data, num3, ptr9, num4);
                SetDIBitsToDevice(ptr6, 0, 0, structure.biWidth, structure.biHeight / 2, 0, 0, 0, structure.biHeight / 2, ptr9, ptr8, 0);
                Marshal.FreeCoTaskMem(ptr9);
                Marshal.FreeCoTaskMem(ptr8);
                SelectObject(ptr6, ptr7);
                DeleteObject(ptr6);
            }
            if (!returnHandle)
            {
                bitmap = Image.FromHbitmap(hBmp);
            }
            return bitmap;
        }

        internal int IconImageDataBytes()
        {
            return this.data.Length;
        }

        private int MaskImageIndex(BITMAPINFOHEADER bmInfoHeader)
        {
            return (this.XorImageIndex(bmInfoHeader) + this.XorImageSize(bmInfoHeader));
        }

        private int MaskImageSize(BITMAPINFOHEADER bmInfoHeader)
        {
            return ((bmInfoHeader.biHeight / 2) * this.WidthBytes(bmInfoHeader.biWidth));
        }

        internal void SaveIconBitmapData(BinaryWriter bw)
        {
            bw.Write(this.data, 0, this.data.Length);
        }

        [DllImport("gdi32")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
        private void setDeviceImage(Size size, ColorDepth colorDepth)
        {
            this.size = size;
            this.colorDepth = colorDepth;
            BITMAPINFOHEADER bmInfoHeader = new BITMAPINFOHEADER(size, colorDepth);
            this.data = new byte[this.MaskImageIndex(bmInfoHeader) + this.MaskImageSize(bmInfoHeader)];
            MemoryStream output = new MemoryStream(this.data, 0, this.data.Length, true);
            BinaryWriter bw = new BinaryWriter(output);
            bmInfoHeader.Write(bw);
            switch (this.colorDepth)
            {
                case ColorDepth.Depth4Bit:
                    this.write16ColorPalette(bw);
                    break;

                case ColorDepth.Depth8Bit:
                    this.write256ColorPalette(bw);
                    goto Label_0077;
            }
        Label_0077:
            bw.Close();
        }

        [DllImport("gdi32")]
        private static extern int SetDIBitsToDevice(IntPtr hdc, int X, int Y, int dx, int dy, int SrcX, int SrcY, int Scan, int NumScans, IntPtr Bits, IntPtr BitsInfo, int wUsage);
        private void setImageBitsFromBitmap(Bitmap bm)
        {
            IntPtr hdc = CreateDC("DISPLAY", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            IntPtr ptr2 = CreateCompatibleDC(hdc);
            DeleteDC(hdc);
            IntPtr hbitmap = bm.GetHbitmap();
            BITMAPINFOHEADER bmInfoHeader = new BITMAPINFOHEADER(this.size, this.colorDepth);
            int cb = this.XorImageIndex(bmInfoHeader);
            int num2 = this.XorImageSize(bmInfoHeader);
            IntPtr destination = Marshal.AllocCoTaskMem(cb);
            Marshal.Copy(this.data, 0, destination, cb);
            Marshal.WriteInt32(destination, 8, bmInfoHeader.biHeight / 2);
            IntPtr bits = Marshal.AllocCoTaskMem(num2);
            GetDIBits(ptr2, hbitmap, 0, this.size.Height, bits, destination, 0);
            Marshal.Copy(bits, this.data, cb, num2);
            Marshal.FreeCoTaskMem(bits);
            Marshal.FreeCoTaskMem(destination);
            DeleteObject(hbitmap);
            DeleteDC(ptr2);
            this.createIcon();
        }

        private void setMaskBitsFromBitmap(Bitmap bm)
        {
            IntPtr hdc = CreateDC("DISPLAY", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            IntPtr ptr2 = CreateCompatibleDC(hdc);
            DeleteDC(hdc);
            IntPtr hbitmap = bm.GetHbitmap();
            BITMAPINFOHEADER structure = new BITMAPINFOHEADER(this.size, this.colorDepth);
            RGBQUAD rgbquad = new RGBQUAD();
            int cb = structure.biSize + (Marshal.SizeOf(rgbquad) * 2);
            IntPtr ptr = Marshal.AllocCoTaskMem(cb);
            Marshal.WriteInt32(ptr, Marshal.SizeOf(structure));
            Marshal.WriteInt32(ptr, 4, this.size.Width);
            Marshal.WriteInt32(ptr, 8, this.size.Height);
            Marshal.WriteInt16(ptr, 12, (short)1);
            Marshal.WriteInt16(ptr, 14, (short)1);
            Marshal.WriteInt32(ptr, 0x10, 0);
            Marshal.WriteInt32(ptr, 20, 0);
            Marshal.WriteInt32(ptr, 0x18, 0);
            Marshal.WriteInt32(ptr, 0x1c, 0);
            Marshal.WriteInt32(ptr, 0x20, 0);
            Marshal.WriteInt32(ptr, 0x24, 0);
            Marshal.WriteInt32(ptr, 40, 0);
            Marshal.WriteByte(ptr, 0x2c, 0xff);
            Marshal.WriteByte(ptr, 0x2d, 0xff);
            Marshal.WriteByte(ptr, 0x2e, 0xff);
            Marshal.WriteByte(ptr, 0x2f, 0);
            int num2 = this.MaskImageSize(structure);
            IntPtr bits = Marshal.AllocCoTaskMem(num2);
            GetDIBits(ptr2, hbitmap, 0, this.size.Height, bits, ptr, 0);
            Marshal.Copy(bits, this.data, this.MaskImageIndex(structure), num2);
            Marshal.FreeCoTaskMem(bits);
            Marshal.FreeCoTaskMem(ptr);
            DeleteObject(hbitmap);
            DeleteDC(ptr2);
            this.createIcon();
        }

        private int WidthBytes(int width)
        {
            return (((width + 0x1f) / 0x20) * 4);
        }

        private void write16ColorPalette(BinaryWriter bw)
        {
            this.writeColor(bw, Color.Black);
            this.writeColor(bw, Color.White);
            this.writeColor(bw, Color.Red);
            this.writeColor(bw, Color.Green);
            this.writeColor(bw, Color.Blue);
            this.writeColor(bw, Color.Yellow);
            this.writeColor(bw, Color.Magenta);
            this.writeColor(bw, Color.Cyan);
            this.writeColor(bw, Color.Gray);
            this.writeColor(bw, Color.DarkRed);
            this.writeColor(bw, Color.DarkGreen);
            this.writeColor(bw, Color.DarkBlue);
            this.writeColor(bw, Color.Olive);
            this.writeColor(bw, Color.Purple);
            this.writeColor(bw, Color.Teal);
            this.writeColor(bw, Color.DarkGray);
        }

        private void write256ColorPalette(BinaryWriter bw)
        {
            Array values = Enum.GetValues(KnownColor.ActiveBorder.GetType());
            int num = 0;
            foreach (KnownColor color2 in values)
            {
                this.writeColor(bw, Color.FromKnownColor(color2));
                num++;
                if (num > 0xff)
                {
                    break;
                }
            }
        }

        private void writeColor(BinaryWriter bw, Color color)
        {
            new RGBQUAD(color).Write(bw);
        }

        private int XorImageIndex(BITMAPINFOHEADER bmInfoHeader)
        {
            RGBQUAD structure = new RGBQUAD();
            return (Marshal.SizeOf(bmInfoHeader) + (this.dibNumColors(bmInfoHeader) * Marshal.SizeOf(structure)));
        }

        private int XorImageSize(BITMAPINFOHEADER bmInfoHeader)
        {
            return ((bmInfoHeader.biHeight / 2) * this.WidthBytes((bmInfoHeader.biWidth * bmInfoHeader.biBitCount) * bmInfoHeader.biPlanes));
        }

        // Properties
        public ColorDepth ColorDepth
        {
            get
            {
                return this.colorDepth;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return this.hIcon;
            }
        }

        public Icon Icon
        {
            get
            {
                return Icon.FromHandle(this.hIcon);
            }
        }

        public Bitmap IconImage
        {
            get
            {
                IntPtr zero = IntPtr.Zero;
                return this.getIconBitmap(false, false, ref zero);
            }
            set
            {
                this.setImageBitsFromBitmap(value);
            }
        }

        public Size IconSize
        {
            get
            {
                return this.size;
            }
        }

        public Bitmap MaskImage
        {
            get
            {
                IntPtr zero = IntPtr.Zero;
                return this.getIconBitmap(true, false, ref zero);
            }
            set
            {
                this.setMaskBitsFromBitmap(value);
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
            public BITMAPINFOHEADER(Size size, ColorDepth colorDepth)
            {
                this.biSize = 0;
                this.biWidth = size.Width;
                this.biHeight = size.Height * 2;
                this.biPlanes = 1;
                this.biCompression = 0;
                this.biSizeImage = 0;
                this.biXPelsPerMeter = 0;
                this.biYPelsPerMeter = 0;
                this.biClrUsed = 0;
                this.biClrImportant = 0;
                switch (colorDepth)
                {
                    case ColorDepth.Depth16Bit:
                        this.biBitCount = 0x10;
                        break;

                    case ColorDepth.Depth24Bit:
                        this.biBitCount = 0x18;
                        break;

                    case ColorDepth.Depth32Bit:
                        this.biBitCount = 0x20;
                        break;

                    case ColorDepth.Depth4Bit:
                        this.biBitCount = 4;
                        break;

                    case ColorDepth.Depth8Bit:
                        this.biBitCount = 8;
                        break;

                    default:
                        this.biBitCount = 4;
                        break;
                }
                this.biSize = Marshal.SizeOf(base.GetType());
            }

            public void Write(BinaryWriter bw)
            {
                bw.Write(this.biSize);
                bw.Write(this.biWidth);
                bw.Write(this.biHeight);
                bw.Write(this.biPlanes);
                bw.Write(this.biBitCount);
                bw.Write(this.biCompression);
                bw.Write(this.biSizeImage);
                bw.Write(this.biXPelsPerMeter);
                bw.Write(this.biYPelsPerMeter);
                bw.Write(this.biClrUsed);
                bw.Write(this.biClrImportant);
            }

            public BITMAPINFOHEADER(byte[] data)
            {
                MemoryStream input = new MemoryStream(data, false);
                BinaryReader reader = new BinaryReader(input);
                this.biSize = reader.ReadInt32();
                this.biWidth = reader.ReadInt32();
                this.biHeight = reader.ReadInt32();
                this.biPlanes = reader.ReadInt16();
                this.biBitCount = reader.ReadInt16();
                this.biCompression = reader.ReadInt32();
                this.biSizeImage = reader.ReadInt32();
                this.biXPelsPerMeter = reader.ReadInt32();
                this.biYPelsPerMeter = reader.ReadInt32();
                this.biClrUsed = reader.ReadInt32();
                this.biClrImportant = reader.ReadInt32();
                reader.Close();
            }

            public override string ToString()
            {
                return string.Format("biSize: {0}, biWidth: {1}, biHeight: {2}, biPlanes: {3}, biBitCount: {4}, biCompression: {5}, biSizeImage: {6}, biXPelsPerMeter: {7}, biYPelsPerMeter {8}, biClrUsed {9}, biClrImportant {10}", new object[] { this.biSize, this.biWidth, this.biHeight, this.biPlanes, this.biBitCount, this.biCompression, this.biSizeImage, this.biXPelsPerMeter, this.biYPelsPerMeter, this.biClrUsed, this.biClrImportant });
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ICONINFO
        {
            public int fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hBmMask;
            public IntPtr hBmColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
            public RGBQUAD(byte r, byte g, byte b, byte alpha)
            {
                this.rgbBlue = b;
                this.rgbGreen = g;
                this.rgbRed = r;
                this.rgbReserved = 0;
            }

            public RGBQUAD(Color c)
            {
                this.rgbBlue = c.B;
                this.rgbGreen = c.G;
                this.rgbRed = c.R;
                this.rgbReserved = 0;
            }

            public void Write(BinaryWriter bw)
            {
                bw.Write(this.rgbBlue);
                bw.Write(this.rgbGreen);
                bw.Write(this.rgbRed);
                bw.Write(this.rgbReserved);
            }

            public override string ToString()
            {
                return string.Format("rgbBlue: {0}, rgbGreen: {1}, rgbRed: {2}", this.rgbBlue, this.rgbGreen, this.rgbRed);
            }
        }
    }


}
