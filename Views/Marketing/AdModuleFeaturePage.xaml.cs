using OvulaeApp.Helpers.Enums;
using OvulaeApp.ViewModels.Marketing;

namespace OvulaeApp.Views.Marketing
{
    public partial class AdModuleFeaturePage : ContentView
    {
        public event EventHandler BackClicked;
        public event EventHandler NextClicked;

        public static readonly BindableProperty DotCurrentIndexProperty =
            BindableProperty.Create(nameof(DotCurrentIndex), typeof(int), typeof(AdModuleFeaturePage), 0);

        public static readonly BindableProperty DotTotalIndicesProperty =
            BindableProperty.Create(nameof(DotTotalIndices), typeof(int), typeof(AdModuleFeaturePage), 12);

        public static readonly BindableProperty FeatureTypeProperty =
            BindableProperty.Create(
                nameof(FeatureType),
                typeof(ModuleFeatureType),
                typeof(AdModuleFeaturePage),
                ModuleFeatureType.None,
                propertyChanged: OnFeatureTypeChanged);

        private static void OnFeatureTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (AdModuleFeaturePage)bindable;
            control.BindingContext = new ModuleAdFeatureViewModel((ModuleFeatureType)newValue);
        }

        public int DotCurrentIndex
        {
            get => (int)GetValue(DotCurrentIndexProperty);
            set => SetValue(DotCurrentIndexProperty, value);
        }

        public int DotTotalIndices
        {
            get => (int)GetValue(DotTotalIndicesProperty);
            set => SetValue(DotTotalIndicesProperty, value);
        }

        public ModuleFeatureType FeatureType
        {
            get => (ModuleFeatureType)GetValue(FeatureTypeProperty);
            set => SetValue(FeatureTypeProperty, value);
        }

        private int _carouselPosition = 0;
        private CancellationTokenSource _carouselTimerToken;

        private DateTime _lastAutoSlideTime = DateTime.MinValue;
        private DateTime _lastManualInteraction = DateTime.MinValue;
        private bool _userInteracted = false;

        public AdModuleFeaturePage()
        {
            try
            {
                InitializeComponent();

                _carouselTimerToken = new CancellationTokenSource();
                StartAutoCarouselTimer(_carouselTimerToken.Token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void OnBackTapped(object sender, EventArgs e)
        {
            BackClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            NextClicked?.Invoke(this, EventArgs.Empty);
        }

        private void StartAutoCarouselTimer(CancellationToken token)
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (token.IsCancellationRequested)
                        return false;

                    var now = DateTime.Now;

                    // Wait 60s after user interaction
                    if (_userInteracted && (now - _lastManualInteraction).TotalSeconds < 40)
                        return true;

                    // Ensure 12s has passed since the last auto-slide
                    if ((now - _lastAutoSlideTime).TotalSeconds < 20)
                        return true;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (ModuleFeatureCarousel?.ItemsSource is IList<string> list && list.Count > 1)
                        {
                            _carouselPosition = (_carouselPosition + 1) % list.Count;
                            ModuleFeatureCarousel.Position = _carouselPosition;

                            _lastAutoSlideTime = DateTime.Now;
                            _userInteracted = false; // resume timer after 60s
                        }
                    });

                    return true;
                });
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsVisibleProperty.PropertyName)
            {
                if (IsVisible)
                {
                    ResetCarousel();
                    RestartCarouselTimer();
                }
                else
                {
                    StopCarouselTimer();
                    ResetCarousel();
                }
            }
        }

        private void ResetCarousel()
        {
            try
            {
                _carouselPosition = 0;
                if (ModuleFeatureCarousel != null)
                {
                    ModuleFeatureCarousel.Position = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ResetCarousel failed: {ex}");
            }
        }

        private void StopCarouselTimer()
        {
            try
            {
                _carouselTimerToken?.Cancel();
                _carouselTimerToken = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StopCarouselTimer failed: {ex}");
            }
        }

        private void RestartCarouselTimer()
        {
            try
            {
                // cancel any existing token first
                _carouselTimerToken?.Cancel();
                _carouselTimerToken = new CancellationTokenSource();
                _lastAutoSlideTime = DateTime.Now;

                StartAutoCarouselTimer(_carouselTimerToken.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RestartCarouselTimer failed: {ex}");
            }
        }

        public void NextBackCouraselBtnTapped(object sender, string param)
        {
            try
            {
                if (ModuleFeatureCarousel?.ItemsSource is IList<string> list && list.Count > 1)
                {
                    int stepper = (param == "prev") ? -1 : 1;

                    // Wrap correctly without negative values
                    _carouselPosition = (_carouselPosition + stepper + list.Count) % list.Count;

                    ModuleFeatureCarousel.Position = _carouselPosition;

                    _lastAutoSlideTime = DateTime.Now;
                    _userInteracted = false; // resume timer after 60s
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Carousel nav failed: {ex}");
            }
        }
    }
}
