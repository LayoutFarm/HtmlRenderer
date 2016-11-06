//MIT 2016, WinterDev
using System;
using Pencil.Gaming;

namespace PixelFarm
{
    public static class GLFWPlatforms
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
        static GLFWContextForOpenTK _glfwContextForOpenTK;
        static OpenTK.ContextHandle _glContextHandler;
        static OpenTK.Graphics.GraphicsContext _externalContext;
        public static void CreateGLESContext()
        {
            //make open gl es current context 
            GlfwWindowPtr currentContext = Glfw.GetCurrentContext();
            _glContextHandler = new OpenTK.ContextHandle(currentContext.inner_ptr);

            _glfwContextForOpenTK = new GLFWContextForOpenTK(_glContextHandler);
            _externalContext = OpenTK.Graphics.GraphicsContext.CreateExternalContext(_glfwContextForOpenTK);

            //glfwContext = OpenTK.Graphics.GraphicsContext.CreateDummyContext(contextHandler);
            bool isCurrent = _glfwContextForOpenTK.IsCurrent;
        }
        public static void MakeCurrentWindow(GlfwWinInfo windowInfo)
        {
            _glfwContextForOpenTK.MakeCurrent(windowInfo);
        }
    }
    public class GlfwWinInfo : OpenTK.Platform.IWindowInfo
    {
        GlfwWindowPtr glfwWindowPtr;
        IntPtr glfwHandle;

        public GlfwWinInfo(GlfwWindowPtr glfwWindowPtr)
        {
            this.glfwWindowPtr = glfwWindowPtr;
            this.glfwHandle = glfwWindowPtr.inner_ptr;
        }
        public void Dispose()
        {
        }
        internal GlfwWindowPtr GlfwWindowPtr
        {
            get { return glfwWindowPtr; }
        }
    }



}
