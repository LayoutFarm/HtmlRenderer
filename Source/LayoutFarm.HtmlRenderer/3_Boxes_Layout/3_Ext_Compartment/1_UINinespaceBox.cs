// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm;


namespace LayoutFarm.HtmlBoxes
{

    class NinespaceBox : LayoutBox
    {
        LayoutBox boxLeftTop;
        LayoutBox boxRightTop;
        LayoutBox boxLeftBottom;
        LayoutBox boxRightBottom;
        //-------------------------------------
        LayoutBox boxLeft;
        LayoutBox boxTop;
        LayoutBox boxRight;
        LayoutBox boxBottom;
        //-------------------------------------
        LayoutBox boxCentral;

        DockSpacesController dockspaceController;
        int boxW;
        int boxH;
        public NinespaceBox(int w, int h)
        {
            SetSize(w, h);
            SetupDockSpaces(SpaceConcept.NineSpace);
        }
        public NinespaceBox(int w, int h, SpaceConcept spaceConcept)
        {
            SetSize(w, h);
            SetupDockSpaces(spaceConcept);
        }

        public bool ShowGrippers
        {
            get;
            set;
        }
        static LayoutBox CreateSpaceBox(SpaceName name)
        {
            int controllerBoxWH = 10;
            LayoutBox spaceBox = new LayoutBox(controllerBoxWH, controllerBoxWH);
            //spaceBox.BackColor = bgcolor;
            //spaceBox.Tag = name;
            return spaceBox;
        }

        void SetupDockSpaces(SpaceConcept spaceConcept)
        {
            //1. controller
            this.dockspaceController = new DockSpacesController(this, spaceConcept);

            //2.  
            this.dockspaceController.LeftTopSpace.Content = boxLeftTop = CreateSpaceBox(SpaceName.LeftTop);
            this.dockspaceController.RightTopSpace.Content = boxRightTop = CreateSpaceBox(SpaceName.RightTop);
            this.dockspaceController.LeftBottomSpace.Content = boxLeftBottom = CreateSpaceBox(SpaceName.LeftBottom);
            this.dockspaceController.RightBottomSpace.Content = boxRightBottom = CreateSpaceBox(SpaceName.RightBottom);
            //3.
            this.dockspaceController.LeftSpace.Content = boxLeft = CreateSpaceBox(SpaceName.Left);
            this.dockspaceController.TopSpace.Content = boxTop = CreateSpaceBox(SpaceName.Top);
            this.dockspaceController.RightSpace.Content = boxRight = CreateSpaceBox(SpaceName.Right);
            this.dockspaceController.BottomSpace.Content = boxBottom = CreateSpaceBox(SpaceName.Bottom);

            this.dockspaceController.CenterSpace.Content = boxCentral = CreateSpaceBox(SpaceName.Center);
            dockspaceController.SetRightSpaceWidth(200);
            dockspaceController.SetLeftSpaceWidth(200);
        }
        public void SetDockSpaceConcept(SpaceConcept concept)
        {

        }

        public void SetSize(int width, int height)
        {
            this.boxW = width;
            this.boxH = height;
            dockspaceController.SetSize(width, height);
        }
        public void PerformContentLayout()
        {
            dockspaceController.ArrangeAllSpaces();
        }

        public LayoutBox LeftSpace { get { return this.boxLeft; } }
        public LayoutBox RightSpace { get { return this.boxRight; } }
        public LayoutBox TopSpace { get { return this.boxTop; } }
        public LayoutBox BottomSpace { get { return this.boxBottom; } }
        public LayoutBox CentralSpace { get { return this.boxCentral; } }

        public void SetLeftSpaceWidth(int w)
        {

            this.dockspaceController.SetLeftSpaceWidth(w);
            //this.ninespaceGrippers.UpdateGripperPositions();

        }
        public void SetRightSpaceWidth(int w)
        {
            this.dockspaceController.SetRightSpaceWidth(w);
            //this.ninespaceGrippers.UpdateGripperPositions();
        }



    }

}