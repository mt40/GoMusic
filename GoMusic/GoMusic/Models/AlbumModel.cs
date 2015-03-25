using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.Models
{
    public class AlbumModel
    {
        public Album baseAlbum;       
        //public Stream Art
        //{
        //    get
        //    {
        //        return baseAlbum.GetAlbumArt();
        //    }
        //}

        public GetAlbumArtDelegate Art
        {
            get
            {
                return new GetAlbumArtDelegate(baseAlbum.GetAlbumArt);           
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            //private set { _name = value; }
        }

        private string _artist;

        public string Artist
        {
            get {return _artist;}
            //private set { _artist = value; }
        }

        public int Count
        {
            get { return Songs == null ? 0 : Songs.Count; }
        }

        public List<Song> Songs
        {
            get;
            set;
        }
        

        public AlbumModel()
        {
            Songs = new List<Song>();
        }       

        public AlbumModel(Album XNAalbum)
        {
            baseAlbum = XNAalbum;

            _name = XNAalbum.Name;
            _artist = XNAalbum.Artist.Name;
            Songs = new List<Song>();

            //foreach(var item in XNAalbum.Songs)
            //{
            //    Songs.Add(item);
            //}

            for (int i = 0; i < XNAalbum.Songs.Count; i++)
            {
                Songs.Add(XNAalbum.Songs[i]);
            }
        }
    }
}
