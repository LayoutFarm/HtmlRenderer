//BSD 2014, WinterDev

using System;


namespace HtmlRenderer.Dom
{

    public class LayoutVisitor : BoxVisitor
    {
        HtmlContainer htmlContainer;
        float totalMarginLeftAndRight;
        ContentTextSplitter contentTextSplitter;

        internal LayoutVisitor(IGraphics gfx, HtmlContainer htmlContainer)
        {
            this.Gfx = gfx;
            this.htmlContainer = htmlContainer;
        }

        internal HtmlContainer HtmlContainer
        {
            get { return this.htmlContainer; }
        }

        internal ContentTextSplitter GetSplitter()
        {    
           
            if (contentTextSplitter == null)
            {
                return contentTextSplitter = new ContentTextSplitter();
            }
            return contentTextSplitter;  
        }

        internal IGraphics Gfx
        {
            get;
            private set;
        }
        protected override void OnPushDifferentContaingBlock(CssBox box)
        {
            this.totalMarginLeftAndRight += (box.ActualMarginLeft + box.ActualMarginRight);
        }
        protected override void OnPopDifferentContaingBlock(CssBox box)
        {
            this.totalMarginLeftAndRight -= (box.ActualMarginLeft + box.ActualMarginRight);
        }
        //-----------------------------------------
        internal CssBox LatestSiblingBox
        {
            get;
            set;
        }
        internal void UpdateRootSize(CssBox box)
        {
            float candidateRootWidth = Math.Max(box.CalculateMinimumWidth() + CalculateWidthMarginTotalUp(box),
                         (box.SizeWidth + this.ContainerBlockGlobalX) < ConstConfig.BOX_MAX_RIGHT ? box.SizeWidth : 0);

            this.htmlContainer.UpdateSizeIfWiderOrHeigher(
                this.ContainerBlockGlobalX + candidateRootWidth,
                this.ContainerBlockGlobalY + box.SizeHeight);
        }
        /// <summary>
        /// Get the total margin value (left and right) from the given box to the given end box.<br/>
        /// </summary>
        /// <param name="box">the box to start calculation from.</param>
        /// <returns>the total margin</returns>
        float CalculateWidthMarginTotalUp(CssBox box)
        {

            if ((box.SizeWidth + this.ContainerBlockGlobalX) > ConstConfig.BOX_MAX_RIGHT ||
                (box.ParentBox != null && (box.ParentBox.SizeWidth + this.ContainerBlockGlobalX) > ConstConfig.BOX_MAX_RIGHT))
            {
                return (box.ActualMarginLeft + box.ActualMarginRight) + totalMarginLeftAndRight;
            }
            return 0;
        }

        internal bool AvoidImageAsyncLoadOrLateBind
        {
            get { return this.htmlContainer.AvoidAsyncImagesLoading || this.htmlContainer.AvoidImagesLateLoading; }
        }

        internal void RequestImage(ImageBinder binder, CssBox requestFrom)
        {
            this.htmlContainer.RequestImage(binder, requestFrom);
        }
        internal void RequestImage(ImageBinder binder, CssBox requestFrom, ReadyStateChangedHandler handler)
        {
            this.htmlContainer.RequestImage(binder, requestFrom, handler);
        }
        internal float MeasureWhiteSpace(CssBox box)
        {
            //depends on Font of this box
            float w = HtmlRenderer.Utils.FontsUtils.MeasureWhitespace(this.Gfx, box.ActualFont);
            if (!(box.WordSpacing.IsEmpty || box.WordSpacing.IsNormalWordSpacing))
            {
                w +=  CssValueParser.ConvertToPxWithFontAdjust(box.WordSpacing, 0, box);
            }
            return w;
        }

    }


}