using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        // reflection no no
        throw new NotImplementedException();
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
