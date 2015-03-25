using GoMusic.Models;
using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Linq;

namespace GoMusic.Views.LightTheme
{
    public partial class NowPlayingPage : PhoneApplicationPage
    {
        public bool isTrayVolumeOpened = false;
        public DispatcherTimer SongTimer;

        public NowPlayingPage()
        {
            InitializeComponent();

            

            //GridStatus.DataContext = ThemeService.Instance;
            //ButtonTrayPause.DataContext = ThemeService.Instance;
            //SliderVolume.DataContext = ThemeService.Instance;
            this.Loaded += NowPlayingPage_Loaded;
        }

        void NowPlayingPage_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustControlAlignment();
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
            //set up the song timer
            if (SongTimer == null)
                SongTimer = new DispatcherTimer();
            SongTimer.Interval = TimeSpan.FromSeconds(1);
            SongTimer.Tick += SongTimer_Tick;
            //start the timer            
            SongTimer.Start();

            if (GeneralService.Instance.IsMediaPlaying)
            {
                ButtonTrayPause.Visibility = System.Windows.Visibility.Visible;
                ButtonTrayPlay.Visibility = System.Windows.Visibility.Collapsed;
            }

            VisualStateManager.GoToState(this, "ToSongCover", true);
            if (GeneralService.Instance.IsMediaPlaying)
            {
                (Resources["DiskSpin"] as Storyboard).Begin();
            }

            SliderVolume.Value = (double)(MediaPlayer.Volume * 10f);

            StackPanel sp = FindVisualChildByType<StackPanel>(ButtonRepeat, "stackPanel") as StackPanel;
            if (MediaPlayer.IsRepeating == true && sp != null)
            {
                MediaPlayer.IsRepeating = false;
                ((sp.Children[0] as Grid).Children[0] as Image).Visibility = Visibility.Visible;
                ((sp.Children[0] as Grid).Children[1] as Image).Visibility = Visibility.Collapsed;
                (sp.Children[1] as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255, 205, 205, 205));
            }

            sp = FindVisualChildByType<StackPanel>(ButtonShuffle, "stackPanel") as StackPanel;
            if (MediaPlayer.IsShuffled == true && sp != null)
            {
                ((sp.Children[0] as Grid).Children[0] as Image).Visibility = Visibility.Collapsed;
                ((sp.Children[0] as Grid).Children[1] as Image).Visibility = Visibility.Visible;
                (sp.Children[1] as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
            }

            sp = null;

            FindLyrics();
        }

        void AdjustControlAlignment()
        {
            double screenWidth = LayoutRoot.ActualWidth;
            double screenHeight = LayoutRoot.ActualHeight;

            if (screenWidth != 480 || screenHeight != 768)
            {
                grid_Song.RowDefinitions[1].Height = new GridLength(400 + screenHeight - 768, GridUnitType.Pixel);
                (LyricsOpened.Storyboard.Children[0] as DoubleAnimation).To = -200 - (screenHeight - 768);
            }
        }

        void SongTimer_Tick(object sender, EventArgs e)
        {

            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();

            TimeSpan time = MediaPlayer.Queue.ActiveSong.Duration;
            TimeSpan pos = MediaPlayer.PlayPosition;

            SliderSongSeeker.Maximum = time.TotalSeconds;
            var seconds = pos.TotalSeconds;
            SliderSongSeeker.Value = seconds;

            if (seconds >= 10)
            {
                var text = TextblockLyricsStatus.Text;
                if (text == "No Internet Connection" || text == "No lyrics" || text == "Cannot connect" || text == "Error processing lyrics")
                    TextblockLyricsStatus.Text = "";
            }

            TextBlockPlayingTime.Text = pos.Minutes.ToString() + ":" + pos.Seconds.ToString();
            TextBlockTotalTime.Text = time.Minutes.ToString() + ":" + time.Seconds.ToString();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //MediaService.Instance.CurrentSong = null;
            //MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;
            if(SongTimer != null && SongTimer.IsEnabled == false)
            {
                //when user switch to other app and go back to this app again
                SongTimer.Start();
                //attempt to search for lyrics because the song can be changed
                if (IsLyricsManuallyClosed == true)
                {
                    TextBlockLyrics.Visibility = System.Windows.Visibility.Visible;
                    IsLyricsManuallyClosed = false;
                }
                TextBlockLyrics.Text = "";
                VisualStateManager.GoToState(this, "LyricsClosed", true);
                FindLyrics();
            }
            var media_sv = MediaService.Instance;
            var general_sv = GeneralService.Instance;

            AlbumModel al = new AlbumModel();
            if(media_sv.CurrentSong != null)
                al.baseAlbum = media_sv.CurrentSong.Album;

            TextBlockSongInfo.DataContext = ImageSongCover.DataContext = grid_Tray.DataContext = null;
            TextBlockSongInfo.DataContext = media_sv.CurrentSong;
            ImageSongCover.DataContext = al;
            grid_Tray.DataContext = al;

            rad_PlayingSongs.ItemsSource = null;
            
            //if (general_sv.IsAlbumPlaylistMenuOpened)
                rad_PlayingSongs.ItemsSource = GeneralService.Instance.CurrentPlayingSongs;
            //else
                //rad_PlayingSongs.ItemsSource = media_sv.SongList;

            if (media_sv.CurrentSong != null)
            {
                RunSong.Text = media_sv.CurrentSong.Name;
                RunArtist.Text = media_sv.CurrentSong.Artist.Name;
            }

            GeneralService.Instance.IsSongMenuOpened = true;
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (SongTimer != null)
                SongTimer.Stop();

            rad_PlayingSongs.ItemsSource = null;

            GeneralService.Instance.IsSongMenuOpened = false;

            GeneralService.CreateAndUpdateLiveTile2();
        }

        private void PolygonToSongList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            VisualStateManager.GoToState(this, "ToSongList", true);

            (Resources["DiskSpin"] as Storyboard).Pause();
        }

        private void PolygonToSongCover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            VisualStateManager.GoToState(this, "ToSongCover", true);

            if (GeneralService.Instance.IsMediaPlaying)
                (Resources["DiskSpin"] as Storyboard).Begin();
        }

        private bool _isSingleRepeat;
        private void StackPanelRepeat_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var sp = sender as StackPanel;
            var tb = sp.Children[1] as TextBlock;
            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            
            if (MediaPlayer.IsRepeating == true)
            {
                //repeat 1 song
                if (tb.Text == "Repeat")
                {                  
                    MediaPlayer.Play(MediaService.Instance.CurrentSong);
                    MediaPlayer.IsRepeating = true;
                    ((sp.Children[0] as Grid).Children[0] as Image).Visibility = Visibility.Collapsed;
                    ((sp.Children[0] as Grid).Children[1] as Image).Visibility = Visibility.Visible;
                    tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
                    tb.Text = "Repeat 1";
                    _isSingleRepeat = true;
                }
                else
                {
                    var playlist = GeneralService.Instance.CurrentSongCollection;
                    MediaPlayer.Play(playlist, GeneralService.Instance.CurrentSongIndex);
                    MediaPlayer.IsRepeating = false;
                    ((sp.Children[0] as Grid).Children[0] as Image).Visibility = Visibility.Visible;
                    ((sp.Children[0] as Grid).Children[1] as Image).Visibility = Visibility.Collapsed;
                    tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 205, 205, 205));
                    tb.Text = "Repeat";
                    _isSingleRepeat = false;
                }

            }
            else
            {
                MediaPlayer.IsRepeating = true;
                ((sp.Children[0] as Grid).Children[0] as Image).Visibility = Visibility.Collapsed;
                ((sp.Children[0] as Grid).Children[1] as Image).Visibility = Visibility.Visible;
                tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
                _isSingleRepeat = false;
            }
            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
        }

        private void StackPanelShuffle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var sp = sender as StackPanel;

            if (MediaPlayer.IsShuffled == true)
            {
                MediaPlayer.IsShuffled = false;
                ((sp.Children[0] as Grid).Children[0] as Image).Visibility = Visibility.Visible;
                ((sp.Children[0] as Grid).Children[1] as Image).Visibility = Visibility.Collapsed;
                (sp.Children[1] as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255, 205, 205, 205));
            }
            else
            {
                MediaPlayer.IsShuffled = true;
                ((sp.Children[0] as Grid).Children[0] as Image).Visibility = Visibility.Collapsed;
                ((sp.Children[0] as Grid).Children[1] as Image).Visibility = Visibility.Visible;
                (sp.Children[1] as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
            }
            Microsoft.Xna.Framework.FrameworkDispatcher.Update();
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;

            if (MediaService.Instance.CurrentSong == null)
                return;

            //grid_Tray.Dispatcher.BeginInvoke(() =>
            //{
            //set the image and info at tray
            AlbumModel al = new AlbumModel();
            al.baseAlbum = MediaService.Instance.CurrentSong.Album;
            grid_Tray.DataContext = al;

            RunSong.Text = MediaService.Instance.CurrentSong.Name;
            RunArtist.Text = MediaService.Instance.CurrentSong.Artist.Name;
            //});

            TextBlockSongInfo.DataContext = MediaService.Instance.CurrentSong;
            rad_PlayingSongs.SelectedItem = MediaService.Instance.CurrentSong;

            TextBlockLyrics.Text = "";

            FindLyrics();
        }

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            switch (MediaPlayer.State)
            {
                case MediaState.Playing:
                    ButtonTrayPlay.Visibility = Visibility.Collapsed;
                    ButtonTrayPause.Visibility = Visibility.Visible;
                    break;
                case MediaState.Paused:
                    ButtonTrayPlay.Visibility = Visibility.Visible;
                    ButtonTrayPause.Visibility = Visibility.Collapsed;
                    (Resources["DiskSpin"] as Storyboard).Pause();


                    MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;
                    TextBlockSongInfo.DataContext = MediaService.Instance.CurrentSong;

                    break;
                case MediaState.Stopped:
                    ButtonTrayPlay.Visibility = Visibility.Visible;
                    ButtonTrayPause.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void stackPanel_SongItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            var song = (sender as StackPanel).DataContext as Song;
            MediaService.Instance.CurrentSong = song;

            //must be called before MediaPlayer.Play() method
            Microsoft.Xna.Framework.FrameworkDispatcher.Update();

            MediaLibrary lib = new MediaLibrary();
            if (_isSingleRepeat)
                MediaPlayer.Play(song);
            else
                MediaPlayer.Play(lib.Songs, MediaService.Instance.SongList.IndexOf(song));

            TextBlockSongInfo.DataContext = song;
            grid_Tray.Dispatcher.BeginInvoke(() =>
            {
                //set the image and info at tray
                AlbumModel al = new AlbumModel();
                al.baseAlbum = song.Album;
                grid_Tray.DataContext = al;

                RunSong.Text = song.Name;
                RunArtist.Text = song.Artist.Name;

                ButtonTrayPlay.Visibility = Visibility.Collapsed;
                ButtonTrayPause.Visibility = Visibility.Visible;
            });

            if (IsLyricsManuallyClosed == true)
                TextBlockLyrics.Visibility = System.Windows.Visibility.Visible;
            TextBlockLyrics.Text = "";

            VisualStateManager.GoToState(this, "LyricsClosed", true);
            IsLyricsManuallyClosed = false;
            (Resources["DiskSpin"] as Storyboard).Begin();
            FindLyrics();


            //FindLyrics();
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newvalue = Math.Round(e.NewValue);
            SliderVolume.Value = newvalue;
            TextBlockVolume.Text = newvalue.ToString();
            Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaPlayer.Volume = (float)newvalue * 0.1f;
        }

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

        private bool IsLyricsManuallyClosed = false;

        private void ImageSongCoverGestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.VerticalVelocity > 50 && (Math.Abs(e.Angle - 90) < 45))
            {
                if (TextBlockLyrics.Text.Length > 3 && IsLyricsManuallyClosed == false)
                {
                    VisualStateManager.GoToState(this, "LyricsClosed", true);
                    TextBlockLyrics.Visibility = Visibility.Collapsed;
                    IsLyricsManuallyClosed = true;
                }
            }
            else if (e.VerticalVelocity < -50 && (Math.Abs(e.Angle - 270) < 45))
            {
                if (IsLyricsManuallyClosed == true)
                {
                    VisualStateManager.GoToState(this, "LyricsOpened", true);
                    TextBlockLyrics.Visibility = System.Windows.Visibility.Visible;
                    IsLyricsManuallyClosed = false;
                }
            }
        }
    }
}