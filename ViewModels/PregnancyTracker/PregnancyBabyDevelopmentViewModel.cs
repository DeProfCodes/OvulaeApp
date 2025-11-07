using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Models.Shared;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeShared.Models.Pregnancy;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyBabyDevelopmentViewModel : BaseViewModel
    {
        private PregnancyBabyData _mainData;
        public PregnancyBabyData MainData
        {
            get => _mainData;
            set
            {
                if (_mainData != value)
                {
                    _mainData = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> _motherChanges;

        public List<string> MotherChanges
        {
            get => _motherChanges;
            set 
            {
                _motherChanges = value;
                OnPropertyChanged();
            }
        }


        private int _week;
        public int Week
        {
            get => _week;
            set
            {
                if (_week != value)
                {
                    _week = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _heroImage;
        public string HeroImage
        {
            get => _heroImage;
            set
            {
                if (_heroImage != value)
                {
                    _heroImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _babySize;
        public string BabySize
        {
            get => _babySize;
            set
            {
                if (_babySize != value)
                {
                    _babySize = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OurGynacologist { get; set; }

        private readonly IPregnancyService _localPregdbServ;

        public PregnancyBabyDevelopmentViewModel(IPregnancyService localPregdbServ, int week)
        {
            _localPregdbServ = localPregdbServ;

            Week = week;
            UpdateViewsForWeek();

            OurGynacologist = $"Dr. {LocalStorageService.OvulaeGynacologist.Firstname} {LocalStorageService.OvulaeGynacologist.Lastname}, OB/GYN";

        }

        public void GoToPreviousWeek()
        {
            if (Week > 1)
            {
                Week--;
                UpdateViewsForWeek();
            }
        }

        public void GoToNextWeek()
        {
            if (Week < 41)
            {
                Week++;
                UpdateViewsForWeek();
            }
        }

        public void UpdateViewsForWeek()
        {
            try
            {
                HeroImage = $"p{Week}d.png";
                MainData = _localPregdbServ.GetBabyDevelopmentForWeek(Week);
                BabySize = Week > 3 ? $"Your baby is a size of {MainData.BabySizeFruit}" : "";
                
                MotherChanges = MainData.MotherChanges;

                for (int i = 0; i < MotherChanges.Count; i++)
                {
                    if (!string.IsNullOrEmpty(MotherChanges[i]))
                    {
                        var enumerator = StringInfo.GetTextElementEnumerator(MotherChanges[i]);
                        enumerator.MoveNext();
                        string firstElement = enumerator.GetTextElement();

                        if (IsEmoji(firstElement))
                        {
                            int startIndex = enumerator.ElementIndex + firstElement.Length;
                            string rest = MotherChanges[i].Substring(startIndex);
                            MotherChanges[i] = $"• {rest}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Baby development week change failed: {ex.Message}");
            }
        }

        bool IsEmoji(string textElement)
        {
            if (string.IsNullOrEmpty(textElement)) return false;

            int codePoint = char.ConvertToUtf32(textElement, 0);

            // Common emoji ranges (not exhaustive but covers most)
            return
                (codePoint >= 0x1F600 && codePoint <= 0x1F64F) || // Emoticons
                (codePoint >= 0x1F300 && codePoint <= 0x1F5FF) || // Symbols & pictographs
                (codePoint >= 0x1F680 && codePoint <= 0x1F6FF) || // Transport & map
                (codePoint >= 0x2600 && codePoint <= 0x26FF) || // Misc symbols
                (codePoint >= 0x2700 && codePoint <= 0x27BF) || // Dingbats
                (codePoint >= 0x1F900 && codePoint <= 0x1F9FF);   // Supplemental symbols
        }

    }
}
