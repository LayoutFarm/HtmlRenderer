//BSD 2014,WinterDev

using System.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Drawing;
using HtmlRenderer.SvgDom;

namespace HtmlRenderer.Boxes
{

    public sealed class SvgRootBox : CssBox
    {

        public SvgRootBox(CssBox owner, object controller, Css.BoxSpec spec, SvgElement svgSpec)
            : base(owner, controller, spec, Css.CssDisplay.Block)
        {
            //create svg node 
            this.SvgSpec = svgSpec;
            ChangeDisplayType(this, Css.CssDisplay.Block);
            SetAsSvgRoot(this);
        }
        protected override void PaintImp(IGraphics g, PaintVisitor p)
        {

            //render this svg
            var cnode = this.SvgSpec.GetFirstNode();
            while (cnode != null)
            {

                cnode.Value.Paint(g);
                cnode = cnode.Next;
            }
        }
        public SvgElement SvgSpec
        {
            get;
            set;
        }

        /// <summary>
        /// for svg
        /// </summary>
        /// <param name="containingBlock"></param>
        internal void ReComputeSvgAspectValue(CssBox containingBlock)
        {

            var svgElement = this.SvgSpec;
            //recompute value if need 
            var cnode = svgElement.GetFirstNode();
            float containerW = containingBlock.SizeWidth;
            float emH = containingBlock.GetEmHeight();
            while (cnode != null)
            {
                cnode.Value.ReEvaluateComputeValue(containerW, 100, emH);
                cnode = cnode.Next;
            }

            this.SetSize(500, 500);
        }

        public void HitTestCore(SvgHitChain chain, float x, float y)
        {
            //1.
            SvgElement root = this.SvgSpec;
            chain.AddHit(root, x, y);
            //2. find hit child
            var child = root.GetFirstNode();
            while (child != null)
            {
                var node = child.Value;
                if (node.HitTestCore(chain, x, y))
                {
                    break;
                }
                child = child.Next;
            }
        }
    }

    public class SvgHitChain
    {

        List<SvgHitInfo> svgList = new List<SvgHitInfo>();
        public SvgHitChain()
        {
        }
        public void AddHit(SvgElement svg, float x, float y)
        {
            svgList.Add(new SvgHitInfo(svg, x, y));
        }
        public int Count
        {
            get
            {
                return this.svgList.Count;
            }
        }
        public SvgHitInfo GetHitInfo(int index)
        {
            return this.svgList[index];
        }
        public SvgHitInfo GetLastHitInfo()
        {
            return this.svgList[svgList.Count - 1];
        }
    }


    public struct SvgHitInfo
    {
        public readonly SvgElement svg;
        public readonly float x;
        public readonly float y;
        public SvgHitInfo(SvgElement svg, float x, float y)
        {
            this.svg = svg;
            this.x = x;
            this.y = y;
        }
    }
}