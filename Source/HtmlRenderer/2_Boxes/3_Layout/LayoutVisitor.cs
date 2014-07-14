//BSD 2014, WinterDev

using System;
using System.Collections.Generic;

using HtmlRenderer.Css;
using HtmlRenderer.Drawing;
namespace HtmlRenderer.Boxes
{

    public class LayoutVisitor : BoxVisitor
    {
        HtmlContainer htmlContainer;
        float totalMarginLeftAndRight;
        Queue<Dictionary<CssBox, PartialBoxStrip>> dicStripPool;
        Dictionary<CssBox, PartialBoxStrip> readyDicStrip = new Dictionary<CssBox, PartialBoxStrip>();
        internal LayoutVisitor(IGraphics gfx, HtmlContainer htmlContainer)
        {
            this.Gfx = gfx;
            this.htmlContainer = htmlContainer;
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
                         (box.SizeWidth + this.ContainerBlockGlobalX) < CssBoxConstConfig.BOX_MAX_RIGHT ? box.SizeWidth : 0);

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

            if ((box.SizeWidth + this.ContainerBlockGlobalX) > CssBoxConstConfig.BOX_MAX_RIGHT ||
                (box.ParentBox != null && (box.ParentBox.SizeWidth + this.ContainerBlockGlobalX) > CssBoxConstConfig.BOX_MAX_RIGHT))
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
            HtmlRenderer.HtmlContainer.RaiseRequestImage(
                this.htmlContainer,
                binder,
                requestFrom,
                false);
        }

        internal float MeasureWhiteSpace(CssBox box)
        {
            //depends on Font of this box           
            float w = this.htmlContainer.MeasureWhitespace(this.Gfx, box.ActualFont);

            if (!(box.WordSpacing.IsEmpty || box.WordSpacing.IsNormalWordSpacing))
            {
                w += CssValueParser.ConvertToPxWithFontAdjust(box.WordSpacing, 0, box);
            }
            return w;
        }
        internal FontInfo GetFontInfo(System.Drawing.Font f)
        {
            return this.htmlContainer.GetFontInfo(f);
        }
        internal float MeasureStringWidth(char[] buffer, int startIndex, int len, System.Drawing.Font f)
        {
            return this.Gfx.MeasureString2(buffer, startIndex, len, f).Width;

        }
        internal IFontPool GetFontPool()
        {
            return this.htmlContainer;
        }
        //---------------------------------------------------------------
        internal Dictionary<CssBox, PartialBoxStrip> GetReadyStripDic()
        {
            if (readyDicStrip == null)
            {
                if (this.dicStripPool == null || this.dicStripPool.Count == 0)
                {
                    return new Dictionary<CssBox, PartialBoxStrip>();
                }
                else
                {
                    return this.dicStripPool.Dequeue();
                }
            }
            else
            {
                var tmpReadyStripDic = this.readyDicStrip;
                this.readyDicStrip = null;
                return tmpReadyStripDic;
            }
        }
        internal void ReleaseStripDic(Dictionary<CssBox, PartialBoxStrip> dic)
        {
            //clear before add to pool
            dic.Clear();

            if (this.readyDicStrip == null)
            {
                this.readyDicStrip = dic;
            }
            else
            {
                if (this.dicStripPool == null)
                {
                    this.dicStripPool = new Queue<Dictionary<CssBox, PartialBoxStrip>>();
                }


                this.dicStripPool.Enqueue(dic);
            }
        }
    }


}