//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.8 TreeView")]
    class Demo_TreeView : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var treeView = new LayoutFarm.SampleControls.UITreeView(300, 400);
            treeView.SetLocation(10, 10);
            treeView.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            viewport.AddContent(treeView);

            //add 
            for (int i = 0; i < 10; ++i)
            {
                var childLevel0 = new LayoutFarm.SampleControls.UITreeNode(400, 40);

                childLevel0.BackColor = ((i % 2) == 0) ?
                         KnownColors.FromKnownColor(KnownColor.Blue) :
                         KnownColors.FromKnownColor(KnownColor.Yellow);                  

                treeView.AddItem(childLevel0);

                for (int n = 0; n < 4; ++n)
                {
                    var childLevel1 = new LayoutFarm.SampleControls.UITreeNode(400, 20);
                    childLevel1.BackColor = ((n % 2) == 0) ?
                          KnownColors.FromKnownColor(KnownColor.Green) :
                          KnownColors.FromKnownColor(KnownColor.YellowGreen);  
                    childLevel0.AddChildNode(childLevel1);

                    for (int m = 0; m < 5; ++m)
                    {

                        var childLevel2 = new LayoutFarm.SampleControls.UITreeNode(400, 20);
                        childLevel2.BackColor = ((m % 2) == 0) ?
                          KnownColors.FromKnownColor(KnownColor.OrangeRed) :
                          KnownColors.FromKnownColor(KnownColor.Wheat);
                        
                        childLevel1.AddChildNode(childLevel2);
                    }

                    //childLevel1.Collapse();
                }
            }
            treeView.PerformContentLayout();
        }
    }
}