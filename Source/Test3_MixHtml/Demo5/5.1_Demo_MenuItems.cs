//Apache2, 2014-present, WinterDev

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.1 MenuItems")]
    class Demo_HingeMixHtml : HtmlDemoBase
    {
        protected override void OnHtmlHostCreated()
        {
            var comboBox1 = CreateComboBox(20, 20);
            AddToViewport(comboBox1);
            var comboBox2 = CreateComboBox(50, 50);
            AddToViewport(comboBox2);
            //------------------------------------------------------------------
            LayoutFarm.HtmlWidgets.MenuBox rootMenuBox = CreateMenuBox(10, 120);
            rootMenuBox.IsLandPart = true;
            //------------------------------------------------------------------
            //add single menu item
            var rootMenuItem = new HtmlWidgets.MenuItem(150, 20);
            rootMenuItem.MenuItemText = "level0";
            rootMenuBox.AddChildBox(rootMenuItem);
            for (int i = 0; i < 10; ++i)
            {
                var menuItem = new HtmlWidgets.MenuItem(150, 20);
                menuItem.MenuItemText = "item" + i;
                //add sub menu level 2

                for (int n = 0; n < 5; ++n)
                {
                    var subMenu = new HtmlWidgets.MenuItem(150, 20);
                    subMenu.MenuItemText = "item" + i + "." + n;
                    menuItem.AddSubMenuItem(subMenu);
                }

                rootMenuItem.AddSubMenuItem(menuItem);
            }
            AddToViewport(rootMenuBox);
        }
        LayoutFarm.HtmlWidgets.ComboBox CreateComboBox(int x, int y)
        {
            LayoutFarm.HtmlWidgets.ComboBox comboBox = new HtmlWidgets.ComboBox(400, 20);
            comboBox.SetLocation(x, y);
            ////--------------------
            ////1. create landing part 
            //var landPart = new LightHtmlBox(this.myHtmlHost, 400, 02);
            //landPart.LoadHtmlDom(CreateMenuItemDetail());
            //comboBox.LandPart = landPart;
            ////--------------------------------------
            ////add small px to land part
            ////image
            ////load bitmap with gdi+                

            ////LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Image.Width, arrowBmp.Image.Height);
            ////imgBox.ImageBinder = arrowBmp;
            ////--------------------------------------
            ////2. float part
            //var floatPart = new LightHtmlBox(this.myHtmlHost, 400, 100);
            //comboBox.FloatPart = floatPart;

            ////--------------------------------------
            ////if click on this image then
            //imgBox.MouseDown += (s, e) =>
            //{
            //    e.CancelBubbling = true;

            //    if (comboBox.IsOpen)
            //    {
            //        comboBox.CloseHinge();
            //    }
            //    else
            //    {
            //        comboBox.OpenHinge();
            //    }
            //};
            //landPart.AddChildBox(imgBox);


            return comboBox;
        }
        LayoutFarm.HtmlWidgets.MenuBox CreateMenuBox(int x, int y)
        {
            var menuBox = new LayoutFarm.HtmlWidgets.MenuBox(200, 400);
            menuBox.SetLocation(x, y);
            return menuBox;
        }
    }
}