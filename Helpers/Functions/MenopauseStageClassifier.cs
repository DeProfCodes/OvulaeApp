using OvulaeApp.Helpers.Enums;
using OvulaeApp.ViewModels.MenopauseTracker;

namespace OvulaeApp.Helpers.Functions
{
    public static class MenopauseStageClassifier
    {
        public static MenopauseStage Classify(MenopauseProfileViewModel profile)
        {
            if (profile == null || !profile.Age.HasValue)
                return MenopauseStage.None;

            var today = DateTime.Today;
            var age = profile.Age.Value;

            // If LMP is known, use it
            if (profile.LastPeriodDate.HasValue)
            {
                var monthsSinceLMP = ((today.Year - profile.LastPeriodDate.Value.Year) * 12) + today.Month - profile.LastPeriodDate.Value.Month;

                if (monthsSinceLMP >= 12)
                {
                    return age >= 55 ? MenopauseStage.Postmenopause : MenopauseStage.Menopause;
                }

                return MenopauseStage.Premenopause;
            }

            if (age >= 55)
                return MenopauseStage.Postmenopause;

            return MenopauseStage.Premenopause;
        }
    }
}
