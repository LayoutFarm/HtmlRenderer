**Hello !**

![nearly acid1](https://cloud.githubusercontent.com/assets/7447159/23646196/5c5c5096-0342-11e7-8d35-75b208206050.png)

_pic 1: HtmlRenderer, Gdi+_

(test file: Source/HtmlRenderer.Demo/Samples/0_acid1_dev/00.html)

**seems promising ?, NEARLY pass ACID1 test :)**

---

I try to make this run cross platform.

This is latest snapshot, HtmlRenderer: OpenGLES2 Backend.

![gles2](https://cloud.githubusercontent.com/assets/7447159/24074437/ee5cb5e8-0c3a-11e7-8df0-53f32617aeac.png)


---
The classic image of original project.
![html_renderer_s01](https://cloud.githubusercontent.com/assets/7447159/24077194/3da7a684-0c78-11e7-8b83-98ebf77d5fdc.png)

 

**more info / screen capture imgs** -> [see wiki](../../wiki/1.-Some-Screen-Captures)

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




**license**
it is a C# project with permissive license( MIT,BSD, Apache2)

**plan**

1) always permissive license (MIT,BSD, Apache2)

2) bind some features from Blink engine

3) add more html5/css3/js support

4) convert to C++ code with some transpiler tools 
   so users can build a final native code web browser

5) to make this runs on .NetCore
 

WinterDev :)
