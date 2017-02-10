//Apache2, 2014-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.DzBoardSample
{
    class UIControllerBox : LayoutFarm.CustomWidgets.EaseBox
    {
        LayoutFarm.UI.UIBox box;
        LayoutFarm.CustomWidgets.EaseBox contentBox;
        //small controller box
        LayoutFarm.CustomWidgets.EaseBox boxLeftTop;
        LayoutFarm.CustomWidgets.EaseBox boxRightTop;
        LayoutFarm.CustomWidgets.EaseBox boxLeftBottom;
        LayoutFarm.CustomWidgets.EaseBox boxRightBottom;
        DockSpacesController dockspaceController;
        Rectangle beginRect;
        public UIControllerBox(int w, int h)
            : base(w, h)
        {
            SetupDockSpaces();
            this.GridSize = 5;
        }

        public DesignBoardModule DesignBoardModule
        {
            get;
            set;
        }
        public int GridSize
        {
            get;
            set;
        }
        public SelectionSet MasterSelectionSet
        {
            get;
            set;
        }
        public LayoutFarm.CustomWidgets.EaseBox ContentBox
        {
            get { return this.contentBox; }
            set
            {
                this.contentBox = value;
            }
        }
        public LayoutFarm.UI.UIBox TargetBox
        {
            get { return this.box; }
            set
            {
                this.box = value;
                //record
                if (value != null)
                {
                    beginRect = new Rectangle(value.Left, value.Top, value.Width, value.Height);
                }
            }
        }
        public void ReleaseTarget()
        {
            this.TargetBox = null;
            this.MasterSelectionSet = null;
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                var renderE = base.GetPrimaryRenderElement(rootgfx);
                if (contentBox != null)
                {
                    renderE.AddChild(contentBox);
                }

                //------------------------------------------------------

                renderE.AddChild(boxLeftTop);
                renderE.AddChild(boxRightTop);
                renderE.AddChild(boxLeftBottom);
                renderE.AddChild(boxRightBottom);
                //------------------------------------------------------
            }
            return base.GetPrimaryRenderElement(rootgfx);
        }
        public override void SetLocation(int left, int top)
        {
            var targetBox = this.TargetBox;
            int diffX = 0, diffY = 0;
            if (targetBox != null)
            {
                var targetBoxLocation = targetBox.GetGlobalLocation();
                diffX = left - targetBoxLocation.X;
                diffY = top - targetBoxLocation.Y;
            }
            base.SetLocation(left, top);
            //move target box together 
            if (targetBox != null)
            {
                //move target box too
                //get global 
                //targetBox.SetLocation(left + this.GridSize, top + this.GridSize);
                targetBox.SetLocation(
                    targetBox.Left + diffX + this.GridSize,
                    targetBox.Top + diffY + this.GridSize);
            }
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            if (e.Ctrl)
            {
                switch (e.KeyCode)
                {
                    case UIKeys.C:
                        {
                            //copy this box to a clipboard***
                            //copy this content to clipboard 
                            this.DesignBoardModule.CopyToClipBoard(this.TargetBox as IDesignBox);
                        }
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case UIKeys.Delete:
                        {
                            //-------------------------------------
                            //add action history to design board?
                            //------------------------------------- 
                            //remove the target and its controller 
                            var masterCollectionSet = this.MasterSelectionSet;
                            this.RelaseTargetAndRemoveSelf();
                            if (masterCollectionSet != null)
                            {
                                //remove other from selection set too!
                                masterCollectionSet.NotifyRemoveUIElement(this);
                            }
                        }
                        break;
                }
            }
            base.OnKeyDown(e);
        }
        internal void RelaseTargetAndRemoveSelf()
        {
            var target = this.TargetBox as IEventListener;
            this.RemoveTarget(target);
            this.ReleaseTarget();
            this.RemoveSelf();
        }

        void RemoveTarget(IEventListener target)
        {
            //remove target from its parent             
            this.DesignBoardModule.RemoveTargetBox(target as IDesignBox);
        }

        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                MoveWithSnapToGrid(this, e.DiffCapturedX, e.DiffCapturedY);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                MoveWithSnapToGrid(this, e);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.StopPropagation();
            }
            base.OnMouseLeave(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            //create commadn history and add?

            var newBeginPoint = new Point(this.TargetBox.Left, this.TargetBox.Top);
            this.DesignBoardModule.HistoryRecordDzElementNewPosition(
                this.TargetBox,
                new Point(this.beginRect.Left, this.beginRect.Top),
                newBeginPoint);
            this.beginRect = new Rectangle(newBeginPoint.X, newBeginPoint.Y, this.TargetBox.Width, this.TargetBox.Height);
            base.OnMouseUp(e);
        }

        static void MoveWithSnapToGrid(UIControllerBox controllerBox, UIMouseEventArgs e)
        {
            //sample move with snap to grid

            Point pos = controllerBox.Position;
            int newX = pos.X + e.XDiff;
            int newY = pos.Y + e.YDiff;
            //snap to gridsize =5;
            //find nearest snap x 

            int gridSize = controllerBox.GridSize;
            float halfGrid = (float)gridSize / 2f;
            int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
            int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;
            controllerBox.MoveTo(nearestX, nearestY);
        }
        static void MoveWithSnapToGrid(UIControllerBox controllerBox, int dx, int dy)
        {
            //sample move with snap to grid

            Point pos = controllerBox.Position;
            int newX = pos.X + dx;
            int newY = pos.Y + dy;
            //snap to gridsize =5;
            //find nearest snap x 

            int gridSize = controllerBox.GridSize;
            float halfGrid = (float)gridSize / 2f;
            int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
            int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;
            controllerBox.MoveTo(nearestX, nearestY);
        }

        public void MoveTo(int left, int top)
        {
            var offsetX = left - this.Left;
            var offsetY = top - this.Top;
            SetLocation(left, top);
            if (this.MasterSelectionSet != null)
            {
                MasterSelectionSet.NotifyMoveFrom(this, offsetX, offsetY);
            }
        }
        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            //---------------------------------
            if (contentBox != null)
            {
                //adjust grid size
                contentBox.SetSize(width - 10, height - 10);
            }
            this.dockspaceController.SetSize(width, height);
            //---------------------------------
        }
        //-----
        void SetupDockSpaces()
        {
            //1. controller
            this.dockspaceController = new DockSpacesController(this, SpaceConcept.NineSpace);
            //2.  
            this.dockspaceController.LeftTopSpace.Content = boxLeftTop = CreateTinyControlBox(SpaceName.LeftTop);
            this.dockspaceController.RightTopSpace.Content = boxRightTop = CreateTinyControlBox(SpaceName.RightTop);
            this.dockspaceController.LeftBottomSpace.Content = boxLeftBottom = CreateTinyControlBox(SpaceName.LeftBottom);
            this.dockspaceController.RightBottomSpace.Content = boxRightBottom = CreateTinyControlBox(SpaceName.RightBottom);
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "ctrlbox");
            this.Describe(visitor);
            visitor.EndElement();
        }

        CustomWidgets.EaseBox CreateTinyControlBox(SpaceName name)
        {
            int controllerBoxWH = 10;
            CustomWidgets.EaseBox tinyBox = new CustomWidgets.SimpleBox(controllerBoxWH, controllerBoxWH);
            tinyBox.BackColor = PixelFarm.Drawing.Color.Red;
            tinyBox.Tag = name;
            //add handler for each tiny box 

            tinyBox.MouseDrag += (s, e) =>
            {
                ResizeTargetWithSnapToGrid2((SpaceName)tinyBox.Tag, this, e, e.DiffCapturedX, e.DiffCapturedY);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
            tinyBox.MouseUp += (s, e) =>
            {
                if (e.IsDragging)
                {
                    ResizeTargetWithSnapToGridWhenDragRelease(this, e);
                }
                //if (e.IsDragging)
                //{
                //    //var targetBox = this.TargetBox;
                //    //var newRect = new Rectangle(targetBox.Left, targetBox.Top, targetBox.Width, targetBox.Height);

                //    //this.DesignBoardModule.HistoryRecordDzElementNewBounds(
                //    //  targetBox,
                //    //  this.beginRect,
                //    //  newRect);
                //    //this.beginRect = newRect;

                //    //ResizeTargetWithSnapToGridWhenMouseUp(this, e);
                //}
                e.MouseCursorStyle = MouseCursorStyle.Default;
                e.CancelBubbling = true;
            };
            return tinyBox;
        }
        void ResizeTargetWithSnapToGrid2(SpaceName tinyBoxSpaceName, UIControllerBox controllerBox, UIMouseEventArgs e, int dx, int dy)
        {
            //sample move with snap to grid
            Point pos = controllerBox.Position;
            int newX = pos.X + dx;// e.XDiff;
            int newY = pos.Y + dy;// e.YDiff;
            //snap to gridsize =5;
            //find nearest snap x 
            int gridSize = 5;
            float halfGrid = (float)gridSize / 2f;
            int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
            int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;
            int xdiff = nearestX - pos.X;
            int ydiff = nearestY - pos.Y;
            int diffX = 0, diffY = 0;
            var targetBox = controllerBox.TargetBox;
            if (targetBox != null)
            {
                var targetBoxLocation = targetBox.GetGlobalLocation();
                diffX = this.Left - targetBoxLocation.X;
                diffY = this.Top - targetBoxLocation.Y;
            }


            switch (tinyBoxSpaceName)
            {
                case SpaceName.LeftTop:
                    {
                        if (xdiff != 0 || ydiff != 0)
                        {
                            controllerBox.SetLocation(controllerBox.Left + xdiff, controllerBox.Top + ydiff);
                            controllerBox.SetSize(controllerBox.Width - xdiff, controllerBox.Height - ydiff);
                            if (targetBox != null)
                            {
                                //move target box too 
                                targetBox.SetBounds(targetBox.Left + diffX + 5,
                                    targetBox.Top + diffY + 5,
                                    controllerBox.Width - 10,
                                    controllerBox.Height - 10);
                            }
                        }
                    }
                    break;
                case SpaceName.RightTop:
                    {
                        if (xdiff != 0 || ydiff != 0)
                        {
                            controllerBox.SetLocation(controllerBox.Left, controllerBox.Top + ydiff);
                            controllerBox.SetSize(controllerBox.Width + xdiff, controllerBox.Height - ydiff);
                            if (targetBox != null)
                            {
                                //move target box too 
                                targetBox.SetBounds(targetBox.Left + diffX + 5,
                                   targetBox.Top + diffY + 5,
                                   controllerBox.Width - 10,
                                   controllerBox.Height - 10);
                            }
                        }
                    }
                    break;
                case SpaceName.RightBottom:
                    {
                        if (xdiff != 0 || ydiff != 0)
                        {
                            controllerBox.SetSize(controllerBox.Width + xdiff, controllerBox.Height + ydiff);
                            if (targetBox != null)
                            {
                                //move target box too 
                                targetBox.SetBounds(targetBox.Left + diffX + 5,
                                   targetBox.Top + diffY + 5,
                                   controllerBox.Width - 10,
                                   controllerBox.Height - 10);
                            }
                        }
                    }
                    break;
                case SpaceName.LeftBottom:
                    {
                        if (xdiff != 0 || ydiff != 0)
                        {
                            controllerBox.SetLocation(controllerBox.Left + xdiff, controllerBox.Top);
                            controllerBox.SetSize(controllerBox.Width - xdiff, controllerBox.Height + ydiff);
                            if (targetBox != null)
                            {
                                //move target box too 
                                targetBox.SetBounds(targetBox.Left + diffX + 5,
                                   targetBox.Top + diffY + 5,
                                   controllerBox.Width - 10,
                                   controllerBox.Height - 10);
                            }
                        }
                    }
                    break;
            }
        }


        static void ResizeTargetWithSnapToGridWhenDragRelease(UIControllerBox controllerBox, UIMouseEventArgs e)
        {
            ////sample move with snap to grid
            //Point pos = controllerBox.Position;
            //int newX = pos.X + e.XDiff;
            //int newY = pos.Y + e.YDiff;
            ////snap to gridsize =5;
            ////find nearest snap x 
            //int gridSize = 5;
            //float halfGrid = (float)gridSize / 2f;
            //int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
            //int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;

            //int xdiff = nearestX - pos.X;

            //if (xdiff == 0)
            //{
            //    return;
            //}


            //controllerBox.SetSize(controllerBox.Width + xdiff, controllerBox.Height);

            var targetBox = controllerBox.TargetBox;
            if (targetBox != null)
            {
                int diffX = 0, diffY = 0;
                var targetBoxLocation = targetBox.GetGlobalLocation();
                diffX = controllerBox.Left - targetBoxLocation.X;
                diffY = controllerBox.Top - targetBoxLocation.Y;
                //move target box too 
                var newRect = new Rectangle(targetBox.Left + diffX + 5,
                    targetBox.Top + diffY + 5,
                    controllerBox.Width - 10,
                    controllerBox.Height - 10);
                controllerBox.DesignBoardModule.HistoryRecordDzElementNewBounds(
                    controllerBox.TargetBox,
                    controllerBox.beginRect,
                    newRect);
                controllerBox.beginRect = newRect;
                targetBox.SetBounds(newRect.Left,
                    newRect.Top,
                    newRect.Width,
                    newRect.Height);
            }
        }
    }


    class UISelectionBox : LayoutFarm.CustomWidgets.EaseBox
    {
        public UISelectionBox(int w, int h)
            : base(w, h)
        {
        }
        public Point LandingPoint
        {
            get;
            set;
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                var myRenderElement = base.GetPrimaryRenderElement(rootgfx) as LayoutFarm.CustomWidgets.CustomRenderBox;
            }
            return base.GetPrimaryRenderElement(rootgfx);
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "selectbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
    enum BoxSelectionMode
    {
        Single,
        Multiple
    }

    class SelectionSet
    {
        List<UIControllerBox> selectionList = new List<UIControllerBox>();
        public void AddSelection(UIControllerBox selectionBox)
        {
            selectionBox.MasterSelectionSet = this;
            this.selectionList.Add(selectionBox);
        }
        public void Clear()
        {
            this.selectionList.Clear();
        }
        public void NotifyMoveFrom(UIControllerBox src, int offsetX, int offsetY)
        {
            for (int i = selectionList.Count - 1; i >= 0; --i)
            {
                var controlBox = selectionList[i];
                if (controlBox != src)
                {
                    controlBox.SetLocation(
                         controlBox.Left + offsetX,
                         controlBox.Top + offsetY);
                }
            }
        }
        public void NotifyRemoveUIElement(UIControllerBox src)
        {
            for (int i = selectionList.Count - 1; i >= 0; --i)
            {
                var controlBox = selectionList[i];
                if (controlBox != src)
                {
                    //send to controller box to remove  
                    var dzModule = controlBox.DesignBoardModule;
                    var target = controlBox.TargetBox;
                    controlBox.RelaseTargetAndRemoveSelf();
                }
            }
        }
    }
    class DesingBoardBackground : LayoutFarm.CustomWidgets.EaseBox
    {
        public DesingBoardBackground(int w, int h)
            : base(w, h)
        {
            this.AcceptKeyboardFocus = true;
        }

        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            //handle special keydown
            if (e.Ctrl)
            {
                //control                 
                switch (e.KeyCode)
                {
                    case UIKeys.V:
                        {
                            this.DesignBoardModule.PasteClipboardData();
                        }
                        break;
                    case UIKeys.Z:
                        {
                            //undo
                            this.DesignBoardModule.UndoLastAction();
                        }
                        break;
                    case UIKeys.Y:
                        {
                            //redo 
                        }
                        break;
                }
            }
            base.OnKeyDown(e);
        }

        public DesignBoardModule DesignBoardModule
        {
            get;
            set;
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "dzBgBoard");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}
