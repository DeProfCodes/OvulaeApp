using OvulaeShared.Enums;
using OvulaeShared.Enums.ModuleEnums;
using OvulaeShared.Services.Module.PeriodTrackerServices;

namespace OvulaeApp.Views.PeriodTracker.Dashboard
{
    [QueryProperty(nameof(ovulationPhase), "ovulationPhase")]
    [QueryProperty(nameof(datesRange), "datesRange")]
    public partial class PeriodPhaseDetailsPage : ContentPage
    {
        public string ovulationPhase { get; set; }
        public string datesRange { get; set; }

        private readonly IPeriodTrackerService _periodServ;
        
        public PeriodPhaseDetailsPage(IPeriodTrackerService periodServ)
        {
            InitializeComponent();

            _periodServ = periodServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (ovulationPhase != OvulationPhase.None.GetDisplayName())
                {
                    var phase = EnumHelper.GetEnumValueFromName<OvulationPhase>(ovulationPhase);
                    var viewModel = _periodServ.GetCurrentPhaseDetailsByPhase(phase);

                    viewModel.PhaseDateRange = datesRange;

                    BindingContext = viewModel;
                }

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch,
                    YesNoModal, MenopauseTrackerOnBoard
                );
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });
            }
            catch (Exception ex)
            {

            }
        }

    }
}
