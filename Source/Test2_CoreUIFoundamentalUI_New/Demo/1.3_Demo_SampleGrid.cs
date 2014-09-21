//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.3 Grid")]
    class Demo_Grid : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {
            //grid0
            {
                var gridBox = new LayoutFarm.SampleControls.UIGridBox(100, 100);
                gridBox.SetLocation(50, 50);
                gridBox.BuildGrid(2, 4, CellSizeStyle.UniformCell);

                viewport.AddContent(gridBox);
            }
            //grid1
            {
                var gridBox = new LayoutFarm.SampleControls.UIGridBox(100, 100);
                gridBox.SetLocation(200, 50);
                gridBox.BuildGrid(2, 4, CellSizeStyle.UniformCell);
                 
                viewport.AddContent(gridBox);

                var simpleButton = new LayoutFarm.SampleControls.UIButton(20, 20);
                simpleButton.BackColor = Color.OliveDrab;
                gridBox.AddUI(simpleButton, 1, 1); 
            }


            //-----
            //grid2
            {
                var gridBox = new LayoutFarm.SampleControls.UIGridBox(100, 100);
                gridBox.SetLocation(350, 50);
                gridBox.BuildGrid(3, 8, CellSizeStyle.UniformCell);

                viewport.AddContent(gridBox);
            }

        }
    }
}