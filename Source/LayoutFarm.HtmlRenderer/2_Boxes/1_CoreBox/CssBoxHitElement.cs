//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm;
namespace HtmlRenderer.Boxes
{


    public class CssBoxHitElement : HitObjectWrapper
    {
        CssBox box;
        public CssBoxHitElement(CssBox box)
        {
            this.box = box;
        }
        public override object HitObject
        {
            get { return this.box; }
        }
        public override object GetController()
        {
            return CssBox.UnsafeGetController(box);
        }
        public override bool IsTestable()
        {
            return true;
        }
        public override bool Contains(LayoutFarm.Drawing.Point p)
        {
            return false;
        }
        public override HitObjectWrapper FindOverlapSibling(LayoutFarm.Drawing.Point p)
        {
            return null;
        }
        public override bool HitTestCore(HitPointChain chain)
        {
            return false;
        }

        public override Point ElementLocation
        {
            get { return new Point(0, 0); }
        }
        public override Point GetElementGlobalLocation()
        {
            return box.GetElementGlobalLocation();
        }

        public override Rectangle ElementBoundRect
        {
            get { return new Rectangle(0, 0, 0, 0); }
        }
        public override bool Focusable
        {
            get
            {
                return box.AcceptKeyboardFocus;
            }
        }
        public override bool HasParent
        {
            get { return box.ParentBox != null; }
        }

        public override bool ContainsSubChain
        {
            get { return false; }
        }




    }
}