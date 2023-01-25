using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;
using OpenTabletDriver.UX.Controls;
using OpenTabletDriver.UX.ViewModels;

namespace OpenTabletDriver.UX.Views.Windows
{
    public class MainWindow : Form
    {
        private readonly Sidebar _sidebar;

        public MainWindow()
        {
            // i wish i could use xaml for this

            Title = "OpenTabletDriver";

            var textBox = new Label();
            var textBoxBinding = Binding.Property((MainWindowViewModel vm) => vm.IsConnected).Convert(b => b ? "Connected" : "Disconnected");
            textBox.TextBinding.BindDataContext(textBoxBinding);

            // // the icon could be just not retrieved from the view model, but cba for now
            // var iconBinding = Binding.Property((MainWindowViewModel vm) => vm.ApplicationIcon).Convert<Icon?>(i => new Icon(1, i));
            // this.BindDataContext(m => m.Icon, iconBinding);

            var expandButton = new ClickablePanel
            {
                Content = new CirclePlaceholder
                {
                    Radius = 13
                },
            };
            expandButton.BindDataContext(c => c.Command, (MainWindowViewModel vm) => vm.ToggleSidebarExpandCommand);

            _sidebar = new Sidebar
            {
                BackgroundColor = Colors.DarkSlateGray,
                Spacing = new Size(0, 0),
                ItemHeight = 60,
                DefaultWidth = 60,
                Items =
                {
                    new SidebarItem
                    {
                        Control = expandButton
                    },
                    new SidebarItem
                    {
                        Control = new CirclePlaceholder
                        {
                            Radius = 15
                        },
                        ExpandedControl = new Label
                        {
                            Text = "XP-Pen G640S",
                            TextAlignment = TextAlignment.Left,
                            Wrap = WrapMode.None,
                            VerticalAlignment = VerticalAlignment.Center,
                            TextColor = Colors.White
                        }
                    },
                    new SidebarItem
                    {
                        Control = new CirclePlaceholder
                        {
                            Radius = 13
                        },
                        ExpandedControl = new Label
                        {
                            Text = "Wacom CTL-4100",
                            TextAlignment = TextAlignment.Left,
                            Wrap = WrapMode.None,
                            VerticalAlignment = VerticalAlignment.Center,
                            TextColor = Colors.White
                        }
                    },
                },
            };
            _sidebar.BindDataContext(s => s.Expand, (MainWindowViewModel vm) => vm.SidebarExpanded);

            var heightBinding = Binding.Property((MainWindow mw) => mw.Size).Convert(s => s.Height);
            _sidebar.Bind(s => s.Height, this, heightBinding);

            Content = new DockingLayout
            {
                Left = _sidebar,
            };
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            Debug.Assert(DataContext is MainWindowViewModel);
            base.OnDataContextChanged(e);
        }
    }
}
