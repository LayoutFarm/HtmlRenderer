// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.4 DemoDrag")]
    class Demo_Drag : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            {
                var box1 = new LayoutFarm.CustomWidgets.EaseBox(50, 50);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10);
                //box1.dbugTag = 1;
                SetupActiveBoxProperties(box1);
                viewport.AddContent(box1);
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.CustomWidgets.EaseBox(30, 30);
                box2.SetLocation(50, 50);
                //box2.dbugTag = 2;
                SetupActiveBoxProperties(box2);
                viewport.AddContent(box2);
            }
        }
        static void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.EaseBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;

            };
            box.MouseMove += (s, e) =>
            {
                if (e.IsDragging)
                {
                    box.BackColor = KnownColors.FromKnownColor(KnownColor.GreenYellow);
                    Point pos = box.Position;
                    box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                    e.CancelBubbling = true;
                }
            };
            box.MouseLeave += (s, e) =>
            {
                if (e.IsDragging)
                {
                    box.BackColor = KnownColors.FromKnownColor(KnownColor.GreenYellow);
                    Point pos = box.Position;
                    //continue dragging on the same element 
                    box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                    e.StopPropagation();
                }
            };

        }

    }
}