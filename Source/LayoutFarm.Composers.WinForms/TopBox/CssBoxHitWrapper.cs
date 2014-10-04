//2014 Apache2, WinterDev
using System;
using LayoutFarm.Drawing;

namespace LayoutFarm
{
    class CssBoxHitWrapper : HitObjectWrapper
    {

        HtmlRenderer.Boxes.CssBox renderE;
        public CssBoxHitWrapper(HtmlRenderer.Boxes.CssBox renderE)
        {
            this.renderE = renderE;
        }
        public override object HitObject
        {
            get { return this.renderE; }
        }
        public override object GetController()
        {
            return HtmlRenderer.Boxes.CssBox.UnsafeGetController(this.renderE);
        }
        public override bool Contains(Point p)
        {
            throw new System.NotSupportedException();
            //return this.renderE.Contains(p);
        }
        public override bool HitTestCore(HitPointChain chain)
        {
            throw new System.NotSupportedException();
            //return this.renderE.HitTestCore(chain);
        }
        public override bool IsTestable()
        {
            return true;
            //return false;
            //return renderE.IsTestable;
        }
        public override HitObjectWrapper FindOverlapSibling(Drawing.Point p)
        {
            //var result = renderE.ParentVisualElement.FindOverlapedChildElementAtPoint(renderE, p);
            //if (result != null)
            //{
            //    return new HitRenderElementWrapper(result);
            //}
            return null;

        }
        public override Point ElementLocation
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        public override Point GetElementGlobalLocation()
        {
            return renderE.GetElementGlobalLocation();

        }
        public override Rectangle ElementBoundRect
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        public override bool Focusable
        {
            get { return renderE.AcceptKeyboardFocus; }
        }
        public override bool HasParent
        {
            get { return true; }
        }
        public override bool ContainsSubChain
        {
            get { return false; }
        }
    }
}