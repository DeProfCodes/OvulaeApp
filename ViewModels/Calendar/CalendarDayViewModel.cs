using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Calendar
{
    public class CalendarDayViewModel : BaseViewModel
    {
        public bool IsDimmed { get; set; }

        public string DayNumber { get; set; }

        public bool HasEvent { get; set; }

        public bool IsToday { get; set; }

        public int? PregnancyWeek { get; set; }

        public string EventIcon { get; set; }

        public Color OriginalBackgroundColor { get; set; }

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color EventBorderColor { get; set; }

        public DateTime? Date { get; set; }

        public string EventDetails { get; set; }

        // Week marker (W 1 8)
        private string weekMarkerText;
        public string WeekMarkerText
        {
            get => weekMarkerText;
            set
            {
                if (weekMarkerText != value)
                {
                    weekMarkerText = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ShowWeekMarker));
                    OnPropertyChanged(nameof(IsDayNumberVisible));
                }
            }
        }

        public bool ShowWeekMarker => !string.IsNullOrEmpty(WeekMarkerText);

        public bool IsDayNumberVisible => !ShowWeekMarker;

        // Event Emoji with visibility
        private string eventEmoji;
        public string EventEmoji
        {
            get => eventEmoji;
            set
            {
                if (eventEmoji != value)
                {
                    eventEmoji = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsEventEmojiVisible));
                }
            }
        }

        public bool IsEventEmojiVisible => !string.IsNullOrEmpty(EventEmoji);

        private Color _weekMarkerTextColor;
        public Color WeekMarkerTextColor
        {
            get => _weekMarkerTextColor;
            set
            {
                if (_weekMarkerTextColor != value)
                {
                    _weekMarkerTextColor = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
