# Stupidly Simple Gallery
## Introduction
Stupidly Simple Gallery (ssgallery) is a really simple static image gallery generator for .Net

I created this software out of frustration with all the bloated and unnecessarily complicated gallery software out there. Would you believe that some gallery software actually waits until somebody requests a thumbnail to resize the source image? This strategy does not work well when using high resolution (10+ megapixel) source images. This software is designed to support as large of images as you like, pre-cache everything and be extremely simple (run it on a folder full of files, upload the result to your web host, and you're done).

## Overview

ssgallery uses an extremely basic template system to generate html pages. Tokens such as %SSG_GALLERY_NAME% are inserted into an html file, and at run-time are replaced with the correct strings. There is also support for lists of items (e.g. on the gallery and album pages), although the templating engine has not been heavily tested.

An ssgallery theme consists of three templates:

- Gallery
  - Shows list of albums
- Album
  - Shows list of images
- Image
  - Shows single image at resolution specified on command line
  - Includes download and view links for original resolution
  - Includes disqus comment area specific to the image
  - Navigate forward and backward by swiping on tablets and phones
  - Navigate forward and backward with onhover buttons
  - Navigate forward by clicking on the image
  
## Example Gallery
See http://lukehunter.net/nielsenphotos/

## Input
Input to ssgallery is a folder full of subfolders of files, with an optional thumbnail.jpg in each subfolder that will be used as the album cover photo (if thumbnail.jpg is not present the first image in the album will be used). In addition there are command line options to control thumbnail and image viewing sizes and to specify the base relative url.

## Output
ssgallery writes a complete web page to the target folder. Image resizing is skipped if the resized image versions have a newer write time than the source images. Html pages will be overwritten if they have changed.

The resulting folder can be transferred via FTP to a webhost. Since files that are up to date are not touched, using the "Overwrite if source file is newer" option in your FTP client (e.g. FileZilla) will make incremental updates faster.

## Deployment Steps
### 1. Prepare files
ssgallery expects a folder structure like the following. It is recommended that all images (including thumbnail.jpg) be at the largest resolution you would like available for users to download. They will be resized for thumbnails and the image viewer depending on the provided command line arguments, and the original file will be available under the View/Download link on the Image page.

- pictures
  - gallery1
    - album1
      - thumbnail.jpg
      - image1.jpg
      - image2.jpg
      - image3.jpg
    - album2
      - thumbnail.jpg 
      - image1.jpg
      - image2.jpg
    - album3
      - image1.jpg
      - image2.jpg
      - image3.jpg
      - image4.jpg
      
Note that album3 does not have a thumbnail.jpg -- in this case album3/image1.jpg will be used as the album3 thumbnail by default.
      
### 2. Run ssgallery.exe

The following is an example command line for ssgallery hosted at http://server/mywebgallery/:

ssgallery.exe --source "C:\Users\luke\pictures\gallery1" --target "c:\inetpub\wwwroot\mywebgallery" --name "My Web Gallery" --thumbwidth 170 --thumbheight 130 --viewerwidth 1500 --viewerheight 1000 --baseurl "/mywebgallery/" --disqus "//your-disqus-url.disqus.com/embed.js"

### 3. Upload the target folder to your web host via FTP

You're done!

## known issues
- If you run ssgallery multiple times and remove some images in between runs, you will end up with extra files in your destination folder. over time this could grow and start wasting disk space. a simple workaround is to completely delete the target folder and re-generate from scratch occasionally.
- Thumbnail sizes other than 170x130 are not currently supported. some changes to the CSS are required.
- Layout is a little small on mobile devices. Could use some responsive design.
- Occasionally File.Copy throws an IOException related to disk space even when plenty of space is available. Setting the following registry key may help:

   HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters\IRPStackSize DWORD 0x0000000f (15) 

## todo
- parallelization
- testing (bad args, corrupt image, non-image, empty folder, nested folder)
- remove unneeded html
-   "       "    css
- clean up url building
- improve nav button ui in default template
  - improve alignment
- fix css to work with custom thumbnail sizes
- download gallery/album links and zip files
- disable disqus if no url provided
- remove extraneous files during generation (..hmm, yes/no/all warning?)
- support nested albums
- password protected albums
- gapless gallery/album layout
- mono support
- more themes
