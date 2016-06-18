//2014,2015 BSD,WinterDev  
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------

using PixelFarm.Agg.Image;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Agg
{
    public abstract class Graphics2D
    {
        protected ActualImage destActualImage;
        protected ScanlineRasterizer sclineRas;
        Affine currentTxMatrix = Affine.IdentityMatrix;
        public Graphics2D()
        {
        }
        //------------------------------------------------------------------------

        public abstract void SetClippingRect(RectInt rect);
        public abstract RectInt GetClippingRect();
        public abstract void Clear(ColorRGBA color);
        //------------------------------------------------------------------------
        //render vertices
        public abstract void Render(VertexStoreSnap vertexSource, ColorRGBA colorBytes);
        //------------------------------------------------------------------------
       
      
        public void Render(VertexStore vxStorage, ColorRGBA c)
        {
            Render(new VertexStoreSnap(vxStorage), c);
        }
        public void Render(VertexStoreSnap vertexSource, double x, double y, ColorRGBA color)
        {
            var inputVxs = vertexSource.GetInternalVxs();
            var vxs = Affine.TranslateTransformToVxs(vertexSource, x, y);//Affine.NewTranslation(x, y).TransformToVxs (inputVxs);
            Render(vxs, color);
        }



        public Affine CurrentTransformMatrix
        {
            get { return this.currentTxMatrix; }
            set
            {
                this.currentTxMatrix = value;
            }
        }

        public ScanlineRasterizer ScanlineRasterizer
        {
            get { return sclineRas; }
        }
        public abstract ScanlinePacked8 ScanlinePacked8
        {
            get;
        }
        public abstract ScanlineRasToDestBitmapRenderer ScanlineRasToDestBitmap
        {
            get;
        }
        public ActualImage DestActualImage
        {
            get { return this.destActualImage; }
        }
        public abstract ImageReaderWriterBase DestImage
        {
            get;
        }
        public abstract IPixelBlender PixelBlender
        {
            get;
            set;
        }
        //================
        public static ImageGraphics2D CreateFromImage(ActualImage actualImage)
        {
            return new ImageGraphics2D(actualImage);
        }
        public abstract bool UseSubPixelRendering
        {
            get;
            set;
        }


#if DEBUG
        public void dbugLine(double x1, double y1, double x2, double y2, ColorRGBA color)
        {
            VertexStore vxs = new VertexStore(8);
            vxs.AddMoveTo(x1, y1);
            vxs.AddLineTo(x2, y2);
            vxs.AddStop();
            Render(new Stroke(1).MakeVxs(vxs), color);
        }
#endif



    }
}
