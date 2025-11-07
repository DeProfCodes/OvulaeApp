using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.ViewModels.Splashscreen;

namespace OvulaeApp.Views.Splashscreen
{
    public partial class SplashscreenRenderPage : ContentPage, ISpinnerService
    {
        private readonly SplashscreenViewModel splashVM;
        public SplashscreenRenderPage()
        {
            InitializeComponent();

            splashVM = new SplashscreenViewModel(Spinner);

            BindingContext = splashVM;
        }

        public async Task ShowSpinnerAsync()
        {
            await Spinner.ShowSpinnerAsync();
        }

        public async Task HideSpinner()
        {
            await Spinner.HideSpinnerAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            if (splashVM.CurrentIndex > 0)
            {
                splashVM.CurrentIndex--;
                return true; 
            }
            else if (splashVM.CurrentIndex == 0)
            {
                Shell.Current.GoToAsync(nameof(SplashPrivacyPage));
                return true;
            }
            return base.OnBackButtonPressed();
        }

        private void NextButtonTapped(object sender, EventArgs e)
        {
            splashVM.GoNext();
        }
    }

}
