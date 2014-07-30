//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;



namespace LayoutFarm.Presentation
{
    partial class ArtVisualElement
    {



        public bool HasOwner
        {
            get
            {
                return this.visualParentLink != null;
            }
        }
#if DEBUG
        public void dbug_WriteOwnerLayerInfo(int i)
        {

            //if (this.visualParentLink != null)
            //{
            //    visualroot.dbug_rootHitChainMsg.AddLast(new dbugLayoutMsg(this, new string('.', i) + " [Ly:" + i + "] " +
            //          visualParentLink.dbugGetLinkInfo()));
            //}
        }
        public void dbug_WriteOwnerLineInfo(int i)
        {

        }
#endif
        public ArtVisualContainerBase GetOwnerContainer()
        {
            if (this.visualParentLink != null)
            {
                return visualParentLink.ParentVisualElement as ArtVisualContainerBase;
            }
            return null;
        }
        public bool IsLineBreak
        {
            get
            {
                return ((uiFlags & IS_LINE_BREAK) == IS_LINE_BREAK);
            }
            set
            {
                if (value)
                {
                    uiFlags |= IS_LINE_BREAK;
                }
                else
                {
                    uiFlags &= ~IS_LINE_BREAK;
                }
            }
        }
        public bool IsStretchable
        {
            get
            {
                return ((uiFlags & IS_STRECHABLE) == IS_STRECHABLE);
            }
            set
            {
                if (value)
                {
                    uiFlags |= IS_STRECHABLE;
                }
                else
                {
                    uiFlags &= ~IS_STRECHABLE;
                }
            }
        }




    }
}