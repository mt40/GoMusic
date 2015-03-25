using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.Models
{
    //function pointer
    public delegate Stream GetAlbumArtDelegate();

    public class ArtistModel
    {
        public Artist baseArtist;

        //public Stream Art
        //{
        //    get
        //    {
        //        foreach(var album in baseArtist.Albums)
        //        {
        //            if (album.HasArt == true)
        //                return album.GetAlbumArt();
        //        }
        //        return null;
        //    }
        //}
        public GetAlbumArtDelegate Art
        {
            get
            {
                foreach (var album in baseArtist.Albums)
                {
                    if (album.HasArt == true)
                        return new GetAlbumArtDelegate(album.GetAlbumArt);
                }
                return null;
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            //private set { _name = value; }
        }

        public int Count
        {
            get { return Songs == null ? 0 : Songs.Count; }
        }

        public int AlbumCount
        {
            get { return Albums == null ? 0 : Albums.Count; }
        }

        public List<Song> Songs
        {
            get;
            private set;
        }

        public List<AlbumModel> Albums
        {
            get;
            private set;
        }

        public ArtistModel()
        {
            Albums = new List<AlbumModel>();
            Songs = new List<Song>();
        }

        public ArtistModel(Artist XNAartist)
        {
            baseArtist = XNAartist;

            _name = XNAartist.Name;
            Songs = new List<Song>();
            Albums = new List<AlbumModel>();

            for (int i = 0; i < XNAartist.Songs.Count; i++ )
            {
                Songs.Add(XNAartist.Songs[i]);
            }

            for(int i = 0; i < XNAartist.Albums.Count; i++)
            {
                Album item = XNAartist.Albums[i];
                AlbumModel al = new AlbumModel(item);
                Albums.Add(al);
            }
        }
    }
}

