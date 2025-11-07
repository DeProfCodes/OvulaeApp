using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Calendar
{
    public class CalendarRowViewModel
    {
        public ObservableCollection<CalendarDayViewModel> Cells { get; set; }
    }
}
