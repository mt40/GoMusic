using GoMusic.Models;
using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using GoMusic.MyControls;

namespace GoMusic.Views.LightTheme
{
    public partial class MainPage : PhoneApplicationPage
    {
        //public Song CurrentSong = MediaPlayer.Queue.ActiveSong;
        public Song CurrentSelectedSong = null;
        public List<Song> CurrentPlaylist = null;
        public bool isTrayVolumeOpened = false;
        public DispatcherTimer SleepTimer;

        private Popup popup;
        private BackgroundWorker backroungWorker;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //initialize crash reporter
            //HockeyApp.CrashHandler.Instance.HandleCrashes();
            if (MediaPlayer.Queue.Count != 0)
            {
                MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;
            }

            this.Loaded += MainPage_Loaded;

            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;

            //initialize theme setting
            //var sv2 = ThemeService.Instance;
            //grid_Theme.DataContext = sv2;
            //grid_Exit.DataContext = sv2;

            this.popup = new Popup();
            this.popup.Child = new SplashScreenControl()
            {
                Height = LayoutRoot.ActualHeight,
                Width = LayoutRoot.ActualWidth
            };
            this.popup.IsOpen = true;

            
            //SideMenuButtonEffect(0);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (GeneralService.Instance.IsFirstLaunch == false)
                return;
            AdjustControlAlignment();
            ShowSplash();
            //InitData();
            //MediaPlayer.IsRepeating = false;

            SliderVolume.Value = (double)(MediaPlayer.Volume * 10f);


            ////play intro animation
            //(Resources["Intro"] as Storyboard).Begin();

            GeneralService generalSv = GeneralService.Instance;
            generalSv.IsFirstLaunch = false;

            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["CurrentPage"] = "main";

            if (generalSv.UseCount % 10 == 0 && generalSv.UseCount != 0 && generalSv.IsRateAllowed)
            {
                ShowRateMessage();
            }
        }

        private void ShowRateMessage()
        {
            Button rateButton = new Button { Content = "Rate", Template = Resources["RateButtonTemplate"] as ControlTemplate },
                    cancelButton = new Button { Content = "Cancel", Template = Resources["RateCancelButtonTemplate"] as ControlTemplate },
                    neverButton = new Button { Content = "Never", Template = Resources["RateNeverButtonTemplate"] as ControlTemplate };

            List<Button> buttonList = new List<Button>
            {
                rateButton,
                cancelButton,
                neverButton
            };
            MessagePrompt msgPrompt = new MessagePrompt
            {
                Title = "What do you think?",
                Body = new TextBlock
                {
                    Text = "Would you like to spend 5 seconds to rate the app?",
                    FontSize = 22,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Segoe WP SemiLight")
                },
                ActionPopUpButtons = buttonList,
                Style = Resources["RateMessageBoxTemplate"] as Style,

            };
            msgPrompt.Show();

            rateButton.Click += (o, args) =>
            {
                MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                marketplaceReviewTask.Show();
                msgPrompt.Hide();
                //never rate again
                GeneralService.Instance.IsRateAllowed = false;
            };
            cancelButton.Click += (o, args) =>
            {
                msgPrompt.Hide();
            };
            neverButton.Click += (o, args) =>
            {
                //never rate again
                GeneralService.Instance.IsRateAllowed = false;
                msgPrompt.Hide();
            };
        }

        private void ShowSplash()
        {
            PivotHome.Dispatcher.BeginInvoke(() =>
            {
                InitData(this.popup.Child as SplashScreenControl);
                this.popup.IsOpen = false;
            });
            //StartLoadingData();
        }

        private void StartLoadingData()
        {
            backroungWorker = new BackgroundWorker();
            backroungWorker.DoWork += new DoWorkEventHandler(backroungWorker_DoWork);
            backroungWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backroungWorker_RunWorkerCompleted);
            backroungWorker.RunWorkerAsync();

        }

        void backroungWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //here we can load data
            System.Threading.Thread.Sleep(9000);
        }

        void backroungWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.popup.IsOpen = false;
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var mediaService = MediaService.Instance;
            var generalService = GeneralService.Instance;

            //update the current song
            try
            {
                mediaService.CurrentSong = MediaPlayer.Queue.ActiveSong;
            }
            catch { }
            if (mediaService.CurrentSong != null)
            {
                RunSong.Text = mediaService.CurrentSong.Name;
                RunArtist.Text = mediaService.CurrentSong.Artist.Name;
            }

            if (generalService.IsMediaPlaying == true)
            {
                ButtonTrayPause.Visibility = System.Windows.Visibility.Visible;
                ButtonTrayPlay.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ButtonTrayPause.Visibility = System.Windows.Visibility.Collapsed;
                ButtonTrayPlay.Visibility = System.Windows.Visibility.Visible;
            }

            //set up the sleep timer
            if (generalService.IsSleepTimerOn)
            {
                if (SleepTimer != null)
                {
                    if (generalService.SleepTime.TotalMinutes != SleepTimer.Interval.TotalMinutes)
                    {
                        SleepTimer.Stop();
                        SleepTimer.Interval = generalService.SleepTime;
                        SleepTimer.Tick += SleepTimer_Tick;
                        SleepTimer.Start();

                        generalService.SleepTimerStartTime = TimeSpan.FromMinutes(DateTime.Now.Hour * 60 + DateTime.Now.Minute);
                    }
                }
                else
                {
                    SleepTimer = new DispatcherTimer();
                    SleepTimer.Interval = generalService.SleepTime;
                    SleepTimer.Tick += SleepTimer_Tick;
                    SleepTimer.Start();
                    generalService.SleepTimerStartTime = TimeSpan.FromMinutes(DateTime.Now.Hour * 60 + DateTime.Now.Minute);
                }
            }

            System.Windows.Input.Touch.FrameReported += Touch_FrameReported;

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// No need for this method anymore since the Canvas structure is gone. But preserve this for maybe future reuse
        /// </summary>
        private void AdjustControlAlignment()
        {

            //double screenWidth = LayoutRoot.ActualWidth;
            //double screenHeight = LayoutRoot.ActualHeight;
            //if (screenHeight != 768 || screenWidth != 480)
            //{
            //    //adjust alignment, size of control depends on screen size
            //    CanvasMain.Height = screenHeight;
            //    grid_Tray.Width = screenWidth;
            //    grid_Tray.Margin = new Thickness(110, screenHeight - 80 + 2, 0, 0);
            //    grid_MainContent.Height = screenHeight - 80;
            //    grid_MainContent.Width = screenWidth;
            //    //ButtonSideMenuSetting.Margin = new Thickness(0, 200 + screenHeight - 768, 0, 0);

            //}
        }

        void SleepTimer_Tick(object sender, EventArgs e)
        {
            MediaPlayer.Pause();
            MediaPlayer.Stop();
            SleepTimer.Stop();
            (Resources["DiskSpin"] as Storyboard).Pause();

            GeneralService.Instance.IsSleepTimerOn = false;
            SleepTimer = null;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //GeneralService.SavePlaylist();
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["CurrentPage"] = "main";
            System.Windows.Input.Touch.FrameReported -= Touch_FrameReported;
            //GeneralService.CreateAndUpdateLiveTile2();
            base.OnNavigatedFrom(e);
        }

        #region old playlist code
        //private void grid_AddToPlaylist_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    isAddToPlaylistMenuOpened = true;

        //    var song = (sender as FrameworkElement).DataContext as Song;
        //    CurrentSelectedSong = song;

        //    grid_AddToPlaylist.Visibility = Visibility.Visible;

        //    //reset itemSource
        //    rad_PlaylistList.ItemsSource = null;
        //    var service = MediaService.Instance;
        //    rad_PlaylistList.ItemsSource = service.PlaylistList;

        //    //close the previous song item
        //    if (previousSongItem != null)
        //    {
        //        var transform = previousSongItem.RenderTransform as CompositeTransform;
        //        transform.TranslateX = -1.0 * transform.CenterX;
        //        previousSongItem = null;

        //        isSongItemGestureListening = false;
        //    }

        //    e.Handled = true;

        //    if (isAlbumPlaylistMenuOpened == true)
        //    {

        //    }
        //}

        //private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    //exit to main page
        //    grid_AddToPlaylist.Visibility = Visibility.Collapsed;
        //    isAddToPlaylistMenuOpened = false;
        //}

        //private void ButtonAddPlaylist_Click(object sender, RoutedEventArgs e)
        //{
        //    if (TextBoxNewPlaylistName.Text == "")
        //        return;

        //    PlaylistModel newPlaylist = new PlaylistModel();
        //    newPlaylist.Name = TextBoxNewPlaylistName.Text;

        //    var service = PlaylistService.Instance;
        //    service.Playlists.Add(newPlaylist);

        //    TextBoxNewPlaylistName.Text = "";

        //    rad_PlaylistList.ItemsSource = null;
        //    rad_PlaylistList.ItemsSource = MediaService.Instance.PlaylistList;

        //    ButtonSideMenuPlaylist.DataContext = null;
        //    ButtonSideMenuPlaylist.DataContext = MediaService.Instance.PlaylistList;
        //}
        #endregion

        private void stackPanel_SongItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            var mediaService = MediaService.Instance;
            var song = (sender as StackPanel).DataContext as Song;
            mediaService.CurrentSong = song;

            MediaLibrary lib = new MediaLibrary();
            MediaPlayer.Play(lib.Songs, mediaService.SongList.IndexOf(song));

            //set the image and info at tray
            AlbumModel al = new AlbumModel();
            al.baseAlbum = song.Album;
            grid_Tray.DataContext = al;

            RunSong.Text = song.Name;
            RunArtist.Text = song.Artist.Name;

            ButtonTrayPlay.Visibility = Visibility.Collapsed;
            ButtonTrayPause.Visibility = Visibility.Visible;

            var generalService = GeneralService.Instance;
            generalService.IsMediaPlaying = true;
            //OpenSongMenu();
            generalService.CurrentPlayingSongs = rad_List.ItemsSource;
            generalService.CurrentSongCollection = lib.Songs;
            generalService.CurrentSongIndex = mediaService.SongList.IndexOf(song);

        }

        void OpenSongMenu()
        {
            var service = GeneralService.Instance;

            GeneralService.Instance.IsSongMenuOpened = true;

            if (ThemeService.Instance.CurrentThemeType == ThemesType.LightSquare)
                NavigationService.Navigate(new Uri("/Views/LightTheme/NowPlayingPage_Square.xaml", UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/Views/LightTheme/NowPlayingPage.xaml", UriKind.Relative));
        }

        private void ButtonPlaylistItemClick(object sender, RoutedEventArgs e)
        {
            var playlist = (sender as FrameworkElement).DataContext as PlaylistModel;
            PhoneApplicationService.Current.State["status"] = "Playlists";
            //pass selected album or playlist;
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] = playlist;
            NavigationService.Navigate(new Uri("/Views/LightTheme/AlbumPlaylistPage.xaml", UriKind.Relative));

            GeneralService.Instance.IsAlbumPlaylistMenuOpened = true;
        }

        private void ButtonAlbumItem_Click(object sender, RoutedEventArgs e)
        {
            var album = (sender as FrameworkElement).DataContext as AlbumModel;
            PhoneApplicationService.Current.State["status"] = "Albums";
            //pass selected album or playlist;
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] = album;
            NavigationService.Navigate(new Uri("/Views/LightTheme/AlbumPlaylistPage.xaml", UriKind.Relative));
        }

        private void ButtonArtistItem_Click(object sender, RoutedEventArgs e)
        {
            var artist = (sender as FrameworkElement).DataContext as ArtistModel;
            PhoneApplicationService.Current.State["status"] = "Artists";
            //pass selected album or playlist;
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] = artist;
            NavigationService.Navigate(new Uri("/Views/LightTheme/AlbumPlaylistPage.xaml", UriKind.Relative));
        }

        private bool isExitConfirm = false;
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var service = GeneralService.Instance;
            //jump list is still open
            if (rad_GridAlbum.IsGroupPickerOpen || rad_List.IsGroupPickerOpen || rad_ListPlaylist.IsGroupPickerOpen || rad_GridArtist.IsGroupPickerOpen)
                return;
            service.IsFirstTime = false;
            //}
            //else
            //{
            if (isExitConfirm == false && rad_List.IsGroupPickerOpen == false)
            {
                VisualStateManager.GoToState(this, "GridExitOpened", true);
                isExitConfirm = true;
                e.Cancel = true;
            }
            GeneralService.CreateAndUpdateLiveTile2();
            //}
            //}
        }

        private void ButtonExitCancel_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "GridExitClosed", true);
            isExitConfirm = false;
        }

        public static object FindVisualChildByType<T>(DependencyObject element, String name)
        {
            if (element == null) return null;
            var frameworkElement = element as FrameworkElement;
            if (frameworkElement == null) return null;
            if (frameworkElement.Name.Equals(name)) return frameworkElement;
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                object childElement = FindVisualChildByType<T>(VisualTreeHelper.GetChild(element, i), name);
                if (childElement != null)
                    return childElement;
            }
            return null;
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newvalue = Math.Round(e.NewValue);
            SliderVolume.Value = newvalue;
            TextBlockVolume.Text = newvalue.ToString();
            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            //MediaPlayer.Volume = (float)newvalue * 0.1f;

            //ISimpleAudioVolume
        }

        //double initialPosition;
        //bool _viewMoved = false;
        //void MoveViewWindow(double left)
        //{
        //    _viewMoved = true;

        //    //((Storyboard)canvas.Resources["moveAnimation"]).SkipToFill();
        //    ((DoubleAnimation)((Storyboard)canvas.Resources["moveAnimation"]).Children[0]).To = left;
        //    //((DoubleAnimation)((Storyboard)canvas.Resources["moveAnimation"]).Children[1]).To = left + 100;
        //    ((Storyboard)canvas.Resources["moveAnimation"]).Begin();
        //}

        //private void GestureListener_DragStarted2(object sender, DragStartedGestureEventArgs e)
        //{
        //    _viewMoved = false;
        //    initialPosition = Canvas.GetLeft(CanvasMain);
        //}

        //private void GestureListener_DragDelta2(object sender, DragDeltaGestureEventArgs e)
        //{
        //    if (e.HorizontalChange != 0)
        //    {
        //        Canvas.SetLeft(CanvasMain, Math.Min(Math.Max(-110, Canvas.GetLeft(CanvasMain) + e.HorizontalChange), 0));
        //        if (PivotHome.IsHitTestVisible)
        //            PivotHome.IsHitTestVisible = false;
        //    }
        //}

        //private void GestureListener_DragCompleted2(object sender, DragCompletedGestureEventArgs e)
        //{
        //    var left = Canvas.GetLeft(CanvasMain);
        //    if (_viewMoved)
        //        return;
        //    if (Math.Abs(initialPosition - left) < 60)
        //    {
        //        //bouncing back
        //        MoveViewWindow(initialPosition);
        //        return;
        //    }
        //    //change of state  
        //    if (initialPosition - left < 0)
        //    {
        //        //slide to the right  
        //        MoveViewWindow(0);
        //    }
        //    else
        //    {
        //        //slide to the left  
        //        MoveViewWindow(-110);
        //    }

        //    PivotHome.IsHitTestVisible = true;
        //}

        private void ImageBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selected = ImageBar.SelectedItem as ListBoxItem;
            //selected.BorderBrush = grid_Theme.Background;
            ((selected.Content as Grid).Children[1] as Grid).Height = 5;
            if (e.RemovedItems.Count > 0)
                (((e.RemovedItems[0] as ListBoxItem).Content as Grid).Children[1] as Grid).Height = 2;

            switch (PivotHome.SelectedIndex)
            {
                case 0:
                    tb_Status.Text = "Songs";
                    break;
                case 1:
                    tb_Status.Text = "Artists";
                    break;
                case 2:
                    tb_Status.Text = "Albums";
                    break;
                case 3:
                    tb_Status.Text = "Playlists";
                    break;
            }
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/LightTheme/SettingPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/LightTheme/SearchPage.xaml", UriKind.RelativeOrAbsolute));
        }

        bool isImageBarHidden = false;
        System.Windows.Input.TouchPoint first_point = null;
        double gesture_distance = 0d;
        void Touch_FrameReported(object sender, System.Windows.Input.TouchFrameEventArgs e)
        {           
            if (first_point == null)
            {
                first_point = e.GetPrimaryTouchPoint(tb_Status);
                return;
            }
            var last_point = e.GetPrimaryTouchPoint(tb_Status);
            bool isScrolling = rad_List.ScrollState == ScrollState.Scrolling || rad_ListPlaylist.ScrollState == ScrollState.Scrolling
                || rad_GridArtist.ScrollState == ScrollState.Scrolling || rad_GridAlbum.ScrollState == ScrollState.Scrolling;
            if (first_point != null && last_point != null && isScrolling)
            {
                double distance = last_point.Position.Y - first_point.Position.Y;
                gesture_distance += last_point.Position.Y;

                if(Math.Abs(gesture_distance) >= 3500d)
                {
                    if (distance < -5d && isImageBarHidden == false)
                    {
                        //HideImageBar();                        
                    }
                    else if(distance > 5d && isImageBarHidden == true)
                    {
                        //ShowImageBar();                       
                    }
                    gesture_distance = 0;
                }
                first_point = null;
            }
            else
                gesture_distance = 0;
        }

        /// <summary>
        /// Hide the bar with 4 images above the pivot
        /// </summary>
        void HideImageBar()
        {
            try
            {
                var anim = Resources["ImageBarHide"] as Storyboard;
                //anim.SkipToFill();
                anim.Begin();
                isImageBarHidden = true;
            }
            catch { }
        }

        void ShowImageBar()
        {
            try
            {
                var anim = Resources["ImageBarShow"] as Storyboard;
                //anim.SkipToFill();
                anim.Begin();
                isImageBarHidden = false;
            }
            catch { }
        }

        private async void rad_GridArtist_DataRequested(object sender, EventArgs e)
        {
            await MediaService.Instance.LoadMoreArtist(18);
        }

        private async void rad_GridAlbum_DataRequested(object sender, EventArgs e)
        {
            await MediaService.Instance.LoadMoreAlbum(18);
        }
    }
}