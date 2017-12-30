**Hello !**

![nearly acid1](https://cloud.githubusercontent.com/assets/7447159/23646196/5c5c5096-0342-11e7-8d35-75b208206050.png)

_pic 1: HtmlRenderer, Gdi+, see [test file](https://github.com/LayoutFarm/HtmlRenderer/blob/master/Source/Test8_HtmlRenderer.Demo/Samples/0_acid1_dev/00.htm)_ 

**seems promising ?, NEARLY pass ACID1 test :)**

---

**Work In Progrss ... : Cross Platform HtmlRenderer**

The HtmlRenderer example!
---


![html_renderer_with_selection](https://user-images.githubusercontent.com/7447159/34452219-1a37545c-ed6d-11e7-969f-b0f5623e4802.png)
_pic 2: HtmlRenderer on GLES2 surface, text are renderered with the Typography_



also, please note the text selection on the Html Surface.


(HtmlRender => https://github.com/LayoutFarm/HtmlRenderer,

Typography => https://github.com/LayoutFarm/Typography)

---
The classic image.
![html_renderer_s01](https://cloud.githubusercontent.com/assets/7447159/24077194/3da7a684-0c78-11e7-8b83-98ebf77d5fdc.png)

 _pic 3: HtmlRenderer's Classic, Gdi+_

**MORE info / screen capture imgs** -> [see wiki](../../wiki/1.-Some-Screen-Captures)

**Build Note** -> [see wiki](../../wiki/3.-Build-The-Project)

-----
I forked this project from https://github.com/ArthurHub/HTML-Renderer (thank you so much)

I added some features

such as

1) dynamic html dom

2) decoupling, dependency analysis

3) optimizing the html,css parser. 
   see: HtmlKit v1.0(https://github.com/jstedfast/HtmlKit)    

4) add svg/canvas support (not complete)

5) abstract canvas backend (GDI+, OpenGL) also not complete for Linux (for the canvas backend, I used it from another project ->https://github.com/prepare/PixelFarm-dev)

6) Javascript (v8) binding (https://github.com/prepare/Espresso)

7) debug view

8) more layout support eg. inline-block,relative, absolute ,fixed, flex  etc 

9) added custom controls eg. text editer control, scrollbar, gridbox etc.

10) some events (eg. mouse /keyboard events)

.. BUT not complete :( 

feel free to fork/ comment/ suggest /pull request 


---
**Plan**

1) always permissive license (MIT,BSD, Apache2)

2) bind some features from Blink engine

3) add more html5/css3/js support

4) convert to C++ code with some transpiler tools 
   so users can build a final native code web browser

5) to make this runs on .NetCore

---
**Licenses**

Source code from multiple projects.

I select ONLY PERMISSIVE ONE :)

Here...

**Html Engine**

BSD, 2009, José Manuel Menéndez Poo, Base HtmlRender code

BSD, 2013-2014, Arthur Teplitzki, from https://github.com/ArthurHub/HTML-Renderer

MIT, 2015, Jeffrey Stedfastm, from HtmlKit https://github.com/jstedfast/HtmlKit

**Javascript Engine**

MIT, 2013, Federico Di Gregorio, from https://github.com/Daniel15/vroomjs

MIT, 2015-2017, WinterDev, from https://github.com/prepare/Espresso


**Geometry**

BSD, 2002-2005, Maxim Shemanarev, from http://www.antigrain.com , Anti-Grain Geometry - Version 2.4,

BSD, 2007-2014, Lars Brubaker, agg-sharp, from  https://github.com/MatterHackers/agg-sharp

ZLIB, 2015, burningmine, CurveUtils.

Boost, 2010-2014, Angus Johnson, Clipper.

BSD, 2009-2010, Poly2Tri Contributors, from https://github.com/PaintLab/poly2tri-cs

SGI, 2000, Eric Veach, Tesselate.

**Font**

Apache2, 2016-2017, WinterDev, from https://github.com/LayoutFarm/Typography

Apache2, 2014-2016, Samuel Carlsson, from https://github.com/vidstige/NRasterizer

MIT, 2015, Michael Popoloski MIT, from https://github.com/MikePopoloski/SharpFont

The FreeType Project LICENSE (3-clauses BSD style),2003-2016, David Turner, Robert Wilhelm, and Werner Lemberg and others, from https://www.freetype.org/

MIT, 2016, Viktor Chlumsky, from https://github.com/Chlumsky/msdfgen

**Platforms**

MIT, 2015-2015, Xamarin, Inc., from https://github.com/mono/SkiaSharp

MIT, 2006-2009,  Stefanos Apostolopoulos and other Open Tool Kit Contributors, from https://github.com/opentk/opentk

MIT, 2013, Antonie Blom, from  https://github.com/andykorth/Pencil.Gaming

MIT, 2004,2007, Novell Inc., for System.Drawing 


---

**Long Live Our Beloved C#**

WinterDev :)
