//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("3.8 Demo_CompartmentWithSpliter2")]
    class Demo_CompartmentWithSpliter2 : DemoBase
    {
        NinespaceBox ninespaceBox;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            //--------------------------------
            {
                //background element
                var bgbox = new LayoutFarm.CustomWidgets.SimpleBox(800, 600);
                bgbox.BackColor = Color.White;
                bgbox.SetLocation(0, 0);
                SetupBackgroundProperties(bgbox);
                viewport.AddContent(bgbox);
            }
            //--------------------------------
            //ninespace compartment
            ninespaceBox = new NinespaceBox(800, 600);
            ninespaceBox.ShowGrippers = true;
            viewport.AddContent(ninespaceBox);
            ninespaceBox.SetSize(800, 600);
            //--------------------------------


            //--------------------------------
            //test add some content to the ninespace box
            var sampleListView = CreateSampleListView();
            ninespaceBox.LeftSpace.ContentLayoutKind = BoxContentLayoutKind.VerticalStack;
            ninespaceBox.LeftSpace.AddChild(sampleListView);
        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.EaseBox backgroundBox)
        {
        }

        static LayoutFarm.CustomWidgets.ListView CreateSampleListView()
        {
            var listview = new LayoutFarm.CustomWidgets.ListView(300, 400);
            listview.SetLocation(10, 10);
            listview.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            //add 
            for (int i = 0; i < 10; ++i)
            {
                var listItem = new LayoutFarm.CustomWidgets.ListItem(400, 20);
                if ((i % 2) == 0)
                {
                    listItem.BackColor = KnownColors.FromKnownColor(KnownColor.OrangeRed);
                }
                else
                {
                    listItem.BackColor = KnownColors.FromKnownColor(KnownColor.Orange);
                }
                listview.AddItem(listItem);
            }
            return listview;
        }
    }
}