//Apache2, 2014-present, WinterDev
using System;
using System.IO;

using PaintLab.Svg;
using LayoutFarm.UI;
using PixelFarm.Drawing;
//
using PixelFarm.DrawingGL;
using YourImplementation;
using TestApp01.iOS;
using PixelFarm.Drawing.GLES2;
namespace LayoutFarm
{
    class AppHostIOS : AppHost
    {

        GameViewController _vw;
        int _formTitleBarHeight;
        GLPainterContext _pcx;
        GLPainter _painter;
        int _canvasW;
        int _canvasH;
        MyRootGraphic _rootGfx;
        MyGLDrawBoard _drawBoard;
        public AppHostIOS(GameViewController vw, int canvasW, int canvasH)
        {
            //---------------------------------------
            //this specific for WindowForm viewport
            //---------------------------------------
            _vw = vw;
            _formTitleBarHeight = 0;
            _canvasW = canvasW;
            _canvasH = canvasH;

            _primaryScreenWorkingAreaW = vw.ViewWidth;
            _primaryScreenWorkingAreaH = vw.ViewHeight;

            string basedir = "";
            PixelFarm.Platforms.StorageService.RegisterProvider(new LocalFileStorageProvider(basedir));
            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new YourImplementation.ImgCodecMemBitmapIO();

            int max = Math.Max(canvasW, canvasH);
            _pcx = GLPainterContext.Create(max, max, canvasW, canvasH, true);
            _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

            _painter = new GLPainter();
            _painter.BindToPainterContext(_pcx);
            _painter.SetClipBox(0, 0, canvasW, canvasH);
            _painter.TextPrinter = new GLBitmapGlyphTextPrinter(_painter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
            //

            //
            _rootGfx = new MyRootGraphic(canvasW, canvasH, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
            SetUpGLSurface();
            _drawBoard = new MyGLDrawBoard(_painter);
        }
        public override DrawBoard GetDrawBoard() => _drawBoard;
        public override Painter GetPainter() => _painter;

        public GLPainter Painter => _painter;

        public override RootGraphic GetRootGraphics() => _rootGfx;

        void SetUpGLSurface()
        {

            //    RootGraphic rootGfx = _vw.RootGfx;
            //    _bridgeUI.CreatePrimaryRenderElement(pcx, glPainter, rootGfx);

            //    //*****
            //    RenderBoxBase renderE = (RenderBoxBase)_bridgeUI.GetPrimaryRenderElement(rootGfx);
            //    rootGfx.AddChild(renderE);
            //    rootGfx.SetPrimaryContainerElement(renderE);
            //    //***
        }
        public override string OwnerFormTitle
        {
            get => "";
            set { }
        }
        //
        public override RootGraphic RootGfx => _rootGfx;
        //
        public override void AddChild(RenderElement renderElement)
        {
            _rootGfx.AddChild(renderElement);
        }
        public override void AddChild(RenderElement renderElement, object owner)
        {
            _rootGfx.AddChild(renderElement);
            //TODO:review thiss
            //_vw.AddChild(renderElement, owner);
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
                        return null;
                        try
                        {

                            //                            //System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName);
                            //                            //GdiPlusBitmap bmp = new GdiPlusBitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
                            //                            //return bmp; 
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
            //backingBmp._dbugNote = "renderVx";
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