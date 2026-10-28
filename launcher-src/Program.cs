using System;
using System.Threading;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    internal static class Program
    {
        private static Mutex _mutex;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--update")
            {
                string dir = args.Length > 1 ? args[1] : null;
                Environment.ExitCode = Cli.RunUpdate(dir);
                return;
            }

            bool created;
            _mutex = new Mutex(true, @"Local\SteampunkLauncher.SingleInstance", out created);
            if (!created)
            {
                MessageBox.Show("Лаунчер уже запущен.", "Steampunk & Swords",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (s, e) => Log.Fatal(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.Fatal(e.ExceptionObject as Exception);
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) => null;

            try
            {
                Application.Run(new MainForm());
            }
            finally
            {
                GC.KeepAlive(_mutex);
            }
        }
    }
}
