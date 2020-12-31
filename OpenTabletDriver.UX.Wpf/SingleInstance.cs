using Eto.Forms;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTabletDriver.UX.Wpf
{
    internal class SingleInstance
    {
        private readonly EventWaitHandle handle;
        public SingleInstance()
        {
            this.handle = new(false, EventResetMode.AutoReset, "OpenTabletDriver.UX.Wpf", out var host);
            if (host)
                _ = Task.Run(Main);
            else
            {
                handle.Set();
                Environment.Exit(0);
            }
        }

        private void Main()
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
        }
    }
}