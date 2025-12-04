using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using OvulaeShared.ViewModel.Education;
using System.Windows.Input;

namespace OvulaeApp.Views.Components.Dashboard
{
    public partial class EducationCoverItem : ContentView
    {
        public event EventHandler<EducationCover> CoverTapped;

        public EducationCoverItem()
        {
            InitializeComponent();
            //this.BindingContext = this;
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    CoverTapped?.Invoke(this, this.Data); // raise event with tapped item
                })
            });
        }

        // ThumbnailSource (Image path)
        public static readonly BindableProperty ThumbnailSourceProperty =
            BindableProperty.Create(nameof(ThumbnailSource), typeof(string), typeof(EducationCoverItem), default(string));

        public string ThumbnailSource
        {
            get => (string)GetValue(ThumbnailSourceProperty);
            set => SetValue(ThumbnailSourceProperty, value);
        }

        // Title (Text below image)
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(EducationCoverItem), default(string));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Bookmarked (toggles icon)
        public static readonly BindableProperty BookmarkedProperty =
            BindableProperty.Create(nameof(Bookmarked), typeof(bool), typeof(EducationCoverItem), default(bool));

        public bool Bookmarked
        {
            get => (bool)GetValue(BookmarkedProperty);
            set => SetValue(BookmarkedProperty, value);
        }

        // Font size of label
        public static readonly BindableProperty LabelFontSizeProperty =
            BindableProperty.Create(nameof(LabelFontSize), typeof(double), typeof(EducationCoverItem), 12.0);

        public double LabelFontSize
        {
            get => (double)GetValue(LabelFontSizeProperty);
            set => SetValue(LabelFontSizeProperty, value);
        }

        // Font attributes (Bold, Italic, etc.)
        public static readonly BindableProperty LabelFontAttributesProperty =
            BindableProperty.Create(nameof(LabelFontAttributes), typeof(FontAttributes), typeof(EducationCoverItem), FontAttributes.Bold);

        public FontAttributes LabelFontAttributes
        {
            get => (FontAttributes)GetValue(LabelFontAttributesProperty);
            set => SetValue(LabelFontAttributesProperty, value);
        }

        // Label text color
        public static readonly BindableProperty LabelTextColorProperty =
            BindableProperty.Create(nameof(LabelTextColor), typeof(Color), typeof(EducationCoverItem), Colors.Black);

        public Color LabelTextColor
        {
            get => (Color)GetValue(LabelTextColorProperty);
            set => SetValue(LabelTextColorProperty, value);
        }

        // Label margin
        public static readonly BindableProperty LabelMarginProperty =
            BindableProperty.Create(nameof(LabelMargin), typeof(Thickness), typeof(EducationCoverItem), new Thickness(5));

        public Thickness LabelMargin
        {
            get => (Thickness)GetValue(LabelMarginProperty);
            set => SetValue(LabelMarginProperty, value);
        }

        // Cover Stroke (Border color)
        public static readonly BindableProperty CoverStrokeProperty =
            BindableProperty.Create(nameof(CoverStroke), typeof(Brush), typeof(EducationCoverItem), Brush.Black);

        public Brush CoverStroke
        {
            get => (Brush)GetValue(CoverStrokeProperty);
            set => SetValue(CoverStrokeProperty, value);
        }

        // Cover Background
        public static readonly BindableProperty CoverBackgroundProperty =
            BindableProperty.Create(nameof(CoverBackground), typeof(Brush), typeof(EducationCoverItem), Brush.White);

        public Brush CoverBackground
        {
            get => (Brush)GetValue(CoverBackgroundProperty);
            set => SetValue(CoverBackgroundProperty, value);
        }

        // Tap Command
        public static readonly BindableProperty CoverTappedCommandProperty =
        BindableProperty.Create(nameof(CoverTappedCommand), typeof(ICommand), typeof(EducationCoverItem));

        public ICommand CoverTappedCommand
        {
            get => (ICommand)GetValue(CoverTappedCommandProperty);
            set => SetValue(CoverTappedCommandProperty, value);
        }

        public static readonly BindableProperty CoverTappedCommandParameterProperty =
            BindableProperty.Create(nameof(CoverTappedCommandParameter), typeof(object), typeof(EducationCoverItem));

        public object CoverTappedCommandParameter
        {
            get => GetValue(CoverTappedCommandParameterProperty);
            set => SetValue(CoverTappedCommandParameterProperty, value);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (CoverTappedCommand?.CanExecute(CoverTappedCommandParameter) ?? false)
                CoverTappedCommand.Execute(CoverTappedCommandParameter);
        }

        public static readonly BindableProperty DataProperty =
            BindableProperty.Create(nameof(Data), typeof(EducationCover), typeof(EducationCoverItem), propertyChanged: OnDataChanged);

        public EducationCover Data
        {
            get => (EducationCover)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EducationCoverItem control && newValue is EducationCover newData)
            {
                control.BindingContext = newData;
            }
        }

        private void HardcodedTapTest(object sender, EventArgs e)
        {
            Console.WriteLine("✔ TAP WORKED: Gesture detected at component level");

            Application.Current.MainPage.DisplayAlert("Tap", "Tap gesture worked on card!", "OK");
        }
    }
}
