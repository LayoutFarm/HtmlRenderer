//Apache2, 2014-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("3.6 Demo_DragSelectionBox")]
    class Demo_DragSelectionBox : DemoBase
    {
        enum ControllerBoxMode
        {
            SingleBox,
            MultipleBoxes,//eg ctrl + mousedown
        }

        UISelectionBox selectionBox;
        bool selectionBoxIsShown;
        RootGraphic rootgfx;
        Queue<UIControllerBox> userControllerPool = new Queue<UIControllerBox>();
        List<UIControllerBox> workingControllerBoxes = new List<UIControllerBox>();
        ControllerBoxMode controllerBoxMode;
        UIControllerBox singleControllerBox;
        SampleViewport viewport;
        LayoutFarm.CustomWidgets.SimpleBox bgbox;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.viewport = viewport;
            this.rootgfx = viewport.ViewportControl.RootGfx;
            //--------------------------------

            bgbox = new LayoutFarm.CustomWidgets.SimpleBox(800, 600);
            bgbox.BackColor = Color.White;
            bgbox.SetLocation(0, 0);
            SetupBackgroundProperties(bgbox);
            viewport.AddContent(bgbox);
            //user box1
            var box1 = new LayoutFarm.CustomWidgets.SimpleBox(150, 150);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            SetupActiveBoxProperties(box1);
            bgbox.AddChild(box1);
            var box2 = new LayoutFarm.CustomWidgets.SimpleBox(60, 60);
            box2.BackColor = Color.Yellow;
            box2.SetLocation(50, 50);
            SetupActiveBoxProperties(box2);
            bgbox.AddChild(box2);
            var box3 = new LayoutFarm.CustomWidgets.SimpleBox(60, 60);
            box3.BackColor = Color.OrangeRed;
            box3.SetLocation(200, 80);
            SetupActiveBoxProperties(box3);
            bgbox.AddChild(box3);
            selectionBox = new UISelectionBox(1, 1);
            selectionBox.Visible = false;
            selectionBox.BackColor = Color.FromArgb(80, Color.Green);
            viewport.AddContent(selectionBox);
            SetupSelectionBoxProperties(selectionBox);
        }

        UIControllerBox GetFreeUserControllerBox()
        {
            if (userControllerPool.Count > 0)
            {
                var controlBox = userControllerPool.Dequeue();
                //-------------------------------------------
                //register to working box list
                workingControllerBoxes.Add(controlBox);
                return controlBox;
            }
            else
            {
                //create new one

                //controller box 1 (red corners)
                var controllerBox1 = new UIControllerBox(40, 40);
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
                controllerBox1.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                controllerBox1.Visible = false;
                SetupControllerBoxProperties(controllerBox1);
                //-------------------------------------------
                //register to working box list
                workingControllerBoxes.Add(controllerBox1);
                return controllerBox1;
            }
        }
        void ReleaseUserControllerBox(UIControllerBox userControllerBox)
        {
            workingControllerBoxes.Remove(userControllerBox);
            userControllerBox.Visible = false;
            userControllerBox.TargetBox = null;
            userControllerBox.RemoveSelf();
            this.userControllerPool.Enqueue(userControllerBox);
        }
        void RemoveAllUserControllerBoxes()
        {
            int j = this.workingControllerBoxes.Count;
            for (int i = j - 1; i >= 0; --i)
            {
                var userControllerBox = this.workingControllerBoxes[i];
                userControllerBox.Visible = false;
                userControllerBox.TargetBox = null;
                userControllerBox.RemoveSelf();
                userControllerPool.Enqueue(userControllerBox);
            }
            this.workingControllerBoxes.Clear();
        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.EaseBox backgroundBox)
        {
            //if click on background
            backgroundBox.MouseDown += (s, e) =>
            {
                //remove all controller box
                RemoveAllUserControllerBoxes();
            };
            //when start drag on bg
            //just show selection box on top most
            backgroundBox.MouseDrag += (s, e) =>
            {
                //move to mouse position 
                if (!selectionBoxIsShown)
                {
                    selectionBox.SetLocation(e.X, e.Y);
                    selectionBox.Visible = true;
                    selectionBoxIsShown = true;
                }
                else
                {
                    int x = e.CapturedMouseX;
                    int y = e.CapturedMouseY;
                    int w = e.DiffCapturedX;
                    int h = e.DiffCapturedY;
                    if (w < 0)
                    {
                        w = -w;
                        x -= w;
                    }
                    if (h < 0)
                    {
                        h = -h;
                        y -= h;
                    }
                    //set width and height
                    selectionBox.SetBounds(x, y, w, h);
                }
            };
            backgroundBox.MouseUp += (s, e) =>
            {
                if (!selectionBoxIsShown)
                {
                    FindSelectedUserBoxes();
                    selectionBox.Visible = false;
                    selectionBox.SetSize(1, 1);
                    selectionBoxIsShown = false;
                }
            };
        }

        void FindSelectedUserBoxes()
        {
            //find users box in selected area



            int j = this.bgbox.ChildCount;
            var primSelectionBox = selectionBox.GetPrimaryRenderElement(rootgfx);
            var primGlobalPoint = primSelectionBox.GetGlobalLocation();
            var selectedRectArea = new Rectangle(primGlobalPoint, primSelectionBox.Size);
            List<UIBox> selectedList = new List<UIBox>();
            for (int i = 0; i < j; ++i)
            {
                var box = bgbox.GetChild(i) as UIBox;
                if (box == null)
                {
                    continue;
                }
                var primElement = box.GetPrimaryRenderElement(rootgfx);
                if (!primElement.Visible)
                {
                    continue;
                }
                //get global area 
                Point globalLocation = primElement.GetGlobalLocation();
                var userElementArea = new Rectangle(globalLocation, primElement.Size);
                if (selectedRectArea.Contains(userElementArea))
                {
                    //selected= true;
                    selectedList.Add(box);
                    //------
                    //create user controller box for the selected box 
                    UIControllerBox userControllerBox = GetFreeUserControllerBox();
                    userControllerBox.TargetBox = box;
                    var globalTargetPos = box.GetGlobalLocation();
                    userControllerBox.SetLocation(globalTargetPos.X - 5, globalTargetPos.Y - 5);
                    userControllerBox.SetSize(box.Width + 10, box.Height + 10);
                    userControllerBox.Visible = true;
                    viewport.AddContent(userControllerBox);
                }
            }
        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.EaseBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                UIControllerBox userControllerBox = null;
                if (this.controllerBoxMode == ControllerBoxMode.MultipleBoxes)
                {
                    //multiple box
                    userControllerBox = GetFreeUserControllerBox();
                }
                else
                {
                    //single box
                    if (singleControllerBox != null)
                    {
                        ReleaseUserControllerBox(singleControllerBox);
                    }
                    //get new one
                    userControllerBox = singleControllerBox = GetFreeUserControllerBox();
                }
                //request user controller for this box
                userControllerBox.TargetBox = box;
                viewport.AddContent(userControllerBox);
                //location relative to global position of target box
                var globalTargetPos = box.GetGlobalLocation();
                userControllerBox.SetLocation(globalTargetPos.X - 5, globalTargetPos.Y - 5);
                userControllerBox.SetSize(box.Width + 10, box.Height + 10);
                userControllerBox.Visible = true;
                //move mouse capture to new controller box
                //for next drag
                e.SetMouseCapture(userControllerBox);
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                RemoveAllUserControllerBoxes();
            };
            box.DragOver += (s, e) =>
            {
                switch (e.UserMsgFlags)
                {
                    case 1:
                        {   //sample only
                            box.BackColor = Color.Green;
                        }
                        break;
                    case 2:
                        {
                            //sample only
                            //leaving
                            box.BackColor = Color.Blue;
                        }
                        break;
                    case 3:
                        {
                            //drop
                            var sender = e.Sender as UIControllerBox;
                            var droppingBox = sender.TargetBox as UIBox;
                            if (droppingBox != null)
                            {
                                //move from original 
                                var parentBox = droppingBox.ParentUI as LayoutFarm.CustomWidgets.SimpleBox;
                                if (parentBox != null)
                                {
                                    var newParentGlobalLoca = box.GetGlobalLocation();
                                    var droppingGlobalLoca = droppingBox.GetGlobalLocation();
                                    parentBox.RemoveChild(droppingBox);
                                    box.AddChild(droppingBox);
                                    //TODO: review here , 
                                    //set location relative to other element
                                    droppingBox.SetLocation(
                                        droppingGlobalLoca.X - newParentGlobalLoca.X,
                                        droppingGlobalLoca.Y - newParentGlobalLoca.Y);
                                }
                                else
                                {
                                }
                            }
                        }
                        break;
                }
            };
        }

        void SetupSelectionBoxProperties(UISelectionBox selectionBox)
        {
            selectionBox.MouseUp += (s, e) =>
            {
                if (selectionBoxIsShown)
                {
                    FindSelectedUserBoxes();
                    selectionBox.Visible = false;
                    selectionBox.SetSize(1, 1);
                    selectionBoxIsShown = false;
                }
            };
            selectionBox.MouseDrag += (s, e) =>
            {
                if (selectionBoxIsShown)
                {
                    Point sel_globalLocation = selectionBox.GetGlobalLocation();
                    int x = e.CapturedMouseX;
                    int y = e.CapturedMouseY;
                    //temp fix here 
                    //TODO: get global position of selected box

                    int w = selectionBox.Left + e.DiffCapturedX;
                    int h = selectionBox.Top + e.DiffCapturedY;
                    if (w < 0)
                    {
                        w = -w;
                        x -= w;
                    }
                    if (h < 0)
                    {
                        h = -h;
                        y -= h;
                    }
                    //set width and height
                    selectionBox.SetBounds(x, y, w, h);
                    e.StopPropagation();
                }
            };
        }
        static void MoveWithSnapToGrid(UIControllerBox controllerBox, int dx, int dy)
        {
            //sample move with snap to grid
            Point pos = controllerBox.Position;
            int newX = pos.X + dx;
            int newY = pos.Y + dy;
            int gridSize = 5;
            float halfGrid = (float)gridSize / 2f;
            int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
            int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;
            controllerBox.SetLocation(nearestX, nearestY);
            UIBox targetBox = controllerBox.TargetBox;
            if (targetBox != null)
            {
                int xdiff = nearestX - pos.X;
                int ydiff = nearestY - pos.Y;
                targetBox.SetLocation(targetBox.Left + xdiff, targetBox.Top + ydiff);
            }
        }
        List<LayoutFarm.UI.UIBox> FindUnderlyingElements(UIControllerBox controllerBox)
        {
            var dragOverElements = new List<LayoutFarm.UI.UIBox>();
            Rectangle controllerBoxArea = controllerBox.Bounds;
            int j = bgbox.ChildCount;
            for (int i = 0; i < j; ++i)
            {
                var box = bgbox.GetChild(i) as LayoutFarm.UI.UIBox;
                if (box == null || controllerBox.TargetBox == box)
                {
                    continue;
                }

                //------------- 
                if (controllerBoxArea.IntersectsWith(box.Bounds))
                {
                    dragOverElements.Add(box);
                }
            }
            return dragOverElements;
        }
        void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box
            controllerBox.MouseUp += (s, e) =>
            {
                if (e.IsDragging)
                {
                    //this is dropping 
                    //find underlying element to drop into  

                    var dragOverElements = FindUnderlyingElements(controllerBox);
                    if (dragOverElements.Count > 0)
                    {
                        //TODO: review here
                        //this version we select the first one

                        var listener = dragOverElements[0] as IEventListener;
                        if (listener != null)
                        {
                            var talkMsg = new UIGuestTalkEventArgs();
                            talkMsg.Sender = controllerBox;
                            talkMsg.UserMsgFlags = 3;//send drop notify                             
                            listener.ListenGuestTalk(talkMsg);
                        }
                    }
                }
            };
            controllerBox.MouseDrag += (s, e) =>
            {
                MoveWithSnapToGrid(controllerBox, e.DiffCapturedX, e.DiffCapturedY);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
                //test here -----------------------------------------------------
                //find dragover element ***

                var dragOverElements = FindUnderlyingElements(controllerBox);
                if (dragOverElements.Count == 0)
                {
                    Dictionary<UIElement, int> prevDragOverElements = controllerBox.LastestDragOverElements;
                    if (prevDragOverElements != null)
                    {
                        foreach (var leavingElement in prevDragOverElements.Keys)
                        {
                            var listener = leavingElement as IEventListener;
                            if (listener != null)
                            {
                                var talkMsg = new UIGuestTalkEventArgs();
                                talkMsg.Sender = controllerBox;
                                talkMsg.UserMsgFlags = 2;//leaving //sample only
                                listener.ListenGuestTalk(talkMsg);
                            }
                        }
                    }

                    controllerBox.LastestDragOverElements = null;
                }
                else
                {
                    //send notification to another box 
                    //use guest talk msg*** 
                    //check state of guest 
                    Dictionary<UIElement, int> prevDragOverElements = controllerBox.LastestDragOverElements;
                    Dictionary<UIElement, int> latestDragOverElements = new Dictionary<UIElement, int>();
                    if (prevDragOverElements != null)
                    {
                        int j = dragOverElements.Count;
                        for (int i = 0; i < j; ++i)
                        {
                            var dragOverE = dragOverElements[i];
                            int state;
                            if (prevDragOverElements.TryGetValue(dragOverE, out state))
                            {
                                //found in latest drag overelement 
                                //remove 
                                prevDragOverElements.Remove(dragOverE);
                            }

                            latestDragOverElements.Add(dragOverE, ++state);
                        }
                        //remaining elements
                        foreach (var leavingElement in prevDragOverElements.Keys)
                        {
                            var listener = leavingElement as IEventListener;
                            if (listener != null)
                            {
                                var talkMsg = new UIGuestTalkEventArgs();
                                talkMsg.Sender = controllerBox;
                                talkMsg.UserMsgFlags = 2;//leaving //sample only
                                listener.ListenGuestTalk(talkMsg);
                            }
                        }
                    }
                    else
                    {
                        foreach (var drgElement in dragOverElements)
                        {
                            latestDragOverElements.Add(drgElement, 0);
                        }
                    }

                    controllerBox.LastestDragOverElements = latestDragOverElements;
                    foreach (var drgElement in latestDragOverElements.Keys)
                    {
                        var listener = drgElement as IEventListener;
                        if (listener != null)
                        {
                            var talkMsg = new UIGuestTalkEventArgs();
                            talkMsg.Sender = controllerBox;
                            talkMsg.UserMsgFlags = 1;//sample only
                            listener.ListenGuestTalk(talkMsg);
                        }
                    }
                }
            };
        }

        //-----------------------------------------------------------------
        class UIControllerBox : LayoutFarm.CustomWidgets.EaseBox
        {
            LayoutFarm.CustomWidgets.GridBox gridBox;
            //small controller box
            LayoutFarm.CustomWidgets.EaseBox boxLeftTop;
            LayoutFarm.CustomWidgets.EaseBox boxRightTop;
            LayoutFarm.CustomWidgets.EaseBox boxLeftBottom;
            LayoutFarm.CustomWidgets.EaseBox boxRightBottom;
            DockSpacesController dockspaceController;
            Dictionary<UIElement, int> latestDragOverElements;
            LayoutFarm.UI.UIBox targetBox;
            public UIControllerBox(int w, int h)
                : base(w, h)
            {
                SetupDockSpaces();
            }
            public LayoutFarm.UI.UIBox TargetBox
            {
                get { return this.targetBox; }
                set
                {
                    this.targetBox = value;
                    //clear latest drag over elements too                    
                    this.latestDragOverElements = null;
                }
            }
            public Dictionary<UIElement, int> LastestDragOverElements
            {
                get { return this.latestDragOverElements; }
                set { this.latestDragOverElements = value; }
            }
            public override void Walk(UIVisitor visitor)
            {
                visitor.BeginElement(this, "ctrlbox");
                this.Describe(visitor);
                visitor.EndElement();
            }
            //get primary render element
            public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
            {
                if (!this.HasReadyRenderElement)
                {
                    int gridW = this.Width - 10;
                    int gridH = this.Height - 10;
                    if (gridW < 3)
                    {
                        gridW = 3;
                    }
                    if (gridH < 3)
                    {
                        gridH = 3;
                    }

                    gridBox = new LayoutFarm.CustomWidgets.GridBox(gridW, gridH);
                    gridBox.SetLocation(5, 5);
                    gridBox.BuildGrid(3, 3, CellSizeStyle.UniformCell);
                    var renderE = base.GetPrimaryRenderElement(rootgfx);
                    renderE.AddChild(gridBox);
                    //------------------------------------------------------
                    renderE.AddChild(boxLeftTop);
                    renderE.AddChild(boxRightTop);
                    renderE.AddChild(boxLeftBottom);
                    renderE.AddChild(boxRightBottom);
                    //------------------------------------------------------
                }
                return base.GetPrimaryRenderElement(rootgfx);
            }

            public override void SetSize(int width, int height)
            {
                base.SetSize(width, height);
                //---------------------------------
                if (gridBox != null)
                {
                    //adjust grid size
                    gridBox.SetSize(width - 10, height - 10);
                }
                //---------------------------------
                this.dockspaceController.SetSize(width, height);
            }

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

            CustomWidgets.EaseBox CreateTinyControlBox(SpaceName name)
            {
                int controllerBoxWH = 10;
                var tinyBox = new CustomWidgets.SimpleBox(controllerBoxWH, controllerBoxWH);
                tinyBox.BackColor = PixelFarm.Drawing.Color.Red;
                tinyBox.Tag = name;
                //add handler for each tiny box
                //---------------------------------------------------------------------

                tinyBox.MouseDrag += (s, e) =>
                {
                    ResizeTargetWithSnapToGrid((SpaceName)tinyBox.Tag, this, e.DiffCapturedX, e.DiffCapturedY);
                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                    e.CancelBubbling = true;
                };
                tinyBox.MouseUp += (s, e) =>
                {
                    e.MouseCursorStyle = MouseCursorStyle.Default;
                    e.CancelBubbling = true;
                };
                return tinyBox;
            }

            static void ResizeTargetWithSnapToGrid(SpaceName tinyBoxSpaceName, UIControllerBox controllerBox, int dx, int dy)
            {
                //sample move with snap to grid
                Point pos = controllerBox.Position;
                int newX = pos.X + dx;
                int newY = pos.Y + dy;
                //snap to gridsize =5;
                //find nearest snap x 
                int gridSize = 5;
                float halfGrid = (float)gridSize / 2f;
                int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
                int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;
                int xdiff = nearestX - pos.X;
                int ydiff = nearestY - pos.Y;
                switch (tinyBoxSpaceName)
                {
                    case SpaceName.LeftTop:
                        {
                            if (xdiff != 0 || ydiff != 0)
                            {
                                controllerBox.SetBounds(controllerBox.Left + xdiff, controllerBox.Top + ydiff,
                                    controllerBox.Width - xdiff, controllerBox.Height - ydiff);
                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    //move target box too                                     
                                    targetBox.SetBounds(targetBox.Left + xdiff, targetBox.Top + ydiff,
                                        targetBox.Width - xdiff, targetBox.Height - ydiff);
                                }
                            }
                        }
                        break;
                    case SpaceName.RightTop:
                        {
                            if (xdiff != 0 || ydiff != 0)
                            {
                                controllerBox.SetBounds(controllerBox.Left, controllerBox.Top + ydiff,
                                    controllerBox.Width + xdiff, controllerBox.Height - ydiff);
                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    targetBox.SetBounds(targetBox.Left, targetBox.Top + ydiff,
                                        targetBox.Width + xdiff, targetBox.Height - ydiff);
                                }
                            }
                        }
                        break;
                    case SpaceName.RightBottom:
                        {
                            if (xdiff != 0 || ydiff != 0)
                            {
                                controllerBox.SetSize(controllerBox.Width + xdiff, controllerBox.Height + ydiff);
                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    //move target box too 
                                    targetBox.SetSize(targetBox.Width + xdiff, targetBox.Height + ydiff);
                                }
                            }
                        }
                        break;
                    case SpaceName.LeftBottom:
                        {
                            if (xdiff != 0 || ydiff != 0)
                            {
                                controllerBox.SetBounds(controllerBox.Left + xdiff, controllerBox.Top,
                                    controllerBox.Width - xdiff, controllerBox.Height + ydiff);
                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    //move target box too 
                                    targetBox.SetBounds(targetBox.Left + xdiff, targetBox.Top,
                                        targetBox.Width - xdiff, targetBox.Height + ydiff);
                                }
                            }
                        }
                        break;
                }
            }

            protected override void OnGuestTalk(UIGuestTalkEventArgs e)
            {
                //test ***
                this.BackColor = Color.Green;
                base.OnGuestTalk(e);
            }
        }

        class UISelectionBox : LayoutFarm.CustomWidgets.EaseBox
        {
            public UISelectionBox(int w, int h)
                : base(w, h)
            {
            }
            public override void Walk(UIVisitor visitor)
            {
                visitor.BeginElement(this, "selectbox");
                this.Describe(visitor);
                visitor.EndElement();
            }
        }
    }
}