<h1>Object-Oriented DirectMedia Layer (ODL)</h1>

This library implements all the major SDL2 components in an Object-Oriented fashion. Surfaces are Bitmaps, Sprites manage Bitmaps, Viewports delimit Sprites, and Windows manage Viewports.

The implementation of all the classes is based on RGSS (Ruby Game Scripting System), but there's quite a few differences.

<h2>Dependencies</h2>

<list>
<li>libfreetype-6
<li>libpng16-16
<li>SDL2 (2.0.7 used)
<li>SDL2_image (2.0.2 used)
<li>SDL2_ttf (2.0.14 used)
<li><a href="https://github.com/flibitijibibo/SDL2-CS">SDL2-CS</a>
<li>zlib (1.2.11 used)
</list>

If using ILMerge to merge your libraries, only old.dll and SDL2-CS.dll can be merged together.
