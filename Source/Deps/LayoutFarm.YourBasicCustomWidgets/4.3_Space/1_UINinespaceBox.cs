//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class NinespaceBox : EaseBox
    {
        SimpleBox boxLeftTop;
        SimpleBox boxRightTop;
        SimpleBox boxLeftBottom;
        SimpleBox boxRightBottom;
        //-------------------------------------
        SimpleBox boxLeft;
        SimpleBox boxTop;
        SimpleBox boxRight;
        SimpleBox boxBottom;
        //-------------------------------------
        SimpleBox boxCentral;
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
        static SimpleBox CreateSpaceBox(SpaceName name, Color bgcolor)
        {
            int controllerBoxWH = 10;
            SimpleBox spaceBox = new SimpleBox(controllerBoxWH, controllerBoxWH);
            spaceBox.BackColor = bgcolor;
            spaceBox.Tag = name;
            return spaceBox;
        }
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
            var gripperBox = new SimpleBox(controllerBoxWH, controllerBoxWH);
            gripperBox.BackColor = bgcolor;
            //---------------------------------------------------------------------

            gripperBox.MouseDrag += (s, e) =>
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
                var renderE = base.GetPrimaryRenderElement(rootgfx);
                //------------------------------------------------------
                renderE.AddChild(boxCentral);
                //------------------------------------------------------
                renderE.AddChild(boxLeftTop);
                renderE.AddChild(boxRightTop);
                renderE.AddChild(boxLeftBottom);
                renderE.AddChild(boxRightBottom);
                //------------------------------------------------------
                renderE.AddChild(boxLeft);
                renderE.AddChild(boxRight);
                renderE.AddChild(boxTop);
                renderE.AddChild(boxBottom);
                //grippers
                if (this.ShowGrippers)
                {
                    renderE.AddChild(gripperLeft);
                    renderE.AddChild(gripperRight);
                    renderE.AddChild(gripperTop);
                    renderE.AddChild(gripperBottom);
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

        public SimpleBox LeftSpace { get { return this.boxLeft; } }
        public SimpleBox RightSpace { get { return this.boxRight; } }
        public SimpleBox TopSpace { get { return this.boxTop; } }
        public SimpleBox BottomSpace { get { return this.boxBottom; } }
        public SimpleBox CentralSpace { get { return this.boxCentral; } }

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

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "ninebox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}