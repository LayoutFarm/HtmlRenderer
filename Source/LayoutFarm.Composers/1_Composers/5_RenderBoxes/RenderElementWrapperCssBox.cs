//2014,2015,2015 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.WebDom;
using LayoutFarm;
using LayoutFarm.Css;
using LayoutFarm.ContentManagers;
using LayoutFarm.Composers;


namespace LayoutFarm.HtmlBoxes
{

    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    sealed class CssExternalRun : CssRun
    {

        RenderElement externalRenderE;
        /// <summary>
        /// the image rectange restriction as returned from image load event
        /// </summary>
        Rectangle _imageRectangle;
        /// <summary>
        /// Creates a new BoxWord which represents an image
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        public CssExternalRun(RenderElement externalRenderE)
            : base(CssRunKind.Image)
        {
            this.externalRenderE = externalRenderE;
        }
        /// <summary>
        /// Gets the image this words represents (if one exists)
        /// </summary>
        RenderElement ExternalRenderE
        {
            get
            {
                return this.externalRenderE;
            }
        }
        public int OriginalImageWidth
        {
            get
            {
                return this.externalRenderE.Width;
                //var img = this.Image;
                //if (img != null)
                //{
                //    return img.Width;
                //}
                //return 1; //default image width
            }
        }
        public int OriginalImageHeight
        {
            get
            {
                return this.externalRenderE.Height;
            }
        }
        public bool HasUserImageContent
        {
            get
            {
                return this.externalRenderE != null;
            }
        }

        public RenderElement RenderElement
        {
            get { return this.externalRenderE; }
            set
            {

                this.externalRenderE = value;
            }
        }
        /// <summary>
        /// the image rectange restriction as returned from image load event
        /// </summary>
        public Rectangle ImageRectangle
        {
            get { return _imageRectangle; }
            set { _imageRectangle = value; }
        }


#if DEBUG
        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Image";
        }
#endif
    }

    /// <summary>
    /// replace element for extrernal
    /// </summary>
    class CssBoxInlineExternal : CssBox
    {
        CssExternalRun _imgRun;
        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="controller">the html tag data of this box</param>
        public CssBoxInlineExternal(object controller, Css.BoxSpec boxSpec,
            IRootGraphics rootgfx, RenderElement re)
            : base(controller, boxSpec, rootgfx)
        {

            this._imgRun = new CssExternalRun(re);
            this._imgRun.SetOwner(this);

            var runlist = new List<CssRun>(1);
            runlist.Add(_imgRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);
            ChangeDisplayType(this, Css.CssDisplay.Inline);

        }
        public override void Clear()
        {
            base.Clear();

            var runlist = new List<CssRun>(1);
            runlist.Add(_imgRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);

        }
        public override void Paint2(PaintVisitor p, RectangleF r)
        {
            Rectangle updateArea = new Rectangle((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height);

            int x = (int)updateArea.Left;
            int y = (int)updateArea.Top;
            var canvasPage = p.InnerCanvas;
            canvasPage.OffsetCanvasOrigin(x, y);
            r.Offset(-x, -y);

            _imgRun.RenderElement.DrawToThisCanvas(canvasPage, updateArea);

            canvasPage.OffsetCanvasOrigin(-x, -y);
            r.Offset(x, y); 

        }
        public RenderElement RenderElement
        {
            get
            {
                return _imgRun.RenderElement;
            }
            set
            {
                this._imgRun.RenderElement = value;
            }
        }
        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(PaintVisitor p)
        {
            // load image iff it is in visible rectangle  
            //1. single image can't be splited  

            Paint2(p, new RectangleF(0, 0, this.SizeWidth, this.SizeHeight));
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g">the device to use</param>
        public override void MeasureRunsSize(LayoutVisitor lay)
        {
            if (this.RunSizeMeasurePass)
            {
                return;
            }
            this.RunSizeMeasurePass = true;
            this._imgRun.Width = this._imgRun.RenderElement.Width;
            this._imgRun.Height = this._imgRun.RenderElement.Height;

        }
    }

    sealed class RenderElementWrapperCssBox : CssBox
    {
        CssBoxWrapperRenderElement wrapper;
        int globalXForRenderElement;
        int globalYForRenderElement;
        public RenderElementWrapperCssBox(object controller,
             BoxSpec spec,
             RenderElement renderElement)
            : base(controller, spec, renderElement.Root, CssDisplay.Block)
        {
            SetAsCustomCssBox(this);
            int mmw = renderElement.Width;
            int mmh = renderElement.Height;

            this.wrapper = new CssBoxWrapperRenderElement(renderElement.Root, mmw, mmh, renderElement);
            ChangeDisplayType(this, CssDisplay.Block);

            this.SetSize(mmw, mmh);

            LayoutFarm.RenderElement.SetParentLink(
             wrapper,
             new RenderBoxWrapperLink(this));

            LayoutFarm.RenderElement.SetParentLink(
                renderElement,
                new RenderBoxWrapperLink2(wrapper));
        }

        //public override void MeasureRunsSize(LayoutVisitor lay)
        //{

        //    if (this.RunSizeMeasurePass)
        //    {
        //        return;
        //    } 
        //    this.RunSizeMeasurePass = true;
        //    if (_blockRun != null)
        //    {
        //        _blockRun.Height = 20;
        //        _blockRun.Width = 200;
        //    }

        //}
        protected override Point GetElementGlobalLocationImpl()
        {
            return new Point(globalXForRenderElement, globalYForRenderElement);
        }
        public override bool CustomContentHitTest(float x, float y, CssBoxHitChain hitChain)
        {
            return false;
        }
        public override void CustomRecomputedValue(CssBox containingBlock, GraphicsPlatform gfxPlatform)
        {
            var ibox = CssBox.UnsafeGetController(this) as IBoxElement;
            if (ibox != null)
            {
                //todo: user minimum font height of the IBoxElement
                int w = (int)this.SizeWidth;
                int h = Math.Max((int)this.SizeHeight, ibox.MinHeight);

                ibox.ChangeElementSize(w, h);
                this.SetSize(w, h);
            }
            else
            {
                this.SetSize(100, 20);
            }

        }
        protected override void PaintImp(PaintVisitor p)
        {
            if (wrapper != null)
            {

                GetParentRenderElement(out this.globalXForRenderElement, out this.globalYForRenderElement);

                Rectangle rect = new Rectangle(0, 0, wrapper.Width, wrapper.Height);
                this.wrapper.DrawToThisCanvas(p.InnerCanvas, rect);
                p.FillRectangle(Color.Red, 0, 0, 10, 10);
            }
            else
            {
                //for debug!
                p.FillRectangle(Color.Red, 0, 0, 100, 100);
            }
        }


        RenderElement GetParentRenderElement(out int globalX, out int globalY)
        {
            CssBox cbox = this;
            globalX = 0;
            globalY = 0;//reset

            while (cbox != null)
            {
                globalX += (int)cbox.LocalX;
                globalY += (int)cbox.LocalY;
                var renderRoot = cbox as LayoutFarm.Composers.CssRenderRoot;

                if (renderRoot != null)
                {
                    this.wrapper.AdjustX = globalX;
                    this.wrapper.AdjustY = globalY;
                    return renderRoot.ContainerElement;
                }
                cbox = cbox.ParentBox;
            }
            return null;
        }



        class CssBoxWrapperRenderElement : RenderElement
        {
            RenderElement renderElement;
            int adjustX;
            int adjustY;

            public CssBoxWrapperRenderElement(RootGraphic rootgfx, int w, int h, RenderElement renderElement)
                : base(rootgfx, w, h)
            {
                this.renderElement = renderElement;
            }
            public int AdjustX
            {
                get { return this.adjustX; }
                set
                {
                    this.adjustX = value;
                }
            }
            public int AdjustY
            {
                get { return this.adjustY; }
                set
                {
                    if (this.adjustY > 0 && value == 0)
                    {

                    }
                    this.adjustY = value;
                }
            }
            public override int BubbleUpX
            {
                get
                {

                    return this.AdjustX;
                }
            }
            public override int BubbleUpY
            {
                get
                {
                    return this.AdjustY;
                }
            }

            public override void CustomDrawToThisCanvas(Canvas canvasPage, Rectangle updateArea)
            {
                //int x = this.adjustX;
                //int y = this.adjustY;
                renderElement.CustomDrawToThisCanvas(canvasPage, updateArea);

            }
        }
        class RenderBoxWrapperLink : LayoutFarm.RenderBoxes.IParentLink
        {
            RenderElementWrapperCssBox box;
            public RenderBoxWrapperLink(RenderElementWrapperCssBox box)
            {
                this.box = box;
            }

            public bool MayHasOverlapChild { get { return false; } }
            public RenderElement ParentRenderElement
            {
                get
                {
                    int globalX;
                    int globalY;
                    return box.GetParentRenderElement(out globalX, out globalY);
                }
            }
            public void AdjustLocation(ref Point p) { }
            public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
            {
                return null;
            }
            public RenderElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, RenderElement ve
#endif
)
            {
                goToFinalExit = false;
                int globalX;
                int globalY;
                var parent = box.GetParentRenderElement(out globalX, out globalY);

                if (parent != null)
                {
                    parent.InvalidateGraphics();
                }
                return parent;
            }

#if DEBUG
            public string dbugGetLinkInfo() { return ""; }
#endif
        }
        class RenderBoxWrapperLink2 : LayoutFarm.RenderBoxes.IParentLink
        {
            RenderElement box;
            public RenderBoxWrapperLink2(RenderElement box)
            {
                this.box = box;
            }

            public bool MayHasOverlapChild { get { return false; } }
            public RenderElement ParentRenderElement
            {
                get
                {
                    return box;
                }
            }
            public void AdjustLocation(ref Point p) { }
            public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
            {
                return null;
            }
            public RenderElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, RenderElement ve
#endif
)
            {
                goToFinalExit = false;
                //int globalX;
                //int globalY;
                var parent = box.ParentRenderElement;

                if (parent != null)
                {
                    parent.InvalidateGraphics();
                }
                return parent;
            }

#if DEBUG
            public string dbugGetLinkInfo() { return ""; }
#endif
        }
    }

}