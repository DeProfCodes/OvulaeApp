using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Models.Notifications;
using OvulaeApp.ViewModels.MenopauseTracker;
using OvulaeApp.ViewModels.Module;
using OvulaeShared.Enums;
using OvulaeShared.Enums.HealthProfile;
using OvulaeShared.Enums.ModuleEnums;
using OvulaeShared.Models.Notifications;
using OvulaeShared.ViewModel.Module;

namespace OvulaeApp.Services.LocalDataService.MenopauseServices
{
    public class MenopauseService : IMenopauseService
    {
        public static List<ModulePhaseDataViewModel> MenopauseDataStore => LoadAllMenopauseData();
        
        public MenopauseService() 
        {

        }

        private MenopauseProfileViewModel GetMenopauseProfile()
        {
            var result = new MenopauseProfileViewModel
            {
                LastPeriodDate = LocalStorageService.UserCycleProfile.LastPeriodDate,
                Age = LocalStorageService.UserBodyMetrics.Year,
                IsUsingHormonalTreatment = LocalStorageService.UserCycleProfile.Treatments.Contains(TreatmentTypes.HRT.GetDisplayDescription()),
                HasIrregularPeriods = LocalStorageService.UserCycleProfile.PeriodIrregularityType == PeriodIrregularityType.OccasionallyIrregular ||
                                      LocalStorageService.UserCycleProfile.PeriodIrregularityType == PeriodIrregularityType.FrequentlyIrregular,
                HasSkippedMultiplePeriods = LocalStorageService.UserCycleProfile.PeriodIrregularityType == PeriodIrregularityType.FrequentlyIrregular,
                HasHotFlashes = LocalStorageService.UserCycleProfile.Symptoms.Contains(SymptomsTypes.HotFlashes.GetDisplayName()),
                HasNightSweats = LocalStorageService.UserCycleProfile.Symptoms.Contains(SymptomsTypes.NightSweats.GetDisplayName()),
                HasMoodSwings = LocalStorageService.UserCycleProfile.Symptoms.Contains(SymptomsTypes.MoodChanges.GetDisplayName()),
                HasSleepDisturbance = LocalStorageService.UserCycleProfile.Symptoms.Contains(SymptomsTypes.SleepProblems.GetDisplayName()),
            };
            return result;
        }

        public MenopauseStage GetMenopauseStage()
        {
            var stage = MenopauseStageClassifier.Classify(GetMenopauseProfile());
            return stage;
        }

        public ModulePhaseDataViewModel GetCurrentPhaseData()
        {
            var phase = GetMenopauseStage();
            return MenopauseDataStore.FirstOrDefault(x => x.MenopauseStage == phase);
        }

        public ModulePhaseDataViewModel GetCurrentPhaseDataForStage(MenopauseStage stage)
        {
            return MenopauseDataStore.FirstOrDefault(x => x.MenopauseStage == stage);
        }

        public ModulePhaseDetailsViewModel GetCurrentPhaseDetails()
        {
            var phase = GetMenopauseStage();
            return MenopauseDataStore.FirstOrDefault(x => x.MenopauseStage == phase)?.PhaseDetails ?? new();
        }

        public ModulePhaseDetailsViewModel GetCurrentPhaseDetailsForStage(MenopauseStage stage)
        {
            return MenopauseDataStore.FirstOrDefault(x => x.MenopauseStage == stage)?.PhaseDetails ?? new();
        }

        public List<ModulePhaseDataViewModel> GetAllPhaseData() => MenopauseDataStore;

        public List<ScheduledNotification> BuildDayPlan(DateTime date, MenopauseNotificationPreferences prefs)
        {
            var result = new List<ScheduledNotification>();

            var phase = GetMenopauseStage();
            var phaseData = GetCurrentPhaseDataForStage(phase);

            if (phaseData == null)
                return result;

            var tips = phaseData.PhaseDetails?.Tips ?? new List<string>();
            var insights = phaseData.PhaseDetails?.Insights ?? new List<string>();
            var notifyTime = prefs.UseOwnTime ? prefs.PreferredNotificationTime : TimeSpan.FromHours(12);

            int tipIndex = (date.Day % tips.Count);
            int insightIndex = (date.Day % insights.Count);

            if (prefs.NotifyHotFlashes)
            {
                result.Add(new ScheduledNotification
                {
                    Title = "🔥 Hot Flashes Tip",
                    Message = tips[tipIndex],
                    ScheduledTime = date.Date + notifyTime
                });
            }

            if (prefs.NotifyMoodSwings)
            {
                result.Add(new ScheduledNotification
                {
                    Title = "🎭 Mood Support Tip",
                    Message = insights[insightIndex],
                    ScheduledTime = date.Date + notifyTime + TimeSpan.FromHours(4)
                });
            }

            if (prefs.NotifyNightSweats)
            {
                result.Add(new ScheduledNotification
                {
                    Title = "😴 Night Sweats Tip",
                    Message = tips[(tipIndex + 1) % tips.Count],
                    ScheduledTime = date.Date + notifyTime + TimeSpan.FromHours(8)
                });
            }

            // Add more notifications as preference flags grow

            return result;
        }

        private static List<ModulePhaseDataViewModel> LoadAllMenopauseData()
        {
            var iconMap = MenopausePhaseIconMapping;

            return new List<ModulePhaseDataViewModel>
            {
                //new ModulePhaseDataViewModel
                //{
                //    MenopauseStage = MenopauseStage.Premenopause,
                //    PhaseSummary = "Premenopause: Normal cycles with no noticeable changes.",
                //    PhaseHighlights = new List<ModuleDashboardCard>
                //    {
                //        new ModuleDashboardCard { Title = "Balanced Hormones ⚖️", Subtitle = "No significant symptoms present.", ImageSource = iconMap[MenopauseStage.Premenopause][0] },
                //        new ModuleDashboardCard { Title = "Track Trends 📊", Subtitle = "Log cycle patterns for later comparison.", ImageSource = iconMap[MenopauseStage.Premenopause][1] }
                //    },
                //    PhaseDetails = new ModulePhaseDetailsViewModel
                //    {
                //        PhaseTitle = "Premenopause 🟢",
                //        Description = "During this phase, menstruation continues regularly and menopause-related symptoms are absent.",
                //        HeroImage = "MenopauseTracker/heros/premenopause_hero.png",
                //        KeyChanges = new List<string> { "• Regular menstrual cycles", "• Balanced estrogen & progesterone", "• No major hormonal shifts" },
                //        Tips = new List<string> { "• Keep tracking your cycles", "• Maintain a healthy lifestyle" },
                //        Insights = new List<string> { "• Early tracking can help future predictions." }
                //    }
                //},
                new ModulePhaseDataViewModel
                {
                    MenopauseStage = MenopauseStage.Premenopause,
                    PhaseSummary = "Premenopause: Hormonal fluctuations cause symptoms and irregular periods.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Cycle Changes 🔄", Subtitle = "Irregular or skipped periods are common.", ImageSource = iconMap[MenopauseStage.Premenopause][0] },
                        new ModuleDashboardCard { Title = "Hot Flashes 🌡️", Subtitle = "A common sign of hormonal shifts.", ImageSource = iconMap[MenopauseStage.Premenopause][1] },
                        new ModuleDashboardCard { Title = "Mood Shifts 🎭", Subtitle = "Track emotional and sleep changes.", ImageSource = iconMap[MenopauseStage.Premenopause][2] },
                        new ModuleDashboardCard { Title = "Lifestyle Tips 💡", Subtitle = "Diet, sleep, and movement help.", ImageSource = iconMap[MenopauseStage.Premenopause][3] }
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Premenopause 🟠",
                        Description = "Hormonal fluctuations begin to affect menstrual cycles and may introduce symptoms like hot flashes, mood swings, and sleep issues.",
                        HeroImage = "perimenopause_hero.png",
                        KeyChanges = new List<string> { "• Irregular menstrual cycles", "• Estrogen levels rise and fall unpredictably", "• Hot flashes and other symptoms appear" },
                        Tips = new List<string> { "• Track symptoms and cycle changes", "• Focus on nutrition and stress reduction" },
                        Insights = new List<string> { "• Every experience is unique—tracking empowers you to take control." }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    MenopauseStage = MenopauseStage.Menopause,
                    PhaseSummary = "Menopause: The official end of menstruation — a powerful life milestone.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "12 Months Clear 🗓️", Subtitle = "Your periods have officially stopped.", ImageSource = iconMap[MenopauseStage.Menopause][0] },
                        new ModuleDashboardCard { Title = "Celebrate You 🎉", Subtitle = "Mark this powerful change in your journey.", ImageSource = iconMap[MenopauseStage.Menopause][1] },
                        new ModuleDashboardCard { Title = "Track Support Needs 🩺", Subtitle = "Monitor any symptoms or changes.", ImageSource = iconMap[MenopauseStage.Menopause][2] }
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Menopause 🔸",
                        Description = "Menopause is officially diagnosed after 12 months without a menstrual period. Hormone levels stabilize at low levels, and symptoms may evolve or persist.",
                        HeroImage = "menopause_hero.png",
                        KeyChanges = new List<string>
                        {
                            "• 12 consecutive months without a period",
                            "• Estrogen and progesterone stay low",
                            "• Some symptoms may continue or shift"
                        },
                        Tips = new List<string>
                        {
                            "• Acknowledge this milestone with care and self-love",
                            "• Track bone and cardiovascular health proactively",
                            "• Join a support group or seek guidance if needed"
                        },
                        Insights = new List<string>
                        {
                            "• Menopause is a natural milestone, not a medical condition.",
                            "• Empower yourself with information and support."
                        }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    MenopauseStage = MenopauseStage.Postmenopause,
                    PhaseSummary = "Postmenopause: Periods have stopped, and symptoms may change or stabilize.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "No More Periods ✅", Subtitle = "12+ months without menstruation.", ImageSource = iconMap[MenopauseStage.Postmenopause][0] },
                        new ModuleDashboardCard { Title = "Bone Health 🦴", Subtitle = "Monitor calcium and vitamin D intake.", ImageSource = iconMap[MenopauseStage.Postmenopause][1] },
                        new ModuleDashboardCard { Title = "Heart Wellness ❤️", Subtitle = "Menopause can increase cardiovascular risks.", ImageSource = iconMap[MenopauseStage.Postmenopause][2] },
                        new ModuleDashboardCard { Title = "Sexual Health 💖", Subtitle = "Support for intimacy and vaginal health.", ImageSource = iconMap[MenopauseStage.Postmenopause][3] }
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Postmenopause 🔵",
                        Description = "Menstruation has ended. It's important to monitor bone, heart, and hormonal health while managing any lingering symptoms.",
                        HeroImage = "postmenopause_hero.png",
                        KeyChanges = new List<string> { "• Menstruation has ceased", "• Risk of osteoporosis increases", "• Hormone levels stabilize at low levels" },
                        Tips = new List<string> { "• Maintain regular health checkups", "• Support your bones and heart with a balanced lifestyle" },
                        Insights = new List<string> { "• This phase marks a new chapter—embrace it with support and knowledge." }
                    }
                }
            };
        }

        public static Dictionary<MenopauseStage, List<string>> MenopausePhaseIconMapping = new()
        {
            [MenopauseStage.Menopause] = new()
            {
                "no_periods.png",        
                "hot_flash.png",         
                "mood_sleep.png",        
                "track_now.png",         
                "heart_health.png"       
            },

            [MenopauseStage.Premenopause] = new() 
            {
                "cycle_irregular.png",
                "hot_flash.png",
                "mood_sleep.png",
                "lifestyle_tips.png"
            },

            [MenopauseStage.Postmenopause] = new() 
            {
                "no_periods.png",
                "bone_health.png",
                "heart_health.png",
                "sexual_health.png"
            }
        };
    }
}
