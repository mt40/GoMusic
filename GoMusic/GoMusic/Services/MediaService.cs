using GoMusic.Models;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.Services
{
    public sealed class MediaService
    {
        //singleton
        private static readonly MediaService instance = new MediaService();

        //constructor
        private MediaService()
        {
            //get data
            MediaLibrary lib = new MediaLibrary();

            songList = new ObservableCollection<Song>();
            //for (int i = 0; i < lib.Songs.Count; i++)
            //{
            //    songList.Add(lib.Songs[i]);
            //}

            artistList = new ObservableCollection<ArtistModel>();
            //for (int i = 0; i < lib.Artists.Count; i++)
            //{
            //    ArtistModel art = new ArtistModel(lib.Artists[i]);
            //    artistList.Add(art);
            //}

            albumList = new ObservableCollection<AlbumModel>();
            //for (int i = 0; i < lib.Albums.Count; i++)
            //{
            //    AlbumModel al = new AlbumModel(lib.Albums[i]);
            //    albumList.Add(al);
            //}

            //this is only playlists created in Music hub, playlists created by this app
            //is loaded in PlaylistService
            playlistList = new ObservableCollection<PlaylistModel>();
            //for (int i = 0; i < lib.Playlists.Count; i++)
            //{
            //    PlaylistModel pl = new PlaylistModel(lib.Playlists[i]);
            //    playlistList.Add(pl);
            //}

        }

        public static MediaService Instance
        {
            get
            {
                return instance;
            }
        }

        public Song CurrentSong { get; set; }

        private ObservableCollection<Song> songList;
        private ObservableCollection<ArtistModel> artistList;
        private ObservableCollection<AlbumModel> albumList;
        private ObservableCollection<PlaylistModel> playlistList;

        public ObservableCollection<Song> SongList
        {
            get
            {
                if (songList.Count == 0)
                {
                    //get data
                    MediaLibrary lib = new MediaLibrary();
                    for (int i = 0; i < lib.Songs.Count; i++)
                    {
                        songList.Add(lib.Songs[i]);
                    }
                }
                return songList;
            }
        }

        public ObservableCollection<ArtistModel> ArtistList
        {
            get
            {
                if (artistList.Count == 0)
                {
                    ////get data
                    //MediaLibrary lib = new MediaLibrary();
                    //for (int i = 0; i < lib.Artists.Count; i++)
                    //{
                    //    ArtistModel art = new ArtistModel(lib.Artists[i]);
                    //    artistList.Add(art);
                    //}
                    LoadMoreArtist(21);
                }
                return artistList;
            }
        }

        public ObservableCollection<AlbumModel> AlbumList
        {
            get
            {
                if (albumList.Count == 0)
                {
                    ////get data
                    //MediaLibrary lib = new MediaLibrary();
                    //for (int i = 0; i < lib.Albums.Count; i++)
                    //{
                    //    AlbumModel al = new AlbumModel(lib.Albums[i]);
                    //    albumList.Add(al);
                    //}
                    LoadMoreAlbum(21);
                }
                return albumList;
            }

        }

        public ObservableCollection<PlaylistModel> PlaylistList
        {
            get
            {
                if (playlistList.Count == 0)
                {
                    //get data
                    MediaLibrary lib = new MediaLibrary();                
                    for (int i = 0; i < lib.Playlists.Count; i++)
                    {
                        PlaylistModel pl = new PlaylistModel(lib.Playlists[i]);
                        playlistList.Add(pl);
                    }
                }
                return playlistList;
            }
            //set;
        }

        public void ClearData()
        {
            if(songList != null)
            {
                songList.Clear();
            }
            if(artistList != null)
            {
                artistList.Clear();
            }
            if(albumList != null)
            {
                albumList.Clear();
            }
            if(playlistList != null)
            {
                playlistList.Clear();
            }
        }

        public async Task<bool> LoadMoreArtist(int count)
        {
            MediaLibrary lib = new MediaLibrary();
            int addedItem = artistList.Count;
            int toAdd = Utility.Clamp(count + addedItem, 0, lib.Artists.Count);
            if (toAdd == 0)
                return false;
            for(int i = addedItem; i < toAdd; i++)
            {
                ArtistModel art = new ArtistModel(lib.Artists[i]);
                artistList.Add(art);
            }
            return true;
        }

        public async Task<bool> LoadMoreAlbum(int count)
        {
            MediaLibrary lib = new MediaLibrary();
            int addedItem = albumList.Count;
            int toAdd = Utility.Clamp(count + addedItem, 0, lib.Albums.Count);
            if (toAdd == 0)
                return false;
            for (int i = addedItem; i < toAdd; i++)
            {
                AlbumModel art = new AlbumModel(lib.Albums[i]);
                albumList.Add(art);
            }
            return true;
        }
    }
}
