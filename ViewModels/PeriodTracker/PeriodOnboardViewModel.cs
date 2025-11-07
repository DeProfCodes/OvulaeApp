using System.Collections.ObjectModel;
using System.Windows.Input;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Components.Shared;
using OvulaeShared.Enums;
using OvulaeShared.Enums.HealthProfile;

namespace OvulaeApp.ViewModels.PeriodTracker
{
    public class PeriodOnboardViewModel : BaseViewModel
    {
        private int _selectedYear = 2005;
        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        public string WeightUnit { set; get; } = "kg";
        
        public string HeightUnit { set; get; } = "cm";

        public ObservableCollection<string> CycleExperiences { get; } = new()
        {
            PeriodIrregularityType.Regular.GetDisplayName(),
            PeriodIrregularityType.OccasionallyIrregular.GetDisplayName(),
            PeriodIrregularityType.FrequentlyIrregular.GetDisplayName(),
            PeriodIrregularityType.NoPeriods.GetDisplayName(),
            PeriodIrregularityType.NotSure.GetDisplayName(),
        };

        public List<List<string>> BirthControl { get; set; } = new()
        {
            new List<string> 
            {
                "Combined pill (estrogen + progestin)",
                "Progestin-only pill (mini pill)",
                "Hormonal IUD (e.g., Mirena, Kyleena)",
                "Implant (e.g., Nexplanon)",
                "Patch (e.g., Ortho Evra)",
                "Vaginal ring (e.g., NuvaRing)",
                "Injection (e.g., Depo-Provera)"
            },
            new List<string>
            {
                "Copper IUD",
                "Condoms or barrier methods",
                "Fertility awareness/natural tracking",
                "Emergency contraception (recent use)"
            },
            new List<string>
            {
                "Not using any method",
                "Not sure / Prefer not to say"
            }
        };

        public ObservableCollection<SelectComponentSwitch.SwitchOption> NotificationOptions { get; } = new()
        {
            new () { Label = "Log symptoms", IsToggled = false },
            new () { Label = "Weekly updates", IsToggled = false },
            new() { Label = "Educational tips", IsToggled = false },
            new() { Label = "Prenatal appointment reminders", IsToggled = false },
        };

        public ObservableCollection<ModuleOnboardSlideModel> Slides { get; set; }
        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
                CurrentSlide = Slides[_currentIndex];
                DotCurrentIndex = _currentIndex >= 4 ? _currentIndex - 1 : _currentIndex;
            }
        }

        private bool _showDotBackButton;
        public bool ShowDotBackButton
        {
            get => _showDotBackButton;
            set
            {
                _showDotBackButton = value;
                OnPropertyChanged(nameof(ShowDotBackButton));
            }
        }

        private int _dotCurrentIndex;
        public int DotCurrentIndex
        {
            get => _dotCurrentIndex;
            set
            {
                _dotCurrentIndex = value;
                OnPropertyChanged(nameof(DotCurrentIndex));
            }
        }

        public ICommand BackCommand => new Command(GoBack);
        public ICommand NextCommand => new Command(GoNext);

        private ModuleOnboardSlideModel _currentSlide;
        public ModuleOnboardSlideModel CurrentSlide
        {
            get => _currentSlide;
            set
            {
                _currentSlide = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> HealthConditionsOptions { get; } = new()
        {
            HealthConditionsTypes.Endometriosis.GetDisplayDescription(),
            HealthConditionsTypes.PCOS.GetDisplayDescription(),
            HealthConditionsTypes.Fibroids.GetDisplayDescription(),
            HealthConditionsTypes.Yeast.GetDisplayDescription(),
            HealthConditionsTypes.Osteoporosis.GetDisplayDescription(),
            HealthConditionsTypes.Thyroid.GetDisplayDescription(),
            HealthConditionsTypes.HeartDisease.GetDisplayDescription(),
            HealthConditionsTypes.Diabetes.GetDisplayDescription(),
            HealthConditionsTypes.NoCondition.GetDisplayDescription(),
            "Not Sure"
        };

        public ObservableCollection<string> TamponsUsageOptions { get; } = new()
        {
            TamponUsageOptions.YesNoPain.GetDisplayDescription(),
            TamponUsageOptions.YesPain.GetDisplayDescription(),
            TamponUsageOptions.No.GetDisplayDescription()
        };

        private SpinnerLoader appSpinner;

        private int LastActiveIndex;

        public PeriodOnboardViewModel(SpinnerLoader appSpinner)
        {
            Slides = new ObservableCollection<ModuleOnboardSlideModel>
            {
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, //Do Benefits Ad Page 0
                new() { Title = "Let’s understand your cycle a bit better", Subtitle = "We’ll start by learning about your menstrual patterns.", ShowSlide = true, IsNextEnabled = false }, //1
                new() { Title = "How old are you?", Subtitle = "We ask this to tailor your cycle experience based on your life stage.", ShowSlide = false, IsNextEnabled = true }, //2
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Dashboard 3
                new() { Title = "What is your current weight?", Subtitle = "Your weight helps us personalize insights related to your cycle and overall health.", ShowSlide = false, IsNextEnabled = true }, //4
                new() { Title = "What is your height?", Subtitle = "Your height, along with weight, helps us calculate BMI and personalize health insights.", ShowSlide = false, IsNextEnabled = true }, //5
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Calendar 6
                new() { Title = "HPV vaccine (12yrs and older)", Subtitle = "Please select Yes/No", ShowSlide = false, IsNextEnabled = false }, //7
                new() { Title = "Can you/have you used a tampon?", Subtitle = "Please select Yes/No and how it how was your experience", ShowSlide = false , IsNextEnabled = false}, // 8
                new() { Title = "What is your blood type?", Subtitle = "Knowing your blood type helps us provide better health insights.", ShowSlide = false, IsNextEnabled = true }, // 9
                new() { Title = "How are you feeling lately?", Subtitle = "Logging your emotions and symptoms helps tailor your insights.", ShowSlide = false , IsNextEnabled = false}, // 10
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Chat with Gyn 11
                new() { Title = "Health Conditions (Optional)", Subtitle = "*This helps us offer the right support and insights for your cycle.", ShowSlide = false , IsNextEnabled = true}, //12
                new() { Title = "PCOS More Details", Subtitle = "Enter for how long you have had PCOS and if you are following a diet.", ShowSlide = false , IsNextEnabled = true}, //13
                new() { Title = "Endometriosis More Details", Subtitle = "Enter for how long you have had Endometriosis and if you are following a diet.", ShowSlide = false , IsNextEnabled = true}, //14
                new() { Title = "Birth Control", Subtitle = "*This helps us adjust your predictions if needed.", ShowSlide = false , IsNextEnabled = true}, //15
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Educational Books 16                                                                             //17
                new() { Title = "Let’s personalize your predictions", Subtitle = "Tell us what you already know, and we’ll fine-tune your experience. You can update this anytime later.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "Let’s personalize your predictions", Subtitle = "Knowing your typical period length helps us better tailor your cycle insights.", ShowSlide = false , IsNextEnabled = true}, //18
                new() { Title = "Let’s personalize your predictions", Subtitle = "Understanding your cycle length helps us provide accurate period predictions.", ShowSlide = false , IsNextEnabled = true}, //19
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Partner Share 20
                new() { Title = "Notifications", Subtitle = "Enable reminders to stay on track with your health and cycle insights.", ShowSlide = false , IsNextEnabled = true}, //21
            };

            CurrentIndex = 0;
            CurrentSlide = Slides[0];
            this.appSpinner = appSpinner;
        }

        public async void JumpToIndex(int index)
        {
            if (index < Slides.Count)
            {
                Slides[CurrentIndex].ShowSlide = false;
                CurrentIndex = index;
                Slides[CurrentIndex].ShowSlide = true;
            }
            ShowDotBackButton = true;
        }

        public async void GoNext()
        {
            if (CurrentIndex < Slides.Count - 1)
            {
                Slides[CurrentIndex].ShowSlide = false;
                CurrentIndex++;
                Slides[CurrentIndex].ShowSlide = true;
            }

            if (CurrentIndex > 0)
            {
                ShowDotBackButton = true;
            }
        }

        public void GoBack()
        {

            if (CurrentIndex > 0)
            {
                Slides[CurrentIndex].ShowSlide = false;
                CurrentIndex--;
                Slides[CurrentIndex].ShowSlide = true;
            }
            if (CurrentIndex == 0)
            {
                ShowDotBackButton = false;
            }
        }
    }
}
