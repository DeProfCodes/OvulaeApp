using OvulaeApp.ViewModels.Marketing;

namespace OvulaeApp.Views.Marketing
{
    public partial class AdBenefitsPage : ContentView
    {
        public event EventHandler BackClicked;
        public event EventHandler NextClicked;

        public static readonly BindableProperty DotCurrentIndexProperty =
            BindableProperty.Create(nameof(DotCurrentIndex), typeof(int), typeof(AdBenefitsPage), 0);

        public static readonly BindableProperty DotTotalIndicesProperty =
            BindableProperty.Create(nameof(DotTotalIndices), typeof(int), typeof(AdBenefitsPage), 12);

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

        public AdBenefitsPage()
        {
            try
            {
                InitializeComponent();

                BindingContext = new OvulaeModuleBenefitsViewModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PeriodTrackerBenefitsPage: {ex}");
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

    }
}
