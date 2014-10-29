//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("3.2 DemoControllerBox")]
    class Demo_ControllerBoxs : DemoBase
    {
        LayoutFarm.SampleControls.UIButton controllerBox1;

        protected override void OnStartDemo(SampleViewport viewport)
        {
            {
                var box1 = new LayoutFarm.SampleControls.UIButton(50, 50);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10);
                box1.dbugTag = 1;
                SetupActiveBoxProperties(box1);
                viewport.AddContent(box1);
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.SampleControls.UIButton(30, 30);
                box2.SetLocation(50, 50);
                box2.dbugTag = 2;
                SetupActiveBoxProperties(box2);
                viewport.AddContent(box2);
            }
            {

                controllerBox1 = new LayoutFarm.SampleControls.UIButton(40, 40);
                Color c = KnownColors.FromKnownColor(KnownColor.Blue);
                controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
                controllerBox1.SetLocation(200, 200);
                controllerBox1.dbugTag = 3;
                controllerBox1.Visible = false;

                SetupControllerBoxProperties(controllerBox1);
                viewport.AddContent(controllerBox1);

            }
        }



        void SetupActiveBoxProperties(LayoutFarm.SampleControls.UIButton box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {

                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                box.InvalidateGraphic();
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //move controller here
                controllerBox1.SetLocation(box.Left - 5, box.Top - 5);
                controllerBox1.SetSize(box.Width + 10, box.Height + 10);

                controllerBox1.Visible = true;
            };

            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                box.InvalidateGraphic();

                //hide controller
                controllerBox1.Visible = false;

            };

            //3. drag
            box.Dragging += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.GreenYellow);
                Point pos = box.Position;
                box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
            };

            box.DragLeave += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.GreenYellow);
                Point pos = box.Position;
                //continue dragging on the same element 
                box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
            box.DragStop += (s, e) =>
            {
                box.BackColor = Color.LightGray;
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.InvalidateGraphic();

            };
        }


        void SetupControllerBoxProperties(LayoutFarm.SampleControls.UIButton box)
        {



        }
    }
}