﻿// 2015,2014 ,BSD, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
//
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
using LayoutFarm.Composers;

namespace LayoutFarm.WebDom
{



    sealed class HtmlRootElement : HtmlElement
    {
        public HtmlRootElement(HtmlDocument ownerDoc)
            : base(ownerDoc, 0, 0)
        {
        }
    }

    sealed class ExternalHtmlElement : HtmlElement
    {
        LazyCssBoxCreator lazyCreator;
        LayoutFarm.HtmlBoxes.InternalWrappers.WrapperBlockCssBox wrapper;
        public ExternalHtmlElement(HtmlDocument owner, int prefix, int localNameIndex, LazyCssBoxCreator lazyCreator)
            : base(owner, prefix, localNameIndex)
        {
            this.lazyCreator = lazyCreator;
        }
        public CssBox GetCssBox(RootGraphic rootgfx)
        {
            if (wrapper != null) return wrapper;
            RenderElement re;
            object controller;
            lazyCreator(rootgfx, out re, out controller);
            return wrapper = new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperBlockCssBox(controller, this.Spec, re);
        } 
    } 
    sealed class ShadowRootElement : HtmlElement
    {
        public ShadowRootElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {

        } 
    } 
}