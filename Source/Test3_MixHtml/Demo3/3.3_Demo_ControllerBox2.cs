//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("3.3 Demo_ControllerBoxs2")]
    class Demo_ControllerBoxs2 : DemoBase
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
                box1.dbugTag = 1;
                SetupActiveBoxProperties(box1);
                viewport.AddContent(box1);
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.CustomWidgets.EaseBox(60, 60);
                box2.SetLocation(50, 50);
                box2.dbugTag = 2;
                SetupActiveBoxProperties(box2);
                viewport.AddContent(box2);
            }
            {

                controllerBox1 = new UIControllerBox(40, 40);
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
                controllerBox1.SetLocation(200, 200);
                controllerBox1.dbugTag = 3;
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
                box.InvalidateGraphic();
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
                box.InvalidateGraphic();

                //hide controller
                controllerBox1.Visible = false;
                controllerBox1.TargetBox = null;
            };

            ////3. drag
            //box.Dragging += (s, e) =>
            //{
            //    box.BackColor = KnownColors.FromKnownColor(KnownColor.GreenYellow);
            //    Point pos = box.Position;
            //    box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
            //    e.MouseCursorStyle = MouseCursorStyle.Pointer;
            //};

            //box.DragLeave += (s, e) =>
            //{
            //    box.BackColor = KnownColors.FromKnownColor(KnownColor.GreenYellow);
            //    Point pos = box.Position;
            //    //continue dragging on the same element 
            //    box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
            //    e.MouseCursorStyle = MouseCursorStyle.Pointer;
            //    e.CancelBubbling = true;
            //};
            //box.DragStop += (s, e) =>
            //{
            //    box.BackColor = Color.LightGray;
            //    e.MouseCursorStyle = MouseCursorStyle.Default;
            //    box.InvalidateGraphic();
            //};
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
            controllerBox.DragBegin += (s, e) =>
            {
                MoveWithSnapToGrid(controllerBox, e);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };

            controllerBox.Dragging += (s, e) =>
            {
                MoveWithSnapToGrid(controllerBox, e);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
            controllerBox.DragLeave += (s, e) =>
            {

                MoveWithSnapToGrid(controllerBox, e);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
            controllerBox.MouseLeave += (s, e) =>
            {
                if (e.IsMouseDown)
                {
                    MoveWithSnapToGrid(controllerBox, e);
                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                    e.CancelBubbling = true;
                }
            };

            controllerBox.MouseMove += (s, e) =>
            {
                if (e.IsMouseDown)
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


            public UIControllerBox(int w, int h)
                : base(w, h)
            {


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
                    if (myRenderElement != null)
                    {
                        VisualLayerCollection layers = new VisualLayerCollection();
                        myRenderElement.Layers = layers;
                        var plain0 = new VisualPlainLayer(myRenderElement);
                        layers.AddLayer(plain0);
                        plain0.AddChild(gridBox.GetPrimaryRenderElement(rootgfx));
                    }
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
            }
        }



    }
}