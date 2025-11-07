using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.GoalSetting;

namespace OvulaeApp.ViewModels.Splashscreen
{
    public class SplashscreenViewModel : BaseViewModel
    {
        private readonly List<SplashContent> _pages = new()
        {
            new("Track Your Period with Ease", "Log flow, moods, and symptoms. Get personalized cycle predictions.", "splash_menst_2.png"),
            new("Understand Your Fertility", "Predict ovulation and fertile windows. Get tips for conception.", "splash_fertility_2.png"),
            new("Your Pregnancy, Week by Week", "Track baby growth, symptoms, and receive trimester tips.", "splash_preg_2.png"),
            new("Navigate Menopause with Confidence", "Track symptoms, get expert tips, and manage hormonal changes effectively.", "splash_menopause_2.png"),
            new("Manage Endometriosis, PCOS, and More", "Learn about chronic conditions and how to manage them.", "splash_education_2.png"),
        };

        public int TotalSteps => _pages.Count;

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (SetProperty(ref _currentIndex, value))
                    UpdatePage();
            }
        }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Image { get; set; }

        public ICommand NextCommand => new Command(GoNext);
        public ICommand NavigateToLoginCommand => new Command(GoToLogin);

        private SpinnerLoader spinner;

        public SplashscreenViewModel(SpinnerLoader spinner)
        {
            CurrentIndex = 0;
            this.spinner = spinner;
            UpdatePage();
        }

        private void UpdatePage()
        {
            var current = _pages[CurrentIndex];
            Title = current.Title;
            Subtitle = current.Subtitle;
            Image = current.Image;
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Subtitle));
            OnPropertyChanged(nameof(Image));
        }

        public async void GoNext()
        {
            if (CurrentIndex < TotalSteps - 1)
            {
                CurrentIndex++;
            }
            else
            {
                await spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(GoalSettingPage));
                await spinner.HideSpinnerAsync();
            }
        }

        private async void GoToLogin()
        {
            await spinner.ShowSpinnerAsync();
            await Shell.Current.GoToAsync(nameof(LoginPage));
            await spinner.HideSpinnerAsync();
        }

        record SplashContent(string Title, string Subtitle, string Image);
    }
}
