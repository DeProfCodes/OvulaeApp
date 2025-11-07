using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Enums;

namespace OvulaeApp.ViewModels.Module
{
    public class ModulePhaseDataViewModel
    {
        public OvulationPhase ModulePhase { get; set; }

        public MenopauseStage MenopauseStage { get; set; }

        public List<ModuleDashboardCard> PhaseHighlights { get; set; }

        public ModulePhaseDetailsViewModel PhaseDetails { get; set; }

        public string PhaseSummary { get; set; }
    }
}
