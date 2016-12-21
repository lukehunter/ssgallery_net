# Stupidly Simple Gallery
Stupidly Simple Gallery (ssgallery) is a really simple static image gallery generator for .Net

Only three page templates are used:
* Gallery
* Album
* Image

The Image page includes Disqus integration, and you can specify a Disqus js embed url via the command line.

## Input
Input to ssgallery is a folder full of subfolders of files, with a single thumbnail.jpg in each subfolder that will be used as the album cover photo. In addition there are command line options to control thumbnail and image viewing sizes and to specify the base relative url.

## Output
ssgallery writes a complete web page to the target folder. Image pre-caching is skipped if the resized image versions have a newer write time than the source images. Html pages are always generated, and any existing html pages will be overwritten.

The resulting folder can be transferred via FTP to a webhost and you are done my man.

## Usage
ssgallery.exe --source "C:\Users\luke\pictures\gallery1" --target "c:\inetpub\wwwroot\gallery1" --name "Gallery Numero Uno" --thumbwidth 150 --thumbheight 150 --lightwidth 1500 --lightheight 1000 --baseurl "/gallery1/" --disqus "//your-disqus-url.disqus.com/embed.js"

## Example Gallery
See http://lukehunter.net/nielsenphotos/
