using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Aries.IconMethods
{
    sealed class IconClass
    {
        public static void ExtractIcon()
        {
            if (Form1.Instance.checkmainicon.Checked & File.Exists("icon.exe"))
            {
                if (Form1.Instance.checkIconM2.Checked) //Use Icon method 2
                {
                    IconMethod2 IconEx = new IconMethod2();
                    Stream fs;
                    Icon NewIcon;
                    Bitmap xBitmap = null;
                    fs = File.OpenWrite("icon.ico");
                    if (Form1.Instance.checkmainicon.Checked)
                    {
                        xBitmap = IconEx.ExtractIcon(Form1.Instance.TextHostFile.Text);
                    }
                    else if (Form1.Instance.textIconPath.Text.EndsWith(".exe".ToLower()))
                    {
                        xBitmap = IconEx.ExtractIcon(Form1.Instance.textIconPath.Text);
                    }
                    IntPtr Hicon = xBitmap.GetHicon();
                    NewIcon = System.Drawing.Icon.FromHandle(Hicon);
                    NewIcon.Save(fs);
                    fs.Close();
                    xBitmap.Dispose();
                    NewIcon.Dispose();
                }
                else if (Form1.Instance.checkIconM1.Checked) //Use Icon method 1
                {
                    string origfile = null;
                    if (Form1.Instance.checkmainicon.Checked)
                    {
                        origfile = (Form1.Instance.TextHostFile.Text);
                    }
                    else if (Form1.Instance.textIconPath.Text.EndsWith(".exe".ToLower()))
                    {
                        origfile = (Form1.Instance.textIconPath.Text);
                    }

                    //Icon extractor by Steve McMahon
                    PictureBox box0 = new PictureBox();
                    box0.Image = Icon.ExtractAssociatedIcon(origfile).ToBitmap();
                    Bitmap MyBMP0 = new Bitmap(box0.Image);
                    PictureBox box = new PictureBox();
                    box.Image = MyBMP0;


                    Bitmap MyBMP = new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Icon MyIcon = Icon.FromHandle(MyBMP.GetHicon());
                    Stream st = new System.IO.FileStream("icon.ico", FileMode.Create);
                    BinaryWriter wr = new System.IO.BinaryWriter(st);
                    MyIcon.Save(st);

                    wr.Close();
                    //-- END icon creation --

                    //Opens icon for editing with IconEX
                    IconMethod1 IconexX = new IconMethod1("icon.ico");
                    //Removes original icon image that we created above
                    IconexX.Items.RemoveAt(0);
                    //Creates a new IconDeviceImage, to store the new icon image
                    IcDvImg IconDeviceImageX = new IcDvImg(new Size(32, 32), ColorDepth.Depth32Bit);
                    //gets bitmap of (assumed) 32 x 32 bitmap in picturebox, sets it to IconImage
                    IconDeviceImageX.IconImage = new Bitmap(box.Image);
                    //adds icondevicimage to the icon file
                    IconexX.Items.Add(IconDeviceImageX);
                    //saves icon 
                    IconexX.Save("icon.ico");

                    box0.Dispose();
                    MyBMP0.Dispose();
                    MyIcon.Dispose();
                    MyBMP.Dispose();
                    st.Dispose();
                    box.Dispose();
                    IconexX.Dispose();
                    IconDeviceImageX.Dispose();
                }
            }
        }
    }
}
