//Apache2, 2014-2016, WinterDev

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.7 ListView")]
    class Demo_ListView : HtmlDemoBase
    {
        protected override void OnHtmlHostCreated()
        {
            var listview = new LayoutFarm.HtmlWidgets.ListView(100, 100);
            listview.SetLocation(30, 20);
            //add listview item 
            for (int i = 0; i < 100; ++i)
            {
                var listItem = new HtmlWidgets.ListItem(100, 20);
                listItem.Text = "item" + i;
                listview.AddItem(listItem);
            }
            AddToViewport(listview);
            ////-------------------------------------------------------------------
            ////add vertical scrollbar 
            //{
            //    //vertical scrollbar
            //    var vscbar = new LayoutFarm.HtmlWidgets.ScrollBar(15, 100);
            //    vscbar.SetLocation(10, 20);
            //    vscbar.MinValue = 0;
            //    vscbar.MaxValue = 100;
            //    vscbar.SmallChange = 20;

            //    this.sampleViewport.AddContent(vscbar);

            //    //add relation between viewpanel and scroll bar 
            //    //var scRelation = new LayoutFarm.HtmlWidgets.ScrollingRelation(vscbar, listview);
            //}
        }
    }
}