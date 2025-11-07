using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.Ovulation;

namespace OvulaeApp.ViewModels.OvulationTracker
{
    public class OvulationLoggerViewModel : BaseViewModel
    {
        private DateTime _currentDate;
        private OvulationCycleLog _currentLogEntry;

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

        public OvulationCycleLog CurrentLogEntry
        {
            get => _currentLogEntry;
            set => SetProperty(ref _currentLogEntry, value);
        }

        public string DisplayDate => CurrentDate.ToString("dd/MM/yyyy");
        public string PreviousDateText => CurrentDate.AddDays(-1).ToString("dd/MM");
        public string NextDateText => CurrentDate.AddDays(1).ToString("dd/MM");

        private readonly IModuleLogsService _moduleLogsServ;

        public OvulationLoggerViewModel(IModuleLogsService moduleLogsServ)
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
                CurrentLogEntry = _moduleLogsServ.GetOvulationLogByDate(CurrentDate);
                if (CurrentLogEntry == null)
                {
                    CurrentLogEntry = DefaultValueHelper.CreateWithDefaults<OvulationCycleLog>();
                    CurrentLogEntry.LogDate = CurrentDate;
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
                var result = await _moduleLogsServ.UpdateTodayOvulationLog(CurrentLogEntry);
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
