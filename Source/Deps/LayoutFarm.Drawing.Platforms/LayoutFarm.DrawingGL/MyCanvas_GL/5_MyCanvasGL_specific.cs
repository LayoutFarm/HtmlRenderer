//MIT 2014, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.DrawingGL;
using DrawingBridge;

namespace LayoutFarm.Drawing.DrawingGL
{

    partial class MyCanvasGL
    {

        Font currentFont;
        CanvasGL2d canvasGL2d;
        //-------
        //platform specific code
        GdiTextBoard gdiTextBoard;
        PixelFarm.Agg.VertexSource.CurveFlattener flattener = new PixelFarm.Agg.VertexSource.CurveFlattener();
        //-------
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        System.Drawing.Rectangle currentClipRect;

        public MyCanvasGL(GraphicsPlatform platform, int hPageNum, int vPageNum, int left, int top, int width, int height)
        {
            canvasGL2d = new CanvasGL2d(width, height);
            //--------------------------------------------
            this.platform = platform; 
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            //--------------------------------------------

            this.CurrentFont = defaultFontInfo;
            this.CurrentTextColor = Color.Black;
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;


            this.currentClipRect = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);
            //------------------------
            //platform specific code
            //-------------------------
            gdiTextBoard = new GdiTextBoard(800, 100, new System.Drawing.Font("tahoma", 10));
            //----------------------- 
        }
        //-------------------------------------------
        public override void SetCanvasOrigin(int x, int y)
        {
            //    ReleaseHdc();
            //    //-----------
            //    //move back to original ?
            //    //this.gx.TranslateTransform(-this.canvasOriginX, -this.canvasOriginY);
            //    //this.gx.TranslateTransform(x, y);

            //this.canvasOriginX = x;
            //this.canvasOriginY = y;
            canvasGL2d.SetCanvasOrigin(x, y);
        }
        public override int CanvasOriginX
        {
            get
            {  
                
                return canvasGL2d.CanvasOriginX;
            }
        }
        public override int CanvasOriginY
        {
            get
            {
                return canvasGL2d.CanvasOriginY;
            }
        }

        public override void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            
            canvasGL2d.EnableClipRect();
            //--------------------------
            canvasGL2d.SetClipRect(
                 rect.X,
                 rect.Y,
                 rect.Width,
                 rect.Height);
            //--------------------------
        }
        //-------------------------------------------
        public override void FillRectangle(Color color, float left, float top, float width, float height)
        {
            canvasGL2d.FillRect(color, left, top, width, height);
        }
        public override void FillRectangle(Brush brush, float left, float top, float width, float height)
        {
            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        var solidBrush = brush as SolidBrush;
                        canvasGL2d.FillRect(solidBrush.Color, left, top, width, height);

                    } break;
                case BrushKind.LinearGradient:
                    {
                        var linearGradientBrush = brush as LinearGradientBrush;
                        //create bg gradient first 
                        //fill linear gradient in spefic area 
                        canvasGL2d.FillRect(linearGradientBrush, left, top, width, height);

                    } break;
                case BrushKind.CircularGraident:
                    {

                    } break;
                case BrushKind.GeometryGradient:
                    {
                    } break;
                case BrushKind.Texture:
                    {
                    } break;
                default:
                    {

                    } break;
            }
        }

        public override void FillPolygon(Color color, PointF[] points)
        {
            //solid color

            int j = points.Length;
            float[] polygonPoints = new float[j * 2];
            int n = 0;
            for (int i = 0; i < j; ++i)
            {
                polygonPoints[n] = points[i].X;
                polygonPoints[n + 1] = points[i].Y;
                n += 2;
            }
            canvasGL2d.Note1 = this.Note1;
            canvasGL2d.FillPolygon(color, polygonPoints);
        }
        public override void FillPolygon(Brush brush, PointF[] points)
        {
            canvasGL2d.Note1 = this.Note1;
            switch (brush.BrushKind)
            {
                case BrushKind.LinearGradient:
                    {
                        //linear grdient brush for polygon 

                        int j = points.Length;
                        float[] polygonPoints = new float[j * 2];
                        int n = 0;
                        for (int i = 0; i < j; ++i)
                        {
                            polygonPoints[n] = points[i].X;
                            polygonPoints[n + 1] = points[i].Y;
                            n += 2;
                        }
                        //use stencil buffer 
                        canvasGL2d.FillPolygon(brush, polygonPoints, polygonPoints.Length);

                    } break;
                case BrushKind.Texture:
                    {

                        int j = points.Length;
                        float[] polygonPoints = new float[j * 2];
                        int n = 0;
                        for (int i = 0; i < j; ++i)
                        {
                            polygonPoints[n] = points[i].X;
                            polygonPoints[n + 1] = points[i].Y;
                            n += 2;
                        }

                        //use stencil buffer 
                        //prepare texture brush  
                        var tbrush = (TextureBrush)brush;
                        GLBitmap bmpTexture = null;
                        if (tbrush.InnerImage2 == null)
                        {
                            //create gl image
                            var textureImage = tbrush.TextureImage;
                            tbrush.InnerImage2 = bmpTexture = GLBitmapTextureHelper.CreateBitmapTexture((System.Drawing.Bitmap)textureImage.InnerImage);

                        }

                        canvasGL2d.FillPolygon(brush, polygonPoints, polygonPoints.Length);


                        //delete gl
                        bmpTexture.Dispose();
                        tbrush.InnerImage2 = null;

                    } break;
                case BrushKind.Solid:
                    {
                        var solidColorBrush = (SolidBrush)brush;
                        this.FillPolygon(solidColorBrush.Color, points);

                    } break;
                default:
                    {


                    } break;
            }

        }

        public override void ClearSurface(Color c)
        {
            canvasGL2d.Clear(c);
        }
        //-------------------------------------------
        public override void DrawImage(Image image, RectangleF destRect)
        {

            if (image.IsReferenceImage)
            {
                //use reference image  
                GLBitmapReference bmpRef = image.InnerImage as GLBitmapReference;
                if (bmpRef != null)
                {
                    canvasGL2d.DrawImage(bmpRef, destRect.X, destRect.Y);
                }
                else
                {
                    var currentInnerImage = image.InnerImage as System.Drawing.Bitmap;
                    if (currentInnerImage != null)
                    {
                        //create  and replace ?
                        //TODO: add to another field
                        image.InnerImage = bmpRef = new GLBitmapReference(
                            GLBitmapTextureHelper.CreateBitmapTexture(currentInnerImage),
                            image.ReferenceX,
                            image.ReferenceY,
                            image.Width,
                            image.Height);
                        canvasGL2d.DrawImage(bmpRef, destRect.X, destRect.Y);
                    }
                    else
                    {
                        var currentGLImage = image.InnerImage as GLBitmap;
                        if (currentGLImage != null)
                        {
                            bmpRef = new GLBitmapReference(
                                  currentGLImage,
                                  image.ReferenceX,
                                  image.ReferenceY,
                                  image.Width,
                                  image.Height);
                            canvasGL2d.DrawImage(bmpRef, destRect.X, destRect.Y);
                        }
                    }
                }
            }
            else
            {

                GLBitmap glBitmapTexture = image.InnerImage as GLBitmap;
                if (glBitmapTexture != null)
                {
                    canvasGL2d.DrawImage(glBitmapTexture, destRect.X, destRect.Y, destRect.Width, destRect.Height);
                }
                else
                {
                    var currentInnerImage = image.InnerImage as System.Drawing.Bitmap;
                    if (currentInnerImage != null)
                    {
                        //create  and replace ?
                        //TODO: add to another field
                        image.InnerImage = glBitmapTexture = GLBitmapTextureHelper.CreateBitmapTexture(currentInnerImage);
                        canvasGL2d.DrawImage(glBitmapTexture, destRect.X, destRect.Y, destRect.Width, destRect.Height);
                    }
                }
            }


        }

        public override void DrawImage(Image image, RectangleF destRect, RectangleF srcRect)
        {
            //copy from src to dest 
            GLBitmap glBitmapTexture = image.InnerImage as GLBitmap;
            if (glBitmapTexture != null)
            {
                canvasGL2d.DrawImage(glBitmapTexture, srcRect,
                    destRect.X, destRect.Y, destRect.Width, destRect.Height);
            }
            else
            {
                var currentInnerImage = image.InnerImage as System.Drawing.Bitmap;
                if (currentInnerImage != null)
                {
                    //create  and replace ?
                    //TODO: add to another field
                    image.InnerImage = glBitmapTexture = GLBitmapTextureHelper.CreateBitmapTexture(currentInnerImage);
                    canvasGL2d.DrawImage(glBitmapTexture,
                       srcRect, destRect.X, destRect.Y, destRect.Width, destRect.Height);
                }
            }
        }
        public override void DrawImages(Image image, RectangleF[] destAndSrcPairs)
        {
            GLBitmap glBitmapTexture = image.InnerImage as GLBitmap;
            if (glBitmapTexture != null)
            {
                canvasGL2d.DrawImages(glBitmapTexture, destAndSrcPairs);
            }
            else
            {
                var currentInnerImage = image.InnerImage as System.Drawing.Bitmap;
                if (currentInnerImage != null)
                {
                    //create  and replace ?
                    //TODO: add to another field
                    image.InnerImage = glBitmapTexture = GLBitmapTextureHelper.CreateBitmapTexture(currentInnerImage);
                    canvasGL2d.DrawImages(glBitmapTexture, destAndSrcPairs);
                }
            }
        }

        public override Color StrokeColor
        {
            get
            {
                return canvasGL2d.StrokeColor;
            }
            set
            {

                canvasGL2d.StrokeColor = value;
            }
        }
        public override void DrawLine(float x1, float y1, float x2, float y2)
        {
            canvasGL2d.DrawLine(x1, y1, x2, y2);
        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            //stroke color
            canvasGL2d.StrokeColor = color;
            canvasGL2d.DrawRect(left, top, width, height);

        }
        //---------------------------------------------------

        public override Font CurrentFont
        {
            get
            {
                return this.currentFont;
            }
            set
            {
                //sample only *** 
                //canvasGL2d.CurrentFont = PixelFarm.Agg.Fonts.NativeFontStore.LoadFont("c:\\Windows\\Fonts\\Tahoma.ttf", 10);

                currentFont = value;
            }
        }
        public override void DrawText(char[] buffer, int x, int y)
        {
            if (this.Note1 == 2)
            {   
                //test draw string to gdi hdc: 
                //if need Gdi+/gdi to draw string                              
                //then draw it to hdc and  copy to canvas2d
                //[ platform specfic code]
                //1. use bitmap  
                gdiTextBoard.DrawText(this, buffer, x, y);

            }
            else
            {
                canvasGL2d.DrawString(buffer, x, y);
            }
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            canvasGL2d.DrawString(buffer, logicalTextBox.X, logicalTextBox.Y);
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            canvasGL2d.DrawString(str, logicalTextBox.X, logicalTextBox.Y);
        }
        public override void FillPath(Color color, GraphicsPath path)
        {
            //solid color
            var innerPath2 = path.InnerPath2;


            if (innerPath2 == null)
            {
                System.Drawing.Drawing2D.PathData pathData = path.GetPathData() as System.Drawing.Drawing2D.PathData;
                PixelFarm.Agg.VertexStore vxs = new PixelFarm.Agg.VertexStore();
                PixelFarm.Agg.GdiPathConverter.ConvertToVxs(pathData, vxs);

                //TODO: reuse flattener 
                PixelFarm.Agg.VertexSource.CurveFlattener flattener = new PixelFarm.Agg.VertexSource.CurveFlattener();
                vxs = flattener.MakeVxs2(vxs);
                path.InnerPath2 = vxs;

                this.canvasGL2d.FillVxs(color, vxs);
            }
            else
            {
                PixelFarm.Agg.VertexStore vxs = innerPath2 as PixelFarm.Agg.VertexStore;
                if (vxs != null)
                {
                    this.canvasGL2d.FillVxs(color, vxs);
                }
            }
        }
        public override void FillPath(Brush brush, GraphicsPath path)
        {
            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        SolidBrush solidBrush = (SolidBrush)brush;


                        var innerPath2 = path.InnerPath2;
                        if (innerPath2 == null)
                        {
                            System.Drawing.Drawing2D.PathData pathData = path.GetPathData() as System.Drawing.Drawing2D.PathData;
                            PixelFarm.Agg.VertexStore vxs = new PixelFarm.Agg.VertexStore();
                            PixelFarm.Agg.GdiPathConverter.ConvertToVxs(pathData, vxs);

                            //TODO: reuse flattener 

                            vxs = flattener.MakeVxs2(vxs);
                            path.InnerPath2 = vxs;

                            this.canvasGL2d.FillVxs(solidBrush.Color, vxs);
                        }
                        else
                        {
                            PixelFarm.Agg.VertexStore vxs = innerPath2 as PixelFarm.Agg.VertexStore;
                            if (vxs != null)
                            {
                                this.canvasGL2d.FillVxs(solidBrush.Color, vxs);
                            }
                        }
                    } break;
                case BrushKind.LinearGradient:
                    {


                    } break;
                default:
                    {
                    } break;
            }

        }
        public override void DrawPath(GraphicsPath gfxPath)
        {
            var innerPath2 = gfxPath.InnerPath2;
            if (innerPath2 == null)
            {
                System.Drawing.Drawing2D.PathData pathData = gfxPath.GetPathData() as System.Drawing.Drawing2D.PathData;
                PixelFarm.Agg.VertexStore vxs = new PixelFarm.Agg.VertexStore();
                PixelFarm.Agg.GdiPathConverter.ConvertToVxs(pathData, vxs);
                PixelFarm.Agg.VertexSource.CurveFlattener flattener = new PixelFarm.Agg.VertexSource.CurveFlattener();
                vxs = flattener.MakeVxs2(vxs);
                gfxPath.InnerPath2 = vxs;
                this.canvasGL2d.DrawVxs(vxs);

            }
            else
            {
                PixelFarm.Agg.VertexStore vxs = innerPath2 as PixelFarm.Agg.VertexStore;
                if (vxs != null)
                {
                    this.canvasGL2d.DrawVxs(vxs);
                }
            }
        }



        //---------------------------------------------------
        public override bool PushClipAreaRect(int width, int height, ref Rect updateArea)
        {
            this.clipRectStack.Push(currentClipRect);

            System.Drawing.Rectangle intersectResult =
                System.Drawing.Rectangle.Intersect(
                    currentClipRect,
                    System.Drawing.Rectangle.Intersect(
                    updateArea.ToRectangle().ToRect(),
                    new System.Drawing.Rectangle(0, 0, width, height)));

            currentClipRect = intersectResult;
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                //not intersec?
                return false;
            }
            else
            {
                canvasGL2d.EnableClipRect();
                canvasGL2d.SetClipRect(currentClipRect.X, currentClipRect.Y, currentClipRect.Width, currentClipRect.Height);
                return true;
            }
        }
        public override void PopClipAreaRect()
        {
            if (clipRectStack.Count > 0)
            {
                currentClipRect = clipRectStack.Pop();
            }


            canvasGL2d.EnableClipRect();
            canvasGL2d.SetClipRect(currentClipRect.X, currentClipRect.Y, currentClipRect.Width, currentClipRect.Height);

        }
        public override Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect.ToRect();
            }
        }
    }
}