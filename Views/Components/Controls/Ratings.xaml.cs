using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls.Xaml;

namespace OvulaeApp.Views.Components.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ratings : ContentView
    {
        public static readonly BindableProperty SelectedRatingProperty =
            BindableProperty.Create(nameof(SelectedRating), typeof(int), typeof(Ratings), 0, propertyChanged: OnSelectedRatingChanged);

        public int SelectedRating
        {
            get => (int)GetValue(SelectedRatingProperty);
            set => SetValue(SelectedRatingProperty, value);
        }

        public event EventHandler<int> RatingChanged;

        private readonly List<Border> _ratingBorders = new();

        public Ratings()
        {
            InitializeComponent();
            BuildRatings();
        }

        private void BuildRatings()
        {
            RatingLayout.Children.Clear();
            _ratingBorders.Clear();

            for (int i = 1; i <= 10; i++)
            {
                var border = new Border
                {
                    WidthRequest = 30,
                    HeightRequest = 30,
                    StrokeShape = new RoundRectangle { CornerRadius = 50 },
                    Stroke = Application.Current.Resources["ThemeClrMain"] as Color ?? Colors.Gray,
                    Background = Application.Current.Resources["ThemeClrLight"] as Color ?? Colors.Transparent,
                    Padding = new Thickness(0, 0),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };

                var label = new Label
                {
                    Text = i.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 11,
                    TextColor = Colors.Black
                };

                border.Content = label;

                var tapGesture = new TapGestureRecognizer();
                int rating = i;
                tapGesture.Tapped += (s, e) => OnRatingTapped(rating);
                border.GestureRecognizers.Add(tapGesture);

                _ratingBorders.Add(border);
                RatingLayout.Children.Add(border);
            }
        }

        private void OnRatingTapped(int rating)
        {
            SelectedRating = rating;
            RatingChanged?.Invoke(this, rating);
            UpdateVisuals();
        }

        private static void OnSelectedRatingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ratings = (Ratings)bindable;
            ratings.UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < _ratingBorders.Count; i++)
            {
                var border = _ratingBorders[i];
                var label = border.Content as Label;
                int ratingValue = i + 1;

                if (ratingValue == SelectedRating)
                {
                    border.Background = GetColorForRating(ratingValue);
                    label.TextColor = Colors.White;
                }
                else
                {
                    border.Background = Application.Current.Resources["ThemeClrLight"] as Color ?? Colors.Transparent;
                    label.TextColor = Colors.Black;
                }
            }
        }

        private Color GetColorForRating(int rating)
        {
            // Red (1) → Orange (5) → Green (10)
            if (rating <= 5)
            {
                double t = (rating - 1) / 4.0; // 0–1 from red to orange
                return Color.FromRgb(
                    1.0,
                    0.5 * t,  // blend red→orange
                    0.0
                );
            }
            else
            {
                double t = (rating - 5) / 5.0; // 0–1 from orange to green
                return Color.FromRgb(
                    1.0 - t,
                    0.5 + 0.5 * t,  // blend orange→green
                    0.0
                );
            }
        }
    }
}
