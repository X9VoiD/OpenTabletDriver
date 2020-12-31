using System;
using System.Threading;
using System.Threading.Tasks;
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
                Task.Run(() =>
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
            }
            else
            {
                handle.Set();
                Environment.Exit(0);
            }
        }
    }
}
