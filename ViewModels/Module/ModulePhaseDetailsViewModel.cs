using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Module
{
    public class ModulePhaseDetailsViewModel
    {
        public string PhaseTitle { get; set; }
        public string PhaseDateRange { get; set; }
        public string Description { get; set; }
        public List<string> KeyChanges { get; set; }
        public List<string> Tips { get; set; }
        public List<string> Insights { get; set; }
        public string HeroImage { get; set; }
    }
}
