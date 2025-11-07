
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Models.Diet;

namespace OvulaeApp.Views.Components.Modals
{
    public partial class YesNoAlertModal : ContentView
    {
        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        public YesNoAlertModal()
        {
            InitializeComponent();
        }

        public Task<ModalCloseType> ShowYesNoModal(string title, string questionMessage)
        {
            QuestionMessage.Text = questionMessage;
            ModalTitle.Text = title;

            this.IsVisible = true;
            _resultCompletionSource = new TaskCompletionSource<ModalCloseType>();

            this.FadeTo(1, 200);

            return _resultCompletionSource.Task;
        }

        public async Task HideAsync(ModalCloseType result = ModalCloseType.None)
        {
            await this.FadeTo(0, 200);
            this.IsVisible = false;

            _resultCompletionSource?.TrySetResult(result);
        }

        private async void CloseButton_Tapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.CloseButton);
        }

        private async void NoButtonTapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.Reject);
        }

        private async void YesButtonTapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.Accept);
        }
    }
}