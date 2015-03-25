using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GoMusic.Services;
using GoMusic.Models;
using Microsoft.Xna.Framework.Media;
using System.Collections.ObjectModel;

namespace GoMusic
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public bool isTrayVolumeOpened = false;

        public SearchPage()
        {
            InitializeComponent();

            //var sv2 = ThemeService.Instance;
            //grid_Theme.DataContext = sv2;

            GeneralService.LoadRecentQuery();
            ListRecent.ItemsSource = GeneralService.Instance.RecentSearch;

            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GeneralService.Instance.IsAlbumPlaylistMenuOpened = true;

            //grid_HeaderInfo.DataContext = null;
            //rad_AlbumPlaylistSongList.ItemsSource = null;
            //rad_AlbumPlaylistAlbumList.ItemsSource = null;
            //string status = PhoneApplicationService.Current.State["status"] as string;
            //tb_Status.Text = status;
            //if (status == "Albums")
            //{
            //    var album = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] as AlbumModel;
            //    TextBlockArtistAlbumCount.Visibility = Visibility.Collapsed;
            //    rad_AlbumPlaylistAlbumList.Visibility = Visibility.Collapsed;
            //    grid_HeaderInfo.DataContext = album;
            //    rad_AlbumPlaylistSongList.ItemsSource = album.Songs;
            //    rad_AlbumPlaylistAlbumList.ItemsSource = null;
            //}
            //else if (status == "Artists")
            //{
            //    var artist = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] as ArtistModel;
            //    grid_AlbumPlaylist.Visibility = Visibility.Visible;
            //    TextBlockArtistAlbumCount.Visibility = Visibility.Visible;
            //    rad_AlbumPlaylistAlbumList.Visibility = Visibility.Visible;
            //    grid_HeaderInfo.DataContext = artist;
            //    rad_AlbumPlaylistSongList.ItemsSource = artist.Songs;
            //    rad_AlbumPlaylistAlbumList.ItemsSource = artist.Albums;
            //}
            //else
            //{
            //    var playlist = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["abplData"] as PlaylistModel;
            //    TextBlockArtistAlbumCount.Visibility = Visibility.Collapsed;
            //    rad_AlbumPlaylistAlbumList.Visibility = Visibility.Collapsed;
            //    grid_HeaderInfo.DataContext = playlist;
            //    rad_AlbumPlaylistSongList.ItemsSource = playlist.Songs;
            //    rad_AlbumPlaylistAlbumList.ItemsSource = null;
            //}

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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                GeneralService.Instance.IsAlbumPlaylistMenuOpened = false;
            }
        }

        private void stackPanel_SongItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //if (isAddToPlaylistMenuOpened == true)
            //    return;
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

            //GeneralService.Instance.CurrentPlayingSongs = rad_AlbumPlaylistSongList.ItemsSource;
            GeneralService.Instance.IsMediaPlaying = true;
            //OpenSongMenu();
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

        void OpenSongMenu()
        {
            var service = GeneralService.Instance;
            //if (service.IsAlbumPlaylistMenuOpened)
            //{
            //service.CurrentPlayingSongs = rad_AlbumPlaylistSongList.ItemsSource;
            //}
            //open grid_Song
            GeneralService.Instance.IsSongMenuOpened = true;

            if (ThemeService.Instance.CurrentThemeType == ThemesType.LightSquare)
                NavigationService.Navigate(new Uri("/Views/LightTheme/NowPlayingPage_Square.xaml", UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/Views/LightTheme/NowPlayingPage.xaml", UriKind.Relative));
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaService.Instance.CurrentSong = MediaPlayer.Queue.ActiveSong;

            if (MediaService.Instance.CurrentSong == null)
                return;

            //set the image and info at tray
            AlbumModel al = new AlbumModel();
            al.baseAlbum = MediaService.Instance.CurrentSong.Album;
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
                    break;
                case MediaState.Paused:
                    ButtonTrayPlay.Visibility = Visibility.Visible;
                    ButtonTrayPause.Visibility = Visibility.Collapsed;
                    break;
                case MediaState.Stopped:
                    ButtonTrayPlay.Visibility = Visibility.Visible;
                    ButtonTrayPause.Visibility = Visibility.Collapsed;
                    break;
            }
        }

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

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newvalue = Math.Round(e.NewValue);
            SliderVolume.Value = newvalue;
            TextBlockVolume.Text = newvalue.ToString();
            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            //MediaPlayer.Volume = (float)newvalue * 0.1f;
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

        private void RadTextBox_ActionButtonTap(object sender, EventArgs e)
        {
            if (RadSearchBox.Text.IsEmptyOrWhiteSpace())
                return;

            SearchQuery query = new SearchQuery(RadSearchBox.Text.Trim());
            List<SearchQuery> recent = GeneralService.Instance.RecentSearch;
            SearchQuery old = recent.SingleOrDefault(r => r.Query == query.Query);
            if (old != null)
            {
                recent.Remove(old);
                recent.Insert(0, query);
            }
            else
            {
                recent.Insert(0, query);
                if (recent.Count > 6)
                    recent.RemoveAt(recent.Count - 1);
            }

            GeneralService.SaveRecentQuery();

            SearchForMedia();

            //close the onscreen keyboard
            this.Focus();
            //ListRecent.ItemsSource = GeneralService.Instance.RecentSearch;
        }

        private void SearchForMedia()
        {
            this.ProgessBarLoading.IsIndeterminate = true;

            PivotHome.Dispatcher.BeginInvoke(() =>
            {
                string search = RadSearchBox.Text.Trim();

                //search for song
                ObservableCollection<Song> song_rs = Utility.SearchForNumberOfSong(search, 15, MediaService.Instance.SongList);
                rad_SongResultList.ItemsSource = song_rs;

                //search for artist
                ObservableCollection<ArtistModel> artist_rs = Utility.SearchForNumberOfArtist(search, 15, MediaService.Instance.ArtistList);
                rad_ArtistResultList.ItemsSource = artist_rs;

                //search for album
                ObservableCollection<AlbumModel> album_rs = Utility.SearchForNumberOfAlbum(search, 15, MediaService.Instance.AlbumList);
                rad_AlbumResultList.ItemsSource = album_rs;


                //search for playlist
                ObservableCollection<PlaylistModel> playlist_rs = Utility.SearchForNumberOfPlaylist(search, 15, MediaService.Instance.PlaylistList);
                rad_PlaylistResultList.ItemsSource = playlist_rs;

                this.ProgessBarLoading.IsIndeterminate = false;
            });
        }

        private void RadSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //show the recent search query
        }

        private void RadSearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //hide the recent search query
        }

        private void ImageBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selected = ImageBar.SelectedItem as ListBoxItem;
            //selected.BorderBrush = grid_Theme.Background;
            ((selected.Content as Grid).Children[1] as Grid).Height = 5;
            if (e.RemovedItems.Count > 0)
                (((e.RemovedItems[0] as ListBoxItem).Content as Grid).Children[1] as Grid).Height = 2;

            //switch (PivotHome.SelectedIndex)
            //{
            //    case 0:
            //        tb_Status.Text = "Songs";
            //        break;
            //    case 1:
            //        tb_Status.Text = "Artists";
            //        break;
            //    case 2:
            //        tb_Status.Text = "Albums";
            //        break;
            //    case 3:
            //        tb_Status.Text = "Playlists";
            //        break;
            //}
        }

        private void ButtonRecentSearchItem_Click(object sender, RoutedEventArgs e)
        {
            RadSearchBox.Text = ((sender as FrameworkElement).DataContext as SearchQuery).Query;

            SearchQuery query = new SearchQuery(RadSearchBox.Text.Trim());
            List<SearchQuery> recent = GeneralService.Instance.RecentSearch;
            SearchQuery old = recent.SingleOrDefault(r => r.Query == query.Query);
            if (old != null)
            {
                recent.Remove(old);
                recent.Insert(0, query);
            }
            else
            {
                recent.Insert(0, query);
                if (recent.Count > 6)
                    recent.RemoveAt(recent.Count - 1);
            }

            GeneralService.SaveRecentQuery();

            SearchForMedia();
        }

        private void RadSearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                if (RadSearchBox.Text.IsEmptyOrWhiteSpace())
                    return;

                SearchQuery query = new SearchQuery(RadSearchBox.Text.Trim());
                List<SearchQuery> recent = GeneralService.Instance.RecentSearch;
                SearchQuery old = recent.SingleOrDefault(r=>r.Query == query.Query);
                if (old != null)
                {
                    recent.Remove(old);
                    recent.Insert(0, query);
                }
                else
                {
                    recent.Insert(0, query);
                    if (recent.Count > 6)
                        recent.RemoveAt(recent.Count - 1);
                }

                GeneralService.SaveRecentQuery();

                SearchForMedia();

                //close the onscreen keyboard
                this.Focus();
            }
        }

    }
}