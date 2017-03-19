//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public delegate void ScrollBarEvaluator(ScrollBar scBar, out double onePixelFore, out int scrollBoxHeight);
    public class ScrollBar : UIBox
    {
        CustomRenderBox mainBox;
        ScrollBarButton minButton;
        ScrollBarButton maxButton;
        ScrollBarButton scrollButton;
        ScrollBarEvaluator customeScrollBarEvaluator;
        float maxValue;
        float minValue;
        float smallChange;
        float largeChange;
        float scrollValue;
        double onePixelFor = 1;
        protected int minmax_boxHeight = 15;
        const int SCROLL_BOX_SIZE_LIMIT = 10;
        public ScrollBar(int width, int height)
            : base(width, height)
        {
        }
        public override RenderElement CurrentPrimaryRenderElement
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
                    case ScrollBarType.Horizontal:
                        {
                            CreateHScrollbarContent(rootgfx);
                        }
                        break;
                    default:
                        {
                            CreateVScrollbarContent(rootgfx);
                        }
                        break;
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

        public int MinMaxButtonHeight { get { return minmax_boxHeight; } }
        public int ScrollBoxSizeLimit { get { return SCROLL_BOX_SIZE_LIMIT; } }

        public int PhysicalScrollLength
        {
            get
            {
                if (ScrollBarType == ScrollBarType.Vertical)
                {
                    return this.Height - (this.minmax_boxHeight + this.minmax_boxHeight);
                }
                else
                {
                    return this.Width - (this.minmax_boxHeight + this.minmax_boxHeight);
                }
            }
        }



        public void StepSmallToMax()
        {
            if (this.scrollValue + smallChange <= this.MaxValue)
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
            switch (this.ScrollBarType)
            {
                default:
                case ScrollBarType.Vertical:
                    {
                        int thumbPosY = CalculateThumbPosition() + minmax_boxHeight;
                        scrollButton.SetLocation(0, thumbPosY);
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        int thumbPosX = CalculateThumbPosition() + minmax_boxHeight;
                        scrollButton.SetLocation(thumbPosX, 0);
                    }
                    break;
            }
        }


        //--------------------------------------------------------------------------
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);
            //---------------------------------------------------------


            //MinButton
            SetupMinButtonProperties(bgBox);
            //MaxButton
            SetupMaxButtonProperties(bgBox);
            //ScrollButton
            SetupVerticalScrollButtonProperties(bgBox);
            //--------------
            this.mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);
            //---------------------------------------------------------

            //MinButton
            SetupMinButtonProperties(bgBox);
            //MaxButton
            SetupMaxButtonProperties(bgBox);
            //ScrollButton
            SetupHorizontalScrollButtonProperties(bgBox);
            //--------------
            this.mainBox = bgBox;
        }

        //----------------------------------------------------------------------- 
        int CalculateThumbPosition()
        {
            return (int)(this.scrollValue / this.onePixelFor);
        }

        void SetupMinButtonProperties(RenderElement container)
        {
            ScrollBarButton min_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                min_button = new ScrollBarButton(minmax_boxHeight, this.Height, this);
            }
            else
            {
                min_button = new ScrollBarButton(this.Width, minmax_boxHeight, this);
            }
            min_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            min_button.MouseUp += (s, e) => this.StepSmallToMin();
            container.AddChild(min_button);
            this.minButton = min_button;
        }
        void SetupMaxButtonProperties(RenderElement container)
        {
            ScrollBarButton max_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                max_button = new ScrollBarButton(minmax_boxHeight, this.Height, this);
                max_button.SetLocation(this.Width - minmax_boxHeight, 0);
            }
            else
            {
                max_button = new ScrollBarButton(this.Width, minmax_boxHeight, this);
                max_button.SetLocation(0, this.Height - minmax_boxHeight);
            }


            max_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            max_button.MouseUp += (s, e) => this.StepSmallToMax();
            container.AddChild(max_button);
            this.maxButton = max_button;
        }

        //---------------------------------------------------------------------------
        //vertical scrollbar

        public void ReEvaluateScrollBar()
        {
            if (this.scrollButton == null)
            {
                return;
            }
            //-------------------------
            switch (this.ScrollBarType)
            {
                default:
                case ScrollBarType.Vertical:
                    {
                        EvaluateVerticalScrollBarProperties();
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        EvaluateHorizontalScrollBarProperties();
                    }
                    break;
            }
        }
        public void SetCustomScrollBarEvaluator(ScrollBarEvaluator scrollBarEvaluator)
        {
            this.customeScrollBarEvaluator = scrollBarEvaluator;
        }
        void EvaluateVerticalScrollBarProperties()
        {
            int scrollBoxLength = 1;
            //--------------------------
            //if use external evaluator
            if (customeScrollBarEvaluator != null)
            {
                customeScrollBarEvaluator(this, out this.onePixelFor, out scrollBoxLength);
            }
            else
            {
                //--------------------------
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = this.maxValue - this.minValue;
                //2. 
                float physicalScrollLength = this.Height - (this.minmax_boxHeight + this.minmax_boxHeight);
                //3.  
                if (contentLength < physicalScrollLength)
                {
                    int nsteps = (int)Math.Round(contentLength / smallChange);
                    //small change value reflect thumbbox size 
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    scrollBoxLength = (int)((physicalScrollLength * physicalScrollLength) / contentLength);
                    //small change value reflect thumbbox size
                    // scrollBoxLength = (int)(ratio1 * this.SmallChange);
                    //thumbbox should not smaller than minimum limit 
                    if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                    {
                        scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                    else
                    {
                        //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                        //this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                }
            }
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                throw new NotSupportedException();
            }
            else
            {
                //vertical scrollbar
                this.scrollButton.SetSize(
                    this.scrollButton.Width,
                    scrollBoxLength);
                this.InvalidateOuterGraphics();
            }
        }
        void SetupVerticalScrollButtonProperties(RenderElement container)
        {
            var scroll_button = new ScrollBarButton(this.Width, 10, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosY = CalculateThumbPosition() + minmax_boxHeight;
            scroll_button.SetLocation(0, thumbPosY);
            container.AddChild(scroll_button);
            this.scrollButton = scroll_button;
            //----------------------------
            EvaluateVerticalScrollBarProperties();
            //----------------------------
            //3. drag
            scroll_button.MouseDrag += (s, e) =>
            {
                //dragging ...
                //find y-diff   

                Point pos = scroll_button.Position;
                //if vscroll bar then move only y axis 
                int newYPos = (int)(pos.Y + e.DiffCapturedY);
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
        }

        //---------------------------------------------------------------------------
        //horizontal scrollbar
        void EvaluateHorizontalScrollBarProperties()
        {
            int scrollBoxLength = 1;
            //--------------------------
            //if use external evaluator
            if (customeScrollBarEvaluator != null)
            {
                customeScrollBarEvaluator(this, out this.onePixelFor, out scrollBoxLength);
            }
            else
            {
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = this.maxValue - this.minValue;
                //2. 
                float physicalScrollLength = this.Width - (this.minmax_boxHeight + this.minmax_boxHeight);
                //3. 
                //double ratio1 = physicalScrollLength / contentLength;
                //if (contentLength < physicalScrollLength)
                //{
                //    int nsteps = (int)Math.Round(contentLength / smallChange);
                //    //small change value reflect thumbbox size
                //    // thumbBoxLength = (int)(ratio1 * this.SmallChange);
                //    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                //    scrollBoxLength = eachStepLength * 2;
                //    //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                //    //this.onePixelFor = contentLength / (physicalScrollLength);
                //    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                //}
                //else
                //{
                //    scrollBoxLength = (int)(ratio1 * this.SmallChange);
                //    //thumbbox should not smaller than minimum limit 
                //    if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                //    {
                //        scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                //        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                //    }
                //    else
                //    {
                //        float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                //        this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                //    }
                //}
                //3.  
                if (contentLength < physicalScrollLength)
                {
                    int nsteps = (int)Math.Round(contentLength / smallChange);
                    //small change value reflect thumbbox size 
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    scrollBoxLength = (int)((physicalScrollLength * physicalScrollLength) / contentLength);
                    //small change value reflect thumbbox size
                    // scrollBoxLength = (int)(ratio1 * this.SmallChange);
                    //thumbbox should not smaller than minimum limit 
                    if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                    {
                        scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                    else
                    {
                        //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                        //this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                }
            }
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                this.scrollButton.SetSize(
                    scrollBoxLength,
                    this.scrollButton.Height);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        void SetupHorizontalScrollButtonProperties(RenderElement container)
        {
            var scroll_button = new ScrollBarButton(10, this.Height, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosX = CalculateThumbPosition() + minmax_boxHeight;
            scroll_button.SetLocation(thumbPosX, 0);
            container.AddChild(scroll_button);
            this.scrollButton = scroll_button;
            //----------------------------

            EvaluateHorizontalScrollBarProperties();
            //----------------------------
            //3. drag


            scroll_button.MouseDrag += (s, e) =>
            {
                //dragging ...

                Point pos = scroll_button.Position;
                //if vscroll bar then move only y axis 
                int newXPos = (int)(pos.X + e.DiffCapturedX);
                //clamp!
                if (newXPos >= this.Width - (minmax_boxHeight + scrollButton.Width))
                {
                    newXPos = this.Width - (minmax_boxHeight + scrollButton.Width);
                }
                else if (newXPos < minmax_boxHeight)
                {
                    newXPos = minmax_boxHeight;
                }

                //calculate value from position 

                int currentMarkAt = (newXPos - minmax_boxHeight);
                this.scrollValue = (float)(onePixelFor * currentMarkAt);
                newXPos = CalculateThumbPosition() + minmax_boxHeight;
                scroll_button.SetLocation(newXPos, pos.Y);
                if (this.UserScroll != null)
                {
                    this.UserScroll(this, EventArgs.Empty);
                }

                e.StopPropagation();
            };
        }


        //----------------------------------------------------------------------- 
        public void SetupScrollBar(ScrollBarCreationParameters creationParameters)
        {
            this.MaxValue = creationParameters.maximum;
            this.MinValue = creationParameters.minmum;
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
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "scrollbar");
            this.Describe(visitor);
            visitor.EndElement();
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

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "scrollbutton");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }



    public class ScrollingRelation
    {
        ScrollBar scBar;
        IScrollable scrollableSurface;
        public ScrollingRelation(ScrollBar scBar, IScrollable scrollableSurface)
        {
            this.scBar = scBar;
            this.scrollableSurface = scrollableSurface;
            switch (scBar.ScrollBarType)
            {
                case ScrollBarType.Vertical:
                    {
                        SetupVerticalScrollRelation();
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        SetupHorizontalScrollRelation();
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        void SetupVerticalScrollRelation()
        {
            this.scBar.SetCustomScrollBarEvaluator((ScrollBar sc, out double onePixelFor, out int scrollBoxLength) =>
            {
                int physicalScrollLength = sc.PhysicalScrollLength;
                onePixelFor = 1;
                scrollBoxLength = 1;
                //1. 
                int contentLength = scrollableSurface.DesiredHeight;
                if (contentLength == 0)
                {
                    return;
                }
                scrollBoxLength = (int)((physicalScrollLength * scrollableSurface.ViewportHeight) / contentLength);
                if (scrollBoxLength < sc.ScrollBoxSizeLimit)
                {
                    scrollBoxLength = sc.ScrollBoxSizeLimit;
                    onePixelFor = (double)contentLength / (double)(physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    onePixelFor = (double)contentLength / (double)physicalScrollLength;
                }

                //temp fix 
                sc.MaxValue = (contentLength > scrollableSurface.ViewportHeight) ?
                    contentLength - scrollableSurface.ViewportHeight :
                    0;
            });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            scBar.MaxValue = scrollableSurface.DesiredHeight;
            scBar.ReEvaluateScrollBar();
            scrollableSurface.LayoutFinished += (s, e) =>
            {
                scBar.MaxValue = scrollableSurface.DesiredHeight;
                scBar.ReEvaluateScrollBar();
            };
            scBar.UserScroll += (s, e) =>
            {
                scrollableSurface.SetViewport(scrollableSurface.ViewportX, (int)scBar.ScrollValue);
            };
        }
        void SetupHorizontalScrollRelation()
        {
            this.scBar.SetCustomScrollBarEvaluator((ScrollBar sc, out double onePixelFor, out int scrollBoxLength) =>
            {
                //horizontal scroll bar
                int physicalScrollLength = sc.PhysicalScrollLength;
                onePixelFor = 1;
                scrollBoxLength = 1;
                //1. 
                int contentLength = scrollableSurface.DesiredWidth;
                if (contentLength == 0) return;
                scrollBoxLength = (int)((physicalScrollLength * scrollableSurface.ViewportWidth) / contentLength);
                if (scrollBoxLength < sc.ScrollBoxSizeLimit)
                {
                    scrollBoxLength = sc.ScrollBoxSizeLimit;
                    onePixelFor = (double)contentLength / (double)(physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    onePixelFor = (double)contentLength / (double)physicalScrollLength;
                }
                //sc.MaxValue = contentLength - scrollableSurface.ViewportWidth;
                sc.MaxValue = (contentLength > scrollableSurface.ViewportWidth) ?
                    contentLength - scrollableSurface.ViewportWidth :
                    0;
            });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            scBar.MaxValue = scrollableSurface.DesiredWidth;
            scBar.ReEvaluateScrollBar();
            scrollableSurface.LayoutFinished += (s, e) =>
            {
                scBar.MaxValue = scrollableSurface.DesiredWidth;
                scBar.ReEvaluateScrollBar();
            };
            scBar.UserScroll += (s, e) =>
            {
                scrollableSurface.SetViewport((int)scBar.ScrollValue, scrollableSurface.ViewportY);
            };
        }
    }
}