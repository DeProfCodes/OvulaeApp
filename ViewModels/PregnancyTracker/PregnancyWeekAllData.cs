using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.ViewModel.Pregnancy;
using OvulaeShared.ViewModel.Symptoms;
using OvulaeShared.ViewModel.Tips;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyAllWeeksData
    {
        public List<PregnancyDataGroupViewModel> PregWeekData { get; set; }

        public List<SymptomsGroupItemsViewModel> Symptoms { get; set; }

        public List<TipsGroupItemsViewModel> Tips { get; set; }
    }
}
