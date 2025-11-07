using System.Collections.ObjectModel;
using System.Windows.Input;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.Views.Components.Controls;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Components.Shared;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyOnboardViewModel : BaseViewModel
    {
        public ObservableCollection<string> AssistanceOptions { get; } = new()
        {
            "Weekly baby growth updates",
            "What to expect each trimester",
            "Managing symptoms",
            "Diet & exercise suggestions",
            "Tracking my moods and sleep",
            "Partner involvement or sharing"
        };

        public ObservableCollection<SelectComponentSwitch.SwitchOption>  NotificationOptions { get; } = new () 
        {
            new () { Label = "Log symptoms", IsToggled = false },
            new () { Label = "Weekly updates", IsToggled = false },
            new() { Label = "Educational tips", IsToggled = false },
            new() { Label = "Prenatal appointment reminders", IsToggled = false },
        };

        private ObservableCollection<ModuleOnboardSlideModel> _slides { get; set; }

        public ObservableCollection<ModuleOnboardSlideModel> Slides
        {
            get => _slides;
            set 
            {
                _slides = value;
                OnPropertyChanged();
            }
        }

        private int _selectedYear = 2005;
        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        public string WeightUnit { set; get; } = "";

        public string HeightUnit { set; get; } = "";


        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
                CurrentSlide = Slides[_currentIndex];
                DotCurrentIndex = (_currentIndex >= 4) ? (_currentIndex - 1) : _currentIndex;
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

        public PregnancyOnboardViewModel(SpinnerLoader appSpinner)
        {
            Slides = new ObservableCollection<ModuleOnboardSlideModel>
            {
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, //Do Benefits Ad Page
                new() { Title="Are you currently pregnant?", Subtitle="We’ll customize your Ovulae experience to support your journey.", ShowSlide = false, IsNextEnabled = false },
                new() { Title="Is this your first pregnancy?", Subtitle="This helps us personalize your content and support better.", ShowSlide = false , IsNextEnabled = false},
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Dashboard
                new() { Title = "What is your age?", Subtitle = "Knowing your age helps us offer relevant insights and support for your stage of pregnancy.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "What is your weight?", Subtitle = "Your current weight helps us track healthy pregnancy progress and provide personalized guidance.", ShowSlide = false, IsNextEnabled = true },
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Calendar
                new() { Title = "What is your height?", Subtitle = "Height, combined with weight, helps us assess your BMI for better pregnancy care insights.", ShowSlide = false, IsNextEnabled = true },
                new() { Title="Do you know how far along you are?", Subtitle="We’ll use this to show your baby’s development and trimester timeline.", ShowSlide = false , IsNextEnabled = false},
                new() { Title="Enter Due Date", Subtitle="We’ll calculate where you are in your pregnancy based on this.", ShowSlide = false , IsNextEnabled = true},
                new() { Title="When did your last period start?", Subtitle="We’ll estimate your due date and track your pregnancy from there.", ShowSlide = false , IsNextEnabled = true},
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Chat with Gyn
                new() { Title = "What is your blood type?", Subtitle = "Knowing your blood type helps us provide better health insights.", ShowSlide = false, IsNextEnabled = true },
                new() { Title="What would you like help with during your pregnancy?", Subtitle="", ShowSlide = false , IsNextEnabled = false},
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Partner Share
                new() { Title="Would you like reminders during your pregnancy?", Subtitle="", ShowSlide = false , IsNextEnabled = true},
                new() { Title = "", Subtitle = "", ShowSlide = true, IsNextEnabled = false }, // Educational Books
            };
            CurrentIndex = 0;
            CurrentSlide = Slides[0];
            this.appSpinner = appSpinner;
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
            Slides[CurrentIndex].ShowSlide = false;

            if (CurrentIndex == 10)
                CurrentIndex = 8;

            else if (CurrentIndex > 0)
                CurrentIndex--;
            
            if (CurrentIndex == 0)
            {
                ShowDotBackButton = false;
            }

            Slides[CurrentIndex].ShowSlide = true;
        }
    }
}
