//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.SampleControls
{


    public class UITextBox : UIBox
    {

        TextEditRenderBox textEditRenderBox;
        public UITextBox(int width, int height)
            : base(width, height)
        {
        }
        public void Focus()
        {

        }
        protected override bool HasReadyRenderElement
        {
            get { return this.textEditRenderBox != null; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.textEditRenderBox; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (this.textEditRenderBox == null)
            {
                var tbox = new TextEditRenderBox(rootgfx, this.Width, this.Height, false);
                RenderElement.DirectSetVisualElementLocation(tbox, this.Left, this.Top);
                tbox.HasSpecificSize = true;
                tbox.SetController(this);



                this.textEditRenderBox = tbox;
            }
            return textEditRenderBox;
        }


        public TextSurfaceEventListener TextDomListener
        {
            get
            {
                if (this.textEditRenderBox == null)
                {
                    return null;
                }
                return this.textEditRenderBox.TextDomListener;
            }
        }
        public TextEditRenderBox VisualTextSurface
        {
            get
            {

                return this.textEditRenderBox;
            }
        }

        public int CurrentLineId
        {
            get
            {
                return textEditRenderBox.CurrentLineNumber;
            }
        }
        public int CurrentLineCharIndex
        {
            get
            {

                return textEditRenderBox.CurrentLineCharIndex;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return textEditRenderBox.CurrentTextRunCharIndex;
            }
        }
        public override void InvalidateGraphic()
        {
            if (textEditRenderBox != null)
                textEditRenderBox.InvalidateGraphic();
        }
    }
}