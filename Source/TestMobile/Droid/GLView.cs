﻿//MIT, 2017, Zou Wei(github/zwcloud)
//MIT, 2017, WinterDev (modified from Xamarin's Android code template)
using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;
using OpenTK.Graphics.ES20;

using PixelFarm;
using CustomApp01;

namespace Test_Android_Glyph
{
    class GLView1 : AndroidGameView
    {

        Demo_UIHtmlBox _htmlBox;
        int _max;
        LayoutFarm.AppHostAndroid _appHost;
        int _viewWidth;
        int _viewHeight;
        public GLView1(Context context) : base(context)
        {

        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

             
            Android.Graphics.Point sc_size = new Android.Graphics.Point();
            Display.GetSize(sc_size);

            _viewWidth = sc_size.X;
            _viewHeight = sc_size.Y;

            MakeCurrent();
            //-----------

            _max = Math.Max(_viewWidth, _viewHeight);
            _appHost = new LayoutFarm.AppHostAndroid(this, _max, _max); 
            _htmlBox = new Demo_UIHtmlBox();
            _appHost.StartApp(_htmlBox);  
            //-----------
            // Run the render loop
            Run();
        }
        public int ViewWidth => _viewWidth;
        public int ViewHeight => _viewHeight;

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer()
        {
            //essential, from https://github.com/xamarin/monodroid-samples/blob/master/GLTriangle20-1.0/PaintingView.cs
            ContextRenderingApi = GLVersion.ES2;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Log.Verbose("GLTriangle", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("GLTriangle", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e); 
            GL.Viewport(0, 0, _max, _max);
            //
            _htmlBox.RenderFrame();
            //
            SwapBuffers();
        }


    }
}
