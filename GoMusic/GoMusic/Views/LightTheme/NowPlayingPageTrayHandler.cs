using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.Linq;
using GoMusic.Models;

namespace GoMusic.Views.LightTheme
{
    public partial class NowPlayingPage : PhoneApplicationPage
    {
        //code of tray bar
        private bool IsAnimationPlayed;

        private void ButtonTrayPlay_Click(object sender, RoutedEventArgs e)
        {
            if (MediaService.Instance.CurrentSong != null)
            {
                Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                MediaPlayer.Resume();

                ButtonTrayPlay.Visibility = Visibility.Collapsed;
                ButtonTrayPause.Visibility = Visibility.Visible;

                GeneralService.Instance.IsMediaPlaying = true;

                if (IsAnimationPlayed)
                {
                    (Resources["DiskSpin"] as Storyboard).Resume();
                }
                else
                {
                    try
                    {
                        (Resources["DiskSpin"] as Storyboard).Begin();
                        IsAnimationPlayed = true;
                    }
                    catch { }
                }
                
                if (SongTimer == null)
                    SongTimer = new System.Windows.Threading.DispatcherTimer();
                SongTimer.Start();
            }
        }

        private void ButtonTrayPause_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaPlayer.Pause();

            ButtonTrayPlay.Visibility = Visibility.Visible;
            ButtonTrayPause.Visibility = Visibility.Collapsed;

            GeneralService.Instance.IsMediaPlaying = false;

            if (GeneralService.Instance.IsSongMenuOpened)
                (Resources["DiskSpin"] as Storyboard).Pause();

            SongTimer.Stop();
        }

        private void ButtonTrayNext_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.MoveNext();

            MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;
            //set the image and info at tray
            AlbumModel al = new AlbumModel();
            //no song in the phone
            if (MediaService.Instance.CurrentSong == null)
                return;

            al.baseAlbum = MediaService.Instance.CurrentSong.Album;
            grid_Tray.DataContext = al;

            TextBlockLyrics.Text = "";
            System.Windows.VisualStateManager.GoToState(this, "LyricsClosed", true);
            FindLyrics();

            TextBlockSongInfo.DataContext = MediaService.Instance.CurrentSong;
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
    }
}