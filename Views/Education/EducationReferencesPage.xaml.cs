using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.ViewModels.Education;

namespace OvulaeApp.Views.Education
{
    [QueryProperty(nameof(BookId), "bookId")]
    public partial class EducationReferencesPage : ContentPage
    {
        private readonly IEducationService _eduServ;
        private EducationReferencesViewModel viewModel;

        public int BookId { get; set; }
        
        public EducationReferencesPage(IEducationService eduServ)
        {
            InitializeComponent();

            _eduServ = eduServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                viewModel = new EducationReferencesViewModel(_eduServ, BookId);
                BindingContext = viewModel;

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoModal, MenopauseTrackerOnBoard,
                    PregnancyComplete
                );
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open education book details: {ex.Message}");
                //Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening education data.", "OK");
            }
        }

        private async void ReferenceLinkTapped(object sender, string urlParameter)
        {
            try
            {
                if (!string.IsNullOrEmpty(urlParameter))
                {
                    Uri uri = new Uri(urlParameter);
                    await Launcher.Default.OpenAsync(uri);
                }
            }
            catch
            {
                
            }
        }
    }
}
