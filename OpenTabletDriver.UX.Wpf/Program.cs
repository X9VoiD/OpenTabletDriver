using System;

namespace OpenTabletDriver.UX.Wpf
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            _ = new SingleInstance();
            App.Run(Eto.Platforms.Wpf, args);
        }
    }
}
