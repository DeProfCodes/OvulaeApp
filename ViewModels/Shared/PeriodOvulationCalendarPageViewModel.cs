using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.ViewModels.Calendar;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums.App;
using OvulaeShared.Services.Module.CycleServices;
using OvulaeShared.ViewModel.Module;
using OvulaeShared.Enums.ModuleEnums;

namespace OvulaeApp.ViewModels.Shared
{
    public class PeriodOvulationCalendarPageViewModel : BaseViewModel
    {
        private ObservableCollection<CalendarMonthViewModel> _calendarMonths;
        public ObservableCollection<CalendarMonthViewModel> CalendarMonths 
        { 
            get => _calendarMonths;
            set
            {
                _calendarMonths = value;
                OnPropertyChanged();
            }
        }

        private CalendarMonthViewModel _currentMonth;
        public CalendarMonthViewModel CurrentMonth
        {
            get => _currentMonth;
            set
            {
                _currentMonth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentMonthTitle));
            }
        }

        private int _currentMonthIndex;
        public int CurrentMonthIndex
        {
            get => _currentMonthIndex;
            set
            {
                if (value >= 0 && value < CalendarMonths.Count)
                {
                    _currentMonthIndex = value;
                    LoadMonth(value);
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentMonthTitle => CurrentMonth?.MonthTitle ?? "";

        public ICommand GoNextMonthCommand => new Command(() =>
        {
            if (CurrentMonthIndex < CalendarMonths.Count - 1)
                CurrentMonthIndex++;
        });

        public ICommand GoPreviousMonthCommand => new Command(() =>
        {
            if (CurrentMonthIndex > 0)
                CurrentMonthIndex--;
        });

        public ICommand SelectDayCommand => new Command<CalendarDayViewModel>(async (day) => await OnDaySelected(day));

        private CalendarDayViewModel _selectedDay;
        public CalendarDayViewModel SelectedDay
        {
            get => _selectedDay;
            set
            {
                _selectedDay = value;
                OnPropertyChanged();
            }
        }

        public string FooterMessage { get; set; } = "*Select a day to view phase details⭐";

        public BottomSheetTray DayDetailsTray { get; set; }

        public string SelectedPhaseTitle { get; set; }
        public string SelectedPhaseDescription { get; set; }
        public ObservableCollection<ModuleDashboardCard> SelectedPhaseCards { get; set; }

        private Dictionary<OvulationPhase, Color> PhaseColors;

        private Dictionary<OvulationPhase, string> PhaseEmojis;

        private SpinnerLoader spinner;

        private List<ModulePhaseDataViewModel> AllModuleData;

        public Dictionary<DateTime, CalendarPeriodOvulationDay> PhaseDaysLookup { get; set; } = new();

        private readonly ICycleService _cycleServ;
        private ModuleType moduleType;

        public PeriodOvulationCalendarPageViewModel(ICycleService cycleServ, List<ModulePhaseDataViewModel> data, ModuleType moduleType, SpinnerLoader spinner)
        {
            _cycleServ = cycleServ;
            _cycleServ.LoadCycleData(LocalStorageService.UserCycleProfile);

            AllModuleData = data;
            this.spinner = spinner;
            this.moduleType = moduleType;

            var ovulationColor = moduleType == ModuleType.Ovulation ? OvulaeColors.COLOR.ThemeClr2 : Color.FromArgb("#BBDEFB");
            var ovulationEmoji = moduleType == ModuleType.Ovulation ? "👶🏼" : "💡";

            PhaseColors = new()
            {
                { OvulationPhase.Menstrual, Color.FromArgb("#FFCDD2") },
                { OvulationPhase.Follicular, Color.FromArgb("#C8E6C9") },
                { OvulationPhase.Ovulation, ovulationColor },
                { OvulationPhase.Luteal, Color.FromArgb("#FFF9C4") },
            };
            
            PhaseEmojis = new()
            {
                { OvulationPhase.Menstrual, "🩸" },
                { OvulationPhase.Follicular, "🌱" },
                { OvulationPhase.Ovulation, ovulationEmoji },
                { OvulationPhase.Luteal, "🌙" },
            };

            

            LoadCalendarData();
        }

        private void LoadCalendarData()
        {
            CalendarMonths = new ObservableCollection<CalendarMonthViewModel>();
            var today = DateTime.Now;

            for (int i = -6; i <= 12; i++)
            {
                var month = today.AddMonths(i);
                var rows = BuildCalendarGridWithFiller(month);

                var monthVM = new CalendarMonthViewModel
                {
                    MonthTitle = month.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                    Rows = BuildCalendarGridWithFiller(month),
                    DaysFlatList = new ObservableCollection<CalendarDayViewModel>(rows.SelectMany(row => row.Cells))
                };
                CalendarMonths.Add(monthVM);
            }

            CurrentMonthIndex = 6;
            LoadMonth(CurrentMonthIndex);
        }

        private void LoadMonth(int index)
        {
            if (index >= 0 && index < CalendarMonths.Count)
                CurrentMonth = CalendarMonths[index];
        }

        private List<CalendarRowViewModel> BuildCalendarGridWithFiller(DateTime monthDate)
        {
            var rows = new List<CalendarRowViewModel>();
            var firstDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
            int leadingEmptyDays = (int)firstDayOfMonth.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);

            var allDays = new List<CalendarDayViewModel>();

            for (int i = leadingEmptyDays - 1; i >= 0; i--)
            {
                allDays.Add(new CalendarDayViewModel
                {
                    DayNumber = "",
                    IsDimmed = true,
                    BackgroundColor = Color.FromArgb("#F2F2F2"),
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(monthDate.Year, monthDate.Month, day);
                var cycleDay = _cycleServ.GetCycleDayForDate(date);
                var phase = _cycleServ.GetPhaseForCycleDay(cycleDay);
                var isFirstOfPhase = _cycleServ.IsFirstDayOfPhase(date); 

                allDays.Add(new CalendarDayViewModel
                {
                    DayNumber = day.ToString(),
                    IsDimmed = false,
                    BackgroundColor = PhaseColors[phase],
                    EventEmoji = (phase == OvulationPhase.Menstrual || phase == OvulationPhase.Ovulation) ? PhaseEmojis[phase] : (isFirstOfPhase ? PhaseEmojis[phase] : ""),
                    Date = date
                });
            }

            while (allDays.Count % 7 != 0)
            {
                allDays.Add(new CalendarDayViewModel
                {
                    DayNumber = "",
                    IsDimmed = true,
                    BackgroundColor = Color.FromArgb("#F2F2F2"),
                });
            }

            for (int i = 0; i < allDays.Count; i += 7)
            {
                rows.Add(new CalendarRowViewModel
                {
                    Cells = new ObservableCollection<CalendarDayViewModel>(allDays.Skip(i).Take(7))
                });
            }

            return rows;
        }

        public void ApplyModuleDataToCalendar()
        {
            PhaseDaysLookup.Clear();

            DateTime endDate = DateTime.Today.AddMonths(12);
            DateTime startDate = DateTime.Today.AddMonths(-12);
            DateTime current = startDate;

            while (current <= endDate)
            {
                var cycleDay = _cycleServ.GetCycleDayForDate(current);
                var phase = _cycleServ.GetPhaseForCycleDay(cycleDay);
                var phaseData = AllModuleData.FirstOrDefault(x => x.ModulePhase == phase);

                bool isFirstDayOfPhase = _cycleServ.IsFirstDayOfPhase(current);

                PhaseDaysLookup[current.Date] = new CalendarPeriodOvulationDay
                {
                    Date = current.Date,
                    Phase = phase,
                    PhaseData = phaseData,
                    IsFirstDayOfPhase = isFirstDayOfPhase
                };

                current = current.AddDays(1);
            }

            foreach (var month in CalendarMonths)
            {
                foreach (var row in month.Rows)
                {
                    foreach (var day in row.Cells)
                    {
                        if (day.Date.HasValue && PhaseDaysLookup.TryGetValue(day.Date.Value.Date, out var phaseDay))
                        {
                            bool isToday = day.Date.Value.Date == DateTime.Today.Date;

                            day.BackgroundColor = !isToday ? PhaseColors[phaseDay.Phase] : OvulaeColors.COLOR.ThemeClrMain;
                            day.OriginalBackgroundColor = day.BackgroundColor;

                            if (phaseDay.IsFirstDayOfPhase || phaseDay.Phase == OvulationPhase.Menstrual || phaseDay.Phase == OvulationPhase.Ovulation)
                            {
                                day.EventEmoji = PhaseEmojis[phaseDay.Phase];
                            }
                            else
                            {
                                day.EventEmoji = "";
                            }
                        }
                    }
                }
            }

            LoadMonth(CurrentMonthIndex);
        }

        private async Task OnDaySelected(CalendarDayViewModel day)
        {
            try
            {
                if (day?.Date == null)
                    return;

                SelectedDay = day;

                var cycleDay = _cycleServ.GetCycleDayForDate(day.Date.Value);
                var phase = _cycleServ.GetPhaseForCycleDay(cycleDay);
                var currentPhaseData = AllModuleData.FirstOrDefault(x => x.ModulePhase == phase);

                SelectedPhaseTitle = $"{day.Date.Value: dd MMM yyyy}: {phase} Phase {PhaseEmojis[phase]}";
                SelectedPhaseDescription = currentPhaseData?.PhaseDetails?.Description ?? "No description available for this phase.";
                SelectedPhaseCards = currentPhaseData != null ? new ObservableCollection<ModuleDashboardCard>(currentPhaseData.PhaseHighlights) : new();

                OnPropertyChanged(nameof(SelectedPhaseTitle));
                OnPropertyChanged(nameof(SelectedPhaseDescription));
                OnPropertyChanged(nameof(SelectedPhaseCards));

                if (DayDetailsTray != null)
                    await DayDetailsTray.ShowAsync();

                FooterMessage = $"Viewing {phase} Phase on {day.Date.Value:dd MMM yyyy}";
                OnPropertyChanged(nameof(FooterMessage));
            }
            catch
            {
                
            }
        }

        public void GoPreviousMonth()
        {
            if (CurrentMonthIndex > 0)
            {
                CurrentMonthIndex--;
                LoadMonth(CurrentMonthIndex);
            }
        }

        public void GoNextMonth()
        {
            if (CurrentMonthIndex < CalendarMonths.Count - 1)
            {
                CurrentMonthIndex++;
                LoadMonth(CurrentMonthIndex);
            }
        }
    }
}
