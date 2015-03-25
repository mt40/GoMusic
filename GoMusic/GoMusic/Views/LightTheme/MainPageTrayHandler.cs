using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.Linq;
using GoMusic.Models;

namespace GoMusic.Views.LightTheme
{
    public partial class MainPage : PhoneApplicationPage
    {
        //code of tray bar

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
    }
}