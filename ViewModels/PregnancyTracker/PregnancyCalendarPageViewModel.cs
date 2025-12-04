using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.ViewModels.Calendar;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.Models.Symptoms;
using OvulaeShared.Models.Tips;
using System.Threading.Tasks;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Helpers.ModuleHelpers;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyCalendarPageViewModel : BaseViewModel
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
                if (_selectedDay != null)
                    _selectedDay.BackgroundColor = _selectedDay.IsDimmed ? Color.FromArgb("#F2F2F2") : _selectedDay.OriginalBackgroundColor;

                _selectedDay = value;

                if (_selectedDay != null)
                    _selectedDay.BackgroundColor = Colors.Gold;

                OnPropertyChanged();
            }
        }

        private DateTime? LMP;
        private DateTime pregnancyStartDate;

        private string _selectedDayHighlight;
        public string SelectedDayHighlight
        {
            get => _selectedDayHighlight;
            set { _selectedDayHighlight = value; OnPropertyChanged(); }
        }

        private string _selectedDayBabyDevelopmentTitle;
        public string SelectedDayBabyDevelopmentTitle
        {
            get => _selectedDayBabyDevelopmentTitle;
            set { _selectedDayBabyDevelopmentTitle = value; OnPropertyChanged(); }
        }

        private string _selectedDayBabyIntro;
        public string SelectedDayBabyIntro
        {
            get => _selectedDayBabyIntro;
            set { _selectedDayBabyIntro = value; OnPropertyChanged(); }
        }

        private string _selectedDayBabyFeatures;
        public string SelectedDayBabyFeatures
        {
            get => _selectedDayBabyFeatures;
            set { _selectedDayBabyFeatures = value; OnPropertyChanged(); }
        }

        private string _selectedDayMotherChanges;
        public string SelectedDayMotherChanges
        {
            get => _selectedDayMotherChanges;
            set { _selectedDayMotherChanges = value; OnPropertyChanged(); }
        }

        private string _selectedDaySymptomsTitle;
        public string SelectedDaySymptomsTitle
        {
            get => _selectedDaySymptomsTitle;
            set { _selectedDaySymptomsTitle = value; OnPropertyChanged(); }
        }

        private string _selectedDaySymptomsDescription;
        public string SelectedDaySymptomsDescription
        {
            get => _selectedDaySymptomsDescription;
            set { _selectedDaySymptomsDescription = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SymptomItem> _selectedDaySymptomItems;
        public ObservableCollection<SymptomItem> SelectedDaySymptomItems
        {
            get => _selectedDaySymptomItems;
            set { _selectedDaySymptomItems = value; OnPropertyChanged(); }
        }

        private string _selectedDayDoctorTip;
        public string SelectedDayDoctorTip
        {
            get => _selectedDayDoctorTip;
            set { _selectedDayDoctorTip = value; OnPropertyChanged(); }
        }

        private string _selectedDayTitle;
        public string SelectedDayTitle
        {
            get => _selectedDayTitle;
            set { _selectedDayTitle = value; OnPropertyChanged(); }
        }

        private bool _showMothChanges;
        public bool ShowMotherChanges
        {
            get => _showMothChanges;
            set { _showMothChanges = value; OnPropertyChanged(); }
        }

        private string _footerMessage;
        public string FooterMessage
        {
            get => _footerMessage;
            set { _footerMessage = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TipItem> _selectedDayTipItems;
        public ObservableCollection<TipItem> SelectedDayTipItems
        {
            get => _selectedDayTipItems;
            set { _selectedDayTipItems = value; OnPropertyChanged(); }
        }

        public BottomSheetTray DayDetailsTray { get; set; }

        public Dictionary<DateTime, CalendarPregnancyDay> PregnancyDaysLookup { get; set; } = new();

        private ObservableCollection<PregnancyWeekLegendItem> _pregnancyWeekLegends;
        public ObservableCollection<PregnancyWeekLegendItem> PregnancyWeekLegends
        {
            get => _pregnancyWeekLegends;
            set { _pregnancyWeekLegends = value; OnPropertyChanged(); }
        }

        public PregnancyCalendarPageViewModel()
        {
            LoadCalendarData();
        }

        private void LoadCalendarData()
        {
            CalendarMonths = new ObservableCollection<CalendarMonthViewModel>();

            var today = DateTime.Now;
            FooterMessage = "*Select a day to view that week's summary⭐";

            LMP = LocalStorageService.UserCycleProfile.LastPeriodDate;

            if (LMP.HasValue)
                (pregnancyStartDate, _) = SharedCommonFunctions.CalculatePregnancyStartDates(LMP.Value);
            else
                pregnancyStartDate = today;

            for (int i = -6; i <= 6; i++)
            {
                var month = today.AddMonths(i);
                var rows = BuildCalendarGridWithFiller(month);
                var monthVM = new CalendarMonthViewModel
                {
                    MonthDate = month,
                    MonthTitle = month.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                    Days = new ObservableCollection<CalendarDayViewModel>(GenerateDaysForMonth(month)),
                    Rows = rows,
                    DaysFlatList = new ObservableCollection<CalendarDayViewModel>(rows.SelectMany(row => row.Cells))
                };
                CalendarMonths.Add(monthVM);
            }

            CurrentMonthIndex = 6;
            LoadMonth(CurrentMonthIndex);
        }

        /// <summary>
        /// Call after ApplyPregnancyDataToCalendar to refresh the legend based on current month's weeks.
        /// </summary>
        public void UpdatePregnancyWeekLegend()
        {
            var legends = new ObservableCollection<PregnancyWeekLegendItem>();

            var firstDay = new DateTime(CurrentMonth.MonthDate.Year, CurrentMonth.MonthDate.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            for (DateTime day = firstDay; day <= lastDay; day = day.AddDays(1))
            {
                int week = (int)((day - pregnancyStartDate).TotalDays / 7) + 1;
                if (week < 1 || week > 42) continue;

                if (!legends.Any(x => x.WeekNumber == week))
                {
                    legends.Add(new PregnancyWeekLegendItem
                    {
                        WeekNumber = week,
                        FruitEmoji = GetWeekEmoji(week),
                        BackgroundColor = GetWeekBackgroundColor(week)
                    });
                }
            }

            PregnancyWeekLegends = legends;
        }

        public Dictionary<int, (DateTime Start, DateTime End)> BuildPregnancyWeekDateRanges(DateTime pregnancyStartDate, int totalWeeks = 42)
        {
            var result = new Dictionary<int, (DateTime Start, DateTime End)>();
            for (int week = 1; week <= totalWeeks; week++)
            {
                var start = pregnancyStartDate.AddDays((week - 1) * 7);
                var end = start.AddDays(6);
                result[week] = (start, end);
            }
            return result;
        }

        public void ApplyPregnancyDataToCalendar(PregnancyAllWeeksData pregnancyData, DateTime pregnancyStartDate)
        {
            PregnancyDaysLookup.Clear();
            var weekRanges = BuildPregnancyWeekDateRanges(pregnancyStartDate);

            foreach (var month in CalendarMonths)
            {
                foreach (var row in month.Rows)
                {
                    foreach (var day in row.Cells)
                    {
                        if (day.Date == null) continue;

                        foreach (var kvp in weekRanges)
                        {
                            int week = kvp.Key;
                            var (start, end) = kvp.Value;

                            if (day.Date.Value.Date >= start.Date && day.Date.Value.Date <= end.Date)
                            {
                                bool isToday = day.Date.Value.Date == DateTime.Now.Date;

                                var weekData = pregnancyData.PregWeekData.FirstOrDefault(p => p.PregnancyInfo.Week == week);
                                var pregData = weekData?.PregnancyInfo;
                                var babyData = weekData?.BabyDevelopmentDetails;
                                var symptomsData = pregnancyData.Symptoms.FirstOrDefault(s => s.SymptomsGroup.Weeks == week.ToString());
                                var tipsData = pregnancyData.Tips.FirstOrDefault(t => t.TipsGroup.Weeks == week.ToString());

                                if (pregData != null)
                                {
                                    day.HasEvent = true;
                                    day.EventIcon = "bell.png";
                                    day.EventDetails = pregData.PregnancyHighlight;
                                    day.BackgroundColor = !isToday ? GetWeekBackgroundColor(week) : OvulaeColors.COLOR.ThemeClrMain;
                                    day.OriginalBackgroundColor = !isToday ? GetWeekBackgroundColor(week) : OvulaeColors.COLOR.ThemeClrMain;
                                }

                                PregnancyDaysLookup[day.Date.Value.Date] = new CalendarPregnancyDay
                                {
                                    Day = day,
                                    BabyDevelopmentDetails = babyData,
                                    SymptomsData = symptomsData,
                                    TipsData = tipsData,
                                    Week = week
                                };

                                break;
                            }
                        }
                    }
                }
            }
            UpdatePregnancyWeekLegend();
        }

        private void LoadMonth(int index)
        {
            if (index >= 0 && index < CalendarMonths.Count)
            {
                CurrentMonth = CalendarMonths[index];
                UpdatePregnancyWeekLegend();
            }
        }

        private async Task OnDaySelected(CalendarDayViewModel day)
        {
            if (day != null && !string.IsNullOrEmpty(day.DayNumber))
                SelectedDay = day;

            SelectedDayHighlight = day.EventDetails ?? "No highlight for this day.";

            if (day.Date.HasValue && PregnancyDaysLookup.TryGetValue(day.Date.Value.Date, out var pregDay))
            {
                SelectedDayTitle = $"{day.Date: dd MMM yyyy}: Week {pregDay.Week}";

                var babyData = pregDay.BabyDevelopmentDetails;
                if (babyData != null)
                {
                    SelectedDayBabyDevelopmentTitle = babyData.Title;
                    SelectedDayBabyIntro = string.Join("\n• ", babyData.BabyIntroInfo);
                    SelectedDayBabyFeatures = string.Join("\n", babyData.Features);
                    SelectedDayMotherChanges = string.Join("\n", babyData.MotherChanges);
                    ShowMotherChanges = babyData.MotherChanges.Count > 0;
                }
                else
                {
                    SelectedDayBabyDevelopmentTitle = SelectedDayBabyIntro = SelectedDayBabyFeatures = SelectedDayMotherChanges = "No data for this day.";
                }

                var symptomsData = pregDay.SymptomsData;
                if (symptomsData != null)
                {
                    SelectedDaySymptomsTitle = symptomsData.SymptomsGroup.Title;
                    SelectedDaySymptomsDescription = symptomsData.SymptomsGroup.Description;
                    SelectedDaySymptomItems = new ObservableCollection<SymptomItem>(symptomsData.SymptomItems);
                }
                else
                {
                    SelectedDaySymptomsTitle = SelectedDaySymptomsDescription = "No symptoms data for this day.";
                    SelectedDaySymptomItems = new ObservableCollection<SymptomItem>();
                }

                var tipsData = pregDay.TipsData;
                if (tipsData != null)
                {
                    SelectedDayDoctorTip = tipsData.TipsGroup.TipFromDoctor;
                    SelectedDayTipItems = new ObservableCollection<TipItem>(tipsData.TipItems);
                }
                else
                {
                    SelectedDayDoctorTip = "No tips for this day.";
                    SelectedDayTipItems = new ObservableCollection<TipItem>();
                }
                FooterMessage = "*Select a day to view that week's summary⭐";
                await DayDetailsTray.ShowAsync();
            }
            else
            {
                SelectedDayBabyDevelopmentTitle = SelectedDayBabyIntro = SelectedDayBabyFeatures = SelectedDayMotherChanges = "No data for this day.";
                SelectedDaySymptomsTitle = SelectedDaySymptomsDescription = "No symptoms data for this day.";
                SelectedDaySymptomItems = new ObservableCollection<SymptomItem>();
                SelectedDayDoctorTip = "No tips for this day.";
                SelectedDayTipItems = new ObservableCollection<TipItem>();
                FooterMessage = "Select Date For Current Month";
            } 
        }

        public List<CalendarDayViewModel> GenerateDaysForMonth(DateTime monthDate)
        {
            var days = new List<CalendarDayViewModel>();
            int daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);
            var today = DateTime.Today;

            for (int i = 1; i <= daysInMonth; i++)
            {
                var currentDate = new DateTime(monthDate.Year, monthDate.Month, i);
                bool isToday = currentDate == today;

                Color backgroundColor = Colors.White;
                string eventEmoji = "";
                string weekMarkerText = "";

                if (LMP.HasValue && currentDate >= LMP)
                {
                    int weekOfPregnancy = (int)((currentDate - LMP.Value).TotalDays / 7) + 1;
                    backgroundColor = GetWeekBackgroundColor(weekOfPregnancy);

                    // Calculate start date of this pregnancy week
                    var weekStartDate = LMP.Value.AddDays((weekOfPregnancy - 1) * 7);

                    if (currentDate.Date == weekStartDate.Date)
                    {
                        // Show fruit if defined, else show "W18"
                        var fruit = GetWeekEmoji(weekOfPregnancy);
                        if (!string.IsNullOrWhiteSpace(fruit))
                        {
                            eventEmoji = fruit; // fruit emoji on week start day
                        }
                        else
                        {
                            weekMarkerText = $"W{weekOfPregnancy}"; // week marker if no fruit
                        }
                    }
                }

                var weekMarkerTextColor = ColorUtils.GetWeekMarkerTextColor(isToday ? OvulaeColors.COLOR.ThemeClrMain : backgroundColor);

                days.Add(new CalendarDayViewModel
                {
                    DayNumber = i.ToString(),
                    IsDimmed = false,
                    BackgroundColor = isToday ? OvulaeColors.COLOR.ThemeClrMain : backgroundColor,
                    EventBorderColor = Colors.Transparent,
                    HasEvent = false,
                    Date = currentDate,
                    EventEmoji = eventEmoji,        // Only shows if fruit exists
                    WeekMarkerText = weekMarkerText,// Only shows if no fruit and it's week start day
                    WeekMarkerTextColor = weekMarkerTextColor
                });
            }

            return days;
        }

        public List<CalendarRowViewModel> BuildCalendarGridWithFiller(DateTime monthDate)
        {
            var result = new List<CalendarRowViewModel>();
            int daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);
            var allDays = new List<CalendarDayViewModel>();

            // Add only current month days
            allDays.AddRange(GenerateDaysForMonth(monthDate));

            int totalRows = (int)Math.Ceiling(allDays.Count / 7.0);
            for (int rowIdx = 0; rowIdx < totalRows; rowIdx++)
            {
                var row = allDays.Skip(rowIdx * 7).Take(7).ToList();

                for (int colIdx = 0; colIdx < row.Count; colIdx++)
                {
                    var day = row[colIdx];
                    if (!string.IsNullOrEmpty(day.EventEmoji) && day.Date.HasValue && LMP.HasValue)
                    {
                        int weekOfPregnancy = (int)((day.Date.Value - LMP.Value).TotalDays / 7) + 1;
                        var markerParts = new List<string> { "W" };
                        markerParts.AddRange(weekOfPregnancy.ToString().Select(c => c.ToString()));

                        int availableInRow = row.Count - (colIdx + 1);
                        int placed = 0;

                        if (availableInRow >= markerParts.Count)
                        {
                            // Write on the same row
                            for (int nextIdx = colIdx + 1; nextIdx < row.Count && placed < markerParts.Count; nextIdx++)
                            {
                                if (!row[nextIdx].IsDimmed)
                                {
                                    row[nextIdx].WeekMarkerText = markerParts[placed];
                                    placed++;
                                }
                            }
                        }
                        else if (rowIdx < totalRows - 1)
                        {
                            // Write on next row
                            var nextRow = allDays.Skip((rowIdx + 1) * 7).Take(7).ToList();
                            for (int nextIdx = 0; nextIdx < nextRow.Count && placed < markerParts.Count; nextIdx++)
                            {
                                if (!nextRow[nextIdx].IsDimmed)
                                {
                                    nextRow[nextIdx].WeekMarkerText = markerParts[placed];
                                    placed++;
                                }
                            }
                        }
                    }
                }

                result.Add(new CalendarRowViewModel
                {
                    Cells = new ObservableCollection<CalendarDayViewModel>(row)
                });
            }

            return result;
        }

        public void GoNextMonth()
        {
            if (CurrentMonthIndex < CalendarMonths.Count - 1)
                CurrentMonthIndex++;
        }

        public void GoPreviousMonth()
        {
            if (CurrentMonthIndex > 0)
                CurrentMonthIndex--;
        }

        private Color GetWeekBackgroundColor(int week)
        {
            return week switch
            {
                1 or 2 or 3 => Color.FromArgb("#FFF9C4"),
                4 => Color.FromArgb("#A5D6A7"),                
                5 => Color.FromArgb("#CE93D8"),                
                6 => Color.FromArgb("#FFCC80"),
                7 => Color.FromArgb("#90CAF9"),
                8 or 10 => Color.FromArgb("#F48FB1"),
                9 => Color.FromArgb("#F06292"),
                11 or 24 or 34 or 35 or 41 => Color.FromArgb("#AED581"),
                12 => Color.FromArgb("#CE93D8"),
                13 => Color.FromArgb("#FFCC80"),
                14 => Color.FromArgb("#FFF176"),
                15 => Color.FromArgb("#EF9A9A"),
                16 => Color.FromArgb("#81C784"),
                17 => Color.FromArgb("#A1887F"),
                18 => Color.FromArgb("#FFB74D"),
                19 => Color.FromArgb("#FFD54F"),
                20 => Color.FromArgb("#FFD54F"),
                21 => Color.FromArgb("#FFB74D"),
                22 => Color.FromArgb("#FFB74D"),
                23 => Color.FromArgb("#FFB74D"),
                25 => Color.FromArgb("#A5D6A7"),
                26 => Color.FromArgb("#AED581"),
                27 => Color.FromArgb("#80CBC4"),
                28 => Color.FromArgb("#BA68C8"),
                29 => Color.FromArgb("#FFB74D"),
                30 => Color.FromArgb("#81C784"),
                31 => Color.FromArgb("#FFF176"),
                32 => Color.FromArgb("#FFB74D"),
                33 => Color.FromArgb("#FFF176"),
                36 => Color.FromArgb("#AED581"),
                37 => Color.FromArgb("#81C784"),
                38 => Color.FromArgb("#FFD54F"),
                39 or 42 => Color.FromArgb("#4FC3F7"),
                40 => Color.FromArgb("#FFB74D"),
                _ => Color.FromArgb("#FFFFFF")
            };
        }

        private string GetWeekEmoji(int week)
        {
            return week switch
            {
                4 => "🌱",
                5 => "🌱",
                6 => "🌱",
                7 => "🫐",
                8 => "🍓",
                9 => "🍒",
                10 => "🍓",
                11 => "🍈",
                12 => "🟣",
                13 => "🍑",
                14 => "🍋",
                15 => "🍎",
                16 => "🥑",
                17 => "🥔",
                18 => "🍠",
                19 => "🥭",
                20 => "🍌",
                21 => "🥕",
                22 => "🧡",
                23 => "🍊",
                24 => "🍈",
                25 => "🥦",
                26 => "🥗",
                27 => "🥒",
                28 => "🍆",
                29 => "🎃",
                30 => "🟢",
                31 => "🥥",
                32 => "🎃",
                33 => "🍍",
                34 => "🍈",
                35 => "🍈",
                36 => "🥬",
                37 => "🌿",
                38 => "🧅",
                39 => "🍉",
                40 => "🎃",
                41 => "🍈",
                42 => "🍉",
                _ => ""
            };
        }
    }
}
