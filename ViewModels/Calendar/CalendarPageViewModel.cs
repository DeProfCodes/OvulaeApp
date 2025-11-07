using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.ViewModels.Calendar
{
    public class CalendarPageViewModel : BaseViewModel
    {
        public ObservableCollection<CalendarMonthViewModel> CalendarMonths { get; set; }

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

        public ICommand SelectDayCommand => new Command<CalendarDayViewModel>(OnDaySelected);

        private CalendarDayViewModel _selectedDay;
        public CalendarDayViewModel SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != null)
                    _selectedDay.BackgroundColor = _selectedDay.IsDimmed ? Color.FromArgb("#F2F2F2") : Colors.White;

                _selectedDay = value;

                if (_selectedDay != null)
                    _selectedDay.BackgroundColor = Colors.Gold;

                SelectedDayDetailText = _selectedDay?.EventDetails;
            }
        }

        private string _selectedDayDetailText;
        public string SelectedDayDetailText
        {
            get => _selectedDayDetailText;
            set
            {
                _selectedDayDetailText = value;
                OnPropertyChanged();
            }
        }

        public CalendarPageViewModel()
        {
            CalendarMonths = new ObservableCollection<CalendarMonthViewModel>();

            var today = DateTime.Now;

            for (int i = -6; i <= 6; i++)
            {
                var month = today.AddMonths(i);
                var monthVM = new CalendarMonthViewModel
                {
                    MonthTitle = month.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                    Days = new ObservableCollection<CalendarDayViewModel>(GenerateDaysForMonth(month)),
                    Rows = BuildCalendarGridWithFiller(month)
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

        private void OnDaySelected(CalendarDayViewModel day)
        {
            if (day != null && !string.IsNullOrEmpty(day.DayNumber))
                SelectedDay = day;
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

                days.Add(new CalendarDayViewModel
                {
                    DayNumber = i.ToString(),
                    IsDimmed = false,
                    BackgroundColor = isToday ? OvulaeColors.COLOR.ThemeClrLight2 : Colors.White,
                    EventBorderColor = Colors.Transparent,
                    HasEvent = FakeEventCheck(currentDate),
                    Date = currentDate,
                    EventIcon = FakeEventCheck(currentDate) ? "bell.png" : null
                });
            }

            return days;
        }

        public List<CalendarRowViewModel> BuildCalendarGridWithFiller(DateTime monthDate)
        {
            var result = new List<CalendarRowViewModel>();
            var firstDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
            int leadingEmptyDays = (int)firstDayOfMonth.DayOfWeek;

            var prevMonth = firstDayOfMonth.AddMonths(-1);
            int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            var allDays = new List<CalendarDayViewModel>();

            // Add dimmed leading days
            for (int i = daysInPrevMonth - leadingEmptyDays + 1; i <= daysInPrevMonth; i++)
            {
                allDays.Add(new CalendarDayViewModel
                {
                    DayNumber = i.ToString(),
                    IsDimmed = true,
                    BackgroundColor = Color.FromArgb("#F2F2F2"),
                    EventBorderColor = Colors.Transparent,
                    HasEvent = false,
                    Date = null
                });
            }

            // Add real month days
            allDays.AddRange(GenerateDaysForMonth(monthDate));

            // Add dimmed trailing days
            int trailingDays = 42 - allDays.Count;
            for (int i = 1; i <= trailingDays; i++)
            {
                allDays.Add(new CalendarDayViewModel
                {
                    DayNumber = i.ToString(),
                    IsDimmed = true,
                    BackgroundColor = Color.FromArgb("#F2F2F2"),
                    EventBorderColor = Colors.Transparent,
                    HasEvent = false,
                    Date = null
                });
            }

            // Group into 6 rows of 7 cells
            for (int row = 0; row < 6; row++)
            {
                var week = allDays.Skip(row * 7).Take(7).ToList();
                result.Add(new CalendarRowViewModel { Cells = new ObservableCollection<CalendarDayViewModel>(week) });
            }

            return result;
        }

        private bool FakeEventCheck(DateTime date)
        {
            return date.Day % 5 == 0;
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
    }
}

