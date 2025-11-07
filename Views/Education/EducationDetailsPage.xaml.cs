using System.Threading.Tasks;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.ViewModels.Education;

namespace OvulaeApp.Views.Education
{
    [QueryProperty(nameof(BookId), "bookId")]
    public partial class EducationDetailsPage : ContentPage
    {
        private readonly IEducationService _eduServ;
        private EducationDetailsViewModel viewModel;

        private bool ArticleLiked;

        public int BookId { get; set; }
        
        public EducationDetailsPage(IEducationService eduServ)
        {
            InitializeComponent();

            _eduServ = eduServ;

            ArticleLiked = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (viewModel == null && BookId > 0)
                {
                    viewModel = new EducationDetailsViewModel(_eduServ, BookId);
                    BindingContext = viewModel;
                }

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
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening education data.", "OK");
            }
        }

        private void LikeArticle_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                ArticleLiked = !ArticleLiked;

                LikeArticleImg.Source = ArticleLiked ? $"love.png" : $"love_icon.png";
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Failed to like education article: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening education data.", "OK");
            }
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
