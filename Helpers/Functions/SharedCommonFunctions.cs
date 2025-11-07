using System;
using OvulaeApp.Helpers.Enums;
using OvulaeShared.Models.User;

namespace OvulaeApp.Helpers.Functions
{
    public static class SharedCommonFunctions
    {
        public static int GetCurrentPregnancyWeekFromLMP(DateTime? lmpDate)
        {
            try
            {
                if (lmpDate == null)
                    return 0;

                DateTime today = DateTime.Today;
                int totalDays = (today - lmpDate.Value.Date).Days;
                int week = (int)Math.Floor(totalDays / 7.0) + 1;

                return Math.Clamp(week, 1, 42);
            }
            catch
            {
                return 1;
            }
        }

        public static DateTime GetPregnancyDueDateFromLMP(DateTime lmpDate)
        {
            return lmpDate.AddDays(280);
        }

        public static int GetCurrentPregnancyWeekFromDueDate(DateTime? dueDate)
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime estimatedLMP = dueDate.Value.AddDays(-280);
                int totalDays = (today - estimatedLMP).Days;
                int week = (int)Math.Floor(totalDays / 7.0) + 1;
                return Math.Clamp(week, 1, 42);
            }
            catch
            {
                return 1;
            }
        }

        public static int GetCurrentPregnancyWeekFromLMPOrDueDate(DateTime? lmpDate, DateTime? dueDate)
        {
            try
            {
                int weekFromLMP = GetCurrentPregnancyWeekFromLMP(lmpDate);
                int weekFromDueDate = GetCurrentPregnancyWeekFromDueDate(dueDate);

                return Math.Max(weekFromLMP, weekFromDueDate);
            }
            catch
            {
                return 1;
            }
        }

        public static int GetCurrentCycleDayFromLMP(DateTime? lmp, int cycleLength)
        {
            DateTime lmpDate = lmp ?? DateTime.Today.AddDays(-5);
            int daysSinceLmp = (DateTime.Today - lmpDate).Days;
            return (daysSinceLmp % cycleLength) + 1;
        }

        public static OvulationPhase GetOvulationPhase(int currentCycleDay)
        {
            if (currentCycleDay >= 1 && currentCycleDay <= 5) return OvulationPhase.Menstrual;
            else if (currentCycleDay >= 6 && currentCycleDay <= 13) return OvulationPhase.Follicular;
            else if (currentCycleDay >= 14 && currentCycleDay <= 16) return OvulationPhase.Ovulation;
            else return OvulationPhase.Luteal;
        }

        /// <summary>
        /// Returns the pregnancy start date (same as LMP) and estimated conception date (LMP + 14 days).
        /// </summary>
        /// <param name="lmpDate">The Last Menstrual Period date provided by the user.</param>
        /// <returns>Tuple: (pregnancyStartDate, estimatedConceptionDate)</returns>
        public static (DateTime PregnancyStartDate, DateTime EstimatedConceptionDate) CalculatePregnancyStartDates(DateTime lmpDate)
        {
            DateTime pregnancyStartDate = lmpDate.Date; // LMP as start of gestation
            DateTime estimatedConceptionDate = lmpDate.AddDays(14).Date; // Approximate conception

            return (pregnancyStartDate, estimatedConceptionDate);
        }

        public static double CalculateBMI(UserBodyMetric bodyMetric)
        {
            bool weightInKg = bodyMetric.WeightUnit?.ToLower() == "kg";
            double weight = weightInKg ? bodyMetric.Weight : ConvertPoundsToKg(bodyMetric.Weight);

            bool heightInCm = bodyMetric.HeightUnit?.ToLower() == "cm";
            double heightCm = heightInCm ? bodyMetric.Height : ConvertInchesToCm(bodyMetric.Height);

            double heightMeters = heightCm / 100.0;

            if (weight <= 0 || heightMeters <= 0)
                return 0;

            double bmi = weight / (heightMeters * heightMeters);
            return Math.Round(bmi, 1);
        }

        // Conversion helpers
        private static double ConvertPoundsToKg(double lbs) => lbs * 0.453592;
        private static double ConvertInchesToCm(double inches) => inches * 2.54;

    }
}
