// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("2.3 SingleLineText_WithSuggestion")]
    class Demo_SingleLineText_WithSuggestion : DemoBase
    {
        LayoutFarm.CustomWidgets.TextBox textbox;
        LayoutFarm.CustomWidgets.ListView listView;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            textbox = new LayoutFarm.CustomWidgets.TextBox(400, 30, false);
            listView = new CustomWidgets.ListView(300, 200);
            listView.SetLocation(0, 40);
            //------------------------------------
            //create special text surface listener
            var textSurfaceListener = new LayoutFarm.Text.TextSurfaceEventListener();
            textSurfaceListener.CharacterAdded += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_CharacterAdded);
            textbox.TextEventListener = textSurfaceListener;
            //------------------------------------

            viewport.AddContent(textbox);

            viewport.AddContent(listView);
        }
        void textSurfaceListener_CharacterAdded(object sender, Text.TextDomEventArgs e)
        {
            Console.WriteLine(e.c);
            //CustomWidgets.ListItem item = new CustomWidgets.ListItem(20,17);
            //item.BackColor = Color.Blue;
            //listView.AddItem(item);
        }
    }
}