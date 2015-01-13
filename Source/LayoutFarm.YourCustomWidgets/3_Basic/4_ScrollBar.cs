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

    public class ScrollBar : UIBox
    {
        CustomRenderBox mainBox;
        ScrollButton minButton;
        ScrollButton maxButton;
        ScrollButton scrollButton;

        float maxValue;
        float minValue;
        float smallChange;
        float largeChange;
        float scrollValue;


        double onePixelFor = 1;
        bool isScrollLengthLargerThanRealValue;


        int minmax_boxHeight = 15;
        public ScrollBar(int width, int height)
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
                    case CustomWidgets.ScrollBarType.Horizontal:
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
        //--------------------------------------------------------------------------

        public void StepSmallToMax()
        {
            if (this.scrollValue + smallChange < this.MaxValue)
            {
                scrollValue = this.scrollValue + smallChange;
            }
            else
            {
                scrollValue = this.MaxValue;
            }
            //---------------------------
            //update visual presentation             
            UpdateScrollButtonPosition();

            if (this.UserScroll != null)
            {
                this.UserScroll(this, EventArgs.Empty);
            }
        }
        public void StepSmallToMin()
        {
            if (this.scrollValue - smallChange >= this.minValue)
            {
                scrollValue = this.scrollValue - smallChange;
            }
            else
            {
                scrollValue = this.MinValue;
            }
            //---------------------------
            //update visual presentation   
            UpdateScrollButtonPosition();
            if (this.UserScroll != null)
            {
                this.UserScroll(this, EventArgs.Empty);
            }
        }
        void UpdateScrollButtonPosition()
        {
            int thumbPosY = CalculateThumbPosition() + minmax_boxHeight;
            scrollButton.SetLocation(0, thumbPosY);

        }
        //--------------------------------------------------------------------------
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            bgBox.SetLocation(this.Left, this.Top);
            //---------------------------------------------------------

            VisualLayerCollection layers = new VisualLayerCollection();
            bgBox.Layers = layers;

            PlainLayer plain = new PlainLayer(bgBox);
            layers.AddLayer(plain);
            //-----------------------------------------------------


            //MinButton
            SetupMinButtonProperties(plain);
            //MaxButton
            SetupMaxButtonProperties(plain);
            //ScrollButton
            SetupScrollButtonProperties(plain);
            //--------------
            this.mainBox = bgBox;
        }
        //----------------------------------------------------------------------- 
        int CalculateThumbPosition()
        {
            if (this.isScrollLengthLargerThanRealValue)
            {
                return (int)(this.scrollValue / this.onePixelFor);
            }
            else
            {
                return (int)(this.scrollValue / this.onePixelFor);
            }
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {

        }
        void SetupMinButtonProperties(PlainLayer plain)
        {

            var min_button = new ScrollButton(this.Width, minmax_boxHeight);
            min_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);

            min_button.MouseUp += (s, e) => this.StepSmallToMin();


            plain.AddUI(min_button);
            this.minButton = min_button;
        }
        void SetupMaxButtonProperties(PlainLayer plain)
        {
            var max_button = new ScrollButton(this.Width, minmax_boxHeight);
            max_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            max_button.SetLocation(0, this.Height - minmax_boxHeight);


            max_button.MouseUp += (s, e) => this.StepSmallToMax();

            plain.AddUI(max_button);
            this.maxButton = max_button;

        }
        void SetupScrollButtonProperties(PlainLayer plain)
        {
            //calculate scroll length ratio
            //scroll button height is ratio with real scroll length
            float scrollValueRange = this.maxValue - this.minValue;
            //2. 
            float physicalScrollLength = this.Height - (this.minmax_boxHeight + this.minmax_boxHeight);
            //3.
            float valueViewLength = physicalScrollLength;

            //-----------------
            //calculate
            // (physicalScrollLength/scrollValueRange) = (thumbBoxLength/physicalScrollLength);

            int thumbBoxLength = 1;
            if (scrollValueRange < physicalScrollLength)
            {
                //physical range is larger than 
                float eachStep = physicalScrollLength / scrollValueRange;
                thumbBoxLength = (int)Math.Round(eachStep) * 2;
                isScrollLengthLargerThanRealValue = true;
                this.onePixelFor = scrollValueRange / (physicalScrollLength - thumbBoxLength);
            }
            else
            {
                float eachStep = (physicalScrollLength * smallChange) / scrollValueRange;
                thumbBoxLength = (int)Math.Round(eachStep) * 2;
                if (thumbBoxLength < 10)
                {
                    thumbBoxLength = 10;//minimum scrollbox lenth
                    this.onePixelFor = scrollValueRange / (physicalScrollLength - eachStep);
                }
                else if (thumbBoxLength > physicalScrollLength)
                {
                    thumbBoxLength = (int)eachStep;
                    this.onePixelFor = scrollValueRange / (physicalScrollLength - eachStep);
                }
                else
                {
                    this.onePixelFor = scrollValueRange / (physicalScrollLength - eachStep);
                }

            }
            //4.  
            if (this.onePixelFor < 1)
            {
                //real range is smaller than physical scrollLength

            }
            else
            {
                //real range is larger than physical scrollLength 
            }


            var scroll_button = new ScrollButton(this.Width, thumbBoxLength);
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);

            int thumbPosY = CalculateThumbPosition() + minmax_boxHeight;
            scroll_button.SetLocation(0, thumbPosY);

            plain.AddUI(scroll_button);
            this.scrollButton = scroll_button;

            //3. drag
            scroll_button.MouseMove += (s, e) =>
            {

                if (!e.IsDragging)
                {
                    return;
                }
                Point pos = scroll_button.Position;
                //if vscroll bar then move only y axis 
                int newYPos = (int)(pos.Y + e.YDiff); 

                //clamp!
                if (newYPos >= this.Height - (minmax_boxHeight + scrollButton.Height))
                {
                    newYPos = this.Height - (minmax_boxHeight + scrollButton.Height);
                }
                else if (newYPos < minmax_boxHeight)
                {
                    newYPos = minmax_boxHeight;
                }

                //calculate value from position 

                int currentMarkAt = (newYPos - minmax_boxHeight);
                this.scrollValue = (float)(onePixelFor * currentMarkAt);
                newYPos = CalculateThumbPosition() + minmax_boxHeight;
                scroll_button.SetLocation(pos.X, newYPos);

                if (this.UserScroll != null)
                {
                    this.UserScroll(this, EventArgs.Empty);
                }
            };
        }
        //----------------------------------------------------------------------- 
        public void SetupScrollBar(ScrollBarCreationParameters creationParameters)
        {
            this.maxValue = creationParameters.maximum;
            this.minValue = creationParameters.minmum;

        }
        public float MaxValue
        {
            get { return this.maxValue; }
            set
            {
                this.maxValue = value;
            }
        }
        public float MinValue
        {
            get { return this.minValue; }
            set
            {
                this.minValue = value;
            }
        }
        public float SmallChange
        {
            get { return this.smallChange; }
            set
            {
                this.smallChange = value;
            }
        }
        public float LargeChange
        {
            get { return this.largeChange; }
            set
            {
                this.largeChange = value;
            }
        }
        public float ScrollValue
        {
            get { return this.scrollValue; }
            set
            {
                this.scrollValue = value;
            }
        }
        //-----------------------------------------------------------------------

        public event EventHandler<EventArgs> UserScroll;

    }
    public enum ScrollBarType
    {
        Vertical,
        Horizontal
    }
    class ScrollButton : UIBox
    {
        CustomRenderBox buttonBox;
        Color backColor;

        public ScrollButton(int width, int height)
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
                button_box.SetLocation(this.Left, this.Top);
                button_box.SetController(this);

                this.buttonBox = button_box;
            }
            return buttonBox;
        }
        //------------------------------------------------------
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        public event EventHandler<UIMouseEventArgs> MouseMove;


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
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (this.MouseMove != null)
            {
                MouseMove(this, e);
            }
            e.CancelBubbling = true; 
        }
      
    }

    public class ScrollBarCreationParameters
    {
        public Rectangle elementBound;
        public Size arrowBoxSize;
        public int thumbBoxLimit;
        public float maximum;
        public float minmum;
        public float largeChange;
        public float smallChange;
        public ScrollBarType scrollBarType;
    }

    public class ScrollingRelation
    {
        ScrollBar scBar;
        UIBox panel;
        public ScrollingRelation(ScrollBar scBar, UIBox panel)
        {
            this.scBar = scBar;
            this.panel = panel;
            scBar.UserScroll += (s, e) =>
            {
                panel.SetViewport(0, (int)scBar.ScrollValue);
            };

        }
    }

}