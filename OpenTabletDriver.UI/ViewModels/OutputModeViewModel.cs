using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenTabletDriver.UI.ViewModels
{
    public partial class OutputModeViewModel : ViewModelBase
    {
        private readonly TabletViewModel _tabletViewModel;

        [ObservableProperty]
        private double _outputXOffset;

        [ObservableProperty]
        private double _outputYOffset;

        [ObservableProperty]
        private double _outputWidth;

        [ObservableProperty]
        private double _outputHeight;

        [ObservableProperty]
        private double _inputXOffset;

        [ObservableProperty]
        private double _inputYOffset;

        [ObservableProperty]
        private double _inputWidth;

        [ObservableProperty]
        private double _inputHeight;

        [ObservableProperty]
        private double _inputRotation;

        [ObservableProperty]
        private bool _matchOutputAspectRatio;

        [ObservableProperty]
        private bool _clipInputWithinArea;

        [ObservableProperty]
        private bool _discardInputOutsideArea;

        [ObservableProperty]
        private bool _keepAreasWithinBounds;

        public ImmutableArray<Area> OutputAreaBounds { get; }
        public Area InputAreaBounds { get; }

        public OutputModeViewModel(TabletViewModel tabletViewModel)
        {
            _tabletViewModel = tabletViewModel;

            OutputAreaBounds = ImmutableArray<Area>.Empty;

            var tabletSpecs = _tabletViewModel.Handler.Configuration.Specifications;
            var tabletDigitizerSpecs = tabletSpecs.Digitizer!;
            InputAreaBounds = new Area
            {
                Width = tabletDigitizerSpecs.Width,
                Height = tabletDigitizerSpecs.Height
            };

            // setup output area
            // setup input area
            // setup output mode selector
            // setup advanced settings
        }
    }
}
