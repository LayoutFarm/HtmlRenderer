using System.Collections.Generic;
namespace PixelFarm.DrawingGL
{
    public enum TextureAtlasAllocResult
    {
        Ok,
        //-------
        //fail
        Unknown,
        FullSpace,
        WidthOverLimit,
        HeightOverLimit
    }
    public class TextureAtlas
    {
        int width;
        int height;
        int currentXPos;
        int currentYPos;
        int currentLineMaxHeight = 0;
        List<PixelFarm.Drawing.RectangleF> areas = new List<Drawing.RectangleF>();
        public TextureAtlas(int w, int h)
        {
            this.width = w;
            this.height = h;
        }
        public int Width { get { return this.width; } }
        public int Height { get { return this.height; } }

        public uint GraphicsTextureId
        {
            get;
            set;
        }
        public void Dispose()
        {
            if (this.GraphicsTextureId != 0)
            {
                //clear GL texture
            }
        }
        public TextureAtlasAllocResult AllocNewRectArea(int w, int h,
            out int areaId, out int x, out int y)
        {
            //simple***
            //alloc new area
            //find new area for w and h 

            //-------------------------
            if (w > this.width)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.WidthOverLimit;
            }
            if (h > this.height)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.HeightOverLimit;
            }
            //-------------------------
            if (this.currentXPos + w > this.width)
            {
                //start to new line
                this.currentXPos = 0;
                this.currentYPos += currentLineMaxHeight;
                this.currentLineMaxHeight = 0;
            }
            if (this.currentYPos + h > this.height)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.FullSpace;
            }
            //-------------------------
            x = currentXPos;
            y = currentYPos;
            areaId = this.areas.Count + 1;
            this.areas.Add(new Drawing.RectangleF(x, y, w, h));
            //move xpos to next
            this.currentXPos += w;
            if (currentLineMaxHeight < h)
            {
                currentLineMaxHeight = h;
            }
            //------------------------- 

            return TextureAtlasAllocResult.Ok;
        }
    }
}