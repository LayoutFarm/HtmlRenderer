// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
    [DemoNote("3.5 Demo_CompartmentBox2")]
    class Demo_CompartmentBox2 : DemoBase
    {

        UINinespaceBox ninespaceBox;
        protected override void OnStartDemo(SampleViewport viewport)
        {

            //--------------------------------
            {
                //background element
                var bgbox = new LayoutFarm.CustomWidgets.EaseBox(800, 600);
                bgbox.BackColor = Color.White;
                bgbox.SetLocation(0, 0);
                SetupBackgroundProperties(bgbox);
                viewport.AddContent(bgbox);
            }
            //--------------------------------
            //ninespace compartment
            ninespaceBox = new UINinespaceBox(800, 600);
            viewport.AddContent(ninespaceBox);
            ninespaceBox.SetSize(800, 600);


        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.EaseBox backgroundBox)
        {
            ////if click on background
            //backgroundBox.MouseDown += (s, e) =>
            //{
            //    controllerBox1.TargetBox = null;//release target box
            //    controllerBox1.Visible = false;
            //};

        }





        class UINinespaceBox : LayoutFarm.CustomWidgets.EaseBox
        {
            //-------------------------------------
            EaseBox boxLeftTop;
            EaseBox boxRightTop;
            EaseBox boxLeftBottom;
            EaseBox boxRightBottom;
            //-------------------------------------
            EaseBox boxLeft;
            EaseBox boxTop;
            EaseBox boxRight;
            EaseBox boxBottom;
            //-------------------------------------

            //-------------------------------------
            EaseBox centerBox;

            DockSpacesController dockspaceController;
           

            public UINinespaceBox(int w, int h)
                : base(w, h)
            {
                SetupDockSpaces();
            }
            void SetupDockSpaces()
            {
                //1. controller
                this.dockspaceController = new DockSpacesController(this, SpaceConcept.NineSpace);

                //2.  
                this.dockspaceController.LeftTopSpace.Content = boxLeftTop = CreateSpaceBox(SpaceName.LeftTop, Color.Red);
                this.dockspaceController.RightTopSpace.Content = boxRightTop = CreateSpaceBox(SpaceName.RightTop, Color.Red);
                this.dockspaceController.LeftBottomSpace.Content = boxLeftBottom = CreateSpaceBox(SpaceName.LeftBottom, Color.Red);
                this.dockspaceController.RightBottomSpace.Content = boxRightBottom = CreateSpaceBox(SpaceName.RightBottom, Color.Red);
                //3.
                this.dockspaceController.LeftSpace.Content = boxLeft = CreateSpaceBox(SpaceName.Left, Color.Blue);
                this.dockspaceController.TopSpace.Content = boxTop = CreateSpaceBox(SpaceName.Top, Color.Yellow);
                this.dockspaceController.RightSpace.Content = boxRight = CreateSpaceBox(SpaceName.Right, Color.Green);
                this.dockspaceController.BottomSpace.Content = boxBottom = CreateSpaceBox(SpaceName.Bottom, Color.Yellow);


                //------------------------------------------------------------------------------------
                //left and right space expansion
                dockspaceController.LeftSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
                dockspaceController.RightSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
                dockspaceController.SetRightSpaceWidth(200);
                dockspaceController.SetLeftSpaceWidth(200);

            
            }

       
            static CustomWidgets.EaseBox CreateSpaceBox(SpaceName name, PixelFarm.Drawing.Color bgcolor)
            {
                int controllerBoxWH = 10;
                CustomWidgets.EaseBox tinyBox = new CustomWidgets.EaseBox(controllerBoxWH, controllerBoxWH);
                tinyBox.BackColor = bgcolor;
                tinyBox.Tag = name;
                return tinyBox;
            }

            public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
            {
                if (!this.HasReadyRenderElement)
                {

                    var myRenderElement = base.GetPrimaryRenderElement(rootgfx) as LayoutFarm.CustomWidgets.CustomRenderBox;
                    PlainLayer plain0 = null;
                    if (myRenderElement != null)
                    {
                        //VisualLayerCollection layers = new VisualLayerCollection();
                        //myRenderElement.Layers = layers;
                        plain0 = new PlainLayer(myRenderElement);
                        //layers.AddLayer(plain0);
                        myRenderElement.Layer = plain0;
                    }
                    //------------------------------------------------------
                    plain0.AddChild(boxLeftTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRightTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxLeftBottom.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRightBottom.GetPrimaryRenderElement(rootgfx));
                    //------------------------------------------------------
                    plain0.AddChild(boxLeft.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRight.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxBottom.GetPrimaryRenderElement(rootgfx));
                    //------------------------------------------------------
                   

                }
                return base.GetPrimaryRenderElement(rootgfx);
            }

            public override void SetSize(int width, int height)
            {
                base.SetSize(width, height);
                dockspaceController.SetSize(width, height);

            }
        }


    }
}