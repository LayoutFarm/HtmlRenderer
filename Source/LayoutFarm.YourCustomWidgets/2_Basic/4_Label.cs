// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing; 
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{

    public class Label : UIBox
    {
        string text;
        Color textColor;
        CustomTextRun myTextRun;
        public Label(int w, int h)
            : base(w, h)
        {

        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (myTextRun == null)
            {
                myTextRun = new CustomTextRun(rootgfx, this.Width, this.Height);
                myTextRun.Text = this.text;
                myTextRun.TextColor = this.textColor;
            }
            //-----------
            return myTextRun;

        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.myTextRun; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.myTextRun != null; }
        }
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;

                if (this.myTextRun != null)
                {
                    this.myTextRun.Text = value;
                }
            }
        }
        public Color Color
        {
            get { return this.textColor; }
            set
            {
                this.textColor = value;
                if (myTextRun != null)
                {
                    myTextRun.TextColor = value;
                }
            }
        }
        public override int DesiredHeight
        {
            get
            {
                return this.Height;
            }
        }
        public override int DesiredWidth
        {
            get
            {
                return this.Width;
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "label");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }

}