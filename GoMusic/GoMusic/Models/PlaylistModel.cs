using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.Models
{
    public class PlaylistModel : INotifyPropertyChanged
    {
        public Playlist basePlaylist;
        public string Name
        {
            get;
            set;
        }

        public List<Song> Songs
        {
            get;
            set;
        }

        public int Count
        { 
            get { return Songs == null? 0 : Songs.Count; } 
        }

        public System.IO.Stream Art
        {
            get
            {
                return Count == 0 ? null : Songs.FirstOrDefault().Album.GetAlbumArt();
            }
        }

        public PlaylistModel()
        {
            Songs = new List<Song>();
        }

        public PlaylistModel(Playlist XNAplaylist)
        {
            basePlaylist = XNAplaylist;
            Name = XNAplaylist.Name;
            Songs = new List<Song>();

            //foreach(var item in XNAplaylist.Songs)
            //{
            //    Songs.Add(item);
                
            //}

            for (int i = 0; i < XNAplaylist.Songs.Count; i++)
            {
                Songs.Add(XNAplaylist.Songs[i]);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
