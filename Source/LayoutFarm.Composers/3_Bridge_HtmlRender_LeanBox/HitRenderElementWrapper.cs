////2014 Apache2, WinterDev
//using System;
//using LayoutFarm.Drawing;

//namespace LayoutFarm
//{
//    public class HitRenderElementWrapper : HitObjectWrapper
//    {
//        RenderElement renderE;
//        public HitRenderElementWrapper(RenderElement renderE)
//        {
//            this.renderE = renderE;
//        }
//        public override object HitObject
//        {
//            get { return this.renderE; }
//        }
//        public override object GetController()
//        {
//            return this.renderE.GetController();
//        }
//        public override bool Contains(Point p)
//        {
//            return this.renderE.Contains(p);
//        }
//        public override bool HitTestCore(HitPointChain chain)
//        {
//            return this.renderE.HitTestCore(chain);
//        }
//        public override bool IsTestable()
//        {
//            return renderE.IsTestable;
//        }
//        public override HitObjectWrapper FindOverlapSibling(Drawing.Point p)
//        {
//            var result = renderE.ParentVisualElement.FindOverlapedChildElementAtPoint(renderE, p);
//            if (result != null)
//            {
//                return new HitRenderElementWrapper(result);
//            }
//            return null;

//        }
//        public override Point ElementLocation
//        {
//            get { return renderE.Location; }
//        }
//        public override Point GetElementGlobalLocation()
//        {
//            return renderE.GetGlobalLocation();
//        }
//        public override Rectangle ElementBoundRect
//        {
//            get { return renderE.BoundRect; }
//        }
//        public override bool Focusable
//        {
//            get { return renderE.Focusable; }
//        }
//        public override bool HasParent
//        {
//            get { return renderE.ParentLink != null; }
//        }
//        public override bool ContainsSubChain
//        {
//            get { return false; }
//        }
//    }
//}