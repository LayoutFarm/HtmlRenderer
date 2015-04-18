// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{
    [DemoNote("3.6 Demo_DragSelectionBox")]
    class Demo_DragSelectionBox : DemoBase
    {

        UISelectionBox selectionBox;
        bool selectionBoxIsShown;
        RootGraphic rootgfx;
        List<LayoutFarm.CustomWidgets.EaseBox> userBoxes = new List<CustomWidgets.EaseBox>();
        Queue<UIControllerBox> userControllerPool = new Queue<UIControllerBox>();
        List<UIControllerBox> workingControllerBoxes = new List<UIControllerBox>();

        SampleViewport viewport;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.viewport = viewport;
            this.rootgfx = viewport.ViewportControl.RootGfx;
            //--------------------------------

            var bgbox = new LayoutFarm.CustomWidgets.SimpleBox(800, 600);
            bgbox.BackColor = Color.White;
            bgbox.SetLocation(0, 0);
            SetupBackgroundProperties(bgbox);
            viewport.AddContent(bgbox);

            //user box1
            var box1 = new LayoutFarm.CustomWidgets.SimpleBox(150, 150);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            SetupActiveBoxProperties(box1);
            viewport.AddContent(box1);

            userBoxes.Add(box1);

            var box2 = new LayoutFarm.CustomWidgets.SimpleBox(60, 60);
            box2.SetLocation(50, 50);
            SetupActiveBoxProperties(box2);
            viewport.AddContent(box2);
            userBoxes.Add(box2);

            var box3 = new LayoutFarm.CustomWidgets.SimpleBox(60, 60);
            box3.SetLocation(200, 80);
            SetupActiveBoxProperties(box3);
            viewport.AddContent(box3);
            userBoxes.Add(box3);


            //--------------------------------

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
            int j = this.userBoxes.Count;

            var primSelectionBox = selectionBox.GetPrimaryRenderElement(rootgfx);
            var primGlobalPoint = primSelectionBox.GetGlobalLocation();
            var selectedRectArea = new Rectangle(primGlobalPoint, primSelectionBox.Size);

            List<CustomWidgets.EaseBox> selectedList = new List<CustomWidgets.EaseBox>();
            for (int i = 0; i < j; ++i)
            {
                var box = userBoxes[i];
                var primElement = userBoxes[i].GetPrimaryRenderElement(rootgfx);
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
                    selectedList.Add(userBoxes[i]);
                    //------
                    //create user controller box for the selected box 
                    UIControllerBox userControllerBox = GetFreeUserControllerBox();
                    userControllerBox.TargetBox = box;
                    userControllerBox.SetLocation(box.Left - 5, box.Top - 5);
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


                //request user controller for this box
                UIControllerBox userControllerBox = GetFreeUserControllerBox();
                userControllerBox.TargetBox = box;
                viewport.AddContent(userControllerBox);
                userControllerBox.SetLocation(box.Left - 5, box.Top - 5);
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
                box.BackColor = Color.Green;
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
            var targetBox = controllerBox.TargetBox;
            if (targetBox != null)
            {
                //move target box too
                targetBox.SetLocation(nearestX + gridSize, nearestY + gridSize);
            }
        }
        static void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box
            controllerBox.MouseDrag += (s, e) =>
            {

                MoveWithSnapToGrid(controllerBox, e.DiffCapturedX, e.DiffCapturedY);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
                //test here -----------------------------------------------------
                //find dragover element 
                var dragOverElements = new List<UIElement>();
                controllerBox.FindDragOverElements(dragOverElements);
                if (dragOverElements.Count > 0)
                {
                    //send notification to another box 
                    //use guest talk msg
                    var easeBox = dragOverElements[0] as IEventListener;
                    if (easeBox != null)
                    {
                        var talkMsg = new UIGuestTalkEventArgs();
                        talkMsg.Sender = controllerBox;
                        easeBox.ListenGuestTalk(talkMsg);
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
            public UIControllerBox(int w, int h)
                : base(w, h)
            {
                SetupDockSpaces();
            }
            public LayoutFarm.UI.UIBox TargetBox
            {
                get;
                set;
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
                                controllerBox.SetLocation(controllerBox.Left + xdiff, controllerBox.Top + ydiff);
                                controllerBox.SetSize(controllerBox.Width - xdiff, controllerBox.Height - ydiff);

                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    //move target box too 
                                    targetBox.SetBounds(controllerBox.Left + 5,
                                        controllerBox.Top + 5,
                                        controllerBox.Width - 10,
                                        controllerBox.Height - 10);
                                }
                            }

                        } break;
                    case SpaceName.RightTop:
                        {
                            if (xdiff != 0 || ydiff != 0)
                            {
                                controllerBox.SetLocation(controllerBox.Left, controllerBox.Top + ydiff);
                                controllerBox.SetSize(controllerBox.Width + xdiff, controllerBox.Height - ydiff);

                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    //move target box too 
                                    targetBox.SetBounds(controllerBox.Left + 5,
                                        controllerBox.Top + 5,
                                        controllerBox.Width - 10,
                                        controllerBox.Height - 10);
                                }
                            }
                        } break;
                    case SpaceName.RightBottom:
                        {
                            if (xdiff != 0 || ydiff != 0)
                            {
                                controllerBox.SetSize(controllerBox.Width + xdiff, controllerBox.Height + ydiff);
                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    //move target box too 
                                    targetBox.SetBounds(controllerBox.Left + 5,
                                        controllerBox.Top + 5,
                                        controllerBox.Width - 10,
                                        controllerBox.Height - 10);
                                }
                            }
                        } break;
                    case SpaceName.LeftBottom:
                        {
                            if (xdiff != 0 || ydiff != 0)
                            {
                                controllerBox.SetLocation(controllerBox.Left + xdiff, controllerBox.Top);
                                controllerBox.SetSize(controllerBox.Width - xdiff, controllerBox.Height + ydiff);

                                var targetBox = controllerBox.TargetBox;
                                if (targetBox != null)
                                {
                                    //move target box too 
                                    targetBox.SetBounds(controllerBox.Left + 5,
                                        controllerBox.Top + 5,
                                        controllerBox.Width - 10,
                                        controllerBox.Height - 10);
                                }
                            }
                        } break;
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