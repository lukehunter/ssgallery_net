using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using ssgallery.Model;

namespace ssgallery
{
    class Program
    {
        private static readonly string CacheFolder = "cache";
        private static Options mOptions;
        private static Gallery mGallery;
        private static readonly Func<string, int, int, string> FormatFilename =
            (imagename, width, height) => string.Format("{0}_{1}_{2}.jpg", imagename, width, height);

        static void Main(string[] args)
        {
            ParseCommandLine(args);
            BuildGallery();
            CopyResources();
            PopulateImageCache();
            CreatePages();
            WaitForExit();
        }

        private static void ParseCommandLine(string[] args)
        {
            mOptions = new Options();
            CommandLine.Parser.Default.ParseArguments(args, mOptions);
        }

        /// <summary>
        /// Build up Gallery object model by examining directories/image files
        /// </summary>
        private static void BuildGallery()
        {
            if (!Directory.Exists(mOptions.Source))
            {
                throw new Exception(string.Format("Could not find path {0}", mOptions.Source));
            }

            mGallery = new Gallery() { Name = mOptions.GalleryName };

            var dirs = Directory.GetDirectories(mOptions.Source);
            Album curAlbum;
            Image curImage;

            Console.WriteLine(string.Format("Found {0} albums...", dirs.Count()));

            foreach (var albumFolder in dirs)
            {
                curAlbum = new Album() { Name = new DirectoryInfo(albumFolder).Name, FolderPath = albumFolder };
                var files = Directory.GetFiles(albumFolder);

                foreach (var image in files)
                {
                    if (image.Contains("thumbnail.jpg"))
                        continue;

                    curImage = new Image() { Name = Path.GetFileNameWithoutExtension(image), Path = image };
                    curAlbum.Images.Add(curImage);
                }

                mGallery.Albums.Add(curAlbum);
            }
        }

        private static void CopyResources()
        {
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "Resources");
            string destPath = mOptions.Target;

            // Create all of the directories
            foreach( string dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories) )
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));

            // Copy all the files & Replace any with the same name
            foreach( string newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories) )
                File.Copy(newPath, newPath.Replace(sourcePath, destPath), true);
        }

        private static void PopulateImageCache()
        {
            foreach (var album in mGallery.Albums)
            {
                Directory.CreateDirectory(Path.Combine(mOptions.Target, album.Name, CacheFolder));

                Console.WriteLine(string.Format("Caching {0} album", album.Name));

                foreach (var image in album.Images)
                {
                    File.Copy(image.Path, Path.Combine(mOptions.Target, album.Name, Path.GetFileName(image.Path)), true);

                    
                    var targetPath = Path.Combine(mOptions.Target, album.Name, CacheFolder, FormatFilename(image.Name, mOptions.MaxThumbnailWidth, mOptions.MaxThumbnailHeight));
                    SaveResizedImage(image, mOptions.MaxThumbnailWidth, mOptions.MaxThumbnailHeight, targetPath);

                    targetPath = Path.Combine(mOptions.Target, album.Name, CacheFolder,
                                              FormatFilename(image.Name, mOptions.MaxViewerWidth,
                                                             mOptions.MaxViewerHeight));
                    SaveResizedImage(image, mOptions.MaxViewerWidth, mOptions.MaxViewerHeight, targetPath);
                }

                var thumbnailPath = Path.Combine(album.FolderPath, "thumbnail.jpg");
                var targetThumbnailPath = Path.Combine(mOptions.Target, album.Name, "thumbnail.jpg");
                
                // Always generate thumbnail -- otherwise if thumbnail.jpg is removed from source (plausible) it will never be re-generated in the target
                if (File.Exists(thumbnailPath))
                {
                    SaveResizedImage(new Image() { Name = string.Format("{0} thumbnail", album.Name), Path = thumbnailPath }, mOptions.MaxThumbnailWidth, mOptions.MaxThumbnailHeight, targetThumbnailPath, false);
                }
                else
                {
                    var image = album.Images[0];
                    Console.WriteLine("File {0} not found, using {1} as album thumbnail", thumbnailPath, image.Path);
                    SaveResizedImage(image, mOptions.MaxThumbnailHeight, mOptions.MaxThumbnailWidth, targetThumbnailPath, false);
                }
            }
        }

        // TODO LAH 2016-12-21: move this to Image
        private static void SaveResizedImage(Image image, int width, int height, string targetPath, bool skipIfNewer = true)
        {
            if( skipIfNewer && File.Exists(targetPath) )
            {
                var imageInfo = new FileInfo(image.Path);
                var existingInfo = new FileInfo(targetPath);

                if( imageInfo.LastWriteTime <= existingInfo.LastWriteTime )
                {
                    MagickImage target = new MagickImage(targetPath);
                    image.Width = target.Width;
                    image.Height = target.Height;

                    Console.WriteLine(string.Format("Skipping resizing for {0} (target file's last write time is newer than source", Path.GetFileName(targetPath)));
                    return;
                }
            }

            var mi = new MagickImage(image.Path);

            Console.WriteLine(string.Format("Generating {0}x{1} for {2}", width, height, image.Name));

            mi.Resize(width, height);

            image.Width = mi.Width;
            image.Height = mi.Height;

            mi.Write(targetPath);

            mi.Dispose();
        }

        private static void CreatePages()
        {
            var galleryTemplateRaw = File.ReadAllText(@"templates\gallery.html");
            var albumTemplateRaw = File.ReadAllText(@"templates\album.html");
            var imageTemplateRaw = File.ReadAllText(@"templates\image.html");

            var galleryTemplate = new Template() { RawHtml = galleryTemplateRaw };

            var galleryValues = new Dictionary<string, string>()
            {
                {"SSG_GALLERY_NAME", mGallery.Name},
                {"SSG_HOME_URL", mOptions.BaseUrl},
                {"SSG_GALLERY_URL", mOptions.BaseUrl + mGallery.Name + "/"},
                {"SSG_DISQUS_URL", mOptions.Disqus}
            };

            galleryTemplate.AddValues(galleryValues);

            foreach (var album in mGallery.Albums)
            {
                var albumThumbnail = Path.Combine(mOptions.Target, album.Name, "thumbnail.jpg");
                var albumThumbImage = new MagickImage(albumThumbnail);

                var AlbumValues = new Dictionary<string, string>()
                {
                    { "SSG_ALBUM_NAME", album.Name },
                    { "SSG_ALBUM_URL", mOptions.BaseUrl + album.Name + "/"},
                    { "SSG_ALBUM_THUMBNAIL_WIDTH", albumThumbImage.Width.ToString()},
                    { "SSG_ALBUM_THUMBNAIL_HEIGHT", albumThumbImage.Height.ToString()}
                };

                var albumTemplate = new Template() { RawHtml = albumTemplateRaw };

                albumTemplate.AddValues(galleryValues);
                albumTemplate.AddValues(AlbumValues);

                galleryTemplate.Items.Add(new TemplateItem()
                {
                    Tag = "SSG_ALBUM_LIST_ITEM",
                    Values = AlbumValues
                });

                foreach (var image in album.Images)
                {
                    var imageIndex = album.Images.IndexOf(image);
                    string nextPage = "", prevPage = "", picToPreload = "";

                    if (imageIndex > 0)
                    {
                        prevPage = string.Format("{0}.html", album.Images[imageIndex - 1].Name);
                    }

                    if (imageIndex < album.Images.Count - 1)
                    {
                        var nextImage = album.Images[imageIndex + 1];
                        nextPage = string.Format("{0}.html", nextImage.Name);
                        picToPreload = CacheFolder + "/" + FormatFilename(nextImage.Name, mOptions.MaxViewerWidth, mOptions.MaxViewerHeight);
                    }

                    var imageThumbnail = Path.Combine(mOptions.Target, album.Name, CacheFolder, FormatFilename(image.Name, mOptions.MaxThumbnailWidth, mOptions.MaxThumbnailHeight));
                    var imageThumbImage = new MagickImage(imageThumbnail);

                    var ImageValues = new Dictionary<string, string>()
                    {
                        {"SSG_IMAGE_NAME", image.Name},
                        {"SSG_PREV_IMAGE_PAGE_URL", prevPage},
                        {"SSG_NEXT_IMAGE_PAGE_URL", nextPage},
                        {"SSG_PRELOAD_URL", picToPreload},
                        {"SSG_IMAGE_WIDTH", image.Width.ToString()},
                        {"SSG_IMAGE_HEIGHT", image.Height.ToString()},
                        {"SSG_IMAGE_URL", CacheFolder + "/" + FormatFilename(image.Name, mOptions.MaxViewerWidth, mOptions.MaxViewerHeight)},
                        {"SSG_IMAGE_PAGE_URL", string.Format("{0}.html", image.Name)},
                        {"SSG_IMAGE_ID", string.Format("{0}-{1}-{2}", mGallery.Name, album.Name, image.Name)},
                        {"SSG_IMAGE_THUMBNAIL_URL", CacheFolder + "/" + FormatFilename(image.Name, mOptions.MaxThumbnailWidth, mOptions.MaxThumbnailHeight)},
                        {"SSG_ORIG_IMAGE_URL", Path.GetFileName(image.Path)},
                        {"SSG_IMAGE_THUMBNAIL_WIDTH", imageThumbImage.Width.ToString()},
                        {"SSG_IMAGE_THUMBNAIL_HEIGHT", imageThumbImage.Height.ToString()}
                    };

                    var imageTemplate = new Template()
                    {
                        RawHtml = imageTemplateRaw
                    };

                    imageTemplate.AddValues(galleryValues);
                    imageTemplate.AddValues(AlbumValues);
                    imageTemplate.AddValues(ImageValues);

                    albumTemplate.Items.Add(new TemplateItem()
                    {
                        Tag = "SSG_IMAGE_LIST_ITEM",
                        Values = ImageValues
                    });

                    imageTemplate.RenderHtml(Path.Combine(mOptions.Target, album.Name, string.Format("{0}.html", image.Name)));
                }

                albumTemplate.RenderHtml(Path.Combine(mOptions.Target, album.Name, "index.html"));
            }

            galleryTemplate.RenderHtml(Path.Combine(mOptions.Target, "index.html"));
        }

        private static void WaitForExit()
        {
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}
