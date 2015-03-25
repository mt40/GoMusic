using GoMusic.Models;
using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System;
using System.Windows;
using System.Linq;
using System.Windows.Media.Animation;

namespace GoMusic.Views.LightTheme
{
    public partial class MainPage : PhoneApplicationPage
    {
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
    }
}