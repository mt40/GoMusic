using GoMusic.Models;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;


namespace GoMusic.Services
{
    public sealed class GeneralService
    {
        //this service provides methods for data storage and some utilities

        private static readonly GeneralService instance = new GeneralService();

        private GeneralService()
        {
            RecentSearch = new List<SearchQuery>();

        }

        public static GeneralService Instance
        {
            get { return instance; }
        }

        #region OldPlaylist Code
        //public static void SavePlaylist()
        //{
        //    var store = IsolatedStorageFile.GetUserStoreForApplication();
        //    string folderName = "data";
        //    string fileName = "playlists";

        //    if (!store.DirectoryExists(folderName))
        //    {
        //        store.CreateDirectory(folderName);
        //    }

        //    string filePath = string.Format("{0}\\{1}.go", folderName, fileName);

        //    using (var stream = new IsolatedStorageFileStream(filePath, FileMode.Create, store))
        //    {
        //        //because Song cannot be serialized directly so we need a fake class
        //        //that only contains Name and Artist properties
        //        List<PlaylistModel> fakePlaylists = new List<PlaylistModel>();
        //        var playlists = PlaylistService.Instance.Playlists;
        //        foreach (var pl in playlists)
        //        {
        //            PlaylistModel fakePl = new PlaylistModel();
        //            fakePl.Name = pl.Name;

        //            foreach (var song in pl.Songs)
        //            {
        //                FakeSong tmp = new FakeSong(song);
        //                fakePl.FakeSongs.Add(tmp);
        //            }
        //            fakePlaylists.Add(fakePl);
        //        }
        //        //serialize the list
        //        var serializer = new XmlSerializer(typeof(List<PlaylistModel>));
        //        serializer.Serialize(stream, fakePlaylists);
        //    }
        //}

        //public static List<PlaylistModel> LoadPlaylist()
        //{
        //    var store = IsolatedStorageFile.GetUserStoreForApplication();
        //    string filePath = "/data/playlists.go";

        //    if (store.FileExists(filePath))
        //    {
        //        using (var stream = new IsolatedStorageFileStream(filePath, FileMode.Open, FileAccess.Read, store))
        //        {
        //            var reader = new StreamReader(stream);

        //            if (!reader.EndOfStream)
        //            {
        //                var serializer = new XmlSerializer(typeof(List<PlaylistModel>));
        //                return (List<PlaylistModel>)serializer.Deserialize(reader);
        //            }
        //        }
        //    }
        //    //failed
        //    return null;
        //}

        #endregion

        public SongCollection CurrentSongCollection { get; set; }

        public System.Collections.IEnumerable CurrentPlayingSongs { get; set; }

        public int CurrentSongIndex { get; set; }

        public bool IsAlbumPlaylistMenuOpened { get; set; }

        public bool IsSongMenuOpened { get; set; }

        public bool IsSideMenuOpened { get; set; }

        public bool IsMediaPlaying { get; set; }

        public bool IsFirstLaunch { get; set; }

        //true if app is reactivated from tombstone
        public bool IsReactivated { get; set; }

        public bool IsFirstTime
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("FirstTime"))
                    return (bool)settings["FirstTime"];
                return true;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["FirstTime"] = value;
                settings.Save();
            }
        }

        public bool IsRateAllowed
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("IsRateAllowed"))
                    return (bool)settings["IsRateAllowed"];
                return true;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["IsRateAllowed"] = value;
                settings.Save();
            }
        }

        public int UseCount
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("UseCount"))
                    return (int)settings["UseCount"];
                return 0;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["UseCount"] = value;
                settings.Save();
            }
        }

        public bool IsSleepTimerOn
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("SleepTimer"))
                    return (bool)settings["SleepTimer"];
                return false;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["SleepTimer"] = value;
                settings.Save();
            }
        }

        public bool IsListViewUsed
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("ListView"))
                    return (bool)settings["ListView"];
                return false;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["ListView"] = value;
                settings.Save();
            }
        }

        public bool IsLiveTileUsed
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("LiveTile"))
                    return (bool)settings["LiveTile"];
                return false;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["LiveTile"] = value;
                settings.Save();
            }
        }

        public bool IsDarkThemeUsed
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("MainTheme"))
                    return (bool)settings["MainTheme"];
                return false;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["MainTheme"] = value;
                settings.Save();
            }
        }

        public string LyricsLanguage
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("LyricsLanguage"))
                    return (string)settings["LyricsLanguage"];
                return "en";
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["LyricsLanguage"] = value;
                settings.Save();
            }
        }

        public TimeSpan SleepTime
        {
            get;
            set;
        }

        public TimeSpan SleepTimerStartTime
        {
            get;
            set;
        }

        public List<SearchQuery> RecentSearch;

        public bool IsLyricsUsed
        {
            get
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("Lyrics"))
                    return (bool)settings["Lyrics"];
                return true;
            }
            set
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["Lyrics"] = value;
                settings.Save();
            }
        }

        //public static int InitPercent = 0;

        public static bool IsLyricsExist(string key)    //key is song name
        {
            try
            {
                var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                var fileName = Utility.CleanInput(key);
                var fileName_hex = BitConverter.ToString(Encoding.Unicode.GetBytes(fileName)) + ".lrc";
                //var filePath = "/Shared/Lyrics/" + fileName.Replace("<", "").Replace(">", "").Replace(@"""", "").Replace("|", "").Replace("\r", "");
                var filePath = "/Shared/Lyrics/" + fileName_hex;
                //filePath = Utility.CleanInput(filePath);
                return isoStore.FileExists(filePath) ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Save lyrics string to Isolated storage</summary>
        /// <param name="key"> file name</param>
        public static void SaveLyricsToStorage(string key, string lyrics)
        {
            var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isoStore.DirectoryExists("/Shared/Lyrics"))
            {
                isoStore.CreateDirectory("/Shared/Lyrics");
            }
            try
            {
                var fileName = Utility.CleanInput(key);
                var fileName_hex = BitConverter.ToString(Encoding.Unicode.GetBytes(fileName)) + ".lrc";
                //var filePath = "/Shared/Lyrics/" + fileName.Replace("<", "").Replace(">", "").Replace(@"""", "").Replace("|", "").Replace("\r", "");
                var filePath = @"/Shared/Lyrics/" + fileName_hex;
                if (!isoStore.FileExists(filePath))
                {
                    using (StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(filePath, FileMode.Create, FileAccess.Write, isoStore)))
                    {
                        writer.WriteLine(lyrics);
                        writer.Close();
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>Return a lyrics string Isolated storage</summary>
        /// <param name="key"> file name</param>
        public static string LoadLyricsFromStorage(string key)
        {
            try
            {
                var isoStore = IsolatedStorageFile.GetUserStoreForApplication();

                var fileName = Utility.CleanInput(key);
                var fileName_hex = BitConverter.ToString(Encoding.Unicode.GetBytes(fileName)) + ".lrc";
                var filePath = @"/Shared/Lyrics/" + fileName_hex;

                if (isoStore.FileExists(filePath))
                {
                    IsolatedStorageFileStream stream = isoStore.OpenFile(filePath, FileMode.Open, FileAccess.Read);

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();
                        reader.Close();
                        return result;
                    }
                }
                return null;
            }
            catch (ArgumentException e)
            {
                return null;
            }

        }

        /// <summary>
        /// <para>Clear all saved lyrics in Isolated storage.</para>
        /// <para>Return true if success, otherwise return false.</para>
        /// </summary>
        public static bool ClearAllLyrics()
        {
            try
            {
                var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoStore.DirectoryExists("/Shared/Lyrics"))
                {
                    string[] files = isoStore.GetFileNames("/Shared/Lyrics\\*");
                    if (files.Length > 0)
                    {
                        for (int i = 0; i < files.Length; ++i)
                        {
                            // Delete the files.
                            isoStore.DeleteFile("/Shared/Lyrics\\" + files[i]);
                        }


                    }
                    // Confirm that no files remain.
                    files = isoStore.GetFileNames("/Shared/Lyrics\\*");
                    if (files.Length == 0)
                    {
                        isoStore.DeleteDirectory("/Shared/Lyrics");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// <para>Create new instance of live tile if there are none.</para>
        /// <para>Else update current live tile</para>
        /// </summary>
        public static void CreateAndUpdateLiveTile(bool special = false)
        {
            // find the tile object for the application tile that using "flip" contains string in it.
            ShellTile oTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString()));

            if (oTile != null && oTile.NavigationUri.ToString().Contains("flip"))
            {
                var service = GeneralService.instance;
                var mediaService = MediaService.Instance;
                if (!service.IsLiveTileUsed)
                    service.IsLiveTileUsed = true;

                string backContent = "No song", wbContent = "No song";
                if (mediaService.CurrentSong != null)
                {
                    if (mediaService.CurrentSong.Name.Length > 27)
                        backContent = mediaService.CurrentSong.Name.Substring(0, 27) + "\n-" + mediaService.CurrentSong.Artist.Name;
                    else
                        backContent = mediaService.CurrentSong.Name + "\n-" + mediaService.CurrentSong.Artist.Name;
                    wbContent = mediaService.CurrentSong.Name + "\n-" + mediaService.CurrentSong.Artist.Name;
                }

                FlipTileData oFliptile = new FlipTileData();
                oFliptile.Title = "GoMusic";
                oFliptile.BackContent = backContent;
                oFliptile.WideBackContent = wbContent;

                oFliptile.SmallBackgroundImage = new Uri("/Assets/appicon5_300.png", UriKind.Relative);
                oFliptile.BackgroundImage = new Uri("/Assets/appicon5.png", UriKind.Relative);
                oFliptile.WideBackgroundImage = new Uri("/Assets/wideTile2.png", UriKind.Relative);

                //oFliptile.BackBackgroundImage = new Uri("/Assets/Images/tile_bg2.png", UriKind.Relative);
                //oFliptile.WideBackBackgroundImage = new Uri("/Assets/wideTile_bg.png", UriKind.Relative);
                oTile.Update(oFliptile);
            }
            else
            {
                var service = GeneralService.instance;
                if (service.IsLiveTileUsed && !service.IsFirstTime && !special)
                {
                    //if user manually delete live tile outside this app
                    service.IsLiveTileUsed = false;
                    return;
                }

                if (GeneralService.instance.IsLiveTileUsed)
                {
                    // once it is created flip tile
                    string uri = "/Views/LightTheme/MainPage.xaml?tile=flip";
                    if (GeneralService.instance.IsDarkThemeUsed)
                        uri = "/Views/DarkTheme/MainPageDark.xaml?tile=flip";
                    Uri tileUri = new Uri("/MainPage.xaml?tile=flip", UriKind.Relative);
                    ShellTileData tileData = CreateFlipTileData();
                    ShellTile.Create(tileUri, tileData, true);
                }
            }
        }

        public static void CreateAndUpdateLiveTile2(bool forceCreate = false)
        {
            ShellTile flipTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString()));
            if (flipTile != null)
            {
                DeleteExistingFlipTile();
                //CreateNormalTile();
            }
            if (forceCreate)
                CreateNormalTile();

            IEnumerator<ShellTile> it = ShellTile.ActiveTiles.GetEnumerator();
            it.MoveNext();
            ShellTile tile = it.Current;

            if (tile != null)
            {
                var newTileData = CreateNormalTileData();
                tile.Update(newTileData);
            }
        }

        public static FlipTileData CreateNormalTileData()
        {
            var service = GeneralService.instance;
            var mediaService = MediaService.Instance;
            if (!service.IsLiveTileUsed)
                service.IsLiveTileUsed = true;

            string backContent = "No song", wbContent = "No song";
            if (mediaService.CurrentSong != null)
            {
                if (mediaService.CurrentSong.Name.Length > 27)
                    backContent = mediaService.CurrentSong.Name.Substring(0, 27) + "\n-" + mediaService.CurrentSong.Artist.Name;
                else
                    backContent = mediaService.CurrentSong.Name + "\n-" + mediaService.CurrentSong.Artist.Name;
                wbContent = mediaService.CurrentSong.Name + "\n-" + mediaService.CurrentSong.Artist.Name;
            }

            FlipTileData oFliptile = new FlipTileData();
            oFliptile.Title = "GoMusic";
            oFliptile.BackContent = backContent;
            oFliptile.WideBackContent = wbContent;
                        
            bool check_img = SaveCurrentSongCover(), check_img_wide = SaveCurrentSongCover(width: 691);
            oFliptile.SmallBackgroundImage = new Uri("/Assets/appicon5_300.png", UriKind.Relative);
            oFliptile.BackgroundImage = new Uri("/Assets/appicon5.png", UriKind.Relative);
            oFliptile.WideBackgroundImage = new Uri("/Assets/wideTile2.png", UriKind.Relative);

            //if there is no image display text instead
            if (check_img)
                oFliptile.BackBackgroundImage = new Uri(string.Format("isostore:{0}", "/Shared/ShellContent/currentCover336x336.jpg"), UriKind.Absolute);   
            //else
                
            if(check_img_wide)
                oFliptile.WideBackBackgroundImage = new Uri(string.Format("isostore:{0}", "/Shared/ShellContent/currentCover691x336.jpg"), UriKind.Absolute);
            //else
                
                

            return oFliptile;
        }

        public static void CreateNormalTile()
        {            
            string uri = "/Views/LightTheme/MainPage.xaml?tile=std";
            if (GeneralService.instance.IsDarkThemeUsed)
                uri = "/Views/DarkTheme/MainPageDark.xaml?tile=std";

            ShellTile.Create(new Uri(uri, UriKind.Relative), CreateFlipTileData(), false);
        }

        public static bool SaveCurrentSongCover(int width = 336, int height = 336)
        {
            // Create a filename for JPEG file in isolated storage.
            string tempJPEG = "/Shared/ShellContent/currentCover" + width.ToString() + "x" + height.ToString() + ".jpg";
            bool result = false;
            // Create virtual store and file stream. Check for duplicate tempJPEG files.
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(tempJPEG))
                {
                    myIsolatedStorage.DeleteFile(tempJPEG);
                }

                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(tempJPEG);

                var song = MediaService.Instance.CurrentSong;
                if (song != null)
                {
                    try
                    {
                        WriteableBitmap wb = Microsoft.Phone.PictureDecoder.DecodeJpeg(song.Album.GetAlbumArt(), width, height);

                        Canvas canvas = new Canvas() { Width = width, Height = height };
                        ImageBrush brush = new ImageBrush();
                        brush.ImageSource = wb; brush.Opacity = 0.5;
                        canvas.Background = brush;
                        Rectangle rec = new Rectangle() { Width = 65, Height = 65, Fill = new SolidColorBrush(Color.FromArgb(255, 36, 35, 38)) };
                        Canvas.SetTop(rec, height - 65); Canvas.SetLeft(rec, width - 65);       
                        Image icon = new Image() { Width = 60, Height = 60 }; Canvas.SetTop(icon, height - 60); Canvas.SetLeft(icon, width - 60);                    
                        icon.Source = new BitmapImage(new Uri("/Assets/appicon5_100.png", UriKind.RelativeOrAbsolute)) { DecodePixelWidth = 60, DecodePixelHeight = 60 };
                        canvas.Children.Add(rec);
                        canvas.Children.Add(icon);

                        WriteableBitmap bg = new WriteableBitmap(width, height);
                        bg.Clear(Colors.Black);
                        bg.Render(canvas, null);
                        bg.Invalidate();
                        // Encode WriteableBitmap object to a JPEG stream.
                        Extensions.SaveJpeg(bg, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 90);
                        result = true;
                    }
                    catch { result = false; }
                }
                //wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                fileStream.Close();
            }
            return result;
        }

        public static void SaveRecentQuery()
        {
            var IsoStore = IsolatedStorageFile.GetUserStoreForApplication();
            string folderName = "/Shared/Lyrics", dataName = "RecentQuery";

            if (!IsoStore.DirectoryExists(folderName))
            {
                IsoStore.CreateDirectory(folderName);
            }

            string fileStreamName = string.Format("{0}\\{1}.dat", folderName, dataName);

            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileStreamName, FileMode.Create, IsoStore))
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(List<SearchQuery>));
                dcs.WriteObject(stream, GeneralService.instance.RecentSearch);
            }
        }

        public static void LoadRecentQuery()
        {
            var result = new List<SearchQuery>();
            //if (result == null)
            //    return;

            var IsoStore = IsolatedStorageFile.GetUserStoreForApplication();
            string folderName = "/Shared/Lyrics", dataName = "RecentQuery";

            if (!IsoStore.DirectoryExists(folderName))
            {
                IsoStore.CreateDirectory(folderName);
            }

            string fileStreamName = string.Format("{0}\\{1}.dat", folderName, dataName);

            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileStreamName, FileMode.OpenOrCreate, IsoStore))
            {
                if (stream.Length > 0)
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(List<SearchQuery>));
                    result = dcs.ReadObject(stream) as List<SearchQuery>;
                }
            }

            GeneralService.instance.RecentSearch = result;
        }

        private static ShellTileData CreateFlipTileData()
        {
            var service = MediaService.Instance;
            string backContent = "No song", wbContent = "No song";
            if (service.CurrentSong != null)
            {
                if (service.CurrentSong.Name.Length > 27)
                    backContent = service.CurrentSong.Name.Substring(0, 27) + "\n-" + service.CurrentSong.Artist.Name;
                else
                    backContent = service.CurrentSong.Name + "\n-" + service.CurrentSong.Artist.Name;
                wbContent = service.CurrentSong.Name + "\n-" + service.CurrentSong.Artist.Name;
            }
            return new FlipTileData()
            {
                Title = "GoMusic",
                BackTitle = "Recent",
                BackContent = backContent,
                WideBackContent = wbContent,
                SmallBackgroundImage = new Uri("/Assets/appicon5_300.png", UriKind.Relative),
                BackgroundImage = new Uri("/Assets/appicon5.png", UriKind.Relative),
                //BackBackgroundImage = new Uri("/Assets/Images/tile_bg2.png", UriKind.Relative),
                WideBackgroundImage = new Uri("/Assets/wideTile2.png", UriKind.Relative),
                //WideBackBackgroundImage = new Uri("/Assets/wideTile.png", UriKind.Relative)
            };
        }

        public static void DeleteExistingFlipTile()
        {
            var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString()));

            // If the Tile was found, then delete it.  
            if (foundTile != null)
            {
                foundTile.Delete();
            }
        }
    }

    public static class Utility
    {
        public static bool IsBetween(this char c, double a, double b)
        {
            return (a <= (int)c && (int)c <= b) ? true : false;
        }

        public static bool IsEmptyOrWhiteSpace(this string s)
        {
            if (s == null)
                return true;
            return s.All(char.IsWhiteSpace);
        }

        public static int Clamp(int n, int min, int max)
        {
            if (n < min) return min;
            if (n > max) return max;
            return n;
        }

        public static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings. 
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "");
            }
            // If we timeout when replacing invalid characters,  
            // we should return Empty. 
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string CleanChineseLyrics(string pattern)
        {
            //replace "[...]" in lyrics
            try
            {
                string rs = Regex.Replace(pattern, @"\[.+\]", "");
                return rs;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Search for songs according to percent
        /// </summary>
        /// <param name="target">song name to search</param>
        /// <param name="percent">matching percent</param>
        /// <returns></returns>
        public static ObservableCollection<Song> SearchForSong(string target, double percent, ObservableCollection<Song> source)
        {
            ObservableCollection<Song> result = new ObservableCollection<Song>(source.Where(s =>
                StringSimilarityAlgorithms.DiceCoefficient.PercentMatchTo(s.Name, target) >= percent).ToList().OrderBy(s => s.Name).ToList());

            return result;
        }

        /// <summary>
        /// Search until get enough results
        /// </summary>
        /// <param name="target">song name to search</param>
        /// <param name="count">number of desired results</param>
        /// <returns></returns>
        public static ObservableCollection<Song> SearchForNumberOfSong(string target, int count, ObservableCollection<Song> source)
        {
            double percent = 1.0;
            List<Song> result = new List<Song>();
            while (result.Count < count && percent > 0.1)
            {
                result.AddRange(source.Where(s => (StringSimilarityAlgorithms.DiceCoefficient.PercentMatchTo(s.Name, target) >= percent
                    && !result.Contains(s))).ToList().OrderBy(s => s.Name).ToList());

                percent -= 0.1;
            }
            if (result.Count > count)
            {
                int dif = result.Count - count;
                result.RemoveRange(count - 1, dif);
            }
            return new ObservableCollection<Song>(result);
        }

        public static ObservableCollection<ArtistModel> SearchForNumberOfArtist(string target, int count, ObservableCollection<ArtistModel> source)
        {
            double percent = 1.0;
            List<ArtistModel> result = new List<ArtistModel>();
            while (result.Count < count && percent > 0.1)
            {
                result.AddRange(source.Where(s => (StringSimilarityAlgorithms.DiceCoefficient.PercentMatchTo(s.Name, target) >= percent
                    && !result.Contains(s))).ToList().OrderBy(s => s.Name).ToList());

                percent -= 0.1;
            }
            if (result.Count > count)
            {
                int dif = result.Count - count;
                result.RemoveRange(count - 1, dif);
            }
            return new ObservableCollection<ArtistModel>(result);
        }

        public static ObservableCollection<AlbumModel> SearchForNumberOfAlbum(string target, int count, ObservableCollection<AlbumModel> source)
        {
            double percent = 1.0;
            List<AlbumModel> result = new List<AlbumModel>();
            while (result.Count < count && percent > 0.1)
            {
                result.AddRange(source.Where(s => (StringSimilarityAlgorithms.DiceCoefficient.PercentMatchTo(s.Name, target) >= percent
                    && !result.Contains(s))).ToList().OrderBy(s => s.Name).ToList());

                percent -= 0.1;
            }
            if (result.Count > count)
            {
                int dif = result.Count - count;
                result.RemoveRange(count - 1, dif);
            }
            return new ObservableCollection<AlbumModel>(result);
        }

        public static ObservableCollection<PlaylistModel> SearchForNumberOfPlaylist(string target, int count, ObservableCollection<PlaylistModel> source)
        {
            double percent = 1.0;
            List<PlaylistModel> result = new List<PlaylistModel>();
            while (result.Count < count && percent > 0.1)
            {
                result.AddRange(source.Where(s => (StringSimilarityAlgorithms.DiceCoefficient.PercentMatchTo(s.Name, target) >= percent
                    && !result.Contains(s))).ToList().OrderBy(s => s.Name).ToList());

                percent -= 0.1;
            }
            if (result.Count > count)
            {
                int dif = result.Count - count;
                result.RemoveRange(count - 1, dif);
            }
            return new ObservableCollection<PlaylistModel>(result);
        }
    }
}
