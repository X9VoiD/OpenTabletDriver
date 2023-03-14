using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.UI.Models;

namespace OpenTabletDriver.UI.ViewModels
{
    public partial class TabletViewModel : ViewModelBase
    {
        internal TabletHandler Handler { get; }

        [ObservableProperty]
        private OutputModeViewModel _outputModeViewModel;

        [ObservableProperty]
        private BindingsViewModel _bindingsViewModel;

        [ObservableProperty]
        private FiltersViewModel _filtersViewModel;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNormal))]
        private InputDeviceState _tabletState;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ApplyProfileCommand),
                                  nameof(DiscardProfileCommand))]
        private bool _profileDirty;

        public int TabletId => Handler.TabletId;
        public bool IsNormal => TabletState == InputDeviceState.Normal;
        public IAsyncRelayCommand StartPipelineCommand { get; }
        public IAsyncRelayCommand ApplyProfileCommand { get; }
        public IAsyncRelayCommand DiscardProfileCommand { get; }
        public IAsyncRelayCommand ResetProfileCommand { get; }

        public TabletViewModel(TabletHandler handler)
        {
            Handler = handler;

            Handler.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(TabletHandler.Profile))
                {
                    OutputModeViewModel = new OutputModeViewModel(this);
                    BindingsViewModel = new BindingsViewModel(this);
                    FiltersViewModel = new FiltersViewModel(this);
                }
                else if (e.PropertyName == nameof(TabletHandler.TabletState))
                {
                    TabletState = Handler.TabletState;
                }
            };

            _tabletState = Handler.TabletState;
            _outputModeViewModel = new OutputModeViewModel(this);
            _bindingsViewModel = new BindingsViewModel(this);
            _filtersViewModel = new FiltersViewModel(this);

            StartPipelineCommand = new AsyncRelayCommand(StartPipelineAsync, canExecute: () => TabletState != InputDeviceState.Normal);
            ApplyProfileCommand = new AsyncRelayCommand(ApplyProfileAsync, canExecute: () => ProfileDirty);
            DiscardProfileCommand = new AsyncRelayCommand(DiscardProfileAsync, canExecute: () => ProfileDirty);
            ResetProfileCommand = new AsyncRelayCommand(ResetProfileAsync);
        }

        private async Task StartPipelineAsync()
        {
            await Handler.SetTabletState(InputDeviceState.Normal);
        }

        private async Task ApplyProfileAsync()
        {
            await Handler.ApplyProfile();
            ProfileDirty = false;
        }

        private async Task DiscardProfileAsync()
        {
            await Handler.DiscardProfile();
            ProfileDirty = false;
        }

        private async Task ResetProfileAsync()
        {
            await Handler.ResetProfile();
            ProfileDirty = false;
        }
    }
}
