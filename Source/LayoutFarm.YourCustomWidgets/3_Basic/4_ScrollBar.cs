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
        ScrollBarButton minButton;
        ScrollBarButton maxButton;
        ScrollBarButton scrollButton;

        float maxValue;
        float minValue;
        float smallChange;
        float largeChange;
        float scrollValue;

        bool isHorizontalScrollBar;


        double onePixelFor = 1;


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
            bgBox.SetController(this);
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
            return (int)(this.scrollValue / this.onePixelFor);
            //if (this.isScrollLengthLargerThanRealValue)
            //{
            //    return (int)(this.scrollValue / this.onePixelFor);
            //}
            //else
            //{
            //    return (int)(this.scrollValue / this.onePixelFor);
            //}
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {

        }
        void SetupMinButtonProperties(PlainLayer plain)
        {

            var min_button = new ScrollBarButton(this.Width, minmax_boxHeight, this);
            min_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);

            min_button.MouseUp += (s, e) => this.StepSmallToMin();


            plain.AddUI(min_button);
            this.minButton = min_button;
        }
        void SetupMaxButtonProperties(PlainLayer plain)
        {
            var max_button = new ScrollBarButton(this.Width, minmax_boxHeight, this);
            max_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            max_button.SetLocation(0, this.Height - minmax_boxHeight);


            max_button.MouseUp += (s, e) => this.StepSmallToMax();

            plain.AddUI(max_button);
            this.maxButton = max_button;

        }

        void EvaluateScrollBarProperties()
        {
            //calculate scroll length ratio
            //scroll button height is ratio with real scroll length
            float contentLength = this.maxValue - this.minValue;
            //2. 
            float physicalScrollLength = this.Height - (this.minmax_boxHeight + this.minmax_boxHeight);
            //3. 
            double ratio1 = physicalScrollLength / contentLength;
            int thumbBoxLength = 1;
            if (contentLength < physicalScrollLength)
            {
                int nsteps = (int)Math.Round(contentLength / smallChange);

                //small change value reflect thumbbox size
                // thumbBoxLength = (int)(ratio1 * this.SmallChange);
                int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                thumbBoxLength = eachStepLength * 2;
                //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                //this.onePixelFor = contentLength / (physicalScrollLength);
                this.onePixelFor = contentLength / (physicalScrollLength - thumbBoxLength);
            }
            else
            {
                //small change value reflect thumbbox size
                thumbBoxLength = (int)(ratio1 * this.SmallChange);
                float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
            }

            if (this.isHorizontalScrollBar)
            {
                throw new NotSupportedException();
            }
            else
            {
                //vertical scrollbar
                this.scrollButton.SetSize(
                    this.scrollButton.Width,
                    thumbBoxLength);
            }
        }
        void SetupScrollButtonProperties(PlainLayer plain)
        {


            var scroll_button = new ScrollBarButton(this.Width, 10, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosY = CalculateThumbPosition() + minmax_boxHeight;
            scroll_button.SetLocation(0, thumbPosY);
            plain.AddUI(scroll_button);
            this.scrollButton = scroll_button;

            //----------------------------

            EvaluateScrollBarProperties();
            //----------------------------
            //3. drag


            scroll_button.MouseMove += (s, e) =>
            {
                if (!e.IsDragging)
                {
                    return;
                }
                //----------------------------------

                //dragging ...
                //find y-diff 
                int ydiff = e.Y - scroll_button.LatestMouseDownY;

                Point pos = scroll_button.Position;

                //if vscroll bar then move only y axis 
                int newYPos = (int)(pos.Y + ydiff);

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

                e.StopPropagation();
            };
            //-------------------------------------------
            //4.
            scroll_button.MouseLeave += (s, e) =>
            {
                if (e.IsDragging)
                {

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

                    e.StopPropagation();
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


        //tempfix here
        internal void ChildNotifyMouseWheel(UIMouseEventArgs e)
        {
            if (e.Delta < 0)
            {   //scroll down
                this.StepSmallToMax();
            }
            else
            {
                //up
                this.StepSmallToMin();
            }

        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            if (e.Delta < 0)
            {   //scroll down
                this.StepSmallToMax();
            }
            else
            {
                //up
                this.StepSmallToMin();
            }
        }
    }
    public enum ScrollBarType
    {
        Vertical,
        Horizontal
    }


    class ScrollBarButton : EaseBox
    {


        public ScrollBarButton(int w, int h, ScrollBar owner)
            : base(w, h)
        {
            this.OwnerScrollBar = owner;
        }
        internal ScrollBar OwnerScrollBar
        {
            get;
            set;
        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            this.OwnerScrollBar.ChildNotifyMouseWheel(e);
        }
        public int LatestMouseDownX
        {
            get;
            set;
        }
        public int LatestMouseDownY
        {
            get;
            set;
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.LatestMouseDownX = e.X;
            this.LatestMouseDownY = e.Y;
            base.OnMouseDown(e);
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