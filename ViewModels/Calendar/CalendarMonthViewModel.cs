using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Calendar
{
    public class CalendarMonthViewModel : BaseViewModel
    {
        public string MonthTitle { get; set; }
        public ObservableCollection<CalendarDayViewModel> Days { get; set; }
        public List<CalendarRowViewModel> Rows { get; set; }
        public DateTime MonthDate { get; set; }

        private ObservableCollection<CalendarDayViewModel> _daysFlatList;
        public ObservableCollection<CalendarDayViewModel> DaysFlatList
        {
            get => _daysFlatList;
            set => SetProperty(ref _daysFlatList, value);
        }
    }
}
