using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OpenTabletDriver.UI.ViewModels.Plugin;

namespace OpenTabletDriver.UI.DataTemplates;

public class PluginSettingViewLocator : IDataTemplate
{
    public bool Match(object? data)
    {
        return data is PluginSettingViewModel;
    }

    public Control Build(object? data)
    {
        return data switch
        {
            // BoolViewModel boolViewModel => new BoolSettingInput(),
            // EnumViewModel enumViewModel => new EnumSettingInput(),
            // IntegerViewModel integerViewModel => new IntegerSettingInput(),
            // NumberViewModel numberViewModel => new NumberSettingInput(),
            // StringViewModel stringViewModel => new StringSettingInput(),
            null => new TextBlock { Text = "eh???" },
            _ => new TextBlock { Text = "Unknown setting type" }
        };
    }
}
