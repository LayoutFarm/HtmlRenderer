//MIT 2016, WinterDev
using System;
using Pencil.Gaming;

namespace PixelFarm
{
    public static class GLPlatforms
    {
       
        public static bool Init()
        {
          
            if (!Glfw.Init())
            {
                return false;
            }

            //---------------------------------------------------
            //specific OpenGLES ***
            Glfw.WindowHint(WindowHint.GLFW_CLIENT_API, (int)OpenGLAPI.OpenGLESAPI);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_CREATION_API, (int)OpenGLContextCreationAPI.GLFW_EGL_CONTEXT_API);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MAJOR, 2);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MINOR, 0);
            //---------------------------------------------------

            return true;
        }
    }
    public class GlfwWinInfo : OpenTK.Platform.IWindowInfo
    {
        IntPtr glfwHandle;
        public GlfwWinInfo(IntPtr glfwHandle)
        {
            this.glfwHandle = glfwHandle;
        }
        public void Dispose()
        {
        }
    }
}
