using OvulaeShared.Enums;
using OvulaeShared.Enums.ModuleEnums;
using OvulaeShared.Services.Module.OvulationServices;

namespace OvulaeApp.Views.OvulationTracker.Dashboard
{
    [QueryProperty(nameof(ovulationPhase), "ovulationPhase")]
    [QueryProperty(nameof(datesRange), "datesRange")]
    public partial class OvulationPhaseDetailsPage : ContentPage
    {
        public string ovulationPhase { get; set; }
        public string datesRange { get; set; }

        private readonly IOvulationService _ovServ;
        
        public OvulationPhaseDetailsPage(IOvulationService ovServ)
        {
            InitializeComponent();

            BaseTabs.SetSpinnerLoader(Spinner);
            _ovServ = ovServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (ovulationPhase != OvulationPhase.None.GetDisplayName())
                {
                    var phase = EnumHelper.GetEnumValueFromName<OvulationPhase>(ovulationPhase);
                    var viewModel = _ovServ.GetCurrentPhaseDetailsByPhase(phase);
                    
                    viewModel.PhaseDateRange = datesRange;

                    BindingContext = viewModel;
                }

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup, MenopauseTrackerOnBoard);
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
