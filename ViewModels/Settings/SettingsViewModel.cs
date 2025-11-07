using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.About;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Dashboard;
using OvulaeApp.Views.MenopauseTracker.Dashboard;
using OvulaeApp.Views.OvulationTracker.Dashboard;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeApp.Views.PrivacyPolicy;
using OvulaeApp.Views.Settings;
using OvulaeApp.Views.Settings.EditSettingsPages;
using OvulaeApp.Views.Settings.EditSettingsPages.GeneralSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.MenopauseSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.OvulationSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.PeriodSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.PregnancySettings;
using OvulaeApp.Views.Settings.EditSettingsPages.SecuritySettings;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;

namespace OvulaeApp.ViewModels.Settings
{
    public class SettingsViewModel
    {
        public ICommand NavigateToSettingCommand { get; }

        private readonly SpinnerLoader spinnner;

        public string FullName { get; set; }

        public string Email { get; set; }


        public bool ShowPregnancySettings { get; set; } = false;
        public bool ShowPeriodSettings { get; set; } = false;
        public bool ShowMenopauseSettings { get; set; } = false;
        public bool ShowOvulationSettings { get; set; } = false;

        public SettingsViewModel(SpinnerLoader spinnner)
        {
            this.spinnner = spinnner;

            FullName = $"{LocalStorageService.UserDetails.Firstname} {LocalStorageService.UserDetails.Lastname}";
            Email = LocalStorageService.UserDetails.Email;

            ShowPregnancySettings = LocalStorageService.AppPrimaryGoal == ModuleType.Pregnancy;
            ShowPeriodSettings = LocalStorageService.AppPrimaryGoal == ModuleType.PeriodTracker;
            ShowOvulationSettings = LocalStorageService.AppPrimaryGoal == ModuleType.Ovulation;
            ShowMenopauseSettings = LocalStorageService.AppPrimaryGoal == ModuleType.MenopauseTracker;

            // Unified command used across all menu items
            NavigateToSettingCommand = new RelayCommand<object>(async (param) =>
            {
                try
                {
                    if (param is Border tappedBorder && tappedBorder.StyleId is string menuType)
                    {
                        VisualEventsHelper.TapDimEffectPurple(tappedBorder);
                        
                        await spinnner.ShowSpinnerAsync();
                        await Shell.Current.GoToAsync(GetEditSettingsPageName(menuType));
                        await spinnner.HideSpinnerAsync();
                    }
                }
                catch (Exception ex)
                {
                    await spinnner.HideSpinnerAsync();// Log or handle error as needed
                }
            });
        }

        public string GetEditSettingsPageName(string parameter)
        {
            switch(parameter)
            {
                //General
                case "PersonalInfo": return nameof(EditSettingsPersonalInfoPage);
                case "HealthProfile": return nameof(EditSettingsHealthInfoPage);
                case "Premium": return nameof(EditSettingsMembershipInfoPage);
                case "Affiliate": return nameof(EditSettingsAffiliateInfoPage);
                case "PartnerSharing": return nameof(PartnerSharingPage);
                //Pregnancy
                case "PregnancyProfile": return nameof(EditSettingsPregnancyDatesPage);
                case "PregnancyBabyInfo": return nameof(EditSettingsPregnancyBabyInfoPage);
                case "PregnancyNotifications": return nameof(PregnancyNotificationsPage);
                //Period
                case "PeriodCycleInfo": return nameof(EditSettingsPeriodCyclePage);
                case "PeriodNotifications": return nameof(PeriodNotificationsPage);
                //Ovulation
                case "OvulationCycleInfo": return nameof(EditSettingsOvulationPage);
                case "OvulationNotifications": return nameof(OvulationNotificationsPage);
                //Menopause
                case "MenopauseProfile": return nameof(EditMenopauseProfilePage);
                case "MenopauseNotifications": return nameof(MenopauseNotificationsPage);
                //Other
                case "Security": return nameof(EditSettingsSecurityPage);
                case "Terms": return nameof(TermsOfUsePage);
                case "About": return nameof(AboutUsPage);
                case "Help": return nameof(HelpPage);

                default: return "";
            }
        }
    }
}
