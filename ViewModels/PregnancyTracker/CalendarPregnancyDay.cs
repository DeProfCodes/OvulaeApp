using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.ViewModels.Calendar;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.ViewModel.Symptoms;
using OvulaeShared.ViewModel.Tips;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class CalendarPregnancyDay
    {
        public CalendarDayViewModel Day { get; set; }
        public PregnancyBabyData BabyDevelopmentDetails { get; set; }
        public SymptomsGroupItemsViewModel SymptomsData { get; set; }
        public TipsGroupItemsViewModel TipsData { get; set; }
        public int Week { get; set; }
    }
}
