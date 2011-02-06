using System;
[assembly: System.Runtime.CompilerServices.SuppressIldasm]

namespace Aries
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Check that the SuppressIldasmAttribute is still applied & the SNK hasn't been removed/tampered
            //Extra code is to mess w/ decompilers like to reflector (which doesn't handle this correctly & displays the wrong output)
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            string key = GetPublicKey();
            if (Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(),
                typeof(System.Runtime.CompilerServices.SuppressIldasmAttribute)) == null || key != "315616aa65aa4c0")
            {
                goto Label_0;
                try { System.Windows.Forms.Application.Run(Form1.Instance); }
                catch { }
            }
            key = null;
            try { System.Windows.Forms.Application.Run(Form1.Instance); }
            catch { }
        Label_1: { goto Label_2; }
        Label_0: { goto Label_1; }
        Label_2: { return; }
        }

        /// <summary>
        /// Returns the assemblies Public KeyToken
        /// </summary>
        static string GetPublicKey()
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetCallingAssembly();
            string sb = null;
            byte[] pt = myAssembly.GetName().GetPublicKeyToken();
            for (int i = 0; i <= (pt.Length - 1); i++)
            {
                sb += pt[i].ToString("x");
            }
            return sb;
        }
    }
}


