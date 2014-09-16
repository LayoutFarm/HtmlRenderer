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

        float maxValue;
        float minValue;

        int minmax_boxHeight = 15;

        

        public UIScrollBar(int width, int height)
            : base(width, height)
        {

           
        }
        public void SetupSrollBar(ScrollBarCreationParameters creationParameters)
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
            var min_button = new UIScrollButton(this.Width, minmax_boxHeight);
            min_button.BackColor = Color.DarkGray;
            plain.AddUI(min_button);
            this.minButton = min_button;
            //--------------
            //MaxButton
            var max_button = new UIScrollButton(this.Width, minmax_boxHeight);
            max_button.BackColor = Color.DarkGray;
            max_button.SetLocation(0, this.Height - minmax_boxHeight);
            plain.AddUI(max_button);
            this.maxButton = max_button;
            //-------------
            //ScrollButton
            var scroll_button = new UIScrollButton(this.Width, minmax_boxHeight);
            scroll_button.BackColor = Color.DarkBlue;
            scroll_button.SetLocation(0, this.Height - (minmax_boxHeight + minmax_boxHeight));

            SetupScrollButtonProperties(scroll_button);

            plain.AddUI(scroll_button);
            this.scrollButton = scroll_button;
            //--------------
            this.mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {

        }
        void SetupScrollButtonProperties(UIScrollButton button)
        {
            //3. drag

            button.Dragging += (s, e) =>
            {
                Point pos = button.Position;
                //if vscroll bar then move only y axis
                int newYPos = pos.Y + e.YDiff;
                //clamp!
                if (newYPos >= this.Height - (minmax_boxHeight + scrollButton.Height))
                {
                    newYPos = this.Height - (minmax_boxHeight + scrollButton.Height);
                }
                else if (newYPos < minmax_boxHeight)
                {
                    newYPos = minmax_boxHeight;
                }
                button.SetLocation(pos.X, newYPos);

                button.InvalidateGraphic();
            };
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
                button_box.SetController(this);
                this.buttonBox = button_box;
            }
            return buttonBox;
        }
        //------------------------------------------------------
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        public event EventHandler<UIDragEventArgs> Dragging;

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
            if (Dragging != null)
            {
                Dragging(this, e);
            }
            base.OnDragging(e);
        }
    }

    public class ScrollBarCreationParameters
    {
        public Rectangle elementBound;
        public Size arrowBoxSize;
        public int thumbBoxLimit;
        public int maximum;
        public int minmum;
        public int largeChange;
        public int smallChange;
        public ScrollBarType scrollBarType;
    }



}