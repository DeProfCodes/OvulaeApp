
using OvulaeApp.Helpers.Styles;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeShared.Models.Diet;
using OvulaeShared.ViewModel.Diet;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class DietDetailsModal : ContentView
    {
        public DietDetailsModal()
        {
            InitializeComponent();
        }

        public async Task ShowDietDetailsAsync(string title, List<DietItem> items)
        {
            try
            {
                ModalTitle.Text = $"Recommended Foods: {title}";

                DietListLayout.Children.Clear();

                foreach (var detail in items)
                {
                    // Heading
                    DietListLayout.Children.Add(new Label
                    {
                        Text = detail.FullHeading,
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = "Arial Black",
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.Black,
                        Margin = new Thickness(0, 10, 0, 0)
                    });

                    // Subheading
                    DietListLayout.Children.Add(new Label
                    {
                        Text = detail.SubHeading,
                        FontSize = 14,
                        FontAttributes = FontAttributes.Italic,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = OvulaeColors.COLOR.ThemeGray2,
                        Margin = new Thickness(15, 10, 15, 10)
                    });

                    // Optional Image
                    if (!string.IsNullOrEmpty(detail.ImageSrc))
                    {
                        DietListLayout.Children.Add(new Image
                        {
                            Source = detail.ImageSrc,
                            HeightRequest = 60,
                            WidthRequest = 60,
                            Aspect = Aspect.AspectFit,
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 0, 0, 10)
                        });
                    }

                    // Bullet Points
                    if (detail.DietListDetails?.Any() == true)
                    {
                        foreach (var bullet in detail.DietListDetails)
                        {
                            DietListLayout.Children.Add(new Label
                            {
                                Text = "• " + bullet,
                                FontSize = 14,
                                TextColor = Colors.Black,
                                LineBreakMode = LineBreakMode.WordWrap,
                                Margin = new Thickness(15, 0, 10, 3)
                            });
                        }
                    }

                    // Importance
                    if (!string.IsNullOrWhiteSpace(detail.Importance))
                    {
                        DietListLayout.Children.Add(new Label
                        {
                            Text = $"💡Why?: {detail.Importance}",
                            FontSize = 12,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = OvulaeColors.COLOR.ThemeClr2,
                            HorizontalTextAlignment = TextAlignment.Center,
                            Margin = new Thickness(5, 10, 5, 20)
                        });
                    }

                    // Optional separator
                }

                this.IsVisible = true;
                await this.FadeTo(1, 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Diet error: {ex.Message}");
            }
        }

        public async Task HideAsync()
        {
            await this.FadeTo(0, 200);
            this.IsVisible = false;
        }

        private async void CloseButton_Tapped(object sender, TappedEventArgs e)
        {
            await HideAsync();
        }
    }
}