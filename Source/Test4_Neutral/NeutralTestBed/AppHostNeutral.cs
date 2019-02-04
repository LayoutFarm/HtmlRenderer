//Apache2, 2014-present, WinterDev
using System;
using System.IO;

using PaintLab.Svg;
using PixelFarm.Drawing;
//

using LayoutFarm.UI.WinNeutral;

namespace LayoutFarm
{
    public class AppHostNeutral : AppHost
    {

        UISurfaceViewportControl _vw;
        public AppHostNeutral(UISurfaceViewportControl vw)
        {
            //---------------------------------------
            //this specific for WindowForm viewport
            //---------------------------------------
            _vw = vw;
        }
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

        /// <summary>
        /// create MemBitmap from input svg
        /// </summary>
        /// <param name="renderVx"></param>
        /// <param name="reqW"></param>
        /// <param name="reqH"></param>
        /// <returns></returns>
        protected PixelFarm.CpuBlit.MemBitmap CreateBitmap(VgVisualElement renderVx, int reqW, int reqH)
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