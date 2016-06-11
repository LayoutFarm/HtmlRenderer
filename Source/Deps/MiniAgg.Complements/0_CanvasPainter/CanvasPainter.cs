// 2015,2014 ,BSD, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// Class StringPrinter.cs
// 
// Class to output the vertex source of a string as a run of glyphs.
//----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Agg;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Fonts;
namespace PixelFarm.Agg
{
    public class CanvasPainter
    {
        Graphics2D gx;
        Stroke stroke;
        ColorRGBA fillColor;
        ColorRGBA strokeColor;
        ScanlinePacked8 scline;
        ScanlineRasterizer sclineRas;
        ScanlineRasToDestBitmapRenderer sclineRasToBmp;
        FilterMan filterMan = new FilterMan();
        //-------------
        //tools
        //-------------
        SimpleRect simpleRect = new SimpleRect();
        Ellipse ellipse = new Ellipse();
        PathWriter lines = new PathWriter();
        RoundedRect roundRect = null;
        MyImageReaderWriter sharedImageWriterReader = new MyImageReaderWriter();
        CurveFlattener curveFlattener = new CurveFlattener();
        TextPrinter textPrinter;
        MyTypeFacePrinter stringPrinter = new MyTypeFacePrinter();
        //-------------
        public CanvasPainter(Graphics2D graphic2d)
        {
            this.gx = graphic2d;
            this.sclineRas = gx.ScanlineRasterizer;
            this.stroke = new Stroke(1);//default
            this.scline = graphic2d.ScanlinePacked8;
            this.sclineRasToBmp = graphic2d.ScanlineRasToDestBitmap;
            this.textPrinter = new TextPrinter();
        }
        public void Clear(ColorRGBA color)
        {
            gx.Clear(color);
        }
        public RectInt ClipBox
        {
            get { return this.gx.GetClippingRect(); }
            set { this.gx.SetClippingRect(value); }
        }
        public void SetClipBox(int x1, int y1, int x2, int y2)
        {
            this.gx.SetClippingRect(new RectInt(x1, y1, x2, y2));
        }
        public Graphics2D Graphics
        {
            get { return this.gx; }
        }

        /// <summary>
        /// draw circle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void FillCircle(double x, double y, double radius, ColorRGBA color)
        {
            ellipse.Reset(x, y, radius, radius);
            gx.Render(ellipse.MakeVxs(), color);
        }
        public void FillCircle(double x, double y, double radius)
        {
            ellipse.Reset(x, y, radius, radius);
            gx.Render(ellipse.MakeVxs(), this.fillColor);
        }

        public void FillEllipse(double left, double bottom, double right, double top, int nsteps)
        {
            ellipse.Reset((left + right) * 0.5,
                          (bottom + top) * 0.5,
                          (right - left) * 0.5,
                          (top - bottom) * 0.5,
                           nsteps);
            gx.Render(ellipse.MakeVxs(), this.fillColor);
            //VertexStoreSnap trans_ell = txBilinear.TransformToVertexSnap(vxs);
        }
        public void DrawEllipse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public void Line(double x1, double y1, double x2, double y2, ColorRGBA color)
        {
            lines.Clear();
            lines.MoveTo(x1, y1);
            lines.LineTo(x2, y2);
            gx.Render(stroke.MakeVxs(lines.Vxs), color);
        }
        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public void Line(double x1, double y1, double x2, double y2)
        {
            lines.Clear();
            lines.MoveTo(x1, y1);
            lines.LineTo(x2, y2);
            gx.Render(stroke.MakeVxs(lines.Vxs), this.strokeColor);
        }
        public double StrokeWidth
        {
            get { return this.stroke.Width; }
            set { this.stroke.Width = value; }
        }
        public void Draw(VertexStore vxs)
        {
            gx.Render(stroke.MakeVxs(vxs), this.strokeColor);
        }

        /// <summary>
        /// draw rectangle
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="color"></param>
        /// <param name="strokeWidth"></param>
        public void Rectangle(double left, double bottom, double right, double top, ColorRGBA color)
        {
            simpleRect.SetRect(left + .5, bottom + .5, right - .5, top - .5);
            gx.Render(stroke.MakeVxs(simpleRect.MakeVxs()), color);
        }
        public void Rectangle(double left, double bottom, double right, double top)
        {
            simpleRect.SetRect(left + .5, bottom + .5, right - .5, top - .5);
            gx.Render(stroke.MakeVxs(simpleRect.MakeVxs()), this.fillColor);
        }
        public void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            simpleRect.SetRect(left, bottom, right, top);
            gx.Render(simpleRect.MakeVertexSnap(), fillColor);
        }
        public void FillRectangle(double left, double bottom, double right, double top)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            simpleRect.SetRect(left, bottom, right, top);
            gx.Render(simpleRect.MakeVertexSnap(), this.fillColor);
        }
        public void FillRectLBWH(double left, double bottom, double width, double height)
        {
            double right = left + width;
            double top = bottom + height;
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            simpleRect.SetRect(left, bottom, right, top);
            gx.Render(simpleRect.MakeVertexSnap(), this.fillColor);
        }
        public void FillRoundRectangle(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }
            this.Fill(roundRect.MakeVxs());
        }
        public void DrawRoundRect(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }
            this.Draw(roundRect.MakeVxs());
        }

        //-------------------------------------------------------
        public Font CurrentFont
        {
            get { return this.textPrinter.CurrentFont; }
            set { this.textPrinter.CurrentFont = value; }
        }

        public void DrawString(
           string text,
           double x,
           double y)
        {
            textPrinter.Print(this, text.ToString(), x, y);
        }
        //-------------------------------------------------------

        /// <summary>
        /// fill vertex store
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="c"></param>
        public void Fill(VertexStoreSnap snap)
        {
            sclineRas.AddPath(snap);
            sclineRasToBmp.RenderWithColor(this.gx.DestImage, sclineRas, scline, fillColor);
        }
        public void Fill(VertexStore vxs)
        {
            sclineRas.AddPath(vxs);
            sclineRasToBmp.RenderWithColor(this.gx.DestImage, sclineRas, scline, fillColor);
        }
        public bool UseSubPixelRendering
        {
            get { return sclineRasToBmp.ScanlineRenderMode == ScanlineRenderMode.SubPixelRendering; }
            set { this.sclineRasToBmp.ScanlineRenderMode = value ? ScanlineRenderMode.SubPixelRendering : ScanlineRenderMode.Default; }
        }
        public ColorRGBA FillColor
        {
            get { return fillColor; }
            set { this.fillColor = value; }
        }
        public ColorRGBA StrokeColor
        {
            get { return strokeColor; }
            set { this.strokeColor = value; }
        }
        public void PaintSeries(VertexStore vxs, ColorRGBA[] colors, int[] pathIndexs, int numPath)
        {
            sclineRasToBmp.RenderSolidAllPaths(this.gx.DestImage,
                this.sclineRas,
                this.scline,
                vxs,
                colors,
                pathIndexs,
                numPath);
        }
        public void Fill(VertexStore vxs, ISpanGenerator spanGen)
        {
            this.sclineRas.AddPath(vxs);
            sclineRasToBmp.RenderWithSpan(this.gx.DestImage, sclineRas, scline, spanGen);
        }
        public void DrawImage(ActualImage actualImage, double x, double y)
        {
            this.sharedImageWriterReader.ReloadImage(actualImage);
            this.gx.Render(this.sharedImageWriterReader, x, y);
        }
        public void DrawImage(ActualImage actualImage, params Transform.AffinePlan[] affinePlans)
        {
            this.sharedImageWriterReader.ReloadImage(actualImage);
            this.gx.Render(sharedImageWriterReader, affinePlans);
        }

        //----------------------
        /// <summary>
        /// do filter at specific area
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="area"></param>
        public void DoFilterBlurStack(RectInt area, int r)
        {
            ChildImage img = new ChildImage(this.gx.DestImage, gx.PixelBlender,
                area.Left, area.Bottom, area.Right, area.Top);
            filterMan.DoStackBlur(img, r);
        }
        public void DoFilterBlurRecursive(RectInt area, int r)
        {
            ChildImage img = new ChildImage(this.gx.DestImage, gx.PixelBlender,
                area.Left, area.Bottom, area.Right, area.Top);
            filterMan.DoRecursiveBlur(img, r);
        }
        //---------------- 
        public void DrawBezierCurve(float startX, float startY, float endX, float endY,
           float controlX1, float controlY1,
           float controlX2, float controlY2)
        {
            VertexStore vxs = new VertexStore();
            PixelFarm.Agg.VertexSource.BezierCurve.CreateBezierVxs4(vxs,
                new PixelFarm.VectorMath.Vector2(startX, startY),
                new PixelFarm.VectorMath.Vector2(endX, endY),
                new PixelFarm.VectorMath.Vector2(controlX1, controlY1),
                new PixelFarm.VectorMath.Vector2(controlX2, controlY2));
            vxs = this.stroke.MakeVxs(vxs);
            sclineRas.Reset();
            sclineRas.AddPath(vxs);
            //sclineRasToBmp.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
            sclineRasToBmp.RenderWithColor(this.gx.DestImage, sclineRas, scline, this.strokeColor);
        }
        //---------------- 
        public VertexStore FlattenCurves(VertexStore srcVxs)
        {
            return curveFlattener.MakeVxs(srcVxs);
        }
        //---------------- 

    }
}