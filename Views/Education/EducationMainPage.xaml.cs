using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.ViewModels.Education;

namespace OvulaeApp.Views.Education
{
    public partial class EducationMainPage : ContentPage
    {
        private readonly IEducationService _eduServ;

        private EducationCoverViewModel viewModel;

        public EducationMainPage(IEducationService eduServ)
        {
            InitializeComponent();

            _eduServ = eduServ;

            BaseTabs.SetSpinnerLoader(Spinner);
            Header.SetSpinnerLoader(Spinner);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                viewModel = new EducationCoverViewModel(_eduServ, Spinner);

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
                Console.WriteLine($"Education page opening failed: {ex.Message}");

                //Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening Education page.", "OK");
            }
        }

        private void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBoxCloseBtn.IsVisible = !string.IsNullOrEmpty(SearchTextBox.Text);

            viewModel?.ApplySearchFilter(SearchTextBox.Text);
        }

        private void SeachboxCancelTextSearch_Tapped(object sender, TappedEventArgs e)
        {
            SearchTextBox.Text = "";
            SearchBoxCloseBtn.IsVisible = false;

            viewModel?.ApplySearchFilter(SearchTextBox.Text);
        }

        private async void ViewAllReferencesTapped(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Opening references....");

                await Shell.Current.GoToAsync(nameof(EducationReferencesPage));

                await AppLoader.HideAsync();
            }
            catch
            {

            }
        }
    }
}
