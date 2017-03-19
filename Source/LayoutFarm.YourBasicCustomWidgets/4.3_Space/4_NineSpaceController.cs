//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.UI
{
    public abstract class NinespaceController
    {
        //----------------------------
        //for five and nine space
        protected const int C = 0;
        protected const int L = 1;
        protected const int R = 2;
        protected const int T = 3;
        protected const int B = 4;
        protected const int LT = 5;
        protected const int LB = 6;
        protected const int RT = 7;
        protected const int RB = 8;
        //----------------------------
        protected const int FOURSPACE_LT = 0;
        protected const int FOURSPACE_LB = 1;
        protected const int FOURSPACE_RT = 2;
        protected const int FOURSPACE_RB = 3;
        protected SpacePart[] spaces;
        //from user intention value
        protected int topSpaceHeight = 1;
        protected int bottomSpaceHeight = 1;
        protected int leftSpaceWidth = 1;
        protected int rightSpaceWidth = 1;
        protected int centerSpaceWidth = -1;
        protected int sizeW;
        protected int sizeH;
        protected SpaceConcept dockSpaceConcept = SpaceConcept.FiveSpace;
        protected UIBox myOwner;
        public NinespaceController(UIBox owner, SpaceConcept initConcept)
        {
            this.myOwner = owner;
            this.dockSpaceConcept = initConcept;
            switch (initConcept)
            {
                case SpaceConcept.NineSpaceFree:
                case SpaceConcept.NineSpace:
                    {
                        spaces = new SpacePart[9];
                    }
                    break;
                default:
                    {
                        spaces = new SpacePart[5];
                    }
                    break;
            }
            this.sizeH = owner.Height;
            this.sizeW = owner.Width;
        }

        protected SpacePart InitSpace(SpaceName name)
        {
            //only call from ctor?
            SpacePart dockspace = new SpacePart(this, 10, 10, name);
            return dockspace;
        }
        public UIBox Owner
        {
            get
            {
                return this.myOwner;
            }
        }

        public bool ControlChildPosition
        {
            get
            {
                return true;
            }
        }



#if DEBUG
        public string dbugGetLinkInfo()
        {
            return this.ToString();
        }
#endif
        internal SpacePart[] GetAllSpaces()
        {
            return spaces;
        }
        public IEnumerable<UIElement> GetVisualElementIter()
        {
            for (int i = spaces.Length - 1; i >= 0; --i)
            {
                SpacePart sp = spaces[i];
                if (sp != null)
                {
                    yield return sp.Content;
                }
            }
        }
        public IEnumerable<UIElement> GetVisualElementReverseIter()
        {
            for (int i = spaces.Length - 1; i >= 0; --i)
            {
                SpacePart sp = spaces[i];
                if (sp != null)
                {
                    yield return sp.Content;
                }
            }
        }
        public SpaceConcept SpaceConcept
        {
            get
            {
                return dockSpaceConcept;
            }
            set
            {
                dockSpaceConcept = value;
                //TODO: change concept?

            }
        }

        /// <summary>
        /// left area of 2,3,5,9 space
        /// </summary>
        public SpacePart LeftSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceHorizontal:
                    case SpaceConcept.ThreeSpaceHorizontal:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return spaces[L];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// right area of 2,3,5,9 space
        /// </summary>
        public SpacePart RightSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceHorizontal:
                    case SpaceConcept.ThreeSpaceHorizontal:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return spaces[R];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// top area of 2,3,5,9 space
        /// </summary>
        public SpacePart TopSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceVertical:
                    case SpaceConcept.ThreeSpaceVertical:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return spaces[T];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// bottom area of 2,3,5,9 space
        /// </summary>
        public SpacePart BottomSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceVertical:
                    case SpaceConcept.ThreeSpaceVertical:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return spaces[B];
                        }
                }
                return null;
            }
        }
        /// <summary>
        /// center area of  3,5,9 space
        /// </summary>
        public SpacePart CenterSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.ThreeSpaceVertical:
                    case SpaceConcept.ThreeSpaceHorizontal:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return spaces[C];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart LeftTopSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:
                        {
                            return spaces[FOURSPACE_LT];
                        }
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                        {
                            return spaces[LT];
                        }
                }
                return null;
            }
        }
        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart RightTopSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:
                        {
                            return spaces[FOURSPACE_RT];
                        }
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.NineSpace:
                        {
                            return spaces[RT];
                        }
                }
                return null;
            }
        }
        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart LeftBottomSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:
                        {
                            return spaces[FOURSPACE_LB];
                        }
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                        {
                            return spaces[LB];
                        }
                }
                return null;
            }
        }
        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart RightBottomSpace
        {
            get
            {
                switch (dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:
                        {
                            return spaces[FOURSPACE_RB];
                        }
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                        {
                            return spaces[RB];
                        }
                }
                return null;
            }
        }

        public int TopSpaceHeight
        {
            get
            {
                return topSpaceHeight;
            }
        }
        protected UIBox OwnerVisualElement
        {
            get
            {
                return this.myOwner;
            }
        }
        public void SetTopSpaceHeight(int value)
        {
            if (dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value > OwnerVisualElement.Height - 2)
                {
                    value = OwnerVisualElement.Height - 2;
                }
                topSpaceHeight = value;
                if (spaces[C] == null)
                {
                    bottomSpaceHeight = OwnerVisualElement.Height - topSpaceHeight;
                }
                else
                {
                    if (topSpaceHeight >= OwnerVisualElement.Height - bottomSpaceHeight - 1)
                    {
                        bottomSpaceHeight = OwnerVisualElement.Height - topSpaceHeight - 1;
                    }
                }
            }
            else
            {
                if (value > OwnerVisualElement.Height - 1)
                {
                    value = OwnerVisualElement.Height - 1;
                }
                topSpaceHeight = value;
                bottomSpaceHeight = OwnerVisualElement.Height - topSpaceHeight;
            }
            this.HasSpecificTopSpaceHeight = true;
            this.InvalidateArrangementInAllDockSpaces();
            ArrangeAllSpaces();
        }

        public int BottomSpaceHeight
        {
            get
            {
                return bottomSpaceHeight;
            }
        }
        public void SetBottomSpaceHeight(int value)
        {
            if (dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value > OwnerVisualElement.Height - 2)
                {
                    value = OwnerVisualElement.Height - 2;
                }
                bottomSpaceHeight = value;
                if (spaces[C] == null)
                {
                    topSpaceHeight = OwnerVisualElement.Height - bottomSpaceHeight;
                }
                else
                {
                    if (topSpaceHeight >= OwnerVisualElement.Height - bottomSpaceHeight - 1)
                    {
                        topSpaceHeight = OwnerVisualElement.Height - bottomSpaceHeight - 1;
                    }
                }
            }
            else
            {
                if (value > OwnerVisualElement.Height - 1)
                {
                    value = OwnerVisualElement.Height - 1;
                }
                bottomSpaceHeight = value;
                topSpaceHeight = OwnerVisualElement.Height - bottomSpaceHeight;
            }
            this.HasSpecificBottomSpaceHeight = true;
            this.InvalidateArrangementInAllDockSpaces();
            ArrangeAllSpaces();
        }
        public bool HasSpecificLeftSpaceWidth
        {
            get;
            set;
        }
        public bool HasSpecificCenterSpaceWidth
        {
            get;
            set;
        }
        public bool HasSpecificRightSpaceWidth
        {
            get;
            set;
        }
        public bool HasSpecificTopSpaceHeight
        {
            get;
            set;
        }
        public bool HasSpecificBottomSpaceHeight
        {
            get;
            set;
        }

        public int LeftSpaceWidth
        {
            get
            {
                return leftSpaceWidth;
            }
        }
        public void SetLeftSpaceWidth(int value)
        {
            if (dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value > OwnerVisualElement.Width - 2)
                {
                    value = OwnerVisualElement.Width - 2;
                }
                leftSpaceWidth = value;
                if (spaces[C] == null)
                {
                    rightSpaceWidth = OwnerVisualElement.Width - leftSpaceWidth;
                }
                else
                {
                    if (leftSpaceWidth >= OwnerVisualElement.Width - rightSpaceWidth - 1)
                    {
                        rightSpaceWidth = OwnerVisualElement.Width - leftSpaceWidth - 1;
                    }
                }
            }
            else
            {
                if (value > OwnerVisualElement.Width - 1)
                {
                    value = OwnerVisualElement.Width - 1;
                }
                leftSpaceWidth = value;
                rightSpaceWidth = OwnerVisualElement.Width - leftSpaceWidth;
            }

#if DEBUG

            //this.dbugVRoot.dbug_PushLayoutTraceMessage("^Set LeftSpaceWidth=" + value);
#endif
            this.HasSpecificLeftSpaceWidth = true;
            this.InvalidateArrangementInAllDockSpaces();
            this.ArrangeAllSpaces();
        }

        public int RightSpaceWidth
        {
            get
            {
                return rightSpaceWidth;
            }
        }
        public void SetRightSpaceWidth(int value)
        {
            if (dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value >= OwnerVisualElement.Width - 2)
                {
                    value = OwnerVisualElement.Width - 2;
                }
                rightSpaceWidth = value;
                if (spaces[C] == null)
                {
                    //if no centerspace ,then use right space width
                    leftSpaceWidth = OwnerVisualElement.Width - rightSpaceWidth;
                }
                else
                {
                    if (this.leftSpaceWidth > OwnerVisualElement.Width - rightSpaceWidth - 1)
                    {
                        leftSpaceWidth = OwnerVisualElement.Width - rightSpaceWidth - 1;
                    }
                }
            }
            else
            {
                if (value >= OwnerVisualElement.Width - 1)
                {
                    value = OwnerVisualElement.Width - 1;
                }

                rightSpaceWidth = value;
                leftSpaceWidth = OwnerVisualElement.Width - rightSpaceWidth;
            }

            this.HasSpecificRightSpaceWidth = true;
            this.InvalidateArrangementInAllDockSpaces();
            ArrangeAllSpaces();
        }

        public int CenterSpaceWidth
        {
            get
            {
                return centerSpaceWidth;
            }
        }
        public void SetCenterSpaceWidth(int value)
        {
            this.centerSpaceWidth = value;
            if (value > -1)
            {
                this.HasSpecificCenterSpaceWidth = true;
                this.InvalidateArrangementInAllDockSpaces();
                ArrangeAllSpaces();
            }
        }

        public virtual void TopDownReCalculateContentSize()
        {
#if DEBUG

#endif

            for (int i = spaces.Length - 1; i > -1; i--)
            {
                var docspace = spaces[i];
                if (docspace != null)
                {
                    if (!docspace.HasCalculateSize)
                    {
                        docspace.CalculateContentSize();
                    }
                    else
                    {
                        //contentArrVisitor.dbug_WriteInfo("SKIP " + docspace.dbug_FullElementDescription);
#if DEBUG
                        //vinv.dbug_WriteInfo(dbugVisitorMessage.SKIP, docspace);
#endif
                    }
                }
            }
            //---------------------------------------------------------

            if (this.dockSpaceConcept == SpaceConcept.NineSpaceFree)
            {
                int maxWidth = CalculateTotalFreeSpacesDesiredWidth();
                int maxHeight = CalculateTotalFreeSpacesDesiredHeight();
            }
            else
            {
                int maxWidth = CalculateTotalDockSpaceDesiredWidth();
                int maxHeight = CalculateTotalDockSpaceDesiredHeight();
            }
            //---------------------------------------------------------
#if DEBUG
            //vinv.dbug_ExitLayerReCalculateContent();
#endif

        }

        int CalculateTotalFreeSpacesDesiredWidth()
        {
            int maxWidth = 0;
            int w = CalculateTotalFreeSpacesDesiredWidth(spaces[LT], spaces[T], spaces[RT]);
            maxWidth = w;
            w = CalculateTotalFreeSpacesDesiredWidth(spaces[L], spaces[C], spaces[R]);
            if (w > maxWidth)
            {
                maxWidth = w;
            }
            w = CalculateTotalFreeSpacesDesiredWidth(spaces[LB], spaces[B], spaces[RB]);
            if (w > maxWidth)
            {
                maxWidth = w;
            }
            return maxWidth;
        }
        int CalculateTotalDockSpaceDesiredWidth()
        {
            int maxWidth = 0;
            switch (this.dockSpaceConcept)
            {
                case SpaceConcept.FiveSpace:
                    {
                        int w = spaces[T].DesiredWidth;
                        maxWidth = w;
                        w = CalculateTotalDockSpaceDesiredWidth(spaces[L], spaces[C], spaces[R]);
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        w = spaces[B].DesiredWidth;
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        return maxWidth;
                    }
                case SpaceConcept.FourSpace:
                    {
                        throw new NotImplementedException();
                    }
                default:
                    {
                        int w = CalculateTotalDockSpaceDesiredWidth(spaces[LT], spaces[T], spaces[RT]);
                        maxWidth = w;
                        w = CalculateTotalDockSpaceDesiredWidth(spaces[L], spaces[C], spaces[R]);
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        w = CalculateTotalDockSpaceDesiredWidth(spaces[LB], spaces[B], spaces[RB]);
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        return maxWidth;
                    }
            }
        }
        int CalculateTotalDockSpaceDesiredHeight()
        {
            switch (this.dockSpaceConcept)
            {
                case SpaceConcept.FiveSpace:
                    {
                        int maxHeight = 0;
                        int h = spaces[L].DesiredHeight;
                        maxHeight = h;
                        h = CalculateTotalDockSpaceDesiredHeight(spaces[T], spaces[C], spaces[B]);
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        h = spaces[R].DesiredHeight;
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        return maxHeight;
                    }
                default:
                    {
                        int maxHeight = 0;
                        int h = CalculateTotalDockSpaceDesiredHeight(spaces[LT], spaces[L], spaces[LB]);
                        maxHeight = h;
                        h = CalculateTotalDockSpaceDesiredHeight(spaces[T], spaces[C], spaces[B]);
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        h = CalculateTotalDockSpaceDesiredHeight(spaces[RT], spaces[R], spaces[RB]);
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        return maxHeight;
                    }
            }
        }
        static int CalculateTotalDockSpaceDesiredWidth(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int total = 0;
            if (bx1 != null)
            {
                total += bx1.DesiredWidth;
            }
            if (bx2 != null)
            {
                total += bx2.DesiredWidth;
            }
            if (bx3 != null)
            {
                total += bx3.DesiredWidth;
            }
            return total;
        }
        static int CalculateTotalDockSpaceDesiredHeight(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int total = 0;
            if (bx1 != null)
            {
                total += bx1.DesiredHeight;
            }
            if (bx2 != null)
            {
                total += bx2.DesiredHeight;
            }
            if (bx3 != null)
            {
                total += bx3.DesiredHeight;
            }
            return total;
        }
        int CalculateTotalFreeSpacesDesiredHeight()
        {
            int maxHeight = 0;
            int h = CalculateTotalFreeSpacesDesiredHeight(spaces[LT], spaces[T], spaces[RT]);
            maxHeight = h;
            h = CalculateTotalFreeSpacesDesiredHeight(spaces[L], spaces[C], spaces[R]);
            if (h > maxHeight)
            {
                maxHeight = h;
            }
            h = CalculateTotalFreeSpacesDesiredHeight(spaces[LB], spaces[B], spaces[RB]);
            if (h > maxHeight)
            {
                maxHeight = h;
            }
            return maxHeight;
        }
        static int CalculateTotalFreeSpacesDesiredWidth(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int totalWidth = 0;
            if (bx1 != null)
            {
                switch (bx1.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalWidth += (bx1.DesiredWidth / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalWidth += bx1.DesiredWidth;
                        }
                        break;
                }
            }
            if (bx2 != null)
            {
                //center not care overlapping
                totalWidth += bx2.DesiredWidth;
            }
            if (bx3 != null)
            {
                switch (bx3.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalWidth += (bx3.DesiredWidth / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalWidth += bx3.DesiredWidth;
                        }
                        break;
                }
            }
            return totalWidth;
        }
        static int CalculateTotalFreeSpacesDesiredHeight(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int totalHeight = 0;
            if (bx1 != null)
            {
                switch (bx1.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalHeight += (bx1.DesiredHeight / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalHeight += bx1.DesiredHeight;
                        }
                        break;
                }
            }
            if (bx2 != null)
            {
                //center not care overlapping
                totalHeight += bx2.DesiredHeight;
            }
            if (bx3 != null)
            {
                switch (bx3.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalHeight += (bx3.DesiredHeight / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalHeight += bx3.DesiredHeight;
                        }
                        break;
                }
            }
            return totalHeight;
        }

        protected void InvalidateArrangementInAllDockSpaces()
        {
            //int j = this.spaces.Length;
            //for (int i = this.spaces.Length - 1; i >= 0; --i)
            //{
            //    spaces[i].InvalidateArrangeStatus();
            //}
        }

        public abstract void ArrangeAllSpaces();
#if DEBUG
        //public override string ToString()
        //{
        //    //if (dockSpaceConcept == SpaceConcept.NineSpaceFree)
        //    //{
        //    //    return "FREE_NINE_LAY (L" + dbug_layer_id + this.dbugLayerState + "):" + this.PostCalculateContentSize.ToString() + " of " + ownerVisualElement.ToString();
        //    //}
        //    //else
        //    //{
        //    //    return "dock layer (L" + dbug_layer_id + this.dbugLayerState + "):" + this.PostCalculateContentSize.ToString() + " of " + ownerVisualElement.ToString();
        //    //}
        //}
#endif

        protected static void SetDockBound(SpacePart dock, int x, int y, int newWidth, int newHeight)
        {
            if (dock == null)
            {
                return;
            }
            dock.SetBound(x, y, newWidth, newHeight);
        }

        //public virtual void TopDownReArrangeContent()
        //{
        //}
    }
}