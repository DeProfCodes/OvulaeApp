using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.MenopauseTracker
{
    public class MenopauseProfileViewModel
    {
        public DateTime? LastPeriodDate { get; set; }
        public int? Age { get; set; }
        public bool IsUsingHormonalTreatment { get; set; }

        public bool HasIrregularPeriods { get; set; }
        public bool HasSkippedMultiplePeriods { get; set; }

        public bool HasHotFlashes { get; set; }
        public bool HasNightSweats { get; set; }
        public bool HasMoodSwings { get; set; }
        public bool HasSleepDisturbance { get; set; }
    }
}
