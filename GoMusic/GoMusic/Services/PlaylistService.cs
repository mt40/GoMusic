using GoMusic.Models;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.Services
{
    public sealed class PlaylistService
    {
        private static readonly PlaylistService instance = new PlaylistService();

        private PlaylistService()
        {
            playlists = new List<PlaylistModel>();

            ////load grom isolated storage
            //List<PlaylistModel> fakeList = GeneralService.LoadPlaylist();

            ////if load failed
            //if (fakeList == null)
            //    return;

            //foreach(var fakePl in fakeList)
            //{
            //    PlaylistModel pl = new PlaylistModel();
            //    pl.Name = fakePl.Name;

            //    foreach(var fakeSong in fakePl.FakeSongs)
            //    {
            //        //use the string Name and Artist of fake song to find the real song
            //        Song song = MediaService.Instance.SongList.Find(x => (x.Name == fakeSong.Name) && (x.Artist.Name == fakeSong.Artist));
            //        if(song != null)
            //        {
            //            pl.Songs.Add(song);
            //        }
            //    }
            //    playlists.Add(pl);
            //}            
        }

        public static PlaylistService Instance
        {
            get { return instance; }
        }

        private List<PlaylistModel> playlists;

        public List<PlaylistModel> Playlists
        {
            get { return playlists; }
        }
    }
}
