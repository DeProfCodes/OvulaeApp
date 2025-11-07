using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.Menopause;

namespace OvulaeApp.ViewModels.MenopauseTracker
{
    public class MenopauseLoggerViewModel : BaseViewModel
    {
        private DateTime _currentDate;
        private MenopauseLogEntry _currentLogEntry;

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

        public MenopauseLogEntry CurrentLogEntry
        {
            get => _currentLogEntry;
            set => SetProperty(ref _currentLogEntry, value);
        }

        public string DisplayDate => CurrentDate.ToString("dd/MM/yyyy");
        public string PreviousDateText => CurrentDate.AddDays(-1).ToString("dd/MM");
        public string NextDateText => CurrentDate.AddDays(1).ToString("dd/MM");

        private readonly IModuleLogsService _moduleLogsServ;

        public MenopauseLoggerViewModel(IModuleLogsService moduleLogsServ)
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
                CurrentLogEntry = _moduleLogsServ.GetMenopauseLogByDate(CurrentDate);
                if (CurrentLogEntry == null)
                {
                    CurrentLogEntry = DefaultValueHelper.CreateWithDefaults<MenopauseLogEntry>();
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
                var result = await _moduleLogsServ.UpdateTodayMenopauseLog(CurrentLogEntry);
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
