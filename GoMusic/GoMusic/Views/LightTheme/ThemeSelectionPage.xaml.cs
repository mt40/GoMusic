#define Free

using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoMusic.Views.LightTheme
{
    public partial class ThemeSelectionPage : PhoneApplicationPage
    {
        Grid chosen = null;

        public ThemeSelectionPage()
        {
            InitializeComponent();
            Loaded += ThemeSelectionPage_Loaded;

#if Free
            gridLightSquare.Children[3].Visibility = System.Windows.Visibility.Visible;
            gridDarkSquare.Children[3].Visibility = System.Windows.Visibility.Visible;
            adDuplexAd.Visibility = System.Windows.Visibility.Visible;
#endif
        }

        void ThemeSelectionPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (adDuplexAd.Visibility == System.Windows.Visibility.Visible)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        GoogleAds.AdRequest request = new GoogleAds.AdRequest();
                        adDuplexAd.LoadAd(request);
                    }
                    catch { }
                });
            }
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid target = sender as Grid;
            if (target == null || target == chosen)
                return;
            else
            {
#if Free
                if (target.Children.Count >= 4 && target.Children[3] is Grid)
                {
                    GoToMarket();
                    return;
                }
#endif
                if (chosen != null)
                    chosen.Children.RemoveAt(chosen.Children.Count - 1);

                ChooseAGrid(ref target);

                chosen = target;
            }
        }

        private void GoToMarket()
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();

            marketplaceDetailTask.ContentIdentifier = "97c9181c-bee9-451b-bd01-a32122f588f7";
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;

            marketplaceDetailTask.Show();
        }

        private void ChooseAGrid(ref Grid target)
        {
            Rectangle rect = new Rectangle()
            {
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 165, 32)),
                StrokeThickness = 3,
            };
            Grid.SetRowSpan(rect, 2);
            target.Children.Add(rect);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ThemesType theme = ThemeService.Instance.CurrentThemeType;
            Grid tmp = null;
            if (theme == ThemesType.Light)
                tmp = gridLight;
            else if (theme == ThemesType.Dark)
                tmp = gridDark;
            else if (theme == ThemesType.LightSquare)
                tmp = gridLightSquare;
            else if (theme == ThemesType.DarkSquare)
                tmp = gridDarkSquare;

            if (tmp != null)
            {
                ChooseAGrid(ref tmp);
                chosen = tmp;
            }
            else
            {
                ChooseAGrid(ref gridLight);
                chosen = gridLight;
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (chosen != null)
            {
                string theme = (chosen.Children[1] as TextBlock).Text;
                if (theme == "Light")
                {
                    ThemeService.Instance.CurrentThemeType = ThemesType.Light;
                    GeneralService.Instance.IsDarkThemeUsed = false;
                }
                else if (theme == "Dark")
                {
                    ThemeService.Instance.CurrentThemeType = ThemesType.Dark;
                    GeneralService.Instance.IsDarkThemeUsed = true;
                }
                else if (theme == "Light Square")
                {
                    ThemeService.Instance.CurrentThemeType = ThemesType.LightSquare;
                    GeneralService.Instance.IsDarkThemeUsed = false;
                }
                else if (theme == "Dark Square")
                {
                    ThemeService.Instance.CurrentThemeType = ThemesType.DarkSquare;
                    GeneralService.Instance.IsDarkThemeUsed = true;
                }
            }
            ThemeService.SaveTheme();

            base.OnNavigatedFrom(e);
        }
    }
}