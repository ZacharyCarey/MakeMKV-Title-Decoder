# MakeMKV Title Decoder

# Environment
Visual studio 2022 \
.NET 8 using C# \
WinForms application with debug console window. \
\
[MakeMKV](https://www.makemkv.com/download/) must be installed (with valid license) to your ProgramFiles \
[mkvtoolnix](https://mkvtoolnix.download/downloads.html) is included with the program in it's portable form, there is no need to download it.\
\
NOTE: The following README is probably incorrect. I will update it once the program is in a more stable state.

# Description
I mainly made this for myself so it's just duct-taped together without a lot of QoL.
You are welcome to attempt to use it, but be warned I hardcoded it for MakeMKV to be maximized on a 1440p monitor, and it won't work with anything else unless you fix MakeMKVInput.cs

# Usage
After loading MakeMKV and loading the list of MKV files found on your disc, make sure MakeMKV is maximized. The scraper will read every entry and determine which one is the main feature/s.
It will then automatically deselect any MKV files that are repeated sections of the main feature. It will also attempt find bonus content and break them down into the smallest videos possible,
while disabling the "Play All" videos so you are only ripping the smallest amount of data possible.

There is an optional feature to automatically detect MKV files with no sound, which are usually federal warning screens, and deselect them automatically.
Assuming you didn't change the output folder after scraping, the "Rename" button will rename any main features or episodes found to assist in renaming files to something more useful.

## Movie
When "Movie" is selected, the main feature is considered the longest file in the list, and any files that contain the same segments are automatically deselected.

## TV
When "TV" is selected, the main feature is found, but deselected. Instead the individual files that make up that main feature are kept i.e. it tries to keep the individual episodes.
If you would rather keep all the episodes as one long "play all" file, simply select "Movie" instead.

# References
http://www.mpucoder.com/DVD/
https://en.wikipedia.org/wiki/DVD-Video
