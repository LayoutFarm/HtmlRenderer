//MIT, 2014-2016, WinterDev 
using System;
namespace PixelFarm.Drawing
{
    public abstract class GraphicsPlatform
    {

        public abstract Canvas CreateCanvas(
            int left,
            int top,
            int width,
            int height,
            CanvasInitParameters canvasInitPars = new CanvasInitParameters());

        public abstract GraphicsPath CreateGraphicsPath();
        /// <summary>
        /// font management system for this graphics platform
        /// </summary>
        public abstract IFonts Fonts { get; }
        public abstract Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp);

        //------------------------------------------------------------------
        public bool SetAsCurrentPlatform()
        {
            return SetCurrentPlatform(this);
        }

        static GraphicsPlatform s_actualImpl;
        static object initLock = new object();
        //set current graphic platform
        static bool SetCurrentPlatform(GraphicsPlatform actualImpl)
        {
            //must init once
            lock (initLock)
            {
                if (s_actualImpl == null)
                {
                    s_actualImpl = actualImpl;
                    return true;
                }
            }
            return false;
        }
        public static GraphicsPlatform CurrentPlatform
        {
            get { return s_actualImpl; }
        }
    }



    public struct CanvasInitParameters
    {

        public object externalCanvas;
        public CanvasBackEnd canvasBackEnd;

        internal bool IsEmpty()
        {
            return externalCanvas == null && canvasBackEnd == CanvasBackEnd.Software;
        }
    }


}