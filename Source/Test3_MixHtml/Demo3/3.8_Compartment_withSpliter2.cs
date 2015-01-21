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
    [DemoNote("3.8 Demo_CompartmentWithSpliter2")]
    class Demo_CompartmentWithSpliter2 : DemoBase
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
            //--------------------------------
            //test add some content to the ninespace box
            var sampleListView = CreateSampleListView();
            ninespaceBox.LeftSpace.AddChildBox(sampleListView);

        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.EaseBox backgroundBox)
        {

        }
        static LayoutFarm.CustomWidgets.ListView CreateSampleListView()
        {
            var listview = new LayoutFarm.CustomWidgets.ListView(300, 400);
            listview.SetLocation(10, 10);
            listview.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            //add 
            for (int i = 0; i < 10; ++i)
            {
                var listItem = new LayoutFarm.CustomWidgets.ListItem(400, 20);
                if ((i % 2) == 0)
                {
                    listItem.BackColor = KnownColors.FromKnownColor(KnownColor.OrangeRed);
                }
                else
                {
                    listItem.BackColor = KnownColors.FromKnownColor(KnownColor.Orange);
                }
                listview.AddItem(listItem);
            }
            return listview;
        }

        class UINinespaceBox : LayoutFarm.CustomWidgets.EaseBox
        {
            Panel boxLeftTop;
            Panel boxRightTop;
            Panel boxLeftBottom;
            Panel boxRightBottom;
            //-------------------------------------
            Panel boxLeft;
            Panel boxTop;
            Panel boxRight;
            Panel boxBottom;
            //-------------------------------------
            Panel centerBox;



            EaseBox gripperLeft;
            EaseBox gripperRight;
            EaseBox gripperTop;
            EaseBox gripperBottom;


            DockSpacesController dockspaceController;
            NinespaceGrippers ninespaceGrippers;
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


                //--------------------------------
                //left and right space expansion
                dockspaceController.LeftSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
                dockspaceController.RightSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
                dockspaceController.SetRightSpaceWidth(200);
                dockspaceController.SetLeftSpaceWidth(200);

                //------------------------------------------------------------------------------------
                this.ninespaceGrippers = new NinespaceGrippers(this.dockspaceController);
                this.ninespaceGrippers.LeftGripper = gripperLeft = CreateGripper(Color.Red, false);
                this.ninespaceGrippers.RightGripper = gripperRight = CreateGripper(Color.Red, false);
                this.ninespaceGrippers.TopGripper = gripperTop = CreateGripper(Color.Red, true);
                this.ninespaceGrippers.BottomGripper = gripperBottom = CreateGripper(Color.Red, true);
                this.ninespaceGrippers.UpdateGripperPositions();
                //------------------------------------------------------------------------------------
            }

            CustomWidgets.EaseBox CreateGripper(PixelFarm.Drawing.Color bgcolor, bool isVertical)
            {
                int controllerBoxWH = 10;
                CustomWidgets.EaseBox gripperBox = new CustomWidgets.EaseBox(controllerBoxWH, controllerBoxWH);
                gripperBox.BackColor = bgcolor;
                ////---------------------------------------------------------------------
                gripperBox.MouseLeave += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        Point pos = gripperBox.Position;
                        if (isVertical)
                        {
                            gripperBox.SetLocation(pos.X, pos.Y + e.YDiff);
                        }
                        else
                        {
                            gripperBox.SetLocation(pos.X + e.XDiff, pos.Y);
                        }
                        this.ninespaceGrippers.UpdateNinespaces();
                        e.MouseCursorStyle = MouseCursorStyle.Pointer;
                        e.CancelBubbling = true;
                    }
                };
                gripperBox.MouseMove += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        Point pos = gripperBox.Position;
                        if (isVertical)
                        {
                            gripperBox.SetLocation(pos.X, pos.Y + e.YDiff);
                        }
                        else
                        {
                            gripperBox.SetLocation(pos.X + e.XDiff, pos.Y);
                        }

                        this.ninespaceGrippers.UpdateNinespaces();
                        e.MouseCursorStyle = MouseCursorStyle.Pointer;
                        e.CancelBubbling = true;
                    }
                };
                gripperBox.MouseUp += (s, e) =>
                {
                    e.MouseCursorStyle = MouseCursorStyle.Default;
                    e.CancelBubbling = true;
                };

                return gripperBox;
            }
            static CustomWidgets.Panel CreateSpaceBox(SpaceName name, PixelFarm.Drawing.Color bgcolor)
            {
                int controllerBoxWH = 10;
                CustomWidgets.Panel spaceBox = new CustomWidgets.Panel(controllerBoxWH, controllerBoxWH);
                spaceBox.BackColor = bgcolor;
                spaceBox.Tag = name;
                return spaceBox;
            }

            public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
            {
                if (!this.HasReadyRenderElement)
                {

                    var myRenderElement = base.GetPrimaryRenderElement(rootgfx) as LayoutFarm.CustomWidgets.CustomRenderBox;
                    PlainLayer plain0 = null;
                    if (myRenderElement != null)
                    {
                        VisualLayerCollection layers = new VisualLayerCollection();
                        myRenderElement.Layers = layers;
                        plain0 = new PlainLayer(myRenderElement);
                        layers.AddLayer(plain0);

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

                    //grippers
                    plain0.AddChild(gripperLeft.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperRight.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperBottom.GetPrimaryRenderElement(rootgfx));

                    //------------------------------------------------------
                }
                return base.GetPrimaryRenderElement(rootgfx);
            }

            public override void SetSize(int width, int height)
            {
                base.SetSize(width, height);
                dockspaceController.SetSize(width, height);

            }

            public Panel LeftSpace { get { return this.boxLeft; } }
            public Panel RightSpace { get { return this.boxRight; } }
            public Panel TopSpace { get { return this.boxTop; } }
            public Panel BottomSpace { get { return this.boxBottom; } }



        }


    }
}