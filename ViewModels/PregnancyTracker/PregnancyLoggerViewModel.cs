using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.ViewModels.Dashboard;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.Pregnancy;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyLoggerViewModel : BaseViewModel
    {
        private DateTime _currentDate;
        private PregnancyLogEntryItem _currentLogEntry;

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    OnPropertyChanged();
                    LoadLogForDate();
                    OnPropertyChanged(nameof(DisplayDate));
                    OnPropertyChanged(nameof(PreviousDateText));
                    OnPropertyChanged(nameof(NextDateText));
                }
            }
        }

        public PregnancyLogEntryItem CurrentLogEntry
        {
            get => _currentLogEntry;
            set => SetProperty(ref _currentLogEntry, value);
        }

        public string DisplayDate => CurrentDate.ToString("dd/MM/yyyy");
        public string PreviousDateText => CurrentDate.AddDays(-1).ToString("dd/MM");
        public string NextDateText => CurrentDate.AddDays(1).ToString("dd/MM");

        private readonly IModuleLogsService _moduleLogsServ;

        public PregnancyLoggerViewModel(IModuleLogsService moduleLogsServ)
        {
            _moduleLogsServ = moduleLogsServ;
            CurrentDate = DateTime.Today;
        }

        public void GoToPreviousDay()
        {
            CurrentDate = CurrentDate.AddDays(-1);
        }

        public void GoToNextDay()
        {
            if (CurrentDate < DateTime.Today)
            {
                CurrentDate = CurrentDate.AddDays(1);
            }
        }

        private void LoadLogForDate()
        {
            try
            {
                CurrentLogEntry = _moduleLogsServ.GetPregnancyLogByDate(CurrentDate);
                if (CurrentLogEntry == null)
                {
                    // Create a new empty log entry for this date
                    CurrentLogEntry = DefaultValueHelper.CreateWithDefaults<PregnancyLogEntryItem>();
                    CurrentLogEntry.LogDate = CurrentDate;

                    // Calculate week and day based on LMP
                    var userCycle = LocalStorageService.UserCycleProfile;
                    if (userCycle?.LastPeriodDate != null)
                    {
                        int totalDays = (CurrentDate.Date - userCycle.LastPeriodDate.Value.Date).Days;
                        CurrentLogEntry.Week = (totalDays / 7) + 1;
                        CurrentLogEntry.DayOfWeekNo = (totalDays % 7) + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load log for date {CurrentDate}: {ex.Message}");
            }
        }

        public async Task<bool> SaveCurrentLog()
        {
            try
            {
                var result = await _moduleLogsServ.UpdatePregnancyLog(CurrentLogEntry);
                return result.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save log: {ex.Message}");
                return false;
            }
        }
    }
}
