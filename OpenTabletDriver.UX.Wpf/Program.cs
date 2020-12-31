using System;
using System.Threading;
using Eto.Forms;

namespace OpenTabletDriver.UX.Wpf
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstance();
            App.Run(Eto.Platforms.Wpf, args);
        }

        private static void SingleInstance()
        {
            var handle = new EventWaitHandle(false, EventResetMode.AutoReset, "OpenTabletDriver.UX.Wpf", out var host);
            if (host)
            {
                var bg_thread = new Thread(() =>
                {
                    while (true)
                    {
                        handle.WaitOne();
                        Application.Instance?.AsyncInvoke(() =>
                        {
                            var form = Application.Instance?.MainForm;
                            form?.Show();
                            form.WindowState = WindowState.Normal;
                            form.BringToFront();
                            form.WindowStyle = WindowStyle.Default;
                        });
                    }
                });
                bg_thread.IsBackground = true;
                bg_thread.Start();
            }
            else
            {
                handle.Set();
                Environment.Exit(0);
            }
        }
    }
}
