using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace OvulaeApp.Views.Components.Modals
{
    public partial class BottomSheetTray : ContentView
    {
        public BottomSheetTray()
        {
            InitializeComponent();

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => HideAsync();
            this.GestureRecognizers.Add(tapGesture);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(BottomSheetTray), default(string));

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ContentSlot.BindingContext = this.BindingContext;
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public View TrayContent
        {
            get => ContentSlot.Content;
            set => ContentSlot.Content = value;
        }

        public async Task ShowAsync()
        {
            this.IsVisible = true;
            this.Opacity = 0;

            TrayContainer.TranslationY = 500; // Ensure off-screen before animate

            await Task.WhenAll(TrayContainer.TranslateTo(0, 0, 250, Easing.SinOut), this.FadeTo(1, 250, Easing.SinOut));
        }

        public async Task HideAsync()
        {
            await TrayContainer.TranslateTo(0, 500, 250, Easing.SinIn);
            this.IsVisible = false;
        }

        private async void CloseTrayTapped(object sender, TappedEventArgs e)
        {
            await HideAsync();
        }
    }
}
