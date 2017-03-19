//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public class FreeSpacesController : NinespaceController
    {
        public FreeSpacesController(UIBox owner)
            : base(owner, SpaceConcept.NineSpaceFree)
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

        public override void ArrangeAllSpaces()
        {
#if DEBUG
            // vinv.dbug_EnterLayerReArrangeContent(this);
#endif

            //vinv.ForceReArrange = true; 
            var centerspace = spaces[C];
            bool suddenArr = false;
            int ownerWidth = this.OwnerVisualElement.Width;
            int ownerHeight = this.OwnerVisualElement.Height;
            if (centerspace != null)
            {
                int dw = centerspace.Width;
                int dh = centerspace.Height;
                if (dw > ownerWidth)
                {
                    dw = ownerWidth;
                }
                if (dh > ownerHeight)
                {
                    dh = ownerHeight;
                }

                SetDockBound(centerspace, 0, 0, dw, dh);
            }
            //-------------------------------------------------

            for (int i = spaces.Length - 1; i >= 0; --i)
            {
                if (i != C)
                {
                    //skip center space
                    SetBoundOfFreeSpace(spaces[i], centerspace, suddenArr);
                }
            }
            for (int i = spaces.Length - 1; i >= 0; --i)
            {
                if (i != C)
                {
                    //skip center space

                    spaces[i].ArrangeContent();
                }
            }
            //-------------------------------------------------
            centerspace.ArrangeContent();
            //-------------------------------------------------

            int offsetx = FindMaxWidthBar(spaces[LT], spaces[L], spaces[LB]);
            int offsety = FindMaxHeightBar(spaces[LT], spaces[T], spaces[RT]);
            if (offsetx != 0 || offsety != 0)
            {
                for (int i = spaces.Length - 1; i > -1; --i)
                {
                    //arrange element of free space
                    //var space = space[i];
                    //if (spaces[i] != null)
                    //{
                    //    spaces[i].SetBound(space.X + offsetx, space.Y + offsety);

                    //}

                }
            }
#if DEBUG
            //vinv.dbug_ExitLayerReArrangeContent();
#endif
        }
        void SetBoundOfFreeSpace(SpacePart space,
            SpacePart centerspace,
            bool suddenArr)
        {
            if (space == null)
            {
                return;
            }

            suddenArr = false;
            var centerspacex = centerspace.Content;
            int x_pos = centerspace.X;
            int y_pos = centerspace.Y;
            int dw = OwnerVisualElement.Width;
            int dh = OwnerVisualElement.Height;
            if (space.DesiredWidth < dw)
            {
                dw = space.DesiredWidth;
            }
            if (space.DesiredHeight < dh)
            {
                dh = space.DesiredHeight;
            }


            SetDockBound(space,
                0,
                0,
                dw,
                dh);
            switch (space.SpaceName)
            {
                case SpaceName.Left:
                    {
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Outer:
                                {
                                    x_pos -= space.Width;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    x_pos -= space.Width / 2;
                                }
                                break;
                        }
                        space.SetLocation(x_pos, y_pos);
                    }
                    break;
                case SpaceName.Right:
                    {
                        x_pos = centerspace.Right;
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Inner:
                                {
                                    x_pos -= space.Width;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    x_pos -= space.Width / 2;
                                }
                                break;
                        }
                    }
                    break;
                case SpaceName.Top:
                    {
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Outer:
                                {
                                    y_pos -= space.Height;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    y_pos -= space.Height / 2;
                                }
                                break;
                        }
                    }
                    break;
                case SpaceName.Bottom:
                    {
                        y_pos = centerspace.Bottom;
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Inner:
                                {
                                    y_pos -= space.Height;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    y_pos -= space.Height / 2;
                                }
                                break;
                        }
                    }
                    break;
                case SpaceName.LeftTop:
                    {
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Outer:
                                {
                                    x_pos -= space.Width;
                                    y_pos -= space.Height;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    x_pos -= space.Width / 2;
                                    y_pos -= space.Height / 2;
                                }
                                break;
                        }
                    }
                    break;
                case SpaceName.LeftBottom:
                    {
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Outer:
                                {
                                    x_pos -= space.Width;
                                    y_pos = centerspace.Bottom;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Inner:
                                {
                                    y_pos = centerspace.Bottom - space.Height;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    x_pos -= space.Width / 2;
                                    y_pos = centerspace.Bottom - space.Height / 2;
                                }
                                break;
                        }
                    }
                    break;
                case SpaceName.RightTop:
                    {
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Outer:
                                {
                                    x_pos = centerspace.Right;
                                    y_pos -= space.Height;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Inner:
                                {
                                    x_pos = centerspace.Right - space.Width;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    //outter - half of grip
                                    x_pos = centerspace.Right - (space.Width / 2);
                                    y_pos -= space.Height - (space.Height / 2);
                                }
                                break;
                        }
                    }
                    break;
                case SpaceName.RightBottom:
                    {
                        switch (space.OverlapMode)
                        {
                            case NamedSpaceContainerOverlapMode.Outer:
                                {
                                    x_pos = centerspace.Right;
                                    y_pos = centerspace.Bottom;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Inner:
                                {
                                    x_pos = centerspace.Right - space.Width;
                                    y_pos = centerspace.Bottom - space.Height;
                                }
                                break;
                            case NamedSpaceContainerOverlapMode.Middle:
                                {
                                    //outer - half of grid box
                                    x_pos = centerspace.Right - (space.Width / 2);
                                    y_pos = centerspace.Bottom - (space.Height / 2);
                                }
                                break;
                        }
                    }
                    break;
            }

            //VisualInvalidateCanvasArgs vinv = contentArrVisitor.GetVisualInvalidateCanvasArgs();
            space.SetLocation(x_pos, y_pos);
        }
        static int FindMaxHeightBar(SpacePart b1,
            SpacePart b2, SpacePart b3)
        {
            //find bar with max height
            int maxHeight = 0;
            int h = 0;
            if (b1 != null)
            {
                h = FindMaxHeightBar(b1);
                if (h > maxHeight)
                {
                    maxHeight = h;
                }
            }
            if (b2 != null)
            {
                h = FindMaxHeightBar(b2);
                if (h > maxHeight)
                {
                    maxHeight = h;
                }
            }
            if (b3 != null)
            {
                h = FindMaxHeightBar(b3);
                if (h > maxHeight)
                {
                    maxHeight = h;
                }
            }

            return maxHeight;
        }
        static int FindMaxWidthBar(SpacePart b1,
            SpacePart b2, SpacePart b3)
        {
            int maxWidth = 0;
            int w = 0;
            if (b1 != null)
            {
                w = FindMaxWidthBar(b1);
                if (w > maxWidth)
                {
                    maxWidth = w;
                }
            }
            if (b2 != null)
            {
                w = FindMaxWidthBar(b2);
                if (w > maxWidth)
                {
                    maxWidth = w;
                }
            }
            if (b3 != null)
            {
                w = FindMaxWidthBar(b3);
                if (w > maxWidth)
                {
                    maxWidth = w;
                }
            }
            return maxWidth;
        }
        static int FindMaxHeightBar(SpacePart bx)
        {
            switch (bx.OverlapMode)
            {
                case NamedSpaceContainerOverlapMode.Middle:
                    {
                        return bx.Height / 2;
                    }
                case NamedSpaceContainerOverlapMode.Outer:
                    {
                        return bx.Height;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
        static int FindMaxWidthBar(SpacePart bx)
        {
            switch (bx.OverlapMode)
            {
                case NamedSpaceContainerOverlapMode.Middle:
                    {
                        return bx.Width / 2;
                    }
                case NamedSpaceContainerOverlapMode.Outer:
                    {
                        return bx.Width;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
    }
}