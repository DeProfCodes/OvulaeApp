using OvulaeShared.Models.Education;

namespace OvulaeApp.Views.Components.Dashboard
{
    public partial class EducationCardGroupView : ContentView
    {
        public EducationCardGroupView()
        {
            InitializeComponent();
        }

        // Super Title
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(EducationCardGroupView), string.Empty);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Description
        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(nameof(Description), typeof(string), typeof(EducationCardGroupView), string.Empty);

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        // Super Title styling
        public static readonly BindableProperty SuperTitleTextColorProperty =
            BindableProperty.Create(nameof(SuperTitleTextColor), typeof(Color), typeof(EducationCardGroupView), Colors.Black);

        public Color SuperTitleTextColor
        {
            get => (Color)GetValue(SuperTitleTextColorProperty);
            set => SetValue(SuperTitleTextColorProperty, value);
        }

        public static readonly BindableProperty SuperTitleFontSizeProperty =
            BindableProperty.Create(nameof(SuperTitleFontSize), typeof(double), typeof(EducationCardGroupView), 16.0);

        public double SuperTitleFontSize
        {
            get => (double)GetValue(SuperTitleFontSizeProperty);
            set => SetValue(SuperTitleFontSizeProperty, value);
        }

        // Description styling
        public static readonly BindableProperty DescriptionTextColorProperty =
            BindableProperty.Create(nameof(DescriptionTextColor), typeof(Color), typeof(EducationCardGroupView), Colors.Gray);

        public Color DescriptionTextColor
        {
            get => (Color)GetValue(DescriptionTextColorProperty);
            set => SetValue(DescriptionTextColorProperty, value);
        }

        public static readonly BindableProperty DescriptionFontSizeProperty =
            BindableProperty.Create(nameof(DescriptionFontSize), typeof(double), typeof(EducationCardGroupView), 12.0);

        public double DescriptionFontSize
        {
            get => (double)GetValue(DescriptionFontSizeProperty);
            set => SetValue(DescriptionFontSizeProperty, value);
        }

        // Education Cards
        public static readonly BindableProperty EducationCardsProperty =
            BindableProperty.Create(nameof(EducationCards), typeof(List<EducationSubItem>), typeof(EducationCardGroupView), null, propertyChanged: OnCardsChanged);

        public List<EducationSubItem> EducationCards
        {
            get => (List<EducationSubItem>)GetValue(EducationCardsProperty);
            set => SetValue(EducationCardsProperty, value);
        }

        // Card Title styling
        public static readonly BindableProperty CardTitleFontSizeProperty =
            BindableProperty.Create(nameof(CardTitleFontSize), typeof(double), typeof(EducationCardGroupView), 14.0);

        public double CardTitleFontSize
        {
            get => (double)GetValue(CardTitleFontSizeProperty);
            set => SetValue(CardTitleFontSizeProperty, value);
        }

        public static readonly BindableProperty CardTitleColorProperty =
            BindableProperty.Create(nameof(CardTitleColor), typeof(Color), typeof(EducationCardGroupView), Colors.Black);

        public Color CardTitleColor
        {
            get => (Color)GetValue(CardTitleColorProperty);
            set => SetValue(CardTitleColorProperty, value);
        }

        // Bullet styling
        public static readonly BindableProperty BulletFontSizeProperty =
            BindableProperty.Create(nameof(BulletFontSize), typeof(double), typeof(EducationCardGroupView), 12.0);

        public double BulletFontSize
        {
            get => (double)GetValue(BulletFontSizeProperty);
            set => SetValue(BulletFontSizeProperty, value);
        }

        public static readonly BindableProperty BulletTextColorProperty =
            BindableProperty.Create(nameof(BulletTextColor), typeof(Color), typeof(EducationCardGroupView), Colors.Gray);

        public Color BulletTextColor
        {
            get => (Color)GetValue(BulletTextColorProperty);
            set => SetValue(BulletTextColorProperty, value);
        }

        public static readonly BindableProperty BulletFontAttributesProperty =
            BindableProperty.Create(nameof(BulletFontAttributes), typeof(FontAttributes), typeof(EducationCardGroupView), FontAttributes.None);

        public FontAttributes BulletFontAttributes
        {
            get => (FontAttributes)GetValue(BulletFontAttributesProperty);
            set => SetValue(BulletFontAttributesProperty, value);
        }

        // Card rendering
        private static void OnCardsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (EducationCardGroupView)bindable;
            control.RenderCards();
        }

        private void RenderCards()
        {
            CardListLayout.Children.Clear();

            if (EducationCards == null)
                return;

            foreach (var card in EducationCards)
            {
                var section = new VerticalStackLayout
                {
                    Spacing = 5
                };

                section.Children.Add(new Label
                {
                    Text = card.Title,
                    FontSize = CardTitleFontSize,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = CardTitleColor
                });

                var bullets = new VerticalStackLayout();

                foreach (var point in card.RawContent)
                {
                    var noBullet = point[0] != '✅' && point[0] != '❌' && point[0] != '•' && point[0] != '-';
                    var bullet = noBullet ? "• " : $"{point[0]} ";
                    var substringIdx = noBullet ? 0 : 1; 

                    bullets.Children.Add(new Label
                    {
                        Text = $"{bullet}{point.Substring(substringIdx)}",
                        FontSize = BulletFontSize,
                        FontAttributes = BulletFontAttributes,
                        TextColor = BulletTextColor,
                        LineBreakMode = LineBreakMode.WordWrap,
                        Margin = new Thickness(0)
                    });
                }

                section.Children.Add(bullets);
                CardListLayout.Children.Add(section);
            }
        }
    }
}
