using System;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Views.Components.Modals
{
    public partial class TrackerSwitcher : ContentView
    {
        public TrackerSwitcher()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        public async Task ShowAsync(ModuleType moduleName)
        {
            try
            {
                TrackerName.Text = moduleName.GetDisplayDescription();

                TrackerImage.Source = moduleName switch
                {
                    ModuleType.Pregnancy => "pregnancy_tracker_badge.png",
                    ModuleType.PeriodTracker => "period_tracker_badge.png",
                    ModuleType.Ovulation => "fertility_tracker_badge.png",
                    ModuleType.MenopauseTracker => "menopause_tracker_badge.png",
                    _ => "logo_main.png"
                };

                PregnancyFeatures.IsVisible = moduleName == ModuleType.Pregnancy;
                PeriodFeatures.IsVisible = moduleName == ModuleType.PeriodTracker;
                OvulationFeatures.IsVisible = moduleName == ModuleType.Ovulation;
                MenopauseFeatures.IsVisible = moduleName == ModuleType.MenopauseTracker;

                this.IsVisible = true;
                this.Opacity = 0;

                //this.ForceLayout();
                await Task.Delay(100); 

                await this.FadeTo(1, 110);
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
                await this.FadeTo(0, 110);
                this.IsVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bar error: {ex.Message}");
            }
        }
    }
}