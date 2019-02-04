//Apache2, 2014-present, WinterDev
using System;
using System.IO;

using PaintLab.Svg;
using LayoutFarm.UI;
using PixelFarm.Drawing;
//
using PixelFarm.DrawingGL;
using YourImplementation;
using LayoutFarm.UI.WinNeutral;

namespace LayoutFarm
{
    public class AppHostNeutral : AppHost
    {
        ////if ENABLE OPENGL
        ////-----------------------------------
        //OpenTK.MyGLControl _glControl;
        //CpuBlitGLESUIElement _bridgeUI; 
        ////----------------------------------- 
        //LayoutFarm.UI.UISurfaceViewportControl _vw;
        //System.Windows.Forms.Form _ownerForm;
        UISurfaceViewportControl _vw;
        public AppHostNeutral(UISurfaceViewportControl vw)
        {
            //---------------------------------------
            //this specific for WindowForm viewport
            //---------------------------------------
            _vw = vw;
            GLPainterContext pcx = _vw.GetGLRenderSurface();
            GLPainter glPainter = _vw.GetGLPainter();
            RootGraphic rootGfx = _vw.RootGfx;
            //
        }
        //void SetUpGLSurface(OpenTK.MyGLControl glControl)
        //{
        //    if (glControl == null) return;
        //    //TODO: review here
        //    //Temp: 
        //    _glControl = glControl;
        //    _glControl.SetGLPaintHandler(null);
        //    //
        //    IntPtr hh1 = _glControl.Handle; //ensure that contrl handler is created
        //    _glControl.MakeCurrent();

        //    if (_vw.InnerViewportKind == InnerViewportKind.GdiPlusOnGLES)
        //    {
        //        _bridgeUI = new GdiOnGLESUIElement(glControl.Width, glControl.Height);
        //    }
        //    else
        //    {
        //        //pure agg's cpu blit 
        //        _bridgeUI = new CpuBlitGLESUIElement(glControl.Width, glControl.Height);
        //    }


        //    //optional***
        //    //_bridgeUI.SetUpdateCpuBlitSurfaceDelegate((p, area) =>
        //    //{
        //    //    _client.DrawToThisCanvas(_bridgeUI.GetDrawBoard(), area);
        //    //});


        //    GLPainterContext pcx = _vw.GetGLRenderSurface();
        //    GLPainter glPainter = _vw.GetGLPainter();

        //    RootGraphic rootGfx = _vw.RootGfx;
        //    _bridgeUI.CreatePrimaryRenderElement(pcx, glPainter, rootGfx);



        //    //*****
        //    RenderBoxBase renderE = (RenderBoxBase)_bridgeUI.GetPrimaryRenderElement(rootGfx);
        //    rootGfx.AddChild(renderE);
        //    rootGfx.SetPrimaryContainerElement(renderE);
        //    //***
        //}

        ////
        //protected override UISurfaceViewportControl GetHostSurfaceViewportControl()
        //{
        //    return _vw;
        //}

        public override string OwnerFormTitle
        {
            get => "";
            set { }
        }
        //
        public override RootGraphic RootGfx => _vw.RootGfx;
        //
        public override void AddChild(RenderElement renderElement)
        {
            _vw.AddChild(renderElement);
        }
        public override void AddChild(RenderElement renderElement, object owner)
        {
            _vw.AddChild(renderElement, owner);
        }

        public override Image LoadImage(string imgName, int reqW, int reqH)
        {
            if (!File.Exists(imgName)) //resolve to actual img 
            {
                return null;
            }

            //we support svg as src of img
            //...
            //THIS version => just check an extension of the request file
            string ext = System.IO.Path.GetExtension(imgName).ToLower();
            switch (ext)
            {
                default: return null;
                case ".svg":
                    try
                    {
                        //***
                        //TODO: do not access local file system directly
                        //MUST ask the host for a specific file****
                        string svg_str = File.ReadAllText(imgName);
                        VgVisualElement vgVisElem = VgVisualDocHelper.CreateVgVisualDocFromFile(imgName).VgRootElem;
                        return CreateBitmap(vgVisElem, reqW, reqH);

                    }
                    catch (System.Exception ex)
                    {
                        return null;
                    }
                case ".png":
                case ".jpg":
                    {
                        try
                        {

                            return null;
                            //System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName);
                            //GdiPlusBitmap bmp = new GdiPlusBitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
                            //return bmp; 
                            //                            using (System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName))
                            //                            {
                            //                                PixelFarm.CpuBlit.MemBitmap memBmp = new PixelFarm.CpuBlit.MemBitmap(gdiBmp.Width, gdiBmp.Height);
                            //#if DEBUG
                            //                                memBmp._dbugNote = "img" + imgName;
                            //#endif
                            //                                PixelFarm.CpuBlit.Imaging.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(gdiBmp, memBmp);
                            //                                return memBmp;
                            //                            }

                        }
                        catch (System.Exception ex)
                        {
                            //return error img
                            return null;
                        }
                    }

            }

        }


        PixelFarm.CpuBlit.MemBitmap CreateBitmap(VgVisualElement renderVx, int reqW, int reqH)
        {

            PixelFarm.CpuBlit.RectD bound = renderVx.GetRectBounds();
            //create
            PixelFarm.CpuBlit.MemBitmap backingBmp = new PixelFarm.CpuBlit.MemBitmap((int)bound.Width + 10, (int)bound.Height + 10);
#if DEBUG
            backingBmp._dbugNote = "renderVx";
#endif
            //PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backingBmp);

            using (PixelFarm.CpuBlit.AggPainterPool.Borrow(backingBmp, out PixelFarm.CpuBlit.AggPainter painter))
            using (VgPaintArgsPool.Borrow(painter, out VgPaintArgs paintArgs))
            {
                double prevStrokeW = painter.StrokeWidth;

                renderVx.Paint(paintArgs);
                painter.StrokeWidth = prevStrokeW;//restore 
            }

            return backingBmp;
        }
    }
}