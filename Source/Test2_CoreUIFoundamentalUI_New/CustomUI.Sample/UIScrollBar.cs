//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;
using LayoutFarm.Grids;

namespace LayoutFarm.SampleControls
{

    public class UIScrollBar : UIBox
    {
        CustomRenderBox mainBox;
        UIScrollButton minButton;
        UIScrollButton maxButton;
        UIScrollButton scrollButton;

        public UIScrollBar(int width, int height)
            : base(width, height)
        {

        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.mainBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.mainBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (mainBox == null)
            {

                switch (this.ScrollBarType)
                {
                    case SampleControls.ScrollBarType.Horizontal:
                        {

                        } break;
                    default:
                        {
                            CreateVScrollbarContent(rootgfx);
                        } break;

                }

            }
            return mainBox;
        }
        public ScrollBarType ScrollBarType
        {
            get;
            set;
        }
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            RenderElement.DirectSetVisualElementLocation(bgBox, this.Left, this.Top);
            //-------------- 

            VisualLayerCollection layers = new VisualLayerCollection();
            bgBox.Layers = layers;

            VisualPlainLayer plain = new VisualPlainLayer(bgBox);
            layers.AddLayer(plain);
            //--------------
            //MinButton
            var min_button = new UIScrollButton(this.Width, 15);
            min_button.BackColor = Color.DarkGray;
            plain.AddUI(min_button);
            this.minButton = min_button;
            //--------------
            //MaxButton
            var max_button = new UIScrollButton(this.Width, 15);
            max_button.BackColor = Color.DarkGray;
            max_button.SetLocation(0, this.Height - 15);
            plain.AddUI(max_button);
            this.maxButton = max_button;
            //-------------
            //ScrollButton
            var scroll_button = new UIScrollButton(this.Width, 15);
            scroll_button.BackColor = Color.DarkBlue;
            scroll_button.SetLocation(0, this.Height - 30);

            plain.AddUI(scroll_button);
            this.scrollButton = scroll_button;
            //--------------
            this.mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {
        }
    }
    public enum ScrollBarType
    {
        Vertical,
        Horizontal
    }
    class UIScrollButton : UIBox
    {
        CustomRenderBox buttonBox;
        Color backColor;

        public UIScrollButton(int width, int height)
            : base(width, height)
        {

        }


        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (this.HasReadyRenderElement)
                {
                    this.buttonBox.BackColor = value;
                }
            }
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get
            {
                return this.buttonBox;
            }
        }
        protected override bool HasReadyRenderElement
        {
            get { return buttonBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (buttonBox == null)
            {
                var button_box = new CustomRenderBox(rootgfx, this.Width, this.Height);
                button_box.HasSpecificSize = true;
                button_box.BackColor = this.backColor;
                RenderElement.DirectSetVisualElementLocation(button_box, this.Left, this.Top);

                this.buttonBox = button_box;
            }
            return buttonBox;
        }
        //------------------------------------------------------
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (MouseDown != null)
            {
                MouseDown(this, e);
            }
            e.CancelBubbling = true;
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (MouseUp != null)
            {
                MouseUp(this, e);
            }
            e.CancelBubbling = true;
        }
        protected override void OnDragging(UIDragEventArgs e)
        {
            base.OnDragging(e);
        }
    }
}