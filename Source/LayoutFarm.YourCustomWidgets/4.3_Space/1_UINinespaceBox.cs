// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;
using LayoutFarm.Composers;
using LayoutFarm;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.CustomWidgets
{
    public class NinespaceBox : EaseBox
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
        Panel boxCentral;
        EaseBox gripperLeft;
        EaseBox gripperRight;
        EaseBox gripperTop;
        EaseBox gripperBottom;

        DockSpacesController dockspaceController;
        NinespaceGrippers ninespaceGrippers;

        public NinespaceBox(int w, int h)
            : base(w, h)
        {
            SetupDockSpaces(SpaceConcept.NineSpace);
        }
        public NinespaceBox(int w, int h, SpaceConcept spaceConcept)
            : base(w, h)
        {
            SetupDockSpaces(spaceConcept);
        }
        public bool ShowGrippers
        {
            get;
            set;
        }
        static Panel CreateSpaceBox(SpaceName name, Color bgcolor)
        {
            int controllerBoxWH = 10;
            Panel spaceBox = new Panel(controllerBoxWH, controllerBoxWH);
            spaceBox.BackColor = bgcolor;
            spaceBox.Tag = name;
            return spaceBox;
        }

        //static Color leftTopColor = Color.Red;
        //static Color rightTopColor = Color.Red;
        //static Color leftBottomColor = Color.Red;
        //static Color rightBottomColor = Color.Red;

        //static Color leftColor = Color.Blue;
        //static Color topColor = Color.Yellow;
        //static Color rightColor = Color.Green;
        //static Color bottomColor = Color.OrangeRed;
        //static Color centerColor = Color.White;
        static Color leftTopColor = Color.White;
        static Color rightTopColor = Color.White;
        static Color leftBottomColor = Color.White;
        static Color rightBottomColor = Color.White;

        static Color leftColor = Color.White;
        static Color topColor = Color.White;
        static Color rightColor = Color.White;
        static Color bottomColor = Color.White;
        static Color centerColor = Color.White;

        static Color gripperColor = Color.Gray;

        void SetupDockSpaces(SpaceConcept spaceConcept)
        {
            //1. controller
            this.dockspaceController = new DockSpacesController(this, spaceConcept);

            //2.  
            this.dockspaceController.LeftTopSpace.Content = boxLeftTop = CreateSpaceBox(SpaceName.LeftTop, leftTopColor);
            this.dockspaceController.RightTopSpace.Content = boxRightTop = CreateSpaceBox(SpaceName.RightTop, rightTopColor);
            this.dockspaceController.LeftBottomSpace.Content = boxLeftBottom = CreateSpaceBox(SpaceName.LeftBottom, leftBottomColor);
            this.dockspaceController.RightBottomSpace.Content = boxRightBottom = CreateSpaceBox(SpaceName.RightBottom, rightBottomColor);
            //3.
            this.dockspaceController.LeftSpace.Content = boxLeft = CreateSpaceBox(SpaceName.Left, leftColor);
            this.dockspaceController.TopSpace.Content = boxTop = CreateSpaceBox(SpaceName.Top, topColor);
            this.dockspaceController.RightSpace.Content = boxRight = CreateSpaceBox(SpaceName.Right, rightColor);
            this.dockspaceController.BottomSpace.Content = boxBottom = CreateSpaceBox(SpaceName.Bottom, bottomColor);

            this.dockspaceController.CenterSpace.Content = boxCentral = CreateSpaceBox(SpaceName.Center, centerColor);

            //--------------------------------
            //left and right space expansion
            //dockspaceController.LeftSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
            //dockspaceController.RightSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
            dockspaceController.SetRightSpaceWidth(200);
            dockspaceController.SetLeftSpaceWidth(200);
            //------------------------------------------------------------------------------------
            this.ninespaceGrippers = new NinespaceGrippers(this.dockspaceController);
            this.ninespaceGrippers.LeftGripper = gripperLeft = CreateGripper(gripperColor, false);
            this.ninespaceGrippers.RightGripper = gripperRight = CreateGripper(gripperColor, false);
            this.ninespaceGrippers.TopGripper = gripperTop = CreateGripper(gripperColor, true);
            this.ninespaceGrippers.BottomGripper = gripperBottom = CreateGripper(gripperColor, true);
            this.ninespaceGrippers.UpdateGripperPositions();
            //------------------------------------------------------------------------------------
        }
        public void SetDockSpaceConcept(LayoutFarm.UI.SpaceConcept concept)
        {

        }
        EaseBox CreateGripper(PixelFarm.Drawing.Color bgcolor, bool isVertical)
        {
            int controllerBoxWH = 10;
            EaseBox gripperBox = new EaseBox(controllerBoxWH, controllerBoxWH);
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
                    e.StopPropagation();
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

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {

                var myRenderElement = base.GetPrimaryRenderElement(rootgfx) as LayoutFarm.CustomWidgets.CustomRenderBox;
                PlainLayer plain0 = myRenderElement.GetDefaultLayer();
                //------------------------------------------------------
                plain0.AddChild(boxCentral.GetPrimaryRenderElement(rootgfx));
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
                if (this.ShowGrippers)
                {
                    plain0.AddChild(gripperLeft.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperRight.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperBottom.GetPrimaryRenderElement(rootgfx));
                }
                //------------------------------------------------------
            }
            return base.GetPrimaryRenderElement(rootgfx);
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            dockspaceController.SetSize(width, height);

        }
        public override void PerformContentLayout()
        {
            dockspaceController.ArrangeAllSpaces();
        }

        public Panel LeftSpace { get { return this.boxLeft; } }
        public Panel RightSpace { get { return this.boxRight; } }
        public Panel TopSpace { get { return this.boxTop; } }
        public Panel BottomSpace { get { return this.boxBottom; } }
        public Panel CentralSpace { get { return this.boxCentral; } }

        public void SetLeftSpaceWidth(int w)
        {

            this.dockspaceController.SetLeftSpaceWidth(w);
            this.ninespaceGrippers.UpdateGripperPositions();

        }
        public void SetRightSpaceWidth(int w)
        {
            this.dockspaceController.SetRightSpaceWidth(w);
            this.ninespaceGrippers.UpdateGripperPositions();
        }


    }

}