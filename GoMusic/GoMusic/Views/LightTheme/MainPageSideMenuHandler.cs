using GoMusic.Models;
using GoMusic.Services;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace GoMusic.Views.LightTheme
{
    public partial class MainPage : PhoneApplicationPage
    {
        private void InitData(GoMusic.MyControls.SplashScreenControl spl = null)
        {
            var service = MediaService.Instance;

            //SideMenuButtonEffect(0);
            //(Resources["Loading"] as Storyboard).Begin();
            //grid_Loading.Visibility = Visibility.Visible;

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

            Dispatcher.BeginInvoke(() =>
            {

                rad_ListPlaylist.ItemsSource = service.PlaylistList;

                if (rad_ListPlaylist.GroupDescriptors.Count == 0)
                {
                    //create the group discriptor for rad jumplist
                    GenericGroupDescriptor<PlaylistModel, string> groupByName =
                        new GenericGroupDescriptor<PlaylistModel, string>(pl => pl.Name.Substring(0, 1).ToUpper());
                    rad_ListPlaylist.GroupDescriptors.Add(groupByName);
                }


                rad_GridArtist.ItemsSource = service.ArtistList;
                rad_GridAlbum.ItemsSource = service.AlbumList;
            });
            //close loading grid
            //(Resources["Loading"] as Storyboard).Stop();
            //grid_Loading.Visibility = Visibility.Collapsed;
        }

        private void ButtonSideMenuSong_Click(object sender, RoutedEventArgs e)
        {
            var service = MediaService.Instance;

            //clear itemsource of current list
            (PivotHome.SelectedItem as PivotItem).FindChildByType<RadDataBoundListBox>().ItemsSource = null;
            //change the template because song and playlist use different template
            //**Note: no need to change the template as the playlist system is no longer exist
            //rad_List.ItemTemplate = Resources["SongItemTemplate"] as DataTemplate;
            rad_List.Dispatcher.BeginInvoke(() =>
            {
                rad_List.ItemsSource = service.SongList;
            });

            //rad_Grid.Visibility = Visibility.Collapsed;
            PivotHome.IsLocked = false;
            PivotHome.SelectedIndex = 0;
            PivotHome.IsLocked = true;
            tb_Status.Text = "Songs";

            //some effects
            SideMenuButtonEffect(0);

            ////close grid_albumplaylist
            //if (GeneralService.Instance.IsAlbumPlaylistMenuOpened)
            //{
            //    grid_AlbumPlaylist.Visibility = Visibility.Collapsed;
            //}
        }

        private void ButtonSideMenuArtist_Click(object sender, RoutedEventArgs e)
        {
            var service = MediaService.Instance;

            //clear itemsource of current list
            (PivotHome.SelectedItem as PivotItem).FindChildByType<RadDataBoundListBox>().ItemsSource = null;
            rad_GridArtist.Dispatcher.BeginInvoke(() =>
            {
                rad_GridArtist.ItemsSource = service.ArtistList;

            });

            //rad_List.Visibility = Visibility.Collapsed;
            PivotHome.IsLocked = false;
            PivotHome.SelectedIndex = 1;
            PivotHome.IsLocked = true;
            tb_Status.Text = "Artists";

            //some effects
            SideMenuButtonEffect(1);

            ////close grid_albumplaylist
            //if (GeneralService.Instance.IsAlbumPlaylistMenuOpened)
            //{
            //    grid_AlbumPlaylist.Visibility = Visibility.Collapsed;
            //}
        }

        private void ButtonSideMenuAlbum_Click(object sender, RoutedEventArgs e)
        {
            var service = MediaService.Instance;
            //clear itemsource of current list
            (PivotHome.SelectedItem as PivotItem).FindChildByType<RadDataBoundListBox>().ItemsSource = null;

            rad_GridAlbum.Dispatcher.BeginInvoke(() =>
            {
                rad_GridAlbum.ItemsSource = service.AlbumList;
            });

            //rad_List.Visibility = Visibility.Collapsed;
            PivotHome.IsLocked = false;
            PivotHome.SelectedIndex = 2;
            PivotHome.IsLocked = true;
            tb_Status.Text = "Albums";

            //some effects
            SideMenuButtonEffect(2);

            ////close grid_albumplaylist
            //if (GeneralService.Instance.IsAlbumPlaylistMenuOpened)
            //{
            //    grid_AlbumPlaylist.Visibility = Visibility.Collapsed;
            //}
        }

        private void ButtonSideMenuPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var service = MediaService.Instance;
            //clear itemsource of current list
            (PivotHome.SelectedItem as PivotItem).FindChildByType<RadDataBoundListBox>().ItemsSource = null;
            //(Resources["Loading"] as Storyboard).Begin();
            //grid_Loading.Visibility = Visibility.Visible;

            //rad_List.Dispatcher.BeginInvoke(() =>
            //{
            //    rad_List.ItemTemplate = Resources["PlaylistItemTemplate"] as DataTemplate;
            //rad_List.ItemsSource = null;
            //rad_GridArtist.ItemsSource = null;
            //rad_GridAlbum.ItemsSource = null;

            //    rad_List.GroupDescriptors.Clear();
            rad_ListPlaylist.ItemsSource = service.PlaylistList;
            //    if (rad_List.GroupDescriptors.Count == 0)
            //    {
            //        //create the group discriptor for rad jumplist
            //        var groupByName =
            //            new GenericGroupDescriptor<PlaylistModel, string>(playlist => playlist.Name.Substring(0, 1).ToUpper());
            //        rad_List.GroupDescriptors.Add(groupByName);
            //    }
            //    rad_List.Visibility = Visibility.Visible;

            //    //close loading grid
            //    (Resources["Loading"] as Storyboard).Stop();
            //    grid_Loading.Visibility = Visibility.Collapsed;
            //});

            //rad_Grid.Visibility = Visibility.Collapsed;
            PivotHome.IsLocked = false;
            PivotHome.SelectedIndex = 3;
            PivotHome.IsLocked = true;
            tb_Status.Text = "Playlists";

            //some effects
            SideMenuButtonEffect(3);

            ////close grid_albumplaylist
            //if (GeneralService.Instance.IsAlbumPlaylistMenuOpened)
            //{
            //    grid_AlbumPlaylist.Visibility = Visibility.Collapsed;
            //}
        }

        private void ButtonSideMenuSetting_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/LightTheme/SettingPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SideMenuButtonEffect(int buttonID)
        {
            //var grid_song = FindVisualChildByType<Grid>(ButtonSideMenuSong, "grid_bg") as Grid;
            //var grid_artist = FindVisualChildByType<Grid>(ButtonSideMenuArtist, "grid_bg") as Grid;
            //var grid_album = FindVisualChildByType<Grid>(ButtonSideMenuAlbum, "grid_bg") as Grid;
            //var grid_playlist = FindVisualChildByType<Grid>(ButtonSideMenuPlaylist, "grid_bg") as Grid;

            //if (grid_song == null || grid_artist == null || grid_album == null) //|| grid_playlist == null)
            //    return;

            //grid_song.Background = new SolidColorBrush(Colors.Transparent);
            //grid_artist.Background = new SolidColorBrush(Colors.Transparent);
            //grid_album.Background = new SolidColorBrush(Colors.Transparent);
            //grid_playlist.Background = new SolidColorBrush(Colors.Transparent);

            ////hide shiny icon
            //(grid_song.Children[1] as Image).Visibility = Visibility.Visible;
            //(grid_song.Children[2] as Image).Visibility = Visibility.Collapsed;
            //(grid_artist.Children[1] as Image).Visibility = Visibility.Visible;
            //(grid_artist.Children[2] as Image).Visibility = Visibility.Collapsed;
            //(grid_album.Children[1] as Image).Visibility = Visibility.Visible;
            //(grid_album.Children[2] as Image).Visibility = Visibility.Collapsed;
            //(grid_playlist.Children[1] as Image).Visibility = Visibility.Visible;
            //(grid_playlist.Children[2] as Image).Visibility = Visibility.Collapsed;

            ////hide indicator
            //polygon_Song.Visibility = Visibility.Collapsed;
            //polygon_Artist.Visibility = Visibility.Collapsed;
            //polygon_Album.Visibility = Visibility.Collapsed;
            //polygon_Playlist.Visibility = Visibility.Collapsed;

            //switch (buttonID)
            //{
            //    case 0:
            //        polygon_Song.Visibility = Visibility.Visible;
            //        grid_song.Background = new SolidColorBrush(Color.FromArgb(255, 12, 14, 15));
            //        (grid_song.Children[1] as Image).Visibility = Visibility.Collapsed;
            //        (grid_song.Children[2] as Image).Visibility = Visibility.Visible;
            //        break;
            //    case 1:
            //        polygon_Artist.Visibility = Visibility.Visible;
            //        grid_artist.Background = new SolidColorBrush(Color.FromArgb(255, 12, 14, 15));
            //        (grid_artist.Children[1] as Image).Visibility = Visibility.Collapsed;
            //        (grid_artist.Children[2] as Image).Visibility = Visibility.Visible;
            //        break;
            //    case 2:
            //        polygon_Album.Visibility = Visibility.Visible;
            //        grid_album.Background = new SolidColorBrush(Color.FromArgb(255, 12, 14, 15));
            //        (grid_album.Children[1] as Image).Visibility = Visibility.Collapsed;
            //        (grid_album.Children[2] as Image).Visibility = Visibility.Visible;
            //        break;
            //    case 3:
            //        polygon_Playlist.Visibility = Visibility.Visible;
            //        grid_playlist.Background = new SolidColorBrush(Color.FromArgb(255, 12, 14, 15));
            //        (grid_playlist.Children[1] as Image).Visibility = Visibility.Collapsed;
            //        (grid_playlist.Children[2] as Image).Visibility = Visibility.Visible;
            //        break;
            //};

        }
    }
}