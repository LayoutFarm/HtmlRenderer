//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm;
namespace HtmlRenderer.Boxes
{


    partial class CssBox : IHitElement
    {

        object LayoutFarm.IHitElement.GetController()
        {
            return this._controller;
        }
        bool IHitElement.IsTestable()
        {
            return true;
        }
        bool IHitElement.Contains(LayoutFarm.Drawing.Point p)
        {
            return false;
        }
        IHitElement IHitElement.FindOverlapSibling(LayoutFarm.Drawing.Point p)
        {
            return null;
        }
        bool IHitElement.HitTestCore(HitPointChain chain)
        {
            return false;
        }

        Point IHitElement.ElementLocation
        {
            get { return new Point(0, 0); }
        }
        Point IHitElement.GetElementGlobalLocation()
        {
            return new Point(0, 0);
        }
        Rectangle IHitElement.ElementBoundRect
        {
            get { return new Rectangle(0, 0, 0, 0); }
        }
        bool IHitElement.Focusable
        {
            get
            {
                return this.AcceptKeyboardFocus;
            }
        }
        bool IHitElement.HasParent
        {
            get { return this.ParentBox != null; }
        }

        bool IHitElement.ContainsSubChain
        {
            get { return false; }
        }
        public bool AcceptKeyboardFocus
        {
            get;
            set;
        }
    }
}