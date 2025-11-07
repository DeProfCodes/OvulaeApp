using OvulaeApp.Helpers.Enums;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;

namespace OvulaeApp.ViewModels.Marketing
{
    public class ModuleAdFeatureViewModel
    {
        public string MainTitle { get; set; }

        public string SubTitle { get; set; }

        public List<string> FeaturesImages { get; set; }

        public string FeatureImageSource { get; set; }

        public ModuleAdFeatureViewModel(ModuleFeatureType featureType)
        {
            LoadFeatureData(featureType);
        }

        private void LoadFeatureData(ModuleFeatureType featureType)
        {
            if (featureType == ModuleFeatureType.Calendar)
            {
                LoadModuleCalendarAdData();
            }
            else if (featureType == ModuleFeatureType.ChatBot)
            {
                LoadModuleChatBotAdData();
            }
            else if (featureType == ModuleFeatureType.PartnerShare)
            {
                LoadModulePartnerSharingAdData();
            }
            else if (featureType == ModuleFeatureType.EducationalBooks)
            {
                LoadModuleEducationAdData();
            }
            else if (featureType == ModuleFeatureType.Dashboard)
            {
                LoadModuleDashboardAdData();
            }
            else if (featureType == ModuleFeatureType.Affiliate)
            {
                MainTitle = "Your Success Dashboard";
                SubTitle = "Apply to become an affiliate and monitor your top stats, referrals, and earnings — all in one place 📈";
                FeatureImageSource = "settings_affiliate.png";
                FeaturesImages = new List<string>
                {
                    "become_affiliate_1.jpg", "become_affiliate_2.jpg",
                };
            }
            else
            {
                MainTitle = "Feature Not Available";
                SubTitle = "This feature is currently not available.";
                FeaturesImages = new List<string>();
                FeatureImageSource = "";
            }
        }

        private void LoadModuleCalendarAdData()
        {
            MainTitle = "Your Complete Calendar";
            SubTitle = "See the full month ahead with predictions tailored for you 🌸";
            FeatureImageSource = "calendar_clr_3.png";

            var goal = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;
            
            if (goal == ModuleType.PeriodTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    "period_calendar_1.jpg", "period_calendar_2.jpg",
                };
            }
            else if (goal == ModuleType.Ovulation.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    "ovulation_calendar_1.jpg", "ovulation_calendar_2.jpg"
                };
            }
            else if (goal == ModuleType.Pregnancy.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    "pregnancy_calendar_1.jpg", "pregnancy_calendar_2.jpg",
                };
            }
            else if (goal == ModuleType.MenopauseTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    "period_calendar_1.jpg", "period_calendar_2.jpg"
                };
            }
        }

        private void LoadModuleChatBotAdData()
        {
            MainTitle = "Your Trusted Gynecologist";
            SubTitle = "One‑on‑one support from a seasoned expert who’s here for you 💜";
            FeatureImageSource = "dr_chat_icon.png";

            var goal = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;

            var pregP = "pregnancy_chat_bot";
            var perP = "period_chat_bot";
            var ovrP = "ovulation_chat_bot";

            if (goal == ModuleType.PeriodTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"{perP}_1.jpg", $"{perP}_2.jpg", $"{perP}_3.jpg", $"{perP}_4.jpg", $"{perP}_5.jpg", $"{perP}_6.jpg",
                };
            }
            else if (goal == ModuleType.Ovulation.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"{ovrP}_1.jpg", $"{ovrP}_2.jpg", $"{ovrP}_3.jpg", $"{ovrP}_4.jpg", $"{ovrP}_5.jpg", $"{ovrP}_6.jpg"
                };
            }
            else if (goal == ModuleType.Pregnancy.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"{pregP}_1.jpg", $"{pregP}_2.jpg", $"{pregP}_3.jpg", $"{pregP}_4.jpg", $"{pregP}_5.jpg", $"{pregP}_6.jpg"
                };
            }
            else if (goal == ModuleType.MenopauseTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"{perP}_1.jpg", $"{perP}_2.jpg", $"{perP}_3.jpg", $"{perP}_4.jpg", $"{perP}_5.jpg", $"{perP}_6.jpg"
                };
            }
        }

        private void LoadModulePartnerSharingAdData()
        {
            MainTitle = "Choose to Share";
            SubTitle = "Invite someone you trust to follow your journey — a partner, friend, or loved one 🤝";
            FeatureImageSource = "partner_share.png";

            var goal = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;

            if (goal == ModuleType.PeriodTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"partner_sharing_onboard.jpg", $"period_partner_share.jpg"
                };
            }
            else if (goal == ModuleType.MenopauseTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"partner_sharing_onboard.jpg", $"period_partner_share.jpg"
                };
            }
            else if (goal == ModuleType.Ovulation.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"partner_sharing_onboard.jpg", $"ovulation_partner_share.jpg"
                };
            }
            else if (goal == ModuleType.Pregnancy.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"partner_sharing_onboard.jpg", $"pregnancy_partner_sharing.jpg"
                };
            }
        }

        private void LoadModuleEducationAdData()
        {
            MainTitle = "Learn. Grow. Thrive.";
            SubTitle = "Access a growing library of gynecologist‑approved books and up‑to‑date resources, constantly updated just for you 📚";
            FeatureImageSource = "books_stack_icon.png";

            var goal = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;

            if (goal == ModuleType.PeriodTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"period_books_1.jpg", $"period_books_2.jpg"
                };
            }
            else if (goal == ModuleType.MenopauseTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"period_books_1.jpg", $"period_books_2.jpg"
                };
            }
            else if (goal == ModuleType.Ovulation.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                   $"period_books_1.jpg", $"period_books_2.jpg", "pregnancy_books_1.jpg", $"pregnancy_books_2.jpg"
                };
            }
            else if (goal == ModuleType.Pregnancy.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"pregnancy_books_1.jpg", $"pregnancy_books_2.jpg"
                };
            }
        }

        private void LoadModuleDashboardAdData()
        {
            MainTitle = "All‑in‑One View";
            SubTitle = "Track your progress, see insights, and get expert tips — all in one beautiful dashboard 🌸";
            FeatureImageSource = "home_clr_3_icon.png";

            var goal = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;

            if (goal == ModuleType.PeriodTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    $"period_tracker_dash_1.jpg", $"ovulation_read_more.jpg",
                };
            }
            else if (goal == ModuleType.MenopauseTracker.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    "menopause_dash_1.jpg", $"menopause_dash_2.jpg", 
                };
            }
            else if (goal == ModuleType.Ovulation.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                   $"ovulation_read_more.jpg", $"period_tracker_dash_1.jpg", "pregnancy_dashboard_1.jpg"
                };
            }
            else if (goal == ModuleType.Pregnancy.GetDisplayName())
            {
                FeaturesImages = new List<string>
                {
                    "pregnancy_dashboard_1.jpg", $"ovulation_read_more.jpg"
                };
            }
        }
    }
}
