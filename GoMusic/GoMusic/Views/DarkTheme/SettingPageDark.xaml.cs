//#define Pro

using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace GoMusic.Views.DarkTheme
{
    public partial class SettingPageDark : PhoneApplicationPage
    {

        public SettingPageDark()
        {

            InitializeComponent();

            this.Loaded += SettingPage_Loaded;
#if Pro
            radListPicker.ItemsSource = new List<string>()
            {
                "Red", "Blue", "Pink", "Green", "Magenta", "Olive", "Brown", "Orange", "Yellow"
            };
            ButtonBuy.Visibility = Visibility.Collapsed;
            BorderPro.Visibility = Visibility.Visible;
            
            
            adDuplexAd.Visibility = System.Windows.Visibility.Collapsed;
#else
            radListPicker.ItemsSource = new List<string>()
                {
                    "Red", "Blue", "Pink", "Green", "Magenta (Pro)", "Olive (Pro)", "Brown (Pro)", "Orange (Pro)", "Yellow (Pro)"
                };
            
#endif

        }

        async void SettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var service = GeneralService.Instance;
                if (service.IsSleepTimerOn == true)
                {
                    SwitchSleepTimer.IsSwitchOn = true;
                    radTimeSpanPicker.IsHitTestVisible = true;
                    radTimeSpanPicker.Opacity = 1;
                    radTimeSpanPicker.Value = service.SleepTime;
                }

                if (service.IsListViewUsed == true)
                {
                    SwitchListView.IsSwitchOn = true;
                }

                if (service.IsLyricsUsed == true)
                {
                    SwitchLyrics.IsSwitchOn = true;
                }


                if (service.LyricsLanguage == "en")
                    radListPickerLanguage.SelectedIndex = 0;
                else if (service.LyricsLanguage == "vi")
                    radListPickerLanguage.SelectedIndex = 2;
                else
                    radListPickerLanguage.SelectedIndex = 1;

                var themeService = ThemeService.Instance;
#if Pro
                switch (themeService.ThemeName)
                {
                    case "Red":
                        radListPicker.SelectedIndex = 0;
                        break;
                    case "Blue":
                        radListPicker.SelectedIndex = 1;
                        break;
                    case "Pink":
                        radListPicker.SelectedIndex = 2;
                        break;
                    case "Green":
                        radListPicker.SelectedIndex = 3;
                        break;
                    case "Magneta":
                        radListPicker.SelectedIndex = 4;
                        break;
                    case "Olive":
                        radListPicker.SelectedIndex = 5;
                        break;
                    case "Brown":
                        radListPicker.SelectedIndex = 6;
                        break;
                    case "Orange":
                        radListPicker.SelectedIndex = 7;
                        break;
                    case "Yellow":
                        radListPicker.SelectedIndex = 8;
                        break;
                    default:
                        radListPicker.SelectedIndex = 0;
                        break;
                }
#else
                switch (themeService.ThemeName)
                    {
                        case "Red":
                            radListPicker.SelectedIndex = 0;
                            break;
                        case "Blue":
                            radListPicker.SelectedIndex = 1;
                            break;
                        case "Pink":
                            radListPicker.SelectedIndex = 2;
                            break;
                        case "Green":
                            radListPicker.SelectedIndex = 3;
                            break;
                        default:
                            radListPicker.SelectedIndex = 0;
                            break;
                    }

#endif

            });

            radListPicker.SelectionChanged += radListPicker_SelectionChanged;
            //save current page
            PhoneApplicationService.Current.State["CurrentPage"] = "setting";

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var service = GeneralService.Instance;

            service.IsSleepTimerOn = SwitchSleepTimer.IsSwitchOn;
            service.IsListViewUsed = SwitchListView.IsSwitchOn;
            service.IsLyricsUsed = SwitchLyrics.IsSwitchOn;

            if (radListPickerLanguage.SelectedIndex == 0)
                service.LyricsLanguage = "en";
            else if (radListPickerLanguage.SelectedIndex == 2)
                service.LyricsLanguage = "vi";
            else
                service.LyricsLanguage = "cn";
            try
            {
                service.SleepTime = (TimeSpan)radTimeSpanPicker.Value;
            }
            catch
            {
                SwitchSleepTimer.IsSwitchOn = false;
                service.IsSleepTimerOn = false;
            }
            ThemeService.SaveTheme();

            base.OnNavigatedFrom(e);
        }

        private void SwitchSleepTimer_Switched(object sender, EventArgs e)
        {
            if (SwitchSleepTimer.IsSwitchOn == true)
            {
                radTimeSpanPicker.IsHitTestVisible = true;
                radTimeSpanPicker.Opacity = 1;
            }
            else
            {
                radTimeSpanPicker.IsHitTestVisible = false;
                radTimeSpanPicker.Opacity = 0.5;
            }
        }

        private void SwitchListView_Switched(object sender, EventArgs e)
        {

        }

        private void SwitchLyrics_Switched(object sender, EventArgs e)
        {

        }

        private void ButtonClearLyrics_Click(object sender, RoutedEventArgs e)
        {
            //grid_Deleting.Visibility = System.Windows.Visibility.Visible;

            Dispatcher.BeginInvoke(() =>
            {
                bool success = GeneralService.ClearAllLyrics();

                //grid_Deleting.Visibility = System.Windows.Visibility.Collapsed;
                if (success)
                {
                    MessageBox.Show("Saved lyrics cleared.");
                }
                else
                {
                    MessageBox.Show("Failed to clear saved lyrics. Please try again later.");
                }
            });
        }

        private void ButtonLiveTile_Click(object sender, RoutedEventArgs e)
        {
            GeneralService.Instance.IsLiveTileUsed = true;
            GeneralService.CreateAndUpdateLiveTile2(true);
        }

        private void radListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var index = radListPicker.SelectedIndex;
            var service = ThemeService.Instance;
            switch (index)
            {
                case 0:
                    service.AccentColor = Color.FromArgb(255, 255, 56, 56);
                    service.ThemeName = "Red";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 255, 56, 56);
                    break;
                case 1:
                    service.AccentColor = Color.FromArgb(255, 49, 179, 255);
                    service.ThemeName = "Blue";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 49, 179, 255);
                    break;
                case 2:
                    service.AccentColor = Color.FromArgb(255, 232, 84, 127);
                    service.ThemeName = "Pink";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 232, 84, 127);
                    break;
                case 3:
                    service.AccentColor = Color.FromArgb(255, 91, 255, 0);
                    service.ThemeName = "Green";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 91, 255, 0);
                    break;
#if Pro
                case 4:
                    service.AccentColor = Color.FromArgb(255, 216, 0, 115);
                    service.ThemeName = "Magenta";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 216, 0, 115);
                    break;
                case 5:
                    service.AccentColor = Color.FromArgb(255, 109, 135, 100);
                    service.ThemeName = "Olive";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 109, 135, 100);
                    break;
                case 6:
                    service.AccentColor = Color.FromArgb(255, 130, 90, 44);
                    service.ThemeName = "Brown";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 130, 90, 44);
                    break;
                case 7:
                    service.AccentColor = Color.FromArgb(255, 250, 104, 0);
                    service.ThemeName = "Orange";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 250, 104, 0);
                    break;
                case 8:
                    service.AccentColor = Color.FromArgb(255, 227, 200, 0);
                    service.ThemeName = "Yellow";
                    ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 227, 200, 0);
                    break;
#endif
            }


        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/LightTheme/InfoPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ImageBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selected = ImageBar.SelectedItem as ListBoxItem;
            //selected.BorderBrush = grid_Theme.Background;
            ((selected.Content as Grid).Children[1] as Grid).Height = 5;
            if (e.RemovedItems.Count > 0)
                (((e.RemovedItems[0] as ListBoxItem).Content as Grid).Children[1] as Grid).Height = 2;

            TextBlockSongCount.Text = "0";
            TextBlockArtistCount.Text = "0";
            TextBlockAlbumCount.Text = "0";
            TextBlockPlaylistCount.Text = "0";

            if (PivotHome.SelectedIndex == 1)
            {
                int count1 = 0, count2 = 0, count3 = 0, count4 = 0;
                ObjectKeyFrameCollection
                    keyframes1 = ((Resources["StoryboardSongCount"] as Storyboard).Children[0] as ObjectAnimationUsingKeyFrames).KeyFrames,
                    keyframes2 = ((Resources["StoryboardSongCount"] as Storyboard).Children[1] as ObjectAnimationUsingKeyFrames).KeyFrames,
                    keyframes3 = ((Resources["StoryboardSongCount"] as Storyboard).Children[2] as ObjectAnimationUsingKeyFrames).KeyFrames,
                    keyframes4 = ((Resources["StoryboardSongCount"] as Storyboard).Children[3] as ObjectAnimationUsingKeyFrames).KeyFrames;

                count1 = MediaService.Instance.SongList.Count;
                count2 = MediaService.Instance.ArtistList.Count;
                count3 = MediaService.Instance.AlbumList.Count;
                count4 = MediaService.Instance.PlaylistList.Count;

                string count1_str = count1.ToString(), count2_str = count2.ToString(),
                    count3_str = count3.ToString(), count4_str = count4.ToString();
                char pad = '0';
                //TextBlockSongCount.Text = count1_str;
                //TextBlockSongCount.Text = count2_str;
                //TextBlockSongCount.Text = count3_str;
                //TextBlockSongCount.Text = count4_str;

                keyframes1[5].Value = count1_str;
                keyframes1[4].Value = ((int)(count1 * 0.9)).ToString().PadLeft(count1_str.Length, pad);
                keyframes1[3].Value = ((int)(count1 * 0.7)).ToString().PadLeft(count1_str.Length, pad);
                keyframes1[2].Value = ((int)(count1 * 0.3)).ToString().PadLeft(count1_str.Length, pad);
                keyframes1[1].Value = ((int)(count1 * 0.1)).ToString().PadLeft(count1_str.Length, pad);

                keyframes2[5].Value = count2_str;
                keyframes2[4].Value = ((int)(count2 * 0.9)).ToString().PadLeft(count2_str.Length, pad);
                keyframes2[3].Value = ((int)(count2 * 0.7)).ToString().PadLeft(count2_str.Length, pad);
                keyframes2[2].Value = ((int)(count2 * 0.3)).ToString().PadLeft(count2_str.Length, pad);
                keyframes2[1].Value = ((int)(count2 * 0.1)).ToString().PadLeft(count2_str.Length, pad);

                keyframes3[5].Value = count3_str;
                keyframes3[4].Value = ((int)(count3 * 0.9)).ToString().PadLeft(count3_str.Length, pad);
                keyframes3[3].Value = ((int)(count3 * 0.7)).ToString().PadLeft(count3_str.Length, pad);
                keyframes3[2].Value = ((int)(count3 * 0.3)).ToString().PadLeft(count3_str.Length, pad);
                keyframes3[1].Value = ((int)(count3 * 0.1)).ToString().PadLeft(count3_str.Length, pad);

                keyframes4[5].Value = count4_str;
                keyframes4[4].Value = ((int)(count1 * 0.9)).ToString().PadLeft(count1_str.Length, pad);
                keyframes4[3].Value = ((int)(count1 * 0.7)).ToString().PadLeft(count1_str.Length, pad);
                keyframes4[2].Value = ((int)(count1 * 0.3)).ToString().PadLeft(count1_str.Length, pad);
                keyframes4[1].Value = ((int)(count1 * 0.1)).ToString().PadLeft(count1_str.Length, pad);

                (Resources["StoryboardSongCount"] as Storyboard).SkipToFill();
                (Resources["StoryboardSongCount"] as Storyboard).Begin();

            }
        }

        private void ButtonContact_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "GoMusic Suggestion";
            emailComposeTask.To = "GoMusic@mailinator.com";

            emailComposeTask.Show();
        }

        private void ButtonBuy_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();

            marketplaceDetailTask.ContentIdentifier = "97c9181c-bee9-451b-bd01-a32122f588f7";
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;

            marketplaceDetailTask.Show();
        }

        private void ButtonTheme_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/DarkTheme/ThemeSelectionPageDark.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}