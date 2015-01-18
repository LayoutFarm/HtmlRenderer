﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{
    [DemoNote("3.4 Demo_CompartmentBox")]
    class Demo_CompartmentBox : DemoBase
    {
        UIControllerBox controllerBox1;

        protected override void OnStartDemo(SampleViewport viewport)
        {

            //--------------------------------
            {
                var bgbox = new LayoutFarm.CustomWidgets.EaseBox(800, 600);
                bgbox.BackColor = Color.White;
                bgbox.SetLocation(0, 0);
                SetupBackgroundProperties(bgbox);
                viewport.AddContent(bgbox);
            }
            //--------------------------------
            {
                var box1 = new LayoutFarm.CustomWidgets.EaseBox(150, 150);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10);
                //box1.dbugTag = 1;
                SetupActiveBoxProperties(box1);
                viewport.AddContent(box1);
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.CustomWidgets.EaseBox(60, 60);
                box2.SetLocation(50, 50);
                //box2.dbugTag = 2;
                SetupActiveBoxProperties(box2);
                viewport.AddContent(box2);
            }
            {

                controllerBox1 = new UIControllerBox(40, 40);
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
                controllerBox1.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                controllerBox1.Visible = false;
                SetupControllerBoxProperties(controllerBox1);
                viewport.AddContent(controllerBox1);
            }
        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.EaseBox backgroundBox)
        {
            //if click on background
            backgroundBox.MouseDown += (s, e) =>
            {
                controllerBox1.TargetBox = null;//release target box
                controllerBox1.Visible = false;
            };

        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.EaseBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;

                //--------------------------------------------
                //move controller here
                controllerBox1.TargetBox = box;
                controllerBox1.SetLocation(box.Left - 5, box.Top - 5);
                controllerBox1.SetSize(box.Width + 10, box.Height + 10);
                controllerBox1.Visible = true;

                //--------------------------------------------
            };

            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                //hide controller
                controllerBox1.Visible = false;
                controllerBox1.TargetBox = null;
            };
            
        }

        static void MoveWithSnapToGrid(UIControllerBox controllerBox, UIMouseEventArgs e)
        {
            //sample move with snap to grid
            Point pos = controllerBox.Position;
            int newX = pos.X + e.XDiff;
            int newY = pos.Y + e.YDiff;
            //snap to gridsize =5;
            //find nearest snap x 
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

            controllerBox.MouseLeave += (s, e) =>
            {
                if (e.IsDragging)
                {
                    MoveWithSnapToGrid(controllerBox, e);
                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                    e.CancelBubbling = true;
                }
            };
            controllerBox.MouseMove += (s, e) =>
            {
                if (e.IsDragging)
                {
                    MoveWithSnapToGrid(controllerBox, e);
                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                    e.CancelBubbling = true;
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

            //get primary render element
            public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
            {
                if (!this.HasReadyRenderElement)
                {
                    gridBox = new LayoutFarm.CustomWidgets.GridBox(30, 30);
                    gridBox.SetLocation(5, 5);
                    gridBox.BuildGrid(3, 3, CellSizeStyle.UniformCell);

                    var myRenderElement = base.GetPrimaryRenderElement(rootgfx) as LayoutFarm.CustomWidgets.CustomRenderBox;
                    PlainLayer plain0 = null;
                    if (myRenderElement != null)
                    {
                        VisualLayerCollection layers = new VisualLayerCollection();
                        myRenderElement.Layers = layers;
                        plain0 = new PlainLayer(myRenderElement);
                        layers.AddLayer(plain0);
                        plain0.AddChild(gridBox.GetPrimaryRenderElement(rootgfx));
                    }
                    //------------------------------------------------------
                    plain0.AddChild(boxLeftTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRightTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxLeftBottom.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRightBottom.GetPrimaryRenderElement(rootgfx));
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
                    this.dockspaceController.SetSize(width, height);
                }
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

            CustomWidgets.EaseBox CreateTinyControlBox(SpaceName name)
            {
                int controllerBoxWH = 10;
                CustomWidgets.EaseBox tinyBox = new CustomWidgets.EaseBox(controllerBoxWH, controllerBoxWH);
                tinyBox.BackColor = PixelFarm.Drawing.Color.Red;
                tinyBox.Tag = name;
                //add handler for each tiny box

                //---------------------------------------------------------------------

                tinyBox.MouseMove += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        ResizeTargetWithSnapToGrid((SpaceName)tinyBox.Tag, this, e);
                        e.MouseCursorStyle = MouseCursorStyle.Pointer;
                        e.CancelBubbling = true;
                    }
                };
                tinyBox.MouseLeave += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        ResizeTargetWithSnapToGrid((SpaceName)tinyBox.Tag, this, e);
                        e.MouseCursorStyle = MouseCursorStyle.Pointer;
                        e.CancelBubbling = true;
                    }
                };
                tinyBox.MouseUp += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        ResizeTargetWithSnapToGrid2(this, e);
                    }
                    e.MouseCursorStyle = MouseCursorStyle.Default;
                    e.CancelBubbling = true;
                };
                return tinyBox;
            }

            static void ResizeTargetWithSnapToGrid(SpaceName tinyBoxSpaceName, UIControllerBox controllerBox, UIMouseEventArgs e)
            {
                //sample move with snap to grid
                Point pos = controllerBox.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
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
            static void ResizeTargetWithSnapToGrid2(UIControllerBox controllerBox, UIMouseEventArgs e)
            {
                //sample move with snap to grid
                Point pos = controllerBox.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
                //snap to gridsize =5;
                //find nearest snap x 
                int gridSize = 5;
                float halfGrid = (float)gridSize / 2f;
                int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
                int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;

                int xdiff = nearestX - pos.X;
                if (xdiff != 0)
                {
                    controllerBox.SetSize(controllerBox.Width + xdiff, controllerBox.Height);
                }

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
        }



    }
}