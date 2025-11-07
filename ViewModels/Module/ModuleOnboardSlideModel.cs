
namespace OvulaeApp.ViewModels.Module
{
    public class ModuleOnboardSlideModel : BaseViewModel
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        private bool _showSlide { get; set; }
        public bool ShowSlide
        {
            get => _showSlide;
            set
            {
                _showSlide = value;
                OnPropertyChanged();
            }
        }

        private bool _isNextEnabled { get; set; }
        public bool IsNextEnabled
        {
            get => _isNextEnabled;
            set
            {
                _isNextEnabled = value;
                OnPropertyChanged();
            }
        }

    }
}
