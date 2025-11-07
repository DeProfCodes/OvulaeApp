using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;

namespace OvulaeApp.Views.Components.Shared
{
    public partial class InputBlockerOverlay : ContentView
    {
        public InputBlockerOverlay()
        {
            InitializeComponent();
        }

        public async Task ShowAsync(uint fadeDuration = 150)
        {
            IsVisible = true;
            Opacity = 0;
            await this.FadeTo(1, fadeDuration);
        }

        public async Task HideAsync(uint fadeDuration = 150)
        {
            await this.FadeTo(0, fadeDuration);
            IsVisible = false;
        }
    }
}