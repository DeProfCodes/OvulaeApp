using OvulaeApp.Helpers.Enums;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Components.Modals.Dashboard;
using OvulaeApp.Views.PeriodTracker.Dashboard;

namespace OvulaeApp.Views.Base
{
    public partial class BaseContentPage : ContentPage
    {
        // Public properties for global modal access in ALL child pages
        public BrandedLoader AppLoader => AppLoaderField;
        public SpinnerLoader Spinner => SpinnerField;
        public YesNoAlertModal YesNoPopup => YesNoPopupField;
        //public PregnancyCompleteModal PregnancyComplete => PregnancyCompleteField;
        public YesNoAlertModal YesNoModal => YesNoModalField;

        public BaseContentPage()
        {
            InitializeComponent();
        }

        public async Task ShowSpinnerAsync()
        {
            OverlayBlocker.IsVisible = true;
            await Spinner.ShowSpinnerAsync();
        }

        public async Task HideSpinnerAsync()
        {
            await Spinner.HideSpinnerAsync();
            OverlayBlocker.IsVisible = false;
        }

        public async Task ShowAppLoaderAsync(string message = "Loading...")
        {
            OverlayBlocker.IsVisible = true;
            await AppLoader.ShowAsync(message);
        }

        public async Task HideAppLoaderAsync()
        {
            await AppLoader.HideAsync();
            OverlayBlocker.IsVisible = false;
        }

        public async Task<ModalCloseType> ShowYesNoModalAsync(string title, string message)
        {
            OverlayBlocker.IsVisible = true;
            var result = await YesNoModal.ShowYesNoModal(title, message);
            OverlayBlocker.IsVisible = false;

            return result;
        }

        public async Task<ModalCloseType> ShowPregnancyCompleteOnboardingModal()
        {
            OverlayBlocker.IsVisible = true;
            var result = await PregnancyCompleteField.ShowOnboardingModal();
            OverlayBlocker.IsVisible = false;

            return result;
        }

        public async Task NavigateToPageWithLoader(string destination, string loaderMessage)
        {
            await ShowAppLoaderAsync(loaderMessage);

            await Shell.Current.GoToAsync(destination);

            await HideAppLoaderAsync();
        }

        // Similarly, add wrapper methods for DietModal, PregnancyCompleteModal, etc.
        public void BlockUserInteraction() => OverlayBlocker.IsVisible = true;
        public void UnblockUserInteraction() => OverlayBlocker.IsVisible = false;

    }
}