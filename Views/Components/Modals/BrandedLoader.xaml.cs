using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Views.Components.Modals
{
    public partial class BrandedLoader : ContentView
    {
        public BrandedLoader()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        public async Task ShowAsync(string message = "Loading...", int lines = 1)
        {
            try
            {
                if (lines == 1)
                {
                    LoaderContainer.HeightRequest = 130;
                }
                else if (lines == 2)
                {
                    LoaderContainer.HeightRequest = 145;
                }

                LoaderMessage.Text = message;
                this.IsVisible = true;
                this.Opacity = 1; // Set opacity directly to 1 (fully visible)

                this.ForceLayout();
                await Task.Delay(50); // You might keep this if you need it for layout purposes
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
                this.ForceLayout();
                this.Opacity = 0;
                this.IsVisible = false;
                // No need to set Opacity here since the control will be hidden
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bar error: {ex.Message}");
            }
        }
    }
}