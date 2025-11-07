using OvulaeApp.Helpers.Enums;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.ViewModels.Module;
using OvulaeShared.Enums;

namespace OvulaeApp.Views.MenopauseTracker.Dashboard
{
    [QueryProperty(nameof(menopauseStage), "menopauseStage")]
    public partial class MenopauseStageDetailsPage : ContentPage
    {
        public string menopauseStage { get; set; }
        
        private readonly IMenopauseService _menopauseServ;
        
        public MenopauseStageDetailsPage(IMenopauseService menopauseServ)
        {
            InitializeComponent();

            _menopauseServ = menopauseServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (!string.IsNullOrEmpty(menopauseStage))
                {
                    var stage = EnumHelper.GetEnumValueFromName<MenopauseStage>(menopauseStage);
                    var viewModel = _menopauseServ.GetCurrentPhaseDetailsForStage(stage);
                    BindingContext = viewModel;
                }

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup
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
