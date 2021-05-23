using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace DailyPlannerWPF
{
    public partial class App : Application
    {
        public static TaskbarIcon NotifyIcon { get; set; }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int cmdShow);
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr handle);

        private Mutex mutex = new Mutex(false, "DailyPlannerWPF");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            NotifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            if (!mutex.WaitOne(500, false))
            {
                MessageBox.Show("Приложение уже запущено!", "Ошибка");
                string processName = Process.GetCurrentProcess().ProcessName;
                Process process = Process.GetProcesses().Where(p => p.ProcessName == processName).FirstOrDefault();
                if (process != null)
                {
                    IntPtr handle = process.MainWindowHandle;
                    ShowWindow(handle, 1);
                    SetForegroundWindow(handle);
                }
                Current.MainWindow.Close();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            NotifyIcon.Dispose();
            base.OnExit(e);
        }

    }
}
