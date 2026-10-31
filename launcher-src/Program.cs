using System;
using System.Threading;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--update")
            {
                string dir = args.Length > 1 ? args[1] : null;
                Environment.ExitCode = Cli.RunUpdate(dir);
                return;
            }

            if (args.Length > 0 && args[0] == "--launch")
            {
                string dir = null;
                string nick = null;
                if (args.Length == 2)
                {
                    // SteampunkLauncher.exe --launch Nickname
                    nick = args[1];
                }
                else if (args.Length > 2)
                {
                    // SteampunkLauncher.exe --launch "GameDir" Nickname
                    dir = args[1];
                    nick = args[2];
                }
                Environment.ExitCode = Cli.RunLaunch(dir, nick);
                return;
            }

            if (args.Length > 1 && (args[0] == "--nick" || args[0] == "-n"))
            {
                try
                {
                    var cfg = LauncherConfig.Load();
                    cfg.Nickname = args[1];
                    cfg.Save();
                }
                catch { }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (s, e) => Log.Fatal(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.Fatal(e.ExceptionObject as Exception);
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) => null;

            Application.Run(new MainForm());
        }
    }
}
