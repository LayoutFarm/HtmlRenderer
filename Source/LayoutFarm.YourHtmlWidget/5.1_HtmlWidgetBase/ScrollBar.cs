//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
namespace LayoutFarm.HtmlWidgets
{
    public delegate void ScrollBarEvaluator(ScrollBar scBar, out double onePixelFore, out int scrollBoxHeight);
    public class ScrollBar : AbstractBox
    {
        CustomRenderBox _mainBox;
        ScrollBarButton _minButton;
        ScrollBarButton _maxButton;
        ScrollBarButton _scrollButton;
        ScrollBarEvaluator _customeScrollBarEvaluator;
        float _maxValue;
        float _minValue;
        float _smallChange;
        float _largeChange;
        float _scrollValue;
        double _onePixelFor = 1;
        protected int _minmax_boxHeight = 15;
        const int SCROLL_BOX_SIZE_LIMIT = 10;
        public ScrollBar(int width, int height)
            : base(width, height)
        {
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _mainBox;
        protected override bool HasReadyRenderElement => _mainBox != null;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_mainBox == null)
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
            return _mainBox;
        }
        public ScrollBarType ScrollBarType
        {
            get;
            set;
        }
        //--------------------------------------------------------------------------

        public int MinMaxButtonHeight => _minmax_boxHeight;
        public int ScrollBoxSizeLimit => SCROLL_BOX_SIZE_LIMIT;
        public int PhysicalScrollLength
        {
            get
            {
                if (ScrollBarType == ScrollBarType.Vertical)
                {
                    return this.Height - (_minmax_boxHeight + _minmax_boxHeight);
                }
                else
                {
                    return this.Width - (_minmax_boxHeight + _minmax_boxHeight);
                }
            }
        }
        public void StepSmallToMax()
        {
            if (_scrollValue + _smallChange <= this.MaxValue)
            {
                _scrollValue = _scrollValue + _smallChange;
            }
            else
            {
                _scrollValue = this.MaxValue;
            }
            //---------------------------
            //update visual presentation             
            UpdateScrollButtonPosition();


            this.UserScroll?.Invoke(this, EventArgs.Empty);

        }
        public void StepSmallToMin()
        {
            if (_scrollValue - _smallChange >= _minValue)
            {
                _scrollValue = _scrollValue - _smallChange;
            }
            else
            {
                _scrollValue = this.MinValue;
            }
            //---------------------------
            //update visual presentation   
            UpdateScrollButtonPosition();

            this.UserScroll?.Invoke(this, EventArgs.Empty);

        }

        void UpdateScrollButtonPosition()
        {
            switch (this.ScrollBarType)
            {
                default:
                case ScrollBarType.Vertical:
                    {
                        int thumbPosY = CalculateThumbPosition() + _minmax_boxHeight;
                        _scrollButton.SetLocation(0, thumbPosY);
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        int thumbPosX = CalculateThumbPosition() + _minmax_boxHeight;
                        _scrollButton.SetLocation(thumbPosX, 0);
                    }
                    break;
            }
        }


        //--------------------------------------------------------------------------
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificWidthAndHeight = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);
            //MinButton
            SetupMinButtonProperties(bgBox);
            //MaxButton
            SetupMaxButtonProperties(bgBox);
            //ScrollButton
            SetupVerticalScrollButtonProperties(bgBox);
            //--------------
            _mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificWidthAndHeight = true;
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
            _mainBox = bgBox;
        }

        //----------------------------------------------------------------------- 
        //
        int CalculateThumbPosition() => (int)(_scrollValue / _onePixelFor);
        //
        void SetupMinButtonProperties(RenderElement container)
        {
            ScrollBarButton min_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                min_button = new ScrollBarButton(_minmax_boxHeight, this.Height, this);
            }
            else
            {
                min_button = new ScrollBarButton(this.Width, _minmax_boxHeight, this);
            }
            min_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            min_button.MouseUp += (s, e) => this.StepSmallToMin();
            container.AddChild(min_button);
            _minButton = min_button;
        }
        void SetupMaxButtonProperties(RenderElement container)
        {
            ScrollBarButton max_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                max_button = new ScrollBarButton(_minmax_boxHeight, this.Height, this);

                max_button.SetLocation(this.Width - _minmax_boxHeight, 0);
            }
            else
            {
                max_button = new ScrollBarButton(this.Width, _minmax_boxHeight, this);
                max_button.SetLocation(0, this.Height - _minmax_boxHeight);
            }


            max_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            max_button.MouseUp += (s, e) => this.StepSmallToMax();
            container.AddChild(max_button);
            _maxButton = max_button;
        }

        //---------------------------------------------------------------------------
        //vertical scrollbar

        internal void ReEvaluateScrollBar()
        {
            if (_scrollButton == null)
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
            _customeScrollBarEvaluator = scrollBarEvaluator;
        }
        void EvaluateVerticalScrollBarProperties()
        {
            int scrollBoxLength = 1;
            //--------------------------
            //if use external evaluator
            if (_customeScrollBarEvaluator != null)
            {
                _customeScrollBarEvaluator(this, out _onePixelFor, out scrollBoxLength);
            }
            else
            {
                //--------------------------
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = _maxValue - _minValue;
                //2. 
                float physicalScrollLength = this.Height - (_minmax_boxHeight + _minmax_boxHeight);
                //3.  
                if (contentLength < physicalScrollLength)
                {
                    int nsteps = (int)Math.Round(contentLength / _smallChange);
                    //small change value reflect thumbbox size 
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    _onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
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
                        _onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                    else
                    {
                        //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                        //this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                        _onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
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
                _scrollButton.SetSize(
                    _scrollButton.Width,
                    scrollBoxLength);
                this.InvalidateOuterGraphics();
            }
        }
        void SetupVerticalScrollButtonProperties(RenderElement container)
        {
            var scroll_button = new ScrollBarButton(this.Width, 10, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosY = CalculateThumbPosition() + _minmax_boxHeight;
            scroll_button.SetLocation(0, thumbPosY);
            container.AddChild(scroll_button);
            _scrollButton = scroll_button;
            //----------------------------
            EvaluateVerticalScrollBarProperties();
            //----------------------------
            //3. drag
            scroll_button.MouseDrag += (s, e) =>
            {
                //dragging ... 

                Point pos = scroll_button.Position;
                //if vscroll bar then move only y axis 
                int newYPos = (int)(pos.Y + e.DiffCapturedY);
                //clamp!
                if (newYPos >= this.Height - (_minmax_boxHeight + _scrollButton.Height))
                {
                    newYPos = this.Height - (_minmax_boxHeight + _scrollButton.Height);
                }
                else if (newYPos < _minmax_boxHeight)
                {
                    newYPos = _minmax_boxHeight;
                }

                //calculate value from position 

                int currentMarkAt = (newYPos - _minmax_boxHeight);
                _scrollValue = (float)(_onePixelFor * currentMarkAt);
                newYPos = CalculateThumbPosition() + _minmax_boxHeight;
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
            if (_customeScrollBarEvaluator != null)
            {
                _customeScrollBarEvaluator(this, out _onePixelFor, out scrollBoxLength);
            }
            else
            {
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = _maxValue - _minValue;
                //2. 
                float physicalScrollLength = this.Width - (_minmax_boxHeight + _minmax_boxHeight);
                //3. 
                double ratio1 = physicalScrollLength / contentLength;
                if (contentLength < physicalScrollLength)
                {
                    int nsteps = (int)Math.Round(contentLength / _smallChange);
                    //small change value reflect thumbbox size
                    // thumbBoxLength = (int)(ratio1 * this.SmallChange);
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                    //this.onePixelFor = contentLength / (physicalScrollLength);
                    _onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    scrollBoxLength = (int)(ratio1 * this.SmallChange);
                    //thumbbox should not smaller than minimum limit 
                    if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                    {
                        scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                        _onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                    else
                    {
                        float physicalSmallEach = (physicalScrollLength / contentLength) * _smallChange;
                        _onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                    }
                }
            }
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                _scrollButton.SetSize(
                     scrollBoxLength,
                     _scrollButton.Height);
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
            int thumbPosX = CalculateThumbPosition() + _minmax_boxHeight;
            scroll_button.SetLocation(thumbPosX, 0);
            container.AddChild(scroll_button);
            _scrollButton = scroll_button;
            //----------------------------

            EvaluateHorizontalScrollBarProperties();
            //----------------------------
            //3. drag


            scroll_button.MouseDrag += (s, e) =>
            {
                //dragging ...
                //find x-diff                 

                Point pos = scroll_button.Position;
                //if vscroll bar then move only y axis 
                int newXPos = (int)(pos.X + e.DiffCapturedX);
                //clamp!
                if (newXPos >= this.Width - (_minmax_boxHeight + _scrollButton.Width))
                {
                    newXPos = this.Width - (_minmax_boxHeight + _scrollButton.Width);
                }
                else if (newXPos < _minmax_boxHeight)
                {
                    newXPos = _minmax_boxHeight;
                }

                //calculate value from position 

                int currentMarkAt = (newXPos - _minmax_boxHeight);
                _scrollValue = (float)(_onePixelFor * currentMarkAt);
                newXPos = CalculateThumbPosition() + _minmax_boxHeight;
                scroll_button.SetLocation(newXPos, pos.Y);
                //
                this.UserScroll?.Invoke(this, EventArgs.Empty);
                //
                e.StopPropagation();
            };
        }


        //----------------------------------------------------------------------- 
        public void SetupScrollBar(ScrollBarCreationParameters creationParameters)
        {
            _maxValue = creationParameters.maximum;
            _minValue = creationParameters.minmum;
        }
        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }
        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }
        public float SmallChange
        {
            get => _smallChange;
            set => _smallChange = value;
        }
        public float LargeChange
        {
            get => _largeChange;
            set => _largeChange = value;
        }
        public float ScrollValue
        {
            get => _scrollValue;
            set => _scrollValue = value;
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
            //walk to control
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


    class ScrollBarButton : AbstractBox
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
        ScrollBar _scBar;
        IScrollable _scrollableSurface;
        public ScrollingRelation(ScrollBar scBar, IScrollable scrollableSurface)
        {
            _scBar = scBar;
            _scrollableSurface = scrollableSurface;
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
            _scBar.SetCustomScrollBarEvaluator((ScrollBar sc, out double onePixelFor, out int scrollBoxLength) =>
            {
                int physicalScrollLength = sc.PhysicalScrollLength;
                onePixelFor = 1;
                scrollBoxLength = 1;
                //1. 
                int contentLength = _scrollableSurface.InnerHeight;
                if (contentLength == 0)
                {
                    contentLength = 1;
                }
                scrollBoxLength = (int)((physicalScrollLength * _scrollableSurface.ViewportHeight) / contentLength);
                if (scrollBoxLength < sc.ScrollBoxSizeLimit)
                {
                    scrollBoxLength = sc.ScrollBoxSizeLimit;
                    onePixelFor = (double)contentLength / (double)(physicalScrollLength - (scrollBoxLength));
                }
                else
                {
                    onePixelFor = (double)contentLength / (double)physicalScrollLength;
                }
                sc.MaxValue = contentLength - _scrollableSurface.ViewportHeight;
            });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            _scBar.MaxValue = _scrollableSurface.InnerHeight;
            _scBar.ReEvaluateScrollBar();
            _scrollableSurface.ViewportChanged += (s, e) =>
            {
                if (e.Kind == ViewportChangedEventArgs.ChangeKind.LayoutDone)
                {
                    _scBar.MaxValue = _scrollableSurface.InnerHeight;
                    _scBar.ReEvaluateScrollBar();
                }
            };
            _scBar.UserScroll += (s, e) =>
            {
                _scrollableSurface.SetViewport(_scrollableSurface.ViewportLeft, (int)_scBar.ScrollValue, _scBar);
            };
        }
        void SetupHorizontalScrollRelation()
        {
            _scBar.SetCustomScrollBarEvaluator((ScrollBar sc, out double onePixelFor, out int scrollBoxLength) =>
           {
               //horizontal scroll bar
               int physicalScrollLength = sc.PhysicalScrollLength;
               onePixelFor = 1;
               scrollBoxLength = 1;
               //1. 
               int contentLength = _scrollableSurface.InnerWidth;
               scrollBoxLength = (int)((physicalScrollLength * _scrollableSurface.ViewportWidth) / contentLength);
               if (scrollBoxLength < sc.ScrollBoxSizeLimit)
               {
                   scrollBoxLength = sc.ScrollBoxSizeLimit;
                   onePixelFor = (double)contentLength / (double)(physicalScrollLength - scrollBoxLength);
               }
               else
               {
                   onePixelFor = (double)contentLength / (double)physicalScrollLength;
               }
               sc.MaxValue = contentLength - _scrollableSurface.ViewportWidth;
           });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            _scBar.MaxValue = _scrollableSurface.InnerWidth;
            _scBar.ReEvaluateScrollBar();
            _scrollableSurface.ViewportChanged += (s, e) =>
            {
                if (e.Kind == ViewportChangedEventArgs.ChangeKind.LayoutDone)
                {
                    _scBar.MaxValue = _scrollableSurface.InnerWidth;
                    _scBar.ReEvaluateScrollBar();
                }
            };
            _scBar.UserScroll += (s, e) =>
            {
                _scrollableSurface.SetViewport((int)_scBar.ScrollValue, _scrollableSurface.ViewportTop, _scBar);
            };
        }
    }
}