//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.Css;
using HtmlRenderer.Composers;
using HtmlRenderer.Composers.BridgeHtml;


using HtmlRenderer.WebDom;
using LayoutFarm.UI;
using LayoutFarm.SvgDom;

namespace HtmlRenderer.Composers.BridgeHtml
{
    static class SvgElementPortal
    {

        public static void HandleSvgMouseDown(CssBoxSvgRoot svgBox, UIEventArgs e)
        {

            SvgHitChain hitChain = new SvgHitChain();
            svgBox.HitTestCore(hitChain, e.X, e.Y);
            PropagateEventOnBubblingPhase(hitChain, e);
        }

        static void PropagateEventOnBubblingPhase(SvgHitChain hitChain, UIEventArgs eventArgs)
        {
            int hitCount = hitChain.Count;
            //then propagate
            for (int i = hitCount - 1; i >= 0; --i)
            {
                SvgHitInfo hitInfo = hitChain.GetHitInfo(i);
                SvgElement svg = hitInfo.svg;
                if (svg != null)
                {
                    var controller = SvgElement.UnsafeGetController(hitInfo.svg) as IEventListener;
                    if (controller != null)
                    {
                        //dispatch event 
                    }
                }
            }
        }
    }
}