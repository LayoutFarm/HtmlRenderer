//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.2 MultpleBox")]
    class Demo_MultipleBox : DemoBase
    {
        LayoutFarm.SampleControls.UICheckBox currentSingleCheckedBox;

        protected override void OnStartDemo(SampleViewport viewport)
        {
            SetupImageList();
            for (int i = 1; i < 5; ++i)
            {
                var textbox = new LayoutFarm.SampleControls.UIEaseBox(30, 30);
                textbox.SetLocation(i * 40, i * 40);
                viewport.AddContent(textbox);
            }
            //--------------------
            //image box
            //load bitmap with gdi+           
            Bitmap bmp = LoadBitmap("../../Demo/favorites32.png");
            var imgBox = new SampleControls.UIImageBox(bmp.Width, bmp.Height);
            imgBox.Image = bmp;
            viewport.AddContent(imgBox);

            //--------------------
            //checked box
            int boxHeight = 20;

            int boxY = 50;
            //multiple select
            for (int i = 0; i < 4; ++i)
            {
                var statedBox = new LayoutFarm.SampleControls.UICheckBox(20, boxHeight);
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;

                viewport.AddContent(statedBox);
            }
            //-------------------------------------------------------------------------
            //single select 
            boxY += 50;
            for (int i = 0; i < 4; ++i)
            {
                var statedBox = new LayoutFarm.SampleControls.UICheckBox(20, boxHeight);
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5; 
                viewport.AddContent(statedBox);
                statedBox.WhenChecked += (s, e) =>
                {
                    var selectedBox = (LayoutFarm.SampleControls.UICheckBox)s;
                    if (selectedBox != currentSingleCheckedBox)
                    {
                        if (currentSingleCheckedBox != null)
                        {
                            currentSingleCheckedBox.Checked = false;
                        }
                        currentSingleCheckedBox = selectedBox;
                    }
                };
            }
        }
        static void SetupImageList()
        {
            if (!LayoutFarm.SampleControls.ResImageList.HasImages)
            {
                //set imagelists
                var imgdic = new Dictionary<SampleControls.ImageName, Image>();
                imgdic[SampleControls.ImageName.CheckBoxUnChecked] = LoadBitmap("../../Demo/arrow_close.png");
                imgdic[SampleControls.ImageName.CheckBoxChecked] = LoadBitmap("../../Demo/arrow_open.png");
                LayoutFarm.SampleControls.ResImageList.SetImageList(imgdic);
            }
        }
        static Bitmap LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
    }
}