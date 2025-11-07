
namespace OvulaeApp.Views.Components.Modals
{
    public partial class SpinnerLoader : ContentView
    {
        public SpinnerLoader()
        {
            InitializeComponent();
            this.Opacity = 1; // Start fully visible (if needed)
            this.IsVisible = false; // Start hidden
        }

        public async Task ShowSpinnerAsync()
        {
            try
            {
                this.IsVisible = true;
                this.Opacity = 1; // Ensure fully visible
                this.ForceLayout(); // Force UI update (if needed)
                await Task.Delay(50); // Small delay (optional, remove if unnecessary)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Spinner error: {ex.Message}");
            }
        }

        public async Task HideSpinnerAsync()
        {
            try
            {
                this.IsVisible = false;
                // No need to change Opacity since the control is hidden
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Spinner error: {ex.Message}");
            }
        }
    }
}