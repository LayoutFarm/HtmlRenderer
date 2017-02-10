//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class DockSpacesController : NinespaceController
    {
        VerticalBoxExpansion leftBoxVerticalExpansionFlags = VerticalBoxExpansion.Default;
        VerticalBoxExpansion rightBoxVerticalExpansionFlags = VerticalBoxExpansion.Default;
        int topSplitterHeight;
        int bottomSplitterHeight;
        int leftSplitterWidth;
        int rightSplitterWidth;
        public event EventHandler FinishNineSpaceArrangement;
        public DockSpacesController(UIBox owner, SpaceConcept initConcept)
            : base(owner, initConcept)
        {
            this.myOwner = owner;
            switch (initConcept)
            {
                case SpaceConcept.TwoSpaceHorizontal: //top-bottom
                    {
                        spaces[L] = InitSpace(SpaceName.Left);
                        spaces[R] = InitSpace(SpaceName.Right);
                    }
                    break;
                case SpaceConcept.TwoSpaceVertical: //left-right
                    {
                        spaces[T] = InitSpace(SpaceName.Top);
                        spaces[B] = InitSpace(SpaceName.Bottom);
                    }
                    break;
                case SpaceConcept.ThreeSpaceHorizontal:
                    {
                        //left-center-right 
                        spaces[L] = InitSpace(SpaceName.Left);
                        spaces[C] = InitSpace(SpaceName.Center);
                        spaces[R] = InitSpace(SpaceName.Right);
                    }
                    break;
                case SpaceConcept.ThreeSpaceVertical:
                    {
                        //top-center-bottom                        
                        spaces[T] = InitSpace(SpaceName.Top);
                        spaces[C] = InitSpace(SpaceName.Center);
                        spaces[B] = InitSpace(SpaceName.Bottom);
                    }
                    break;
                case SpaceConcept.FourSpace:
                    {
                        spaces[FOURSPACE_LT] = InitSpace(SpaceName.LeftTop);
                        spaces[FOURSPACE_RT] = InitSpace(SpaceName.RightTop);
                        spaces[FOURSPACE_RB] = InitSpace(SpaceName.RightBottom);
                        spaces[FOURSPACE_LB] = InitSpace(SpaceName.LeftBottom);
                    }
                    break;
                case SpaceConcept.FiveSpace:
                    {
                        spaces[L] = InitSpace(SpaceName.Left);
                        spaces[R] = InitSpace(SpaceName.Right);
                        spaces[T] = InitSpace(SpaceName.Top);
                        spaces[B] = InitSpace(SpaceName.Bottom);
                        spaces[C] = InitSpace(SpaceName.Center);
                    }
                    break;
                case SpaceConcept.NineSpace:
                default:
                    {
                        spaces[L] = InitSpace(SpaceName.Left);
                        spaces[R] = InitSpace(SpaceName.Right);
                        spaces[T] = InitSpace(SpaceName.Top);
                        spaces[B] = InitSpace(SpaceName.Bottom);
                        spaces[C] = InitSpace(SpaceName.Center);
                        spaces[LT] = InitSpace(SpaceName.LeftTop);
                        spaces[RT] = InitSpace(SpaceName.RightTop);
                        spaces[RB] = InitSpace(SpaceName.RightBottom);
                        spaces[LB] = InitSpace(SpaceName.LeftBottom);
                    }
                    break;
            }
        }
        public void SetSize(int w, int h)
        {
            //set controller size
            this.sizeW = w;
            this.sizeH = h;
            //-------------
            //arrange all space position 
            this.ArrangeAllSpaces();
        }
        public VerticalBoxExpansion LeftSpaceVerticalExpansion
        {
            get
            {
                return leftBoxVerticalExpansionFlags;
            }
            set
            {
                leftBoxVerticalExpansionFlags = value;
            }
        }
        public int TopSplitterHeight
        {
            get
            {
                return topSplitterHeight;
            }
            set
            {
                this.topSplitterHeight = value;
            }
        }
        public int BottomSplitterHeight
        {
            get
            {
                return this.bottomSplitterHeight;
            }
            set
            {
                this.bottomSplitterHeight = value;
            }
        }
        public int LeftSplitterWidth
        {
            get
            {
                return this.leftSplitterWidth;
            }
            set
            {
                this.leftSplitterWidth = value;
            }
        }
        public int RightSplitterWidth
        {
            get
            {
                return this.rightSplitterWidth;
            }
            set
            {
                this.rightSplitterWidth = value;
            }
        }

        //-------------------------------------------------------------------
        public VerticalBoxExpansion RightSpaceVerticalExpansion
        {
            get
            {
                return rightBoxVerticalExpansionFlags;
            }
            set
            {
                rightBoxVerticalExpansionFlags = value;
            }
        }


        public override void ArrangeAllSpaces()
        {
#if DEBUG

            //vinv.dbug_SetInitObject(this);
            //vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.DockSpaceLayer_ArrAllDockSpaces);

#endif
            for (int i = spaces.Length - 1; i >= 0; --i)
            {
                spaces[i].CalculateContentSize();
            }
#if DEBUG
            //vinv.dbug_EnterLayerReArrangeContent(this);
#endif


            //-------------------------------------------------
            // this.BeginLayerGraphicUpdate(vinv);
            //------------------------------------------------- 
            int w = this.sizeW;
            int h = this.sizeH;
            if (dockSpaceConcept == SpaceConcept.FourSpace)
            {
                //-------------------------------------------------
                if (!this.HasSpecificBottomSpaceHeight)
                {
                }
                if (!this.HasSpecificTopSpaceHeight)
                {
                }
                if (!this.HasSpecificRightSpaceWidth)
                {
                }
                if (!this.HasSpecificLeftSpaceWidth)
                {
                }
                //------------------------------------------------- 
                SetDockBound(spaces[FOURSPACE_LT],
                    0,
                    0,
                    leftSpaceWidth,
                    topSpaceHeight);
                //------------------------------------------------------- 
                SetDockBound(spaces[FOURSPACE_LB],
                    0,
                    topSpaceHeight,
                    leftSpaceWidth,
                    OwnerVisualElement.Height - (topSpaceHeight));
                //------------------------------------------------------ 
                SetDockBound(spaces[FOURSPACE_RT],
                    leftSpaceWidth,
                    0,
                    w - leftSpaceWidth,
                    topSpaceHeight);
                //------------------------------------------------------ 
                SetDockBound(spaces[FOURSPACE_RB],
                    leftSpaceWidth,
                     topSpaceHeight,
                     w - (leftSpaceWidth),
                     h - (topSpaceHeight));
                //------------------------------------------------------
            }
            else
            {
                //start with ninespace , the extend to proper form

                //-------------------------------------------------
                var b_space = spaces[B];
                var t_space = spaces[T];
                var l_space = spaces[L];
                var r_space = spaces[R];
                if (!this.HasSpecificBottomSpaceHeight && b_space != null)
                {
                    b_space.CalculateContentSize();
                    //if (b_space.NeedReCalculateContentSize)
                    //{
                    //    b_space.TopDownReCalculateContentSize(vinv);
                    //}
                    this.bottomSpaceHeight = b_space.DesiredHeight;
                }

                if (!this.HasSpecificTopSpaceHeight && t_space != null)
                {
                    t_space.CalculateContentSize();
                    //if (t_space.NeedReCalculateContentSize)
                    //{
                    //    t_space.TopDownReCalculateContentSize(vinv);
                    //}
                    this.topSpaceHeight = t_space.DesiredHeight;
                }
                if (!this.HasSpecificRightSpaceWidth && r_space != null)
                {
                    r_space.CalculateContentSize();
                    //if (r_space.NeedReCalculateContentSize)
                    //{
                    //    r_space.TopDownReCalculateContentSize(vinv);
                    //}
                    this.rightSpaceWidth = r_space.DesiredWidth;
                }
                if (!this.HasSpecificLeftSpaceWidth && l_space != null)
                {
                    l_space.CalculateContentSize();
                    //if (l_space.NeedReCalculateContentSize)
                    //{
                    //    l_space.TopDownReCalculateContentSize(vinv);
                    //}
                    this.leftSpaceWidth = l_space.DesiredWidth;
                }
                //-------------------------------------------------

                if (l_space != null)
                {
                    int left_y = topSpaceHeight;
                    int left_h = h - topSpaceHeight - bottomSpaceHeight;
                    if ((leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        left_y = 0;
                        left_h += topSpaceHeight;
                    }
                    if ((leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        left_h += bottomSpaceHeight;
                    }
                    SetDockBound(spaces[L],
                        0,//x
                        left_y,
                        leftSpaceWidth,
                        left_h);
                }
                //-------------------------------------------------
                if (r_space != null)
                {
                    int right_y = topSpaceHeight;
                    int right_h = h - topSpaceHeight - bottomSpaceHeight;
                    if (HasSpecificCenterSpaceWidth)
                    {
                        rightSpaceWidth = OwnerVisualElement.Width - (leftSpaceWidth + centerSpaceWidth);
                    }

                    if ((rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        right_y = 0;
                        right_h += topSpaceHeight;
                    }
                    if ((rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        right_h += bottomSpaceHeight;
                    }
                    SetDockBound(spaces[R],
                      w - rightSpaceWidth,
                      right_y,
                      rightSpaceWidth,
                      right_h);
                    //spaces[R].InvalidateArrangeStatus(); 
                }
                //-------------------------------------------------
                if (t_space != null)
                {
                    //top 
                    int top_x = 0;
                    int top_w = w;
                    if (this.dockSpaceConcept == SpaceConcept.NineSpace)
                    {
                        top_x = leftSpaceWidth;
                        top_w = w - (leftSpaceWidth + rightSpaceWidth);
                    }
                    //-------------------------------------------------------

                    if ((leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        top_x = leftSpaceWidth;
                        //top_w -= leftSpaceWidth;
                    }
                    if ((rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        //top_w -= rightSpaceWidth;
                    }
                    SetDockBound(spaces[T],
                     top_x,
                     0,
                     top_w,
                     topSpaceHeight);
                }
                //-------------------------------------------------
                if (b_space != null)
                {
                    int bottom_x = 0;
                    int bottom_w = w;
                    if (this.dockSpaceConcept == SpaceConcept.NineSpace)
                    {
                        bottom_x = leftSpaceWidth;
                        bottom_w = w - (leftSpaceWidth + rightSpaceWidth);
                    }
                    //-----------------------------------------------------

                    if ((leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        bottom_x = leftSpaceWidth;
                        //bottom_w -= leftSpaceWidth;
                    }
                    if ((rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        //bottom_w -= rightSpaceWidth;
                    }


                    bottom_x += leftSplitterWidth;
                    //-----------------------------------------------------
                    SetDockBound(spaces[B],
                    bottom_x,
                    h - bottomSpaceHeight,
                    bottom_w,
                    bottomSpaceHeight);
                }


                //---------------------------------------------------------------------------------
                if (spaces[C] != null)
                {
                    w = OwnerVisualElement.Width - (rightSpaceWidth + leftSpaceWidth) - (leftSplitterWidth + rightSplitterWidth);
                    h = OwnerVisualElement.Height - (topSpaceHeight + bottomSpaceHeight);
                    if (w < 1)
                    {
                        w = 1;
                    }
                    if (h < 1)
                    {
                        h = 1;
                    }

                    int x = leftSpaceWidth + leftSplitterWidth;
                    SetDockBound(spaces[C],
                    x,
                    topSpaceHeight,
                    w,
                    h);
                }
                if (dockSpaceConcept == SpaceConcept.NineSpace)
                {
                    h = OwnerVisualElement.Height;
                    w = OwnerVisualElement.Width;
                    SetDockBound(spaces[LT], 0, 0, leftSpaceWidth, topSpaceHeight);
                    SetDockBound(spaces[LB], 0, h - bottomSpaceHeight, leftSpaceWidth, bottomSpaceHeight);
                    SetDockBound(spaces[RT], w - rightSpaceWidth, 0, rightSpaceWidth, topSpaceHeight);
                    SetDockBound(spaces[RB], w - rightSpaceWidth, h - bottomSpaceHeight, rightSpaceWidth, bottomSpaceHeight);
                }
                //-----------------------------
            }
            for (int i = spaces.Length - 1; i >= 0; i--)
            {
                SpacePart dockSpace = spaces[i];
                if (dockSpace.SpaceName == SpaceName.Left ||
                    dockSpace.SpaceName == SpaceName.Right)
                {
                }
                if (dockSpace != null)
                {
                    dockSpace.ArrangeContent();
                }
            }

            //-------------------------------------------------

            if (this.FinishNineSpaceArrangement != null)
            {
                FinishNineSpaceArrangement(this, EventArgs.Empty);
            }

#if DEBUG

            //vinv.dbug_EndLayoutTrace();

#endif
        }

        //public override void TopDownReArrangeContent()
        //{
        //    ArrangeAllDockSpaces();
        //}
    }
}