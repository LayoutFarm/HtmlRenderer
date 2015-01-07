//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("3.2 DemoControllerBox")]
    class Demo_ControllerBoxs : DemoBase
    {
        UIControllerBox controllerBox1;

        protected override void OnStartDemo(SampleViewport viewport)
        {
            {
                var box1 = new LayoutFarm.CustomWidgets.EaseBox(50, 50);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10);
                box1.dbugTag = 1;
                SetupActiveBoxProperties(box1);
                viewport.AddContent(box1);
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.CustomWidgets.EaseBox(30, 30);
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
                controllerBox1.SetLocation(box.Left - 5, box.Top - 5);
                controllerBox1.SetSize(box.Width + 10, box.Height + 10);
                controllerBox1.Visible = true;
                controllerBox1.TargetBox = box;
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


        static void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box
            controllerBox.DragBegin += (s, e) =>
            {
                Point pos = controllerBox.Position;
                controllerBox.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                var targetBox = controllerBox.TargetBox;
                if (targetBox != null)
                {
                    //move target box too
                    targetBox.SetLocation(pos.X + 5, pos.Y + 5);

                }
            };

            controllerBox.Dragging += (s, e) =>
            {
                Point pos = controllerBox.Position;
                controllerBox.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                var targetBox = controllerBox.TargetBox;
                if (targetBox != null)
                {
                    //move target box too
                    targetBox.SetLocation(pos.X + 5, pos.Y + 5);
                }
            };


            controllerBox.DragLeave += (s, e) =>
            {

                Point pos = controllerBox.Position;
                //continue dragging on the same element 
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
                controllerBox.SetLocation(newX, newY);
                var targetBox = controllerBox.TargetBox;
                if (targetBox != null)
                {
                    //move target box too
                    targetBox.SetLocation(newX + 5, newY + 5);
                }

                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
            controllerBox.MouseLeave += (s, e) =>
            {
                if (e.IsMouseDown)
                {
                    Point pos = controllerBox.Position;
                    int newX = pos.X + e.XDiff;
                    int newY = pos.Y + e.YDiff;
                    controllerBox.SetLocation(newX, newY);
                    var targetBox = controllerBox.TargetBox;
                    if (targetBox != null)
                    {
                        //move target box too
                        targetBox.SetLocation(newX + 5, newY + 5);
                    }

                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                    e.CancelBubbling = true;
                }
            };

            controllerBox.MouseMove += (s, e) =>
            {
                if (e.IsMouseDown)
                {
                    Point pos = controllerBox.Position;
                    int newX = pos.X + e.XDiff;
                    int newY = pos.Y + e.YDiff;


                    controllerBox.SetLocation(newX, newY);
                    var targetBox = controllerBox.TargetBox;
                    if (targetBox != null)
                    {
                        //move target box too
                        targetBox.SetLocation(newX + 5, newY + 5);
                    }
                }

            };

            //controllerBox.DragEnd += (s, e) =>
            //{
            //    controllerBox.TargetBox = null;
            //    controllerBox.Visible = false;

            //};

            //controllerBox.MouseUp += (s, e) =>
            //{
            //    controllerBox.Visible = false;
            //    controllerBox.TargetBox = null;
            //};

        }

        //-----------------------------------------------------------------
        class UIControllerBox : LayoutFarm.CustomWidgets.EaseBox
        {
            public UIControllerBox(int w, int h)
                : base(w, h)
            {
                // 

            }
            public LayoutFarm.UI.UIBox TargetBox
            {
                get;
                set;
            }

        }

    }
}