DVD file structure: https://en.wikipedia.org/wiki/DVD-Video
Some initial good reading on the terminology: https://learn.microsoft.com/en-us/windows/win32/directshow/dvd-basics

The main site I referenced for the DVD spec: https://dvd.sourceforge.net/dvdinfo/index.html and http://www.mpucoder.com/DVD/

The tool I borrowed code from to get me started: https://download.videohelp.com/jsoto/dvdtools.htm
Also libdvdread was heavily utilized to double check parsing: https://code.videolan.org/videolan/libdvdread


Some helpful terminology:
Menu - A collection of streams used for a "menu screen".There are six types:
1 - Title. This is the first menu to be displayed, usually has buttons like "Play All".
2 - Root. Each title can have it's own root menu. The following types are subtypes of the root menu.
	3 - Subpicture. Such as different versions of the menu
	4 - Audio. Such as different languages.
	5 - Angle. Selects the camera angle
	6 - Chapter, or PTT menu. Selects chapters within a title.

Title - A collection of streams that makes up a single "video"
PTT - Titles are divided into "Parts of Titles", often called "Scenes" or "Chapters".

NOTE: It's up to the author of the DVD how the content is divided between titles and chapters. Most movies
place the film into one title, divided into chapters for individual scenes. Deleted scenes or extras might
be put in their own separate titles, however the divisions are arbitrary.

PGC (Sequential) - Chapters can contain branching instructions to instruct the player where to continue playing the next chapter.
This is called a "sequential PGC" (PGC = Program Chain). This is another name for a group of chapters that belong together.

PML - Parental Management Level. Numbered 1 to 8, 8 being the most restrictive. In the US, these would be G, PG, PG-13, NC-17 etc.
There can be two versions of the same chapter in a title, each assigned a different PML, although sometimes they are simply two angles.
