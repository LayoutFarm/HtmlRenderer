////2014 Apache2, WinterDev
//using System;
//using LayoutFarm.Drawing;

//namespace LayoutFarm
//{

//    public abstract class HitObjectWrapper
//    {

//        public abstract object GetController();
//        public abstract bool IsTestable();
//        public abstract bool Contains(LayoutFarm.Drawing.Point p);
//        public abstract HitObjectWrapper FindOverlapSibling(LayoutFarm.Drawing.Point p);
//        public abstract bool HitTestCore(HitPointChain chain);

//        public abstract Point ElementLocation { get; }
//        public abstract Point GetElementGlobalLocation();
//        public abstract Rectangle ElementBoundRect { get; }

//        public abstract bool Focusable { get; }
//        public abstract bool HasParent { get; }
//        public abstract bool ContainsSubChain { get; }
//        public abstract object HitObject { get; }
//    }


//}