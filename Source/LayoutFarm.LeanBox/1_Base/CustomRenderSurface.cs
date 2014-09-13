//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm;

namespace LayoutFarm
{

    public abstract class CustomRenderSurface
    {

        object owner;
        public CustomRenderSurface(object owner)
        {
            this.owner = owner;
        }

        public abstract bool FullModeUpdate
        {
            get;
            set;
        }


        public abstract int Width
        {
            get;
        }
        public abstract int Height
        {
            get;
        }
        public abstract void ConfirmSizeChanged();

        public abstract void QuadPagesCalculateCanvas();
        public abstract Size OwnerInnerContentSize
        {
            get;
        }

        public abstract void WindowRootNotifyInvalidArea(InternalRect clientRect);
        public abstract void DrawToThisPage(CanvasBase destPage, InternalRect updateArea);
        //------------------------------------
    }





}