//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    [DemoNote("1.9 TreeView")]
    class Demo_TreeView : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var treeView = new LayoutFarm.CustomWidgets.TreeView(300, 400);
            treeView.SetLocation(10, 10);
            treeView.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            viewport.AddContent(treeView);
            //prepare node icon

            ImageBinder nodeOpen = new ClientImageBinder(null);
            nodeOpen.SetImage(LoadBitmap("../../Demo/arrow_open.png"));

            //add 
            for (int i = 0; i < 10; ++i)
            {
                var childLevel0 = new LayoutFarm.CustomWidgets.TreeNode(400, 40);
                childLevel0.BackColor = ((i % 2) == 0) ?
                         KnownColors.FromKnownColor(KnownColor.Blue) :
                         KnownColors.FromKnownColor(KnownColor.Yellow);
                treeView.AddItem(childLevel0);
                childLevel0.NodeIconImage = nodeOpen;
                for (int n = 0; n < 4; ++n)
                {
                    var childLevel1 = new LayoutFarm.CustomWidgets.TreeNode(400, 20);
                    childLevel1.BackColor = ((n % 2) == 0) ?
                          KnownColors.FromKnownColor(KnownColor.Green) :
                          KnownColors.FromKnownColor(KnownColor.YellowGreen);
                    childLevel0.AddChildNode(childLevel1);
                    childLevel1.NodeIconImage = nodeOpen;
                    for (int m = 0; m < 5; ++m)
                    {
                        var childLevel2 = new LayoutFarm.CustomWidgets.TreeNode(400, 20);
                        childLevel2.BackColor = ((m % 2) == 0) ?
                          KnownColors.FromKnownColor(KnownColor.OrangeRed) :
                          KnownColors.FromKnownColor(KnownColor.Wheat);
                        childLevel2.NodeIconImage = nodeOpen;
                        childLevel1.AddChildNode(childLevel2);
                    }
                }
            }
        }
        //static Bitmap LoadBitmap(string filename)
        //{
        //    System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
        //    Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
        //    return bmp;
        //}
        //static ImageBinder LoadImage(string filename)
        //{
        //    System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
        //    Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
        //    ImageBinder binder = new ClientImageBinder(null);
        //    binder.SetImage(bmp);
        //    binder.State = ImageBinderState.Loaded;
        //    return binder;
        //}
    }
}