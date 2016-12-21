using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssgallery.Model
{
    class Gallery
    {
        public string Name
        {
            get;
            set;
        }

        public List<Album> Albums
        {
            get;
            set;
        }

        public Gallery()
        {
            Albums = new List<Album>();
        }
    }
}
