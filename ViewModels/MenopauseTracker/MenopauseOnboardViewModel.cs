using System.Collections.ObjectModel;
using System.Windows.Input;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Components.Shared;
using OvulaeShared.Enums;
using OvulaeShared.Enums.HealthProfile;

namespace OvulaeApp.ViewModels.MenopauseTracker
{
    public class MenopauseOnboardViewModel : BaseViewModel
    {
        private int _selectedYear = 2005;
        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        public string WeightUnit { set; get; } = "kg";
        
        public string HeightUnit { set; get; } = "cm";

        //Period
        public ObservableCollection<string> PeriodIrregularility { get; } = new()
        {
            PeriodIrregularityType.Regular.GetDisplayName(),
            PeriodIrregularityType.OccasionallyIrregular.GetDisplayName(),
            PeriodIrregularityType.FrequentlyIrregular.GetDisplayName(),
            PeriodIrregularityType.NoPeriods.GetDisplayName(),
            PeriodIrregularityType.NotSure.GetDisplayName()
        };

        // Symptoms list
        public ObservableCollection<string> MenopauseSymptoms { get; } = new()
        {
            SymptomsTypes.HotFlashes.GetDisplayName(),
            SymptomsTypes.NightSweats.GetDisplayName(),
            SymptomsTypes.SleepProblems.GetDisplayName(),
            SymptomsTypes.MoodChanges.GetDisplayName(),
            SymptomsTypes.VaginalDryness.GetDisplayName(),
            SymptomsTypes.WeightGain.GetDisplayName(),
            SymptomsTypes.HairThinning.GetDisplayName(),
            SymptomsTypes.MemoryProblems.GetDisplayName(),
            SymptomsTypes.LossOfLibido.GetDisplayName(),
            SymptomsTypes.JointPain.GetDisplayName(),
        };

        // Treatment options
        public ObservableCollection<string> TreatmentOptions { get; } = new()
        {
            TreatmentTypes.HRT.GetDisplayDescription(),
            TreatmentTypes.VaginalEstrogen.GetDisplayDescription(),
            TreatmentTypes.Antidepressants.GetDisplayDescription(),
            TreatmentTypes.NaturalRemedies.GetDisplayDescription(),
            TreatmentTypes.Acupuncture.GetDisplayDescription(),
            TreatmentTypes.NoTreatment.GetDisplayDescription(),
            TreatmentTypes.Private.GetDisplayDescription()
        };

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

        private SpinnerLoader appSpinner;

        private int LastActiveIndex;

        public MenopauseOnboardViewModel(SpinnerLoader appSpinner)
        {
            Slides = new ObservableCollection<ModuleOnboardSlideModel>
            {
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Benefits Ad Page
                new() { Title = "Are you still getting periods?", Subtitle = "Your answer helps us understand where you are in your menopause journey and tailor insights accordingly. You can always update this later.", ShowSlide = false, IsNextEnabled = false },
                new() { Title = "Let’s personalize your predictions", Subtitle = "Tell us what you already know, and we’ll fine-tune your experience. You can update this anytime later.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "Let’s personalize your predictions", Subtitle = "Knowing your typical period length helps us better tailor your cycle insights.", ShowSlide = false , IsNextEnabled = true},
                new() { Title = "Let’s personalize your predictions", Subtitle = "Understanding your cycle length helps us provide accurate period predictions.", ShowSlide = false , IsNextEnabled = true},
                new() { Title = "When did your last period start?", Subtitle = "Knowing when your last period was helps us determine your menopause stage and personalize your experience.", ShowSlide = false , IsNextEnabled = false},
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Dashboard
                new() { Title = "Let's understand your menopause journey", Subtitle = "We'll help track symptoms and provide personalized insights.", ShowSlide = false, IsNextEnabled = false },
                new() { Title = "How old are you?", Subtitle = "Age helps us tailor your menopause experience.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Calendar
                new() { Title = "What is your current weight?", Subtitle = "Your weight helps us personalize insights related to your cycle and overall health.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "What is your height?", Subtitle = "Your height, along with weight, helps us calculate BMI and personalize health insights.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Chat with Specialist
                new() { Title = "What is your blood type?", Subtitle = "Knowing your blood type helps us provide better health insights.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "Have your periods become irregular?", Subtitle = "Changes in your cycle are common during perimenopause and help us understand your stage better.", ShowSlide = false, IsNextEnabled = false },
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Educational Resources
                new() { Title = "Are you using any treatments?", Subtitle = "Select all that apply. This helps personalize your insights.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "Health Conditions (Optional)", Subtitle = "This helps us offer better support.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Partner Share
                new() { Title = "Notification Preferences", Subtitle = "Enable reminders for symptom tracking.", ShowSlide = false, IsNextEnabled = true },
            };

            CurrentIndex = 0;
            CurrentSlide = Slides[0];
            this.appSpinner = appSpinner;
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

        public async void JumpToIndex(int index)
        {
            if (index < Slides.Count - 1)
            {
                Slides[CurrentIndex].ShowSlide = false;
                CurrentIndex = index;
                Slides[CurrentIndex].ShowSlide = true;
            }
            ShowDotBackButton = true;
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
