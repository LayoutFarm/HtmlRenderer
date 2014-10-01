//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{

    partial class RenderElement : IHitElement
    {
        object controller;
        public object GetController()
        {
            return controller;
        }
        public void SetController(object controller)
        {
            this.controller = controller;
        }
        bool IHitElement.IsTestable()
        {
            return ((this.uiFlags & HIDDEN) == 0) && (this.parentLink != null);
        }
        IHitElement IHitElement.FindOverlapSibling(Drawing.Point p)
        {
            return this.ParentVisualElement.FindOverlapedChildElementAtPoint(this, p);
        }
        Point IHitElement.ElementLocation
        {
            get { return this.Location; }
        }
        Point IHitElement.GetElementGlobalLocation()
        {
            return this.GetGlobalLocation();
        }
        Rectangle IHitElement.ElementBoundRect
        {
            get { return this.BoundRect; }
        }
        bool IHitElement.Focusable
        {
            get { return this.Focusable; }
        }
        bool IHitElement.HasParent
        {
            get { return this.ParentVisualElement != null; }
        }
    }
}