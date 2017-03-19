//Apache2, 2014-2017, WinterDev


using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class HingeRelation
    {
        bool isOpen;
        //1. land part
        UIBox landPart;
        //2. float part   
        UIBox floatPart;
        RenderElement floatPartRenderElement;
        HingeFloatPartStyle floatPartStyle;
        //----------------------------------------------------  
        public UIBox LandPart
        {
            get { return this.landPart; }
            set
            {
                this.landPart = value;
                if (value != null)
                {
                    //if new value not null
                    //check existing land part
                    if (this.landPart != null)
                    {
                        //remove existing landpart 
                    }
                    //if (primElement != null)
                    //{
                    //    //add 
                    //    var visualPlainLayer = primElement.Layers.GetLayer(0) as VisualPlainLayer;
                    //    if (visualPlainLayer != null)
                    //    {
                    //        visualPlainLayer.AddChild(value.GetPrimaryRenderElement(primElement.Root));
                    //    } 
                    //} 
                }
                else
                {
                    if (this.landPart != null)
                    {
                        //remove existing landpart

                    }
                }
            }
        }
        public UIBox FloatPart
        {
            get { return this.floatPart; }
            set
            {
                this.floatPart = value;
                if (value != null)
                {
                    //attach float part
                }
            }
        }
        //---------------------------------------------------- 
        public bool IsOpen
        {
            get { return this.isOpen; }
        }
        //----------------------------------------------------  

        public void OpenHinge()
        {
            if (isOpen) return;
            this.isOpen = true;
            //-----------------------------------
            if (landPart == null) return;
            if (floatPart == null) return;
            switch (floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        //var topRenderBox = primElement.GetTopWindowRenderBox();
                        //if (topRenderBox != null)
                        //{
                        //    Point globalLocation = primElement.GetGlobalLocation();
                        //    floatPart.SetLocation(globalLocation.X, globalLocation.Y + primElement.Height);
                        //    this.floatPartRenderElement = this.floatPart.GetPrimaryRenderElement(primElement.Root);
                        //    topRenderBox.AddChild(floatPartRenderElement);
                        //}

                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void CloseHinge()
        {
            if (!isOpen) return;
            this.isOpen = false;
            if (this.landPart == null) return;
            if (floatPart == null) return;
            switch (floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (floatPartRenderElement != null)
                        {
                            //temp
                            var parentContainer = floatPartRenderElement.ParentRenderElement as CustomRenderBox;
                            parentContainer.RemoveChild(floatPartRenderElement);
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }

        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this.floatPartStyle; }
            set
            {
                this.floatPartStyle = value;
            }
        }
    }
}