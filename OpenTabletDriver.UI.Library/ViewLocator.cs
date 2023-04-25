using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        throw new InvalidOperationException("This method should not be called");
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
