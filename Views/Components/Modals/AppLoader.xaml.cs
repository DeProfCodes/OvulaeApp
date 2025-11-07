
namespace OvulaeApp.Views.Components.Modals
{
    public partial class AppLoader : ContentView
    {
        public AppLoader()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        public async Task ShowAsync(string message = "Loading...")
        {
            try
            {
                LoaderMessage.Text = message;
                this.IsVisible = true;
                this.Opacity = 0;

                await this.FadeTo(1, 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bar error: {ex.Message}");
            }
        }

        public async Task HideAsync()
        {
            try
            {
                await this.FadeTo(0, 200);
                this.IsVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bar error: {ex.Message}");
            }
        }
    }
}