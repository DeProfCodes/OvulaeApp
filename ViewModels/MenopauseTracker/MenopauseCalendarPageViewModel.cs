using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.ViewModels.Calendar;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.ViewModels.MenopauseTracker
{
    public class MenopauseCalendarPageViewModel : BaseViewModel
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

        public string FooterMessage { get; set; } = "*Select a day to view menopause stage info 🌀";

        public BottomSheetTray DayDetailsTray { get; set; }

        public string SelectedStageTitle { get; set; }
        public string SelectedStageDescription { get; set; }
        public ObservableCollection<ModuleDashboardCard> SelectedStageCards { get; set; }

        private readonly IMenopauseService _menopauseService;

        private readonly Dictionary<MenopauseStage, Color> StageColors = new()
        {
            { MenopauseStage.Premenopause, Color.FromArgb("#FFE0B2") },
            { MenopauseStage.Menopause, Color.FromArgb("#FFCCBC") },
            { MenopauseStage.Postmenopause, Color.FromArgb("#C8E6C9") }
        };

        private readonly Dictionary<MenopauseStage, string> StageEmojis = new()
        {
            { MenopauseStage.Premenopause, "" },
            { MenopauseStage.Menopause, "" },
            { MenopauseStage.Postmenopause, "" }
        };

        private ModulePhaseDataViewModel _currentStageData;
        private MenopauseStage _currentStage;

        public MenopauseCalendarPageViewModel(IMenopauseService menopauseService)
        {
            _menopauseService = menopauseService;

            _currentStage = _menopauseService.GetMenopauseStage();
            _currentStageData = _menopauseService.GetCurrentPhaseDataForStage(_currentStage);

            LoadCalendarData();
        }

        private void LoadCalendarData()
        {
            CalendarMonths = new ObservableCollection<CalendarMonthViewModel>();
            var today = DateTime.Now;

            for (int i = -6; i <= 12; i++)
            {
                var month = today.AddMonths(i);
                var days = BuildCalendarGridWithFiller(month);

                var monthVM = new CalendarMonthViewModel
                {
                    MonthTitle = month.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                    Rows = BuildCalendarGridWithFiller(month),
                    DaysFlatList = new ObservableCollection<CalendarDayViewModel>(days.SelectMany(row => row.Cells))
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
                    BackgroundColor = Color.FromArgb("#F2F2F2")
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(monthDate.Year, monthDate.Month, day);

                allDays.Add(new CalendarDayViewModel
                {
                    DayNumber = day.ToString(),
                    Date = date,
                    IsDimmed = false,
                    BackgroundColor = StageColors[_currentStage],
                    EventEmoji = StageEmojis[_currentStage]
                });
            }

            while (allDays.Count % 7 != 0)
            {
                allDays.Add(new CalendarDayViewModel
                {
                    DayNumber = "",
                    IsDimmed = true,
                    BackgroundColor = Color.FromArgb("#F2F2F2")
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

        private async Task OnDaySelected(CalendarDayViewModel day)
        {
            if (day?.Date == null) return;

            SelectedStageTitle = $"{day.Date.Value:dd MMM yyyy}: {_currentStage} Stage {StageEmojis[_currentStage]}";
            SelectedStageDescription = _currentStageData?.PhaseDetails?.Description ?? "No details available.";
            SelectedStageCards = new ObservableCollection<ModuleDashboardCard>(_currentStageData?.PhaseHighlights ?? new List<ModuleDashboardCard>());

            OnPropertyChanged(nameof(SelectedStageTitle));
            OnPropertyChanged(nameof(SelectedStageDescription));
            OnPropertyChanged(nameof(SelectedStageCards));

            FooterMessage = $"Viewing {_currentStage} stage on {day.Date.Value:dd MMM yyyy}";
            OnPropertyChanged(nameof(FooterMessage));

            if (DayDetailsTray != null)
                await DayDetailsTray.ShowAsync();
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
