using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssgallery.Model
{
    class Album
    {
        public string Name
        {
            get;
            set;
        }

        public string FolderPath
        {
            get;
            set;
        }

        public List<Image> Images
        {
            get;
            set;
        }

        public Album()
        {
            Images = new List<Image>();
        }
    }
}
