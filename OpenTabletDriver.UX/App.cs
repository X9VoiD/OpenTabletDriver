﻿using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using System.Threading;
using Eto.Drawing;
using Eto.Forms;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Contracts;
using OpenTabletDriver.Desktop.Interop;
using OpenTabletDriver.Desktop.Migration;
using OpenTabletDriver.Desktop.RPC;
using OpenTabletDriver.Plugin;

namespace OpenTabletDriver.UX
{
    public static class App
    {
        public static void Run(string platform, string[] args)
        {
            SingleInstance();
            var root = new RootCommand("OpenTabletDriver UX")
            {
                new Option<bool>(new string[] { "-m", "--minimized" }, "Start the application minimized")
                {
                    Argument = new Argument<bool>("minimized")
                }
            };

            bool startMinimized = false;
            root.Handler = CommandHandler.Create<bool>((minimized) =>
            {
                startMinimized = minimized;
            });

            int code = root.Invoke(args);
            if (code != 0)
                Environment.Exit(code);

            var app = new Application(platform);
            var mainForm = new MainForm();
            if (startMinimized)
            {
                mainForm.WindowState = WindowState.Minimized;
                if (EnableTrayIcon)
                {
                    mainForm.Show();
                    mainForm.Visible = true;
                    mainForm.WindowState = WindowState.Minimized;
                    mainForm.ShowInTaskbar = false;
                    mainForm.Visible = false;
                }
            }

            app.Run(mainForm);
        }

        public const string PluginRepositoryUrl = "https://github.com/InfinityGhost/OpenTabletDriver/wiki/Plugin-Repository";
        public const string FaqUrl = "https://github.com/InfinityGhost/OpenTabletDriver/wiki#frequently-asked-questions";

        public static RpcClient<IDriverDaemon> Driver => _daemon.Value;
        public static Bitmap Logo => _logo.Value;

        public static event Action<Settings> SettingsChanged;
        private static Settings settings;
        public static Settings Settings
        {
            set
            {
                settings = SettingsMigrator.Migrate(value);
                SettingsChanged?.Invoke(Settings);
            }
            get => settings;
        }

        public static AboutDialog AboutDialog => new AboutDialog
        {
            Title = "OpenTabletDriver",
            ProgramName = "OpenTabletDriver",
            ProgramDescription = "Open source, cross-platform tablet configurator",
            WebsiteLabel = "OpenTabletDriver GitHub Repository",
            Website = new Uri(@"https://github.com/InfinityGhost/OpenTabletDriver"),
            Version = $"v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}",
            Developers = new string[] { "InfinityGhost" },
            Designers = new string[] { "InfinityGhost" },
            Documenters = new string[] { "InfinityGhost" },
            License = string.Empty,
            Copyright = string.Empty,
            Logo = Logo.WithSize(256, 256)
        };

        public readonly static bool EnableTrayIcon = SystemInterop.CurrentPlatform switch
        {
            PluginPlatform.Windows => true,
            PluginPlatform.MacOS   => true,
            _                       => false
        };

        private static readonly Lazy<RpcClient<IDriverDaemon>> _daemon = new Lazy<RpcClient<IDriverDaemon>>(() =>
        {
            return new RpcClient<IDriverDaemon>("OpenTabletDriver.Daemon");
        });

        private static readonly Lazy<Bitmap> _logo = new Lazy<Bitmap>(() =>
        {
            var dataStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OpenTabletDriver.UX.Assets.otd.png");
            return new Bitmap(dataStream);
        });

        private static void SingleInstance()
        {
            var handle = new EventWaitHandle(false, EventResetMode.AutoReset, "OpenTabletDriver.UX.Wpf", out var host);
            if (host)
            {
                var fg_thread = new Thread(() =>
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
                fg_thread.Start();
            }
            else
            {
                handle.Set();
                Environment.Exit(0);
            }
        }
    }
}
