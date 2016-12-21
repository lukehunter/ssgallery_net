# Stupidly Simple Gallery
## Introduction
Stupidly Simple Gallery (ssgallery) is a really simple static image gallery generator for .Net

I created this software out of frustration with all the bloated and unnecessary complicated gallery software out there. Would you believe that some gallery software actually waits until somebody requests a thumbnail to resize the source image? This strategy does not work well when using high resolution (10+ megapixel) source images. This software is designed to support as large of images as you like, pre-cache everything and be extremely simple (run it on a folder full of files, upload the result to web host, and you're done).

## Overview
Only three page templates are used:
- Gallery
  - Shows list of albums
- Album
  - Shows list of photos
- Image
  - Shows single image at resolution specified on command line
  - Includes download link to original resolution
  - Includes disqus comment area specific to the image
  - Navigate forward and backward with buttons, or click on the image to advance to the next image

## Input
Input to ssgallery is a folder full of subfolders of files, with a single thumbnail.jpg in each subfolder that will be used as the album cover photo. In addition there are command line options to control thumbnail and image viewing sizes and to specify the base relative url.

## Output
ssgallery writes a complete web page to the target folder. Image pre-caching is skipped if the resized image versions have a newer write time than the source images. Html pages are always generated, and any existing html pages will be overwritten.

The resulting folder can be transferred via FTP to a webhost and you are done my man.

## Deployment Steps
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
      - thumbnail.jpg
      - image1.jpg
      - image2.jpg
      - image3.jpg
      - image4.jpg
      
1. Create a file hierarchy like above
      
1. Run ssgallery on your files
ssgallery.exe --source "C:\Users\luke\pictures\gallery1" --target "c:\inetpub\wwwroot\mywebgallery" --name "My Web Gallery" --thumbwidth 150 --thumbheight 150 --lightwidth 1500 --lightheight 1000 --baseurl "/mywebgallery/" --disqus "//your-disqus-url.disqus.com/embed.js"

1. Upload the target folder to your web host via FTP

## Example Gallery
See http://lukehunter.net/nielsenphotos/
