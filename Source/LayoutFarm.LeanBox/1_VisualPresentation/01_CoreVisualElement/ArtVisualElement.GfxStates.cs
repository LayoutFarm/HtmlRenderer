using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D; 

namespace LayoutFarm.Presentation
{
            partial class ArtVisualElement
    {

                                object myBehOrAnimator;

                                public BoxStyle Beh
        {
            get
            {
                if (this.myBehOrAnimator != null)
                {
                    return (BoxStyle)this.myBehOrAnimator;
                   
                }
                return null;
            }
        }
        protected bool HasBeh
        {
            get
            {
                return this.Beh != null;
                            }
        }

 
                                public bool TransparentForAllEvents
        {
            get
            {
                return (uiFlags & TRANSPARENT_FOR_ALL_EVENTS) != 0;

            }
            set
            {
                if (value)
                {
                                        uiFlags |= TRANSPARENT_FOR_ALL_EVENTS;
                }
                else
                {
                    uiFlags &= ~TRANSPARENT_FOR_ALL_EVENTS;
                }

            }
        }


        protected ArtColorBrush GetSharedBgColorBrush()
        {
            return Beh.SharedBgColorBrush;
        }
        protected bool IsOnGroundGfxState
        {
                        get
            {
                return myBehOrAnimator == null || (uiFlags & USE_ANIMATOR) == 0;

            }
        } 
        
                                void ResetEventState(VisualElementArgs vinv)
        {
                                    
                                                                                    if ((uiFlags & IS_IN_ANIMATION_MODE) == 0)
            {
                                                                                                                                                                                                                                                                                            }
            else
            {
                                                uiFlags |= ANIMATION_WAITING_FOR_NORMAL_MODE;
            }

            this.InvalidateGraphic(vinv);
        } 
                                        public virtual void SetBehavior(BoxStyle newbeh, VisualElementArgs vinv)
        {
            
            BoxStyle beh = (BoxStyle)newbeh;
            if (newbeh == null)
            {
                return;
            }

            if ((uiFlags & USE_ANIMATOR) == 0)
            {
                                if (vinv != null)
                {
                    this.InvalidateGraphic(vinv);                 }

                this.myBehOrAnimator = beh;
                if (beh.positionWidth > -1)
                {
                                        this.SetWidth(beh.positionWidth, vinv);
                }
                if (beh.positionHeight > -1)
                {
                                        this.SetHeight(beh.positionHeight, vinv);
                }

                                                                                                
                                
                                                                                                
                                if (vinv != null)
                {
                    this.InvalidateGraphic(vinv);
                }
            }
            else
            {

            }
        } 

         
         
    }
}