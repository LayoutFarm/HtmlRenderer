// 2015,2014 ,Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.DzBoardSample
{
    public abstract class CompoModule
    {
        protected HtmlBoxes.HtmlHost htmlHost;
        protected SampleViewport viewport;
        public void StartModule(HtmlBoxes.HtmlHost htmlHost, SampleViewport viewport)
        {
            this.htmlHost = htmlHost;
            this.viewport = viewport;
            OnStartModule();
        }
        protected virtual void OnStartModule()
        {
        }
    }
}