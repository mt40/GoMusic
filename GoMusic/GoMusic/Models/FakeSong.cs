using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.Models
{
    public class FakeSong
    {
        public string Name
        {
            get;
            set;
        }

        public string Artist
        {
            get;
            set;
        }

        public FakeSong()
        {
            Name = "$noname$";
            Artist = "$noname$";
        }

        public FakeSong(Song XNAsong)
        {
            Name = XNAsong.Name;
            Artist = XNAsong.Artist.Name;
        }
    }
}
