
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;

namespace OvulaeApp.ViewModels.Marketing
{
    public class OvulaeModuleBenefitsViewModel
    {
        public string MainTitle { get; set; }

        public string Subtitle { get; set; }

        public List<ProblemSolutionGroup> ProblemsSolutions { get; set; }

        public OvulaeModuleBenefitsViewModel()
        {
            LoadModuleBenefitsPageData();
        }

        private void LoadModuleBenefitsPageData()
        {
            var goal = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;

            var data = new List<ProblemSolutionGroup>
            {
                new ProblemSolutionGroup
                {
                    Problem = "Unpredictable cycles causing daily stress",
                    Solution = "Smart period & ovulation predictions based on your history"
                },
                new ProblemSolutionGroup
                {
                    Problem = "Painful periods without meaningful support",
                    Solution = "Reminders for periods, symptoms & medication"
                },
                new ProblemSolutionGroup
                {
                    Problem = "Confused about symptoms with no guidance",
                    Solution = "Personalized tips based on tracked symptoms"
                },
                new ProblemSolutionGroup
                {
                    Problem = "Trying to conceive without understanding ovulation",
                    Solution = "Fertile window insights with real-time ovulation tracking"
                },
                new ProblemSolutionGroup
                {
                    Problem = "Entering a new life stage with little menopause support",
                    Solution = "Track symptoms & transitions with expert-approved menopause tools"
                },
                new ProblemSolutionGroup
                {
                    Problem = "Feeling lost in a sea of misinformation",
                    Solution = "Access up-to-date, gynecologist-approved educational content anytime"
                },
                new ProblemSolutionGroup
                {
                    Problem = "Uncertain about baby’s weekly development",
                    Solution = "Track your baby’s growth and changes week-by-week"
                },
                new ProblemSolutionGroup
                {
                    Problem = "Overwhelmed by physical & emotional changes",
                    Solution = "Get guidance, tips & support for every pregnancy stage"
                }
            };

            if (goal == ModuleType.PeriodTracker.GetDisplayName())
            {
                MainTitle = "Ovulae's Period Tracker";
                Subtitle = "Medical-grade tracking with compassionate care";
                ProblemsSolutions = new List<ProblemSolutionGroup>
                {
                    data[0], data[1], data[2], data[3], data[5], data[4]
                };
            }
            else if (goal == ModuleType.Ovulation.GetDisplayName())
            {
                MainTitle = "Ovulae's Fertility Tracker";
                Subtitle = "Personalized support for your fertility journey";
                ProblemsSolutions = new List<ProblemSolutionGroup>
                {
                    data[3], data[0], data[2], data[1], data[5], data[4]
                };
            }
            else if (goal == ModuleType.Pregnancy.GetDisplayName())
            {
                MainTitle = "Ovulae's Pregnancy Tracker";
                Subtitle = "Support for every week of your journey";
                ProblemsSolutions = new List<ProblemSolutionGroup>
                {
                    data[6], data[7], data[2], data[1], data[5], data[4]
                };
            }
            else if (goal == ModuleType.MenopauseTracker.GetDisplayName())
            {
                MainTitle = "Ovulae's Menopause Tracker";
                Subtitle = "Support through every stage of change";
                ProblemsSolutions = new List<ProblemSolutionGroup>
                {
                    data[4], data[2], data[0], data[1], data[5], data[3]
                };
            }
            else
            {
                MainTitle = "Ovulae Health Insights";
                Subtitle = "Your personalized health journey starts here";
                ProblemsSolutions = data.Take(6).ToList();
            }
        }
    }
}
