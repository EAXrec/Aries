using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Forms;

namespace Aries.IconMethods
{
    //Icon extractor by - Steve McMahon
    sealed class IconMethod1 : IDisposable
    {
        // Fields
        private const int DIFFERENCE = 11;
        private IconDeviceImageCollection iconCollection;
        private string iconFile;
        private const short IMAGE_ICON = 1;
        private string libraryFile;
        private const int LOAD_LIBRARY_AS_DATAFILE = 2;
        private int resourceId;
        private string resourceName;
        private const int RT_BITMAP = 2;
        private const int RT_CURSOR = 1;
        private const int RT_GROUP_CURSOR = 12;
        private const int RT_GROUP_ICON = 14;
        private const int RT_ICON = 3;

        // Methods
        public IconMethod1()
        {
            this.iconCollection = null;
            this.iconFile = null;
            this.libraryFile = null;
            this.resourceId = -1;
            this.resourceName = null;
        }

        public IconMethod1(string iconFile)
        {
            this.iconCollection = null;
            this.iconFile = null;
            this.libraryFile = null;
            this.resourceId = -1;
            this.resourceName = null;
            this.loadFromFile(iconFile);
        }

        public IconMethod1(string libraryFile, int resourceId)
        {
            this.iconCollection = null;
            this.iconFile = null;
            this.libraryFile = null;
            this.resourceId = -1;
            this.resourceName = null;
            this.FromLibrary(libraryFile, resourceId);
        }

        public IconMethod1(string libraryFile, string resourceName)
        {
            this.iconCollection = null;
            this.iconFile = null;
            this.libraryFile = null;
            this.resourceId = -1;
            this.resourceName = null;
            this.FromLibrary(libraryFile, resourceName);
        }

        public void Dispose()
        {
            if (this.iconCollection != null)
            {
                this.iconCollection.Dispose();
                this.iconCollection = null;
            }
        }

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern IntPtr FindResource(IntPtr hInstance, [MarshalAs(UnmanagedType.LPTStr)] string lpName, IntPtr lpType);
        [DllImport("kernel32")]
        private static extern int FreeLibrary(IntPtr hLibModule);
        [DllImport("kernel32")]
        private static extern int FreeResource(IntPtr hResData);
        public void FromFile(string iconFile)
        {
            this.loadFromFile(iconFile);
        }

        public void FromLibrary(string libraryFile, int resourceId)
        {
            this.loadInitialise();
            string resourceName = string.Format("#{0:N0}", resourceId);
            this.loadFromLibrary(libraryFile, resourceName);
        }

        public void FromLibrary(string libraryFile, string resourceName)
        {
            this.loadInitialise();
            this.loadFromLibrary(libraryFile, resourceName);
        }

        private void loadFromFile(string iconFile)
        {
            this.loadInitialise();
            FileStream input = new FileStream(iconFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(input);
            try
            {
                int num = this.readIconFileHeader(br);
                ICONDIRENTRY[] icondirentryArray = new ICONDIRENTRY[num];
                for (int i = 0; i < num; i++)
                {
                    icondirentryArray[i] = new ICONDIRENTRY(br);
                }
                IcDvImg[] icons = new IcDvImg[num];
                for (int j = 0; j < num; j++)
                {
                    input.Seek((long)icondirentryArray[j].dwImageOffset, SeekOrigin.Begin);
                    byte[] buffer = new byte[icondirentryArray[j].dwBytesInRes];
                    br.Read(buffer, 0, icondirentryArray[j].dwBytesInRes);
                    icons[j] = new IcDvImg(buffer);
                }
                this.iconCollection = new IconDeviceImageCollection(icons);
            }
            catch (Exception exception)
            {
                if (exception is SystemException)
                {
                    throw exception;
                }
                throw new IconExException("Failed to read icon file.", exception);
            }
            finally
            {
                br.Close();
            }
            this.iconFile = iconFile;
        }

        private void loadFromLibrary(string libraryFile, string resourceName)
        {
            string message = "";
            bool flag = false;
            IntPtr zero = IntPtr.Zero;
            IntPtr hResInfo = IntPtr.Zero;
            IntPtr hInstance = IntPtr.Zero;
            IntPtr lPtr = IntPtr.Zero;
            try
            {
                hInstance = LoadLibraryEx(libraryFile, IntPtr.Zero, 2);
                if (hInstance != IntPtr.Zero)
                {
                    hResInfo = FindResource(hInstance, resourceName, (IntPtr)14);
                    if (hResInfo != IntPtr.Zero)
                    {
                        zero = LoadResource(hInstance, hResInfo);
                        if (zero != IntPtr.Zero)
                        {
                            lPtr = LockResource(zero);
                            if (lPtr != IntPtr.Zero)
                            {
                                int num = this.readResourceIconFileHeader(lPtr);
                                MEMICONDIRENTRY[] memicondirentryArray = new MEMICONDIRENTRY[num];
                                int ofs = 6;
                                for (int i = 0; i < num; i++)
                                {
                                    memicondirentryArray[i] = new MEMICONDIRENTRY(lPtr, ofs);
                                    ofs += 14;
                                }
                                FreeResource(zero);
                                zero = IntPtr.Zero;
                                IcDvImg[] icons = new IcDvImg[num];
                                for (int j = 0; j < num; j++)
                                {
                                    string lpName = string.Format("#{0:N0}", memicondirentryArray[j].nID);
                                    hResInfo = FindResource(hInstance, lpName, (IntPtr)3);
                                    if (hResInfo == IntPtr.Zero)
                                    {
                                        message = string.Format("Could not find the component icon resource with id {0}", memicondirentryArray[j].nID);
                                        flag = true;
                                        break;
                                    }
                                    zero = LoadResource(hInstance, hResInfo);
                                    if (zero == IntPtr.Zero)
                                    {
                                        message = string.Format("Could not load the component icon resource with id {0}", memicondirentryArray[j].nID);
                                        flag = true;
                                        break;
                                    }
                                    int length = SizeofResource(hInstance, hResInfo);
                                    if ((length > 0) && (length == memicondirentryArray[j].dwBytesInRes))
                                    {
                                        lPtr = LockResource(zero);
                                        byte[] destination = new byte[length];
                                        Marshal.Copy(lPtr, destination, 0, length);
                                        icons[j] = new IcDvImg(destination);
                                    }
                                    else
                                    {
                                        message = string.Format("Component icon resource with id {0} is corrupt", memicondirentryArray[j].nID);
                                        flag = true;
                                    }
                                }
                                if (!flag)
                                {
                                    this.iconCollection = new IconDeviceImageCollection(icons);
                                }
                            }
                            else
                            {
                                message = "Can't lock resource for reading.";
                                flag = true;
                            }
                        }
                        else
                        {
                            message = "Can't load resource for reading.";
                            flag = true;
                        }
                    }
                    else
                    {
                        message = "Can't find resource.";
                        flag = true;
                    }
                }
                else
                {
                    message = "Can't load library.";
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                flag = true;
                message = exception.Message;
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    FreeResource(zero);
                }
                if (hInstance != IntPtr.Zero)
                {
                    FreeLibrary(hInstance);
                }
                if (flag)
                {
                    throw new IconExException(message);
                }
            }
        }

        private void loadInitialise()
        {
            this.iconFile = "";
            this.resourceId = -1;
            this.libraryFile = "";
            this.iconCollection = new IconDeviceImageCollection();
        }

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibraryEx([MarshalAs(UnmanagedType.LPTStr)] string lpLibFileName, IntPtr hFile, int dwFlags);
        [DllImport("kernel32")]
        private static extern IntPtr LoadResource(IntPtr hInstance, IntPtr hResInfo);
        [DllImport("kernel32")]
        private static extern IntPtr LockResource(IntPtr hResData);
        private int readIconFileHeader(BinaryReader br)
        {
            int num = br.ReadInt16();
            int num2 = br.ReadInt16();
            int num3 = br.ReadInt16();
            if (((num != 0) || (num2 != 1)) || ((num3 <= 0) || (num3 >= 0x400)))
            {
                throw new IconExException("Invalid Icon File Header");
            }
            return num3;
        }

        private int readResourceIconFileHeader(IntPtr lPtr)
        {
            int num = Marshal.ReadInt16(lPtr);
            int num2 = Marshal.ReadInt16(lPtr, 2);
            int num3 = Marshal.ReadInt16(lPtr, 4);
            if (((num != 0) || (num2 != 1)) || ((num3 <= 0) || (num3 >= 0x400)))
            {
                throw new IconExException("Invalid Icon File Header");
            }
            return num3;
        }

        public void Save(string iconFile)
        {
            FileStream output = new FileStream(iconFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            BinaryWriter bw = null;
            try
            {
                bw = new BinaryWriter(output);
                this.writeIconFileHeader(bw);
                int num = 6 + (0x10 * this.iconCollection.Count);
                foreach (IcDvImg image in this.iconCollection)
                {
                    int num2 = image.IconImageDataBytes();
                    ICONDIRENTRY icondirentry = new ICONDIRENTRY();
                    icondirentry.width = (byte)image.IconSize.Width;
                    icondirentry.height = (byte)image.IconSize.Height;
                    switch (image.ColorDepth)
                    {
                        case ColorDepth.Depth16Bit:
                            icondirentry.colorCount = 0;
                            icondirentry.wBitCount = 0x10;
                            break;

                        case ColorDepth.Depth24Bit:
                            icondirentry.colorCount = 0;
                            icondirentry.wBitCount = 0x18;
                            break;

                        case ColorDepth.Depth32Bit:
                            icondirentry.colorCount = 0;
                            icondirentry.wBitCount = 0x20;
                            break;

                        case ColorDepth.Depth4Bit:
                            icondirentry.colorCount = 0x10;
                            icondirentry.wBitCount = 4;
                            break;

                        case ColorDepth.Depth8Bit:
                            icondirentry.colorCount = 0xff;
                            icondirentry.wBitCount = 8;
                            break;
                    }
                    icondirentry.wPlanes = 1;
                    icondirentry.dwBytesInRes = num2;
                    icondirentry.dwImageOffset = num;
                    icondirentry.Write(bw);
                    num += num2;
                }
                foreach (IcDvImg image2 in this.iconCollection)
                {
                    image2.SaveIconBitmapData(bw);
                }
            }
            catch (Exception exception)
            {
                if (exception is SystemException)
                {
                    throw exception;
                }
                throw new IconExException(exception.Message, exception);
            }
            finally
            {
                if (bw != null)
                {
                    bw.Close();
                }
            }
        }

        [DllImport("kernel32")]
        private static extern int SizeofResource(IntPtr hInstance, IntPtr hResInfo);
        private void writeIconFileHeader(BinaryWriter bw)
        {
            short num = 0;
            bw.Write(num);
            short num2 = 1;
            bw.Write(num2);
            short count = (short)this.Items.Count;
            bw.Write(count);
        }

        // Properties
        public string IconFile
        {
            get
            {
                return this.iconFile;
            }
        }

        public IconDeviceImageCollection Items
        {
            get
            {
                return this.iconCollection;
            }
            set
            {
            }
        }

        public string LibraryFile
        {
            get
            {
                return this.libraryFile;
            }
        }

        public int ResourceId
        {
            get
            {
                return this.resourceId;
            }
        }

        public string ResourceName
        {
            get
            {
                return this.resourceName;
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONDIRENTRY
        {
            public byte width;
            public byte height;
            public byte colorCount;
            public byte reserved;
            public short wPlanes;
            public short wBitCount;
            public int dwBytesInRes;
            public int dwImageOffset;
            public ICONDIRENTRY(BinaryReader br)
            {
                this.width = br.ReadByte();
                this.height = br.ReadByte();
                this.colorCount = br.ReadByte();
                this.reserved = br.ReadByte();
                this.wPlanes = br.ReadInt16();
                this.wBitCount = br.ReadInt16();
                this.dwBytesInRes = br.ReadInt32();
                this.dwImageOffset = br.ReadInt32();
            }

            public void Write(BinaryWriter br)
            {
                br.Write(this.width);
                br.Write(this.height);
                br.Write(this.colorCount);
                br.Write(this.reserved);
                br.Write(this.wPlanes);
                br.Write(this.wBitCount);
                br.Write(this.dwBytesInRes);
                br.Write(this.dwImageOffset);
            }

            public override string ToString()
            {
                return string.Format("Size: ({0},{1}), ColorCount: {2}, Planes: {3}, BitCount {4}, BytesInRes: {5}, ImageOffset {6}", new object[] { this.width, this.height, this.colorCount, this.wPlanes, this.wBitCount, this.dwBytesInRes, this.dwImageOffset });
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMICONDIRENTRY
        {
            public byte width;
            public byte height;
            public byte colorCount;
            public byte reserved;
            public short wPlanes;
            public short wBitCount;
            public int dwBytesInRes;
            public short nID;
            public MEMICONDIRENTRY(IntPtr lPtr, int ofs)
            {
                this.width = Marshal.ReadByte(lPtr, ofs);
                this.height = Marshal.ReadByte(lPtr, ofs + 1);
                this.colorCount = Marshal.ReadByte(lPtr, ofs + 2);
                this.reserved = Marshal.ReadByte(lPtr, ofs + 3);
                this.wPlanes = Marshal.ReadInt16(lPtr, ofs + 4);
                this.wBitCount = Marshal.ReadInt16(lPtr, ofs + 6);
                this.dwBytesInRes = Marshal.ReadInt32(lPtr, ofs + 8);
                this.nID = Marshal.ReadInt16(lPtr, ofs + 12);
            }

            public override string ToString()
            {
                return string.Format("Size: ({0},{1}), ColorCount: {2}, Planes: {3}, BitCount {4}, BytesInRes: {5}, IconResourceID {6}", new object[] { this.width, this.height, this.colorCount, this.wPlanes, this.wBitCount, this.dwBytesInRes, this.nID });
            }
        }
    }

    public class IconDeviceImageCollection : CollectionBase, IDisposable
    {
        // Methods
        public IconDeviceImageCollection()
        {
        }

        public IconDeviceImageCollection(IcDvImg[] icons)
        {
            foreach (IcDvImg image in icons)
            {
                base.InnerList.Add(image);
            }
        }

        public void Add(IcDvImg icon)
        {
            foreach (IcDvImg image in base.InnerList)
            {
                if (icon.IconSize.Equals(image.IconSize) && icon.ColorDepth.Equals(image.ColorDepth))
                {
                    throw new IconExException("An Icon Device Image with the same size and colour depth already exists in this icon");
                }
            }
            base.InnerList.Add(icon);
        }

        public void Dispose()
        {
            if (base.InnerList != null)
            {
                foreach (IcDvImg image in base.InnerList)
                {
                    image.Dispose();
                }
                base.InnerList.Clear();
            }
        }

        // Properties
        public IcDvImg this[int index]
        {
            get
            {
                return (IcDvImg)base.InnerList[index];
            }
        }
    }

    public class IconExException : Exception
    {
        // Methods
        public IconExException()
        {
        }

        public IconExException(string message)
            : base(message)
        {
        }

        public IconExException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
