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
            CanvasInitParameters canvasInitPars= new CanvasInitParameters());

       
        /// <summary>
        /// font management system for this graphics platform
        /// </summary>
        public abstract IFonts Fonts { get; }
        //public abstract Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp);

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