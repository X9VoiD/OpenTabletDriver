using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI;

public static class WindowExtensions
{
    public static IDisposable BootstrapTransparency(this Window window, WindowViewModelBase vm, IDispatcher dispatcher)
    {
        window.TransparencyLevelHint = new[]
        {
            WindowTransparencyLevel.Mica,
            WindowTransparencyLevel.AcrylicBlur
        };

        var windowBg = window.GetControl<Rectangle>("WindowBg");
        var acrylicBorder = window.GetControl<ExperimentalAcrylicBorder>("AcrylicBorder");

        bool windowTransparency = false; // this variable will be captured by the delegates below

        window.Activated += (_, _) => { if (windowTransparency) windowBg.Opacity = 0.65; };
        window.Deactivated += (_, _) => { if (windowTransparency) windowBg.Opacity = 1.0; };

        return vm.HandleProperty(
            nameof(vm.TransparencyEnabled),
            vm => vm.TransparencyEnabled,
            (vm, transparencyEnabled) => dispatcher.ProbablySynchronousPost(() =>
            {
                if (transparencyEnabled)
                {
                    enableWindowTransparency();
                }
                else
                {
                    disableWindowTransparency();
                }
            })
        );

        void enableWindowTransparency()
        {
            if (window.ActualTransparencyLevel != WindowTransparencyLevel.None)
            {
                windowTransparency = true;
                acrylicBorder.IsVisible = true;
                windowBg.Opacity = 0.65;
            }
            else
            {
                // If transparency is not supported, disable it
                disableWindowTransparency();
            }
        }

        void disableWindowTransparency()
        {
            windowTransparency = false;
            windowBg.Opacity = 1.0;
        }
    }
}
