using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.UI.Models;

namespace OpenTabletDriver.UI.ViewModels
{
    public partial class TabletViewModel : ViewModelBase
    {
        private readonly TabletHandler _handler;

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

        public int TabletId => _handler.TabletId;
        public bool IsNormal => TabletState == InputDeviceState.Normal;
        public IAsyncRelayCommand StartPipelineCommand { get; }
        public IAsyncRelayCommand ApplyProfileCommand { get; }
        public IAsyncRelayCommand DiscardProfileCommand { get; }
        public IAsyncRelayCommand ResetProfileCommand { get; }

        public TabletViewModel(TabletHandler handler)
        {
            _handler = handler;

            _handler.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(TabletHandler.Profile))
                {
                    OutputModeViewModel = new OutputModeViewModel(this);
                    BindingsViewModel = new BindingsViewModel(this);
                    FiltersViewModel = new FiltersViewModel(this);
                }
                else if (e.PropertyName == nameof(TabletHandler.TabletState))
                {
                    TabletState = _handler.TabletState;
                }
            };

            _tabletState = _handler.TabletState;
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
            await _handler.SetTabletState(InputDeviceState.Normal);
        }

        private async Task ApplyProfileAsync()
        {
            await _handler.ApplyProfile();
            ProfileDirty = false;
        }

        private async Task DiscardProfileAsync()
        {
            await _handler.DiscardProfile();
            ProfileDirty = false;
        }

        private async Task ResetProfileAsync()
        {
            await _handler.ResetProfile();
            ProfileDirty = false;
        }
    }
}
