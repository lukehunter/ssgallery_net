using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace ssgallery.Model
{
    class Options
    {
        [Option("source", Required = true, HelpText = "Source folder")]
        public string Source
        {
            get;
            set;
        }

        [Option("target", Required = true, HelpText = "Target folder")]
        public string Target
        {
            get;
            set;
        }

        [Option("name", Required = true, HelpText = "Gallery name")]
        public string GalleryName
        {
            get;
            set;
        }

        [Option("thumbwidth", Required = true, HelpText = "Max thumbnail width (px)")]
        public int MaxThumbnailWidth
        {
            get;
            set;
        }

        [Option("thumbheight", Required = true, HelpText = "Max thumbnail height (px)")]
        public int MaxThumbnailHeight
        {
            get;
            set;
        }

        [Option("lightwidth", Required = true, HelpText = "Max lightbox width (px)")]
        public int MaxLightboxWidth
        {
            get;
            set;
        }

        [Option("lightheight", Required = true, HelpText = "Max lightbox height (px)")]
        public int MaxLightboxHeight
        {
            get;
            set;
        }

        [Option("baseurl", Required = true, HelpText = "Base url")]
        public string BaseUrl
        {
            get;
            set;
        }

        [Option("disqus", Required = true, HelpText = "Disqus js embed path")]
        public string Disqus
        {
            get;
            set;
        }
    }
}
