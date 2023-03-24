using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OpenTabletDriver.UI.ViewModels;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        return data switch
        {
            MainWindowViewModel => new MainWindowView(),
            _ => throw new InvalidOperationException("No matching view/control")
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
