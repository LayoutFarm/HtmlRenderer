//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public sealed class SpacePart
    {
        NamedSpaceContainerOverlapMode overlapMode;
        SpaceName spaceName;
        UIBox spaceContent;
        NinespaceController ownerDockspaceController;
        int spaceWidth;
        int spaceHeight;
        int spaceX;
        int spaceY;
        bool hidden;
        bool hasCalculatedSize;
        internal SpacePart(NinespaceController ownerDockspaceController, int spaceWidth, int spaceHeight, SpaceName docSpacename)
        {
            this.ownerDockspaceController = ownerDockspaceController;
            this.spaceWidth = spaceWidth;
            this.spaceHeight = spaceHeight;
            this.spaceName = docSpacename;
        }
        public NinespaceController ParentSpaceSet
        {
            get
            {
                return this.ownerDockspaceController;
            }
        }
        public UIBox Content
        {
            get
            {
                return this.spaceContent;
            }
            set
            {
                this.spaceContent = value;
            }
        }
        public SpaceName SpaceName
        {
            get
            {
                return this.spaceName;
            }
        }
        public NamedSpaceContainerOverlapMode OverlapMode
        {
            get
            {
                return this.overlapMode;
            }
            set
            {
                this.overlapMode = value;
            }
        }
        public int X
        {
            get
            {
                return this.spaceX;
            }
        }
        public int Y
        {
            get
            {
                return this.spaceY;
            }
        }
        public int Width
        {
            get
            {
                return this.spaceWidth;
            }
        }
        public int Height
        {
            get
            {
                return this.spaceHeight;
            }
        }
        public bool Visible
        {
            get
            {
                return !hidden;
            }
        }
        public int Right
        {
            get
            {
                return this.spaceX + this.spaceWidth;
            }
        }
        public int Bottom
        {
            get
            {
                return this.spaceY + this.spaceHeight;
            }
        }

        public int DesiredHeight
        {
            get { return this.Height; }//temp
        }
        public int DesiredWidth
        {
            get { return this.Width; }//temp
        }


        public void SetSize(int w, int h)
        {
            this.spaceWidth = w;
            this.spaceHeight = h;
        }
        public void SetLocation(int x, int y)
        {
            this.spaceX = x;
            this.spaceY = y;
        }
        public void SetBound(int x, int y, int w, int h)
        {
            this.spaceX = x;
            this.spaceY = y;
            this.spaceWidth = w;
            this.spaceHeight = h;
            var uiContent = this.Content;
            if (uiContent != null)
            {
                uiContent.SetBounds(x, y, w, h);
            }
        }
        public void ArrangeContent()
        {
            var uiContent = this.Content;
            if (uiContent != null)
            {
                uiContent.PerformContentLayout();
            }
        }
        public void CalculateContentSize()
        {
            hasCalculatedSize = true;
        }
        public bool HasCalculateSize
        {
            get { return this.hasCalculatedSize; }
        }
        //public void InvalidateArrangeStatus()
        //{
        //}

#if DEBUG
        public override string ToString()
        {
            return "docspace:" + SpaceName + " " + base.ToString();
        }
#endif

    }
}