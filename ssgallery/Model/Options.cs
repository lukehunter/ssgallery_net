using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLineParser.Arguments;

namespace ssgallery.Model
{
    class Options
    {
        [ValueArgument(typeof(string), 's', "source", Description = "Source folder")]
        public string Source
        {
            get;
            set;
        }

        [ValueArgument(typeof(string), 't', "target", Description = "Target folder")]
        public string Target
        {
            get;
            set;
        }

        [ValueArgument(typeof(string), 'n', "name", Description = "Gallery name")]
        public string GalleryName
        {
            get;
            set;
        }

        [ValueArgument(typeof(int), 'w', "thumbwidth", Description = "Max thumbnail width (px)")]
        public int MaxThumbnailWidth
        {
            get;
            set;
        }

        [ValueArgument(typeof(int), 'x', "thumbheight", Description = "Max thumbnail height (px)")]
        public int MaxThumbnailHeight
        {
            get;
            set;
        }

        [ValueArgument(typeof(int), 'y', "lightwidth", Description = "Max lightbox width (px)")]
        public int MaxLightboxWidth
        {
            get;
            set;
        }

        [ValueArgument(typeof(int), 'z', "lightheight", Description = "Max lightbox height (px)")]
        public int MaxLightboxHeight
        {
            get;
            set;
        }

        [ValueArgument(typeof(string), 'r', "link", Description = "Home url")]
        public string HomeUrl
        {
            get;
            set;
        }
    }
}
