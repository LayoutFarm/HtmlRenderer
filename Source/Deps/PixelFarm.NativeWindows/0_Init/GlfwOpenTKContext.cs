//MIT, 2016-2017, WinterDev
using System;
using OpenTK;
using OpenTK.Graphics;
using System.Threading;
using OpenTK.Platform;

namespace Pencil.Gaming
{
    public sealed class GLFWContextForOpenTK : OpenTK.Platform.External.ExternalGraphicsContext
    {
        //--------------------------
        //we can use glfw with OpenTK through CreateDummyContext()
        //without this class
        //--------------------------
        //but 
        //this class should make glfw works too,
        //and should faster than dummy contxt

        //-------------------------- 
        //this class was modified from exising opengl context code


        //TODO: review here
        //-------------------------- 
        bool vsync;
        Thread current_thread;

        public GLFWContextForOpenTK(ContextHandle handle)
            : base(DesktopBackend.ES20)
        {
            Handle = handle;
            current_thread = Thread.CurrentThread;
        }
        public override GraphicsContext.GetCurrentContextDelegate CreateCurrentContextDel()
        {
            return () => Handle;
        }
        public override void SwapBuffers()
        {
        }
        public override void MakeCurrent(IWindowInfo info)
        {

            var glfwWindowInfo = (PixelFarm.GlfwWinInfo)info;
            Glfw.MakeContextCurrent(glfwWindowInfo.GlfwWindowPtr);

            Thread new_thread = Thread.CurrentThread;
            // A context may be current only on one thread at a time.
            if (current_thread != null && new_thread != current_thread)
            {
                throw new GraphicsContextException(
                    "Cannot make context current on two threads at the same time");
            }

            if (info != null)
            {
                current_thread = Thread.CurrentThread;
            }
            else
            {
                current_thread = null;
            }
        }

        public override bool IsCurrent
        {
            get
            {
                return current_thread != null && current_thread == Thread.CurrentThread;
            }
        }

        public override IntPtr GetAddress(string function)
        {

            return IntPtr.Zero;
        }

        public override bool VSync
        {
            get { return vsync; }
            set { vsync = value; }
        }

        public override void Update(IWindowInfo window)
        {

        }

        public override void Dispose()
        {
            IsDisposed = true;
        }


    }


}