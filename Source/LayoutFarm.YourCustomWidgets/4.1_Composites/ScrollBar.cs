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
                    case CustomWidgets.ScrollBarType.Horizontal:
                        {
                            CreateHScrollbarContent(rootgfx);
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
            switch (this.ScrollBarType)
            {
                default:
                case ScrollBarType.Vertical:
                    {
                        int thumbPosY = CalculateThumbPosition() + minmax_boxHeight;
                        scrollButton.SetLocation(0, thumbPosY);
                    } break;
                case ScrollBarType.Horizontal:
                    {
                        int thumbPosX = CalculateThumbPosition() + minmax_boxHeight;
                        scrollButton.SetLocation(thumbPosX, 0);
                    } break;
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
            SetupVerticalScrollButtonProperties(plain);
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
            SetupHorizontalScrollButtonProperties(plain);
            //--------------
            this.mainBox = bgBox;
        }

        //----------------------------------------------------------------------- 
        int CalculateThumbPosition()
        {
            return (int)(this.scrollValue / this.onePixelFor);
        }

        void SetupMinButtonProperties(PlainLayer plain)
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
            plain.AddUI(min_button);
            this.minButton = min_button;
        }
        void SetupMaxButtonProperties(PlainLayer plain)
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
            plain.AddUI(max_button);
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
                case CustomWidgets.ScrollBarType.Vertical:
                    {
                        EvaluateVerticalScrollBarProperties();
                    } break;
                case CustomWidgets.ScrollBarType.Horizontal:
                    {
                        EvaluateHorizontalScrollBarProperties();
                    } break;
            }


        }
        void EvaluateVerticalScrollBarProperties()
        {
            //calculate scroll length ratio
            //scroll button height is ratio with real scroll length
            float contentLength = this.maxValue - this.minValue;
            //2. 
            float physicalScrollLength = this.Height - (this.minmax_boxHeight + this.minmax_boxHeight);
            //3. 
            double ratio1 = physicalScrollLength / contentLength;
            int scrollBoxLength = 1; 

            if (contentLength < physicalScrollLength)
            {
                int nsteps = (int)Math.Round(contentLength / smallChange);

                //small change value reflect thumbbox size
                // thumbBoxLength = (int)(ratio1 * this.SmallChange);
                int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                scrollBoxLength = eachStepLength * 2;
                //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                //this.onePixelFor = contentLength / (physicalScrollLength);
                this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
            }
            else
            {
                //small change value reflect thumbbox size
                scrollBoxLength = (int)(ratio1 * this.SmallChange);
                //thumbbox should not smaller than minimum limit 
                if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                {
                    scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                    this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                }

            }

            if (this.ScrollBarType == CustomWidgets.ScrollBarType.Horizontal)
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

                //this.scrollButton.InvalidateOuterGraphics();

            }
        }
        void SetupVerticalScrollButtonProperties(PlainLayer plain)
        {
            var scroll_button = new ScrollBarButton(this.Width, 10, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosY = CalculateThumbPosition() + minmax_boxHeight;
            scroll_button.SetLocation(0, thumbPosY);
            plain.AddUI(scroll_button);
            this.scrollButton = scroll_button;
            //----------------------------
            EvaluateVerticalScrollBarProperties();
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

        //---------------------------------------------------------------------------
        //horizontal scrollbar
        void EvaluateHorizontalScrollBarProperties()
        {
            //calculate scroll length ratio
            //scroll button height is ratio with real scroll length
            float contentLength = this.maxValue - this.minValue;
            //2. 
            float physicalScrollLength = this.Width - (this.minmax_boxHeight + this.minmax_boxHeight);
            //3. 
            double ratio1 = physicalScrollLength / contentLength;
            int scrollBoxLength = 1;
            if (contentLength < physicalScrollLength)
            {
                int nsteps = (int)Math.Round(contentLength / smallChange);

                //small change value reflect thumbbox size
                // thumbBoxLength = (int)(ratio1 * this.SmallChange);
                int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                scrollBoxLength = eachStepLength * 2;
                //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                //this.onePixelFor = contentLength / (physicalScrollLength);
                this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
            }
            else
            {
                scrollBoxLength = (int)(ratio1 * this.SmallChange);
                //thumbbox should not smaller than minimum limit 
                if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                {
                    scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                    this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                }

            }

            if (this.ScrollBarType == CustomWidgets.ScrollBarType.Horizontal)
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
        void SetupHorizontalScrollButtonProperties(PlainLayer plain)
        {
            var scroll_button = new ScrollBarButton(10, this.Height, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosX = CalculateThumbPosition() + minmax_boxHeight;
            scroll_button.SetLocation(thumbPosX, 0);
            plain.AddUI(scroll_button);
            this.scrollButton = scroll_button;
            //----------------------------

            EvaluateHorizontalScrollBarProperties();
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
                //find x-diff 
                int xdiff = e.X - scroll_button.LatestMouseDownX;

                Point pos = scroll_button.Position;

                //if vscroll bar then move only y axis 
                int newXPos = (int)(pos.X + xdiff);

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
            //-------------------------------------------
            //4.
            scroll_button.MouseLeave += (s, e) =>
            {
                if (e.IsDragging)
                {

                    Point pos = scroll_button.Position;
                    //if vscroll bar then move only y axis 
                    int newXPos = (int)(pos.X + e.XDiff);

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
            if (scBar.ScrollBarType == ScrollBarType.Horizontal)
            {
                scBar.UserScroll += (s, e) =>
                {
                    panel.SetViewport((int)scBar.ScrollValue, panel.ViewportY);
                };
            }
            else
            {
                scBar.UserScroll += (s, e) =>
                {
                    panel.SetViewport(panel.ViewportX, (int)scBar.ScrollValue);
                };
            }
        }
        public ScrollingRelation(ScrollBar scBar, LayoutFarm.CustomWidgets.Panel panel)
        {
            this.scBar = scBar;
            this.panel = panel;

            panel.LayoutFinished += (s, e) =>
            {
                scBar.MaxValue = panel.DesiredHeight;
                scBar.ReEvaluateScrollBar();

            };


            if (scBar.ScrollBarType == ScrollBarType.Horizontal)
            {
                scBar.UserScroll += (s, e) =>
                {
                    panel.SetViewport((int)scBar.ScrollValue, panel.ViewportY);
                };
            }
            else
            {
                scBar.UserScroll += (s, e) =>
                {
                    panel.SetViewport(panel.ViewportX, (int)scBar.ScrollValue);
                };
            }
        }
    }

}