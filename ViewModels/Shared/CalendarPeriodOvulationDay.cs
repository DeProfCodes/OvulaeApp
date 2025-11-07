using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.ViewModels.Module;

namespace OvulaeApp.ViewModels.Shared
{
    public class CalendarPeriodOvulationDay
    {
        public DateTime Date { get; set; }
        public OvulationPhase Phase { get; set; }
        public ModulePhaseDataViewModel PhaseData { get; set; }
        public bool IsFirstDayOfPhase { get; set; }
    }
}
