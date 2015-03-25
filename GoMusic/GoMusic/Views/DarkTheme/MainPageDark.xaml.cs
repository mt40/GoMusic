using Coding4Fun.Toolkit.Controls;
using GoMusic.Models;
using GoMusic.MyControls;
using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using Telerik.Windows.Data;

namespace GoMusic.Views.DarkTheme
{
    public partial class MainPageDark : PhoneApplicationPage
    {
        //public Song CurrentSong = MediaPlayer.Queue.ActiveSong;
        public Song CurrentSelectedSong = null;
        public List<Song> CurrentPlaylist = null;
        public bool isTrayVolumeOpened = false;
        public DispatcherTimer SleepTimer;

        private Popup popup;
        private BackgroundWorker backroungWorker;

        // Constructor
        public MainPageDark()
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
            GC.Collect();
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

            //PivotHome.Dispatcher.BeginInvoke(() =>
            //{
            //    if ((Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("CurrentPage") &&
            //        Microsoft.Phone.Shell.PhoneApplicationService.Current.State["CurrentPage"] as string == "setting") ||
            //        GeneralService.Instance.IsFirstLaunch == true || GeneralService.Instance.IsReactivated == true)
            //    {
            //        InitData();
            //        var service = MediaService.Instance;
            //        //count the number of songs, album,...
            //        ButtonSideMenuSong.DataContext = service.SongList;
            //        ButtonSideMenuArtist.DataContext = service.ArtistList;
            //        ButtonSideMenuAlbum.DataContext = service.AlbumList;
            //        ButtonSideMenuPlaylist.DataContext = service.PlaylistList;
            //    }
            //});
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
            GeneralService.CreateAndUpdateLiveTile();
            base.OnNavigatedFrom(e);
        }

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

            if(ThemeService.Instance.CurrentThemeType == ThemesType.DarkSquare)
                NavigationService.Navigate(new Uri("/Views/DarkTheme/NowPlayingPageDark_Square.xaml", UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/Views/DarkTheme/NowPlayingPageDark.xaml", UriKind.Relative));
        }

        private void ButtonPlaylistItemClick(object sender, RoutedEventArgs e)
        {
            var playlist = (sender as FrameworkElement).DataContext as PlaylistModel;
            PhoneApplicationService.Current.State["status"] = "Playlists";
            //pass selected album or playlist;
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] = playlist;
            NavigationService.Navigate(new Uri("/Views/DarkTheme/AlbumPlaylistPageDark.xaml", UriKind.Relative));

            GeneralService.Instance.IsAlbumPlaylistMenuOpened = true;
        }

        private void ButtonAlbumItem_Click(object sender, RoutedEventArgs e)
        {
            var album = (sender as FrameworkElement).DataContext as AlbumModel;
            PhoneApplicationService.Current.State["status"] = "Albums";
            //pass selected album or playlist;
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] = album;
            NavigationService.Navigate(new Uri("/Views/DarkTheme/AlbumPlaylistPageDark.xaml", UriKind.Relative));
        }

        private void ButtonArtistItem_Click(object sender, RoutedEventArgs e)
        {
            var artist = (sender as FrameworkElement).DataContext as ArtistModel;
            PhoneApplicationService.Current.State["status"] = "Artists";
            //pass selected album or playlist;
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] = artist;
            NavigationService.Navigate(new Uri("/Views/DarkTheme/AlbumPlaylistPageDark.xaml", UriKind.Relative));
        }

        private bool isExitConfirm = false;
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var service = GeneralService.Instance;
            //jump list is still open
            if (rad_GridAlbum.IsGroupPickerOpen || rad_List.IsGroupPickerOpen || rad_ListPlaylist.IsGroupPickerOpen || rad_GridArtist.IsGroupPickerOpen)
                return;
            if (service.IsFirstTime && !service.IsLiveTileUsed)
            {
                if (MessageBox.Show("", "Do you want to create Live Tile?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //e.Cancel = true;
                    service.IsLiveTileUsed = true;
                    GeneralService.CreateAndUpdateLiveTile();
                }
                else
                {
                    service.IsLiveTileUsed = false;
                    service.IsFirstTime = false;
                }

                service.IsFirstTime = false;
            }
            else
            {
                if (isExitConfirm == false && rad_List.IsGroupPickerOpen == false)
                {
                    VisualStateManager.GoToState(this, "GridExitOpened", true);
                    isExitConfirm = true;
                    e.Cancel = true;
                }
                GeneralService.CreateAndUpdateLiveTile();
            }
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
            NavigationService.Navigate(new Uri("/Views/DarkTheme/SettingPageDark.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/DarkTheme/SearchPageDark.xaml", UriKind.RelativeOrAbsolute));
        }

        private void InitData(GoMusic.MyControls.SplashScreenControl spl = null)
        {
            var service = MediaService.Instance;

            //clear all previous data
            rad_GridArtist.ItemsSource = null;
            rad_GridAlbum.ItemsSource = null;
            rad_ListPlaylist.ItemsSource = null;
            //rad_List.ItemsSource = service.SongList;
            rad_GridArtist.GroupDescriptors.Clear();
            rad_GridAlbum.GroupDescriptors.Clear();

            rad_List.ItemsSource = service.SongList;


            if (rad_List.GroupDescriptors.Count == 0)
            {
                //create the group discriptor for rad jumplist
                GenericGroupDescriptor<Song, string> groupByName =
                    new GenericGroupDescriptor<Song, string>(song => song.Name.Substring(0, 1).ToUpper());
                rad_List.GroupDescriptors.Add(groupByName);
            }

            rad_ListPlaylist.ItemsSource = service.PlaylistList;

            if (rad_ListPlaylist.GroupDescriptors.Count == 0)
            {
                //create the group discriptor for rad jumplist
                GenericGroupDescriptor<PlaylistModel, string> groupByName =
                    new GenericGroupDescriptor<PlaylistModel, string>(song => song.Name.Substring(0, 1).ToUpper());
                rad_ListPlaylist.GroupDescriptors.Add(groupByName);
            }

            Dispatcher.BeginInvoke(() =>
            {
                rad_GridArtist.ItemsSource = service.ArtistList;
                rad_GridAlbum.ItemsSource = service.AlbumList;
            });
        }

        #region Tray code
        private void ButtonTrayPlay_Click(object sender, RoutedEventArgs e)
        {
            if (MediaService.Instance.CurrentSong != null)
            {
                Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                MediaPlayer.Resume();

                ButtonTrayPlay.Visibility = Visibility.Collapsed;
                ButtonTrayPause.Visibility = Visibility.Visible;

                GeneralService.Instance.IsMediaPlaying = true;
            }
        }

        private void ButtonTrayPause_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaPlayer.Pause();

            ButtonTrayPlay.Visibility = Visibility.Visible;
            ButtonTrayPause.Visibility = Visibility.Collapsed;

            GeneralService.Instance.IsMediaPlaying = false;
        }

        private void ButtonTrayNext_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaPlayer.MoveNext();

            MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;
            //set the image and info at tray
            AlbumModel al = new AlbumModel();
            //no song in the phone
            if (MediaService.Instance.CurrentSong == null)
                return;

            al.baseAlbum = MediaService.Instance.CurrentSong.Album;
            grid_Tray.DataContext = al;

            RunSong.Text = MediaService.Instance.CurrentSong.Name;
            RunArtist.Text = MediaService.Instance.CurrentSong.Artist.Name;
        }

        private void ButtonTrayPrevious_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaPlayer.MovePrevious();

            MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;
            //set the image and info at tray
            AlbumModel al = new AlbumModel();
            //no song in the phone
            if (MediaService.Instance.CurrentSong == null)
                return;

            al.baseAlbum = MediaService.Instance.CurrentSong.Album;
            grid_Tray.DataContext = al;

            RunSong.Text = MediaService.Instance.CurrentSong.Name;
            RunArtist.Text = MediaService.Instance.CurrentSong.Artist.Name;
        }

        private void TextBlockTraySongInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var CurrentSong = MediaService.Instance.CurrentSong;

            if (!GeneralService.Instance.IsSongMenuOpened && CurrentSong != null)
            {
                OpenSongMenu();
            }
        }

        private void ImageTraySongCover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var CurrentSong = MediaService.Instance.CurrentSong;

            if (!GeneralService.Instance.IsSongMenuOpened && CurrentSong != null)
            {
                //open grid_Song
                OpenSongMenu();
            }
        }
        #endregion

        #region Media code
        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;

            if (MediaService.Instance.CurrentSong == null)
                return;

            //set the image and info at tray
            AlbumModel al = new AlbumModel();
            al.baseAlbum = MediaService.Instance.CurrentSong.Album;
            grid_Tray.DataContext = null;
            grid_Tray.DataContext = al;

            RunSong.Text = MediaService.Instance.CurrentSong.Name;
            RunArtist.Text = MediaService.Instance.CurrentSong.Artist.Name;
        }

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            switch (MediaPlayer.State)
            {
                case MediaState.Playing:
                    ButtonTrayPlay.Visibility = Visibility.Collapsed;
                    ButtonTrayPause.Visibility = Visibility.Visible;
                    GeneralService.Instance.IsMediaPlaying = true;
                    break;
                case MediaState.Paused:
                    ButtonTrayPlay.Visibility = Visibility.Visible;
                    ButtonTrayPause.Visibility = Visibility.Collapsed;
                    GeneralService.Instance.IsMediaPlaying = false;
                    MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;

                    #region old_playlist_code
                    ////play next song
                    //if (CurrentPlaylist != null && CurrentSong != null && MediaPlayer.State == MediaState.Paused)
                    //{
                    //    var index = CurrentPlaylist.IndexOf(CurrentSong) + 1;
                    //    if (index == CurrentPlaylist.Count)
                    //        index = 0;
                    //    CurrentSong = CurrentPlaylist[index];
                    //    MediaPlayer.Play(CurrentSong);

                    //    grid_HeaderInfo.DataContext = CurrentSong;
                    //}
                    #endregion

                    break;
                case MediaState.Stopped:
                    ButtonTrayPlay.Visibility = Visibility.Visible;
                    ButtonTrayPause.Visibility = Visibility.Collapsed;
                    GeneralService.Instance.IsMediaPlaying = false;
                    break;
            }
        }
        #endregion

        #region Gesture code
        private void grid_Tray_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.TotalManipulation.Translation.Y < -40 && isTrayVolumeOpened == false)
            {
                VisualStateManager.GoToState(this, "TrayVolumeOpened", true);
                isTrayVolumeOpened = true;
            }
        }

        private void ButtonTrayVolumeClose_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "TrayVolumeClosed", true);
            isTrayVolumeOpened = false;
        }
        #endregion

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