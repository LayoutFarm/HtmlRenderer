//BSD 2014,WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes;
using System.Threading;

namespace HtmlRenderer.WebDom
{

    public class HtmlEventBridge
    {
        HtmlContainer container;
    
        public HtmlEventBridge()
        {
        }
        public void Bind(HtmlContainer container)
        {
            if (container != null)
            {
                this.container = container;
                
            }
        }
        public void Unbind()
        {
            this.container = null;
        }
        //----------------------------------------------------
        public void MouseDown(int x, int y, int button)
        {

            BoxHitChain hitChain = new BoxHitChain();
            hitChain.SetRootGlobalPosition(x, y); 
            //1. prob hit chain only
            BoxUtils.HitTest(container.GetRootCssBox(), x, y, hitChain);

            //2. invoke css event and script event 
            var hitInfo = hitChain.GetLastHit();


        }
        public void MouseUp(int x, int y, int button)
        {
            BoxHitChain hitChain = new BoxHitChain();
            hitChain.SetRootGlobalPosition(x, y);
            //1. prob hit chain only
            BoxUtils.HitTest(container.GetRootCssBox(), x, y, hitChain);

            //2. invoke css event and script event 
            var hitInfo = hitChain.GetLastHit();
        }
        public void MouseMove(int x, int y, int button)
        {
        }
        public void MouseDoubleClick(int x, int y, int button)
        {
        }
        public void MouseWheel(int x, int y, int button, int delta)
        {
        }

        //-----------------------------------
    }
}