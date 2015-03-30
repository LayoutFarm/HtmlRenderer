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
    public enum PanelLayoutKind
    {
        Absolute,
        VerticalStack,
        HorizontalStack
    }
    public enum PanelStretch
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }

    public class Panel : EaseBox
    {
        PanelLayoutKind panelLayoutKind;
        PanelStretch panelChildStretch;
        CustomRenderBox primElement;
        Color backColor = Color.LightGray;
        UICollection uiList;
        bool needContentLayout;

        public Panel(int width, int height)
            : base(width, height)
        {
            uiList = new UICollection(this); 

        }
        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }

        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {

                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.HasSpecificHeight = this.HasSpecificHeight;
                renderE.HasSpecificWidth = this.HasSpecificWidth;
                renderE.SetController(this);
#if DEBUG
                //if (dbugBreakMe)
                //{
                //    renderE.dbugBreak = true;
                //}
#endif
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.HasSpecificSize = true;
                renderE.SetViewport(this.ViewportX, this.ViewportY);
                //------------------------------------------------
                //create visual layer
                PlainLayer plan0 = renderE.GetDefaultLayer();
                int childCount = this.uiList.Count;
                for (int m = 0; m < childCount; ++m)
                {
                    plan0.AddChild(uiList.GetElement(m).GetPrimaryRenderElement(rootgfx));
                }



                SetPrimaryRenderElement(renderE);
                //---------------------------------
                primElement = renderE;
            }
            return primElement;
        }
        public override bool NeedContentLayout
        {
            get
            {
                return this.needContentLayout;
            }
        }
        public PanelLayoutKind PanelLayoutKind
        {
            get { return this.panelLayoutKind; }
            set
            {
                this.panelLayoutKind = value;
            }
        }


        public IEnumerable<UIElement> GetChildIter()
        {
            if (uiList != null)
            {
                int j = uiList.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return uiList.GetElement(i);
                }
            }
        }
        public void AddChild(UIElement ui)
        {
            needContentLayout = true;
            this.uiList.AddUI(ui);
            if (this.HasReadyRenderElement)
            {
                primElement.AddChild(ui);
                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }

            if (ui.NeedContentLayout)
            {
                ui.InvalidateLayout();
            }
        }
        public void RemoveChild(UIElement ui)
        {
            needContentLayout = true;
            this.uiList.RemoveUI(ui);
            if (this.HasReadyRenderElement)
            {

                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
                this.primElement.RemoveChild(ui.CurrentPrimaryRenderElement);
            }
        }
        public void ClearChildren()
        {
            needContentLayout = true;
            this.uiList.Clear();
            if (this.HasReadyRenderElement)
            {

                primElement.ClearAllChildren();
                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }
        }

        public int ChildCount
        {
            get
            {
                if (this.uiList != null)
                {
                    return this.uiList.Count;
                }
                return 0;
            }
        }
        public UIElement GetChild(int index)
        {
            if (uiList != null)
            {
                return uiList.GetElement(index);
            }
            return null;
        }


        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }

        public override void PerformContentLayout()
        {

            this.InvalidateGraphics();
            //temp : arrange as vertical stack***
            switch (this.PanelLayoutKind)
            {
                case CustomWidgets.PanelLayoutKind.VerticalStack:
                    {

                        int count = this.uiList.Count;
                        int ypos = 0;

                        //todo: implement stretching ...
                        //switch (this.panelChildStretch)
                        //{
                        //    case PanelStretch.Horizontal:
                        //        {
                        //        } break;
                        //    case PanelStretch.Vertical:
                        //        {
                        //        } break;
                        //    case PanelStretch.Both:
                        //        {
                        //        } break;
                        //}
                        int maxRight = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.uiList.GetElement(i) as UIBox;
                            if (element != null)
                            {
                                //if (element.dbugBreakMe)
                                //{

                                //}
                                element.PerformContentLayout();

                                int elemH = element.HasSpecificHeight ?
                                    element.Height :
                                    element.DesiredHeight;
                                int elemW = element.HasSpecificWidth ?
                                    element.Width :
                                    element.DesiredWidth;
                                element.SetBounds(0, ypos, element.Width, elemH);
                                ypos += element.Height;



                                int tmp_right = element.DesiredWidth + element.Left;
                                if (tmp_right > maxRight)
                                {
                                    maxRight = tmp_right;
                                }
                            }
                        }

                        this.SetDesiredSize(maxRight, ypos);

                    } break;
                case CustomWidgets.PanelLayoutKind.HorizontalStack:
                    {

                        int count = this.uiList.Count;
                        int xpos = 0;

                        int maxBottom = 0;

                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.uiList.GetElement(i) as UIBox;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                element.SetBounds(xpos, 0, element.DesiredWidth, element.DesiredHeight);
                                xpos += element.DesiredWidth;

                                int tmp_bottom = element.DesiredHeight + element.Top;
                                if (tmp_bottom > maxBottom)
                                {
                                    maxBottom = tmp_bottom;
                                }

                            }
                        }

                        this.SetDesiredSize(xpos, maxBottom);

                    } break;
                default:
                    {

                        int count = this.uiList.Count;
                        int maxRight = 0;
                        int maxBottom = 0;

                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.uiList.GetElement(i) as UIBox;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                int tmp_right = element.DesiredWidth + element.Left;
                                if (tmp_right > maxRight)
                                {
                                    maxRight = tmp_right;
                                }
                                int tmp_bottom = element.DesiredHeight + element.Top;
                                if (tmp_bottom > maxBottom)
                                {
                                    maxBottom = tmp_bottom;
                                }
                            }
                        }

                        if (!this.HasSpecificWidth)
                        {
                            this.SetDesiredSize(maxRight, this.DesiredHeight);                              
                        }
                        if (!this.HasSpecificHeight)
                        {
                            this.SetDesiredSize(this.DesiredWidth, maxBottom);                             
                        }
                    } break;
            }
            //------------------------------------------------
            base.RaiseLayoutFinished();
        }

    }



}