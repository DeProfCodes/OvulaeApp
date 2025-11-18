
using OvulaeApp.Models.Dashboard.Education;
using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeShared.Models.Notifications;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.ViewModel.Pregnancy;
using OvulaeShared.ViewModel.Symptoms;
using OvulaeShared.ViewModel.Tips;

namespace OvulaeApp.Helpers.Functions
{
    public class PregnancyHelper
    {
        public PregnancyHelper()
        {
            
        }

        private bool IsFirstDayOfWeek(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Monday;
        }

        private bool IsJournalDay(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Wednesday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool IsMilestoneDay(int currentWeek, DateTime date)
        {
            var milestoneWeeks = new HashSet<int> { 4, 8, 12, 16, 20, 24, 28, 32, 36, 40 };

            if (milestoneWeeks.Contains(currentWeek) && IsFirstDayOfWeek(date))
            {
                return true;
            }

            return false;
        }

        private int GetPregnancyWeekFromDate(DateTime date, DateTime? lmp)
        {
            int totalDays = (date - lmp.Value).Days;

            int week = (int)Math.Floor(totalDays / 7.0) + 1;

            return Math.Clamp(week, 1, 42);
        }

        public List<ScheduledNotification> BuildFullWeekPlan(DateTime startDate, DateTime? lmp, PregnancyNotificationPreferences prefs,
                                                                         List<PregnancyDataGroupViewModel> pregData, List<EducationCoverGroup> education,
                                                                         List<SymptomsGroupItemsViewModel> symptoms, List<TipsGroupItemsViewModel> tips)
        {
            var result = new List<ScheduledNotification>();
            var notifyTime = prefs.UseOwnTime ? prefs.PreferredNotificationTime : TimeSpan.FromHours(12);
            var sentTypes = new HashSet<string>(); // prevent repeat types in short period

            for (int i = 0; i < 7; i++)
            {
                DateTime day = startDate.Date.AddDays(i);
                int currentWeek = GetPregnancyWeekFromDate(day, lmp);

                var weekData = pregData.FirstOrDefault(w => w.PregnancyInfo.Week == currentWeek);
                if (weekData == null)
                    continue;

                var weekInfo = weekData.PregnancyInfo;
                var babyDetails = weekData.BabyDevelopmentDetails;

                // WEEKLY UPDATE
                if (prefs.NotifyWeeklyUpdates && IsFirstDayOfWeek(day) && sentTypes.Add("WeeklyUpdate"))
                {
                    result.Add(new ScheduledNotification
                    {
                        Title = $"Week {weekInfo.Week} Begins 🌟",
                        Message = weekInfo.PregnancyHighlight,
                        ScheduledTime = day + notifyTime
                    });
                }

                // BABY GROWTH FEATURE
                if (prefs.NotifyEducationalTips && babyDetails.Features.Any() && sentTypes.Add("BabyFeature"))
                {
                    int index = i % babyDetails.Features.Count;
                    result.Add(new ScheduledNotification
                    {
                        Title = $"Baby Growth Insight 🍼",
                        Message = babyDetails.Features[index],
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(2)
                    });
                }

                // SYMPTOM REMINDER
                if (prefs.NotifyLogSymptoms && sentTypes.Add("LogSymptoms"))
                {
                    result.Add(new ScheduledNotification
                    {
                        Title = "Log Your Symptoms 📝",
                        Message = "Don't forget to record your pregnancy symptoms today.",
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(3)
                    });
                }

                // DOCTOR TIP
                var doctorTip = tips.FirstOrDefault(t => t.TipsGroup.Weeks.Split(',').Select(int.Parse).Contains(currentWeek));
                if (prefs.NotifyEducationalTips && doctorTip != null && doctorTip.TipItems.Any() && sentTypes.Add("TipItem"))
                {
                    var tip = doctorTip.TipItems[i % doctorTip.TipItems.Count];
                    result.Add(new ScheduledNotification
                    {
                        Title = tip.Title,
                        Message = tip.Description,
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(4)
                    });
                }

                // SYMPTOM AWARENESS
                var symptom = symptoms.FirstOrDefault(s => s.SymptomsGroup.Weeks.Split(',').Select(int.Parse).Contains(currentWeek));
                if (prefs.NotifyLogSymptoms && symptom != null && symptom.SymptomItems.Any() && sentTypes.Add("SymptomItem"))
                {
                    var sym = symptom.SymptomItems[i % symptom.SymptomItems.Count];
                    result.Add(new ScheduledNotification
                    {
                        Title = sym.Title,
                        Message = sym.Description,
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(5)
                    });
                }

                // HYDRATION
                if (prefs.NotifyHydration && sentTypes.Add("Hydration"))
                {
                    result.Add(new ScheduledNotification
                    {
                        Title = "Stay Hydrated 💧",
                        Message = "Your body needs extra fluids — drink up!",
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(6)
                    });
                }

                // JOURNAL
                if (prefs.NotifyJournalReminder && IsJournalDay(day) && sentTypes.Add("Journal"))
                {
                    result.Add(new ScheduledNotification
                    {
                        Title = "Reflection Time 📔",
                        Message = "Take 5 minutes to write how you're feeling today.",
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(7)
                    });
                }

                // EDUCATION COVER
                var relevantEdu = education.FirstOrDefault(e => e.EducationCovers.Any(c => c.Categories.Contains("Body") && c.Categories.Any(cat => weekInfo.Week.ToString().Contains(cat))));
                if (prefs.NotifyEducationalTips && relevantEdu != null && sentTypes.Add("Education"))
                {
                    var cover = relevantEdu.EducationCovers[i % relevantEdu.EducationCovers.Count];
                    result.Add(new ScheduledNotification
                    {
                        Title = $"📖 {cover.Title}",
                        Message = cover.Description,
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(8)
                    });
                }

                // AFFIRMATION
                if (prefs.NotifyAffirmations && sentTypes.Add("Affirmation"))
                {
                    result.Add(new ScheduledNotification
                    {
                        Title = "💖 Daily Affirmation",
                        Message = "You are strong, capable, and nurturing life beautifully.",
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(9)
                    });
                }

                // KICK COUNT
                if (prefs.NotifyKickCount && sentTypes.Add("KickCount"))
                {
                    result.Add(new ScheduledNotification
                    {
                        Title = "👣 Baby Kick Count",
                        Message = "Feeling movement? Track baby kicks to monitor activity.",
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(10)
                    });
                }

                // MILESTONE CELEBRATION
                if (prefs.NotifyMilestoneCelebrations && IsMilestoneDay(currentWeek, day) && sentTypes.Add("Milestone"))
                {
                    result.Add(new ScheduledNotification
                    {
                        Title = "🎉 Weekly Milestone",
                        Message = $"You’ve made it to Week {currentWeek}! Keep going strong 💪",
                        ScheduledTime = day + notifyTime + TimeSpan.FromHours(11)
                    });
                }
            }

            return result;
        }
    }
}
