using GoMusic.Services;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using System.Text;
using System.Windows;
using GoMusic.Framework;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;

namespace GoMusic.Views.LightTheme
{
    public partial class NowPlayingPage : PhoneApplicationPage
    {
        private void FindLyrics()
        {
            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            MediaService.Instance.CurrentSong = Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSong;

            var service = GeneralService.Instance;

            var song = MediaService.Instance.CurrentSong;
            string key;
            if (song.Artist.Name.Length > 5)
                key = song.Name + song.Artist.Name.Substring(0, 5);
            else
                key = song.Name + song.Artist.Name;
            if (GeneralService.IsLyricsExist(key))
            {
                //if this lyrics is already downloaded before, use it instead of download it again
                string lyrics = GeneralService.LoadLyricsFromStorage(key);

                TextBlockLyrics.Dispatcher.BeginInvoke(() =>
                {
                    TextBlockLyrics.Text = lyrics;
                    TextblockLyricsStatus.Text = "";
                });
                System.Windows.VisualStateManager.GoToState(this, "LyricsOpened", true);
                return;
            }

            if (service.IsLyricsUsed == false)
            {
                return;
            }



            if (song != null)
            {
                string name = song.Name.Replace("  ", " ").Replace(" ", "-");
                string artist = song.Artist.Name.Replace(";", " ").Replace("ft.", " ").Replace("ft", " ").Replace("Ft.", "").Replace("&", " ").
                    Replace(",", " ").Replace("   ", "-").Replace("  ", "-");

                if (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    HtmlWeb web = new HtmlWeb();
                    web.LoadCompleted += web_LoadCompleted_vn_step1;
                    string url = "http://search.chiasenhac.com/search.php?s=" + name + "-" + artist;
                    if (GeneralService.Instance.LyricsLanguage == "cn")
                    {
                        url = "http://so.1ting.com/song?q=" + name.Replace(' ', '+') + "&s=1";
                        web.LoadCompleted -= web_LoadCompleted_vn_step1;
                        web.LoadCompleted += web_LoadCompleted_cn_step1;
                    }

                    TextblockLyricsStatus.Text = "Finding lyrics ...";
                    grid_SongCover.Dispatcher.BeginInvoke(() =>
                    {
                        web.LoadAsync(url);
                    });
                }
                else
                {
                    TextblockLyricsStatus.Text = "No Internet Connection";
                    return;
                }
            }
        }

        #region code for china lyrics website
        /// <summary>
        /// Analyze structure and get lyrics from Chinese website. The code may look ugly but at least it works!
        /// </summary>
        void web_LoadCompleted_cn_step1(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Document == null)
            {
                TextblockLyricsStatus.Text = "Cannot connect";
                return;
            }
            var song = MediaService.Instance.CurrentSong;

            HtmlDocument doc = e.Document;
            HtmlNode parent = doc.DocumentNode.SelectSingleNode("//div[@class='songList']");
            if (parent == null)
            {
                TextblockLyricsStatus.Text = "No lyrics";
                return;
            }
            HtmlNodeCollection rs_songs = parent.SelectNodes(".//td[@class='song']");
            HtmlNodeCollection rs_artists = parent.SelectNodes(".//td[@class='singer']");
            if (rs_songs == null || rs_artists == null)
            {
                TextblockLyricsStatus.Text = "No lyrics";
                return;
            }
            if (song.Artist.Name == "Unknown" || song.Artist.Name == "unknown" || song.Artist.Name == " ")
            {
                TextblockLyricsStatus.Text = "No lyrics";
                return;
            }

            for (int i = 0; i < rs_songs.Count; i++)
            {
                var item = rs_songs[i];
                //only search for the first 3 songs
                if (i >= 3)
                {
                    TextblockLyricsStatus.Text = "No lyrics";
                    return;
                }

                #region compare local song artist with the result song artist
                var artist_node = rs_artists[i];
                if (artist_node == null || artist_node.InnerText == null) continue;  //check for error

                StringBuilder sb_artist = new StringBuilder(artist_node.InnerText)
                    .Replace(";", " ").Replace("ft.", " ").Replace("ft", " ").Replace("Ft.", "").Replace("&", " ").
                    Replace(",", " ").Replace("   ", " ").Replace("  ", " ");
                StringBuilder sb_songArtist = new StringBuilder(song.Artist.Name)
                    .Replace(";", " ").Replace("ft.", " ").Replace("ft", " ").Replace("Ft.", "").Replace("&", " ").
                    Replace(",", " ").Replace("   ", " ").Replace("  ", " ");

                string artist = sb_artist.ToString().ToLower();
                string songArtist = sb_songArtist.ToString().ToLower();

                double percent = GoMusic.StringSimilarityAlgorithms.DiceCoefficient.PercentMatchTo(artist, songArtist);
                if (percent >= 0.0)
                {
                    var url_node = item.SelectSingleNode(".//a");
                    if (url_node == null) continue;
                    var url = url_node.GetAttributeValue("href", "");
                    if (url != "")
                    {
                        string key = url.Substring(url.LastIndexOf("player_") + 7, url.LastIndexOf(".html") - url.LastIndexOf("player_") - 7);
                        HtmlWeb web = new HtmlWeb();
                        web.LoadCompleted += web_LoadCompleted_cn_step2;

                        grid_SongCover.Dispatcher.BeginInvoke(() =>
                        {
                            web.LoadAsync("http://www.1ting.com/lrc" + key + ".html");
                            return;
                        });
                    }
                    //else
                    //    TextblockLyricsStatus.Text = "No lyrics";
                    //return;
                }
                #endregion
            }
        }

        void web_LoadCompleted_cn_step2(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Document == null)
            {
                TextblockLyricsStatus.Text = "Cannot connect";
                return;
            }

            HtmlDocument doc = e.Document;

            StringBuilder sb_lyrics;
            var lyrics_node = doc.DocumentNode.SelectSingleNode("//div[@id='container']");
            var lyrics_child_node = lyrics_node.SelectSingleNode(".//div[@id='lrc']");
            if (lyrics_node != null)
            {
                if (lyrics_child_node != null)
                    lyrics_node = lyrics_child_node;
                sb_lyrics = new StringBuilder(lyrics_node.InnerText);
            }
            else
            {
                sb_lyrics = new StringBuilder("");
                TextblockLyricsStatus.Text = "No lyrics";
                return;
            }
            string lyrics = sb_lyrics.ToString();
            lyrics = Utility.CleanChineseLyrics(lyrics);

            if (lyrics != "")
            {
                TextBlockLyrics.Text = lyrics.ToString();
                TextblockLyricsStatus.Text = "";

                System.Windows.VisualStateManager.GoToState(this, "LyricsOpened", true);
                var song = MediaService.Instance.CurrentSong;
                if (song.Artist.Name.Length > 5)
                    GeneralService.SaveLyricsToStorage(song.Name + song.Artist.Name.Substring(0, 5), lyrics);
                else
                    GeneralService.SaveLyricsToStorage(song.Name + song.Artist.Name, lyrics);
            }
            else
            {
                TextblockLyricsStatus.Text = "Error processing lyrics";
                return;
            }

            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
        }
        #endregion

        #region code for Vietnamese lyrics website
        /// <summary>
        /// Analyze structure and get lyrics from Vietnamese website. The code may look ugly but at least it works!
        /// </summary>
        void web_LoadCompleted_vn_step1(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Document == null)
            {
                TextblockLyricsStatus.Text = "Cannot connect";
                return;
            }
            var song = MediaService.Instance.CurrentSong;

            HtmlDocument doc = e.Document;
            HtmlNodeCollection items = doc.DocumentNode.SelectNodes("//div[@class='tenbh']");
            if (items == null)
            {
                TextblockLyricsStatus.Text = "No lyrics";
                return;
            }
            if (song.Artist.Name == "Unknown" || song.Artist.Name == "unknown" || song.Artist.Name == " ")
            {
                TextblockLyricsStatus.Text = "No lyrics";
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                //only search for the first 3 songs
                if (i >= 3)
                {
                    TextblockLyricsStatus.Text = "No lyrics";
                    return;
                }

                var artist_node = item.SelectSingleNode("./p[2]");
                if (artist_node == null || artist_node.InnerText == null) continue;  //check for error

                StringBuilder sb_artist = new StringBuilder(artist_node.InnerText)
                    .Replace(";", " ").Replace("ft.", " ").Replace("ft", " ").Replace("Ft.", "").Replace("&", " ").
                    Replace(",", " ").Replace("   ", " ").Replace("  ", " ");
                StringBuilder sb_songArtist = new StringBuilder(song.Artist.Name)
                    .Replace(";", " ").Replace("ft.", " ").Replace("ft", " ").Replace("Ft.", "").Replace("&", " ").
                    Replace(",", " ").Replace("   ", " ").Replace("  ", " ");

                string artist = sb_artist.ToString().ToLower();
                string songArtist = sb_songArtist.ToString().ToLower();

                double percent = GoMusic.StringSimilarityAlgorithms.DiceCoefficient.PercentMatchTo(artist, songArtist);
                if (percent >= 0.5)
                {
                    var url = item.SelectSingleNode("//a[@class='musictitle']").GetAttributeValue("href", "");
                    if (url != "")
                    {
                        HtmlWeb web = new HtmlWeb();
                        web.LoadCompleted += web_LoadCompleted_vn_step2;

                        grid_SongCover.Dispatcher.BeginInvoke(() =>
                        {
                            web.LoadAsync("http://chiasenhac.com/" + url);
                        });
                    }
                    else
                        TextblockLyricsStatus.Text = "No lyrics";
                    return;
                }
            }
        }

        void web_LoadCompleted_vn_step2(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Document == null)
            {
                TextblockLyricsStatus.Text = "Cannot connect";
                return;
            }

            HtmlDocument doc = e.Document;

            StringBuilder sb_lyrics;
            var lyrics_node = doc.DocumentNode.SelectSingleNode("//p[@class='genmed']");

            if (lyrics_node != null)
                sb_lyrics = new StringBuilder(lyrics_node.InnerText).Replace("on ChiaSeNhac.com", "").Replace("&quot;", "");
            else
            {
                sb_lyrics = new StringBuilder("");
                TextblockLyricsStatus.Text = "No lyrics";
                return;
            }
            string lyrics = sb_lyrics.ToString();
            //TextBlockLyrics.Dispatcher.BeginInvoke(() =>
            //    {
            TextBlockLyrics.Text = lyrics.ToString();
            TextblockLyricsStatus.Text = "";
            //   });

            if (lyrics != "")
            {
                System.Windows.VisualStateManager.GoToState(this, "LyricsOpened", true);
                var song = MediaService.Instance.CurrentSong;
                if (song.Artist.Name.Length > 5)
                    GeneralService.SaveLyricsToStorage(song.Name + song.Artist.Name.Substring(0, 5), lyrics);
                else
                    GeneralService.SaveLyricsToStorage(song.Name + song.Artist.Name, lyrics);
            }

            //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
        }
        #endregion
    }
}