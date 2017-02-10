//Apache2, 2014-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("1.2 MultpleBox")]
    class Demo_MultipleBox : DemoBase
    {
        LayoutFarm.CustomWidgets.CheckBox currentSingleCheckedBox;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            SetupImageList();
            for (int i = 1; i < 5; ++i)
            {
                var textbox = new LayoutFarm.CustomWidgets.SimpleBox(30, 30);
                textbox.SetLocation(i * 40, i * 40);
                viewport.AddContent(textbox);
            }
            //--------------------
            //image box
            //load bitmap with gdi+           
            ImageBinder imgBinder = new ClientImageBinder(null);
            imgBinder.SetImage(LoadBitmap("../../Demo/favorites32.png"));
            var imgBox = new CustomWidgets.ImageBox(imgBinder.Image.Width, imgBinder.Image.Height);
            imgBox.ImageBinder = imgBinder;
            viewport.AddContent(imgBox);
            //--------------------
            //checked box
            int boxHeight = 20;
            int boxY = 50;
            //multiple select
            for (int i = 0; i < 4; ++i)
            {
                var statedBox = new LayoutFarm.CustomWidgets.CheckBox(20, boxHeight);
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;
                viewport.AddContent(statedBox);
            }
            //-------------------------------------------------------------------------
            //single select 
            boxY += 50;
            for (int i = 0; i < 4; ++i)
            {
                var statedBox = new LayoutFarm.CustomWidgets.CheckBox(20, boxHeight);
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;
                viewport.AddContent(statedBox);
                statedBox.WhenChecked += (s, e) =>
                {
                    var selectedBox = (LayoutFarm.CustomWidgets.CheckBox)s;
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
            //-------------------------------------------------------------------
            //test canvas
            var canvasBox = new MyDrawingCanvas(300, 300);
            canvasBox.SetLocation(400, 150);
            viewport.AddContent(canvasBox);
            //-------------------------------------------------------------------

        }


        class MyDrawingCanvas : LayoutFarm.CustomWidgets.MiniAggCanvasBox
        {
            int lastX, lastY;
            List<Point> pointList = new List<Point>();
            public MyDrawingCanvas(int w, int h)
                : base(w, h)
            {
            }
            protected override void OnMouseDown(UIMouseEventArgs e)
            {
                ////test only!!!         
                this.lastX = e.X;
                this.lastY = e.Y;
                pointList.Add(new Point(lastX, lastY));
            }
            protected override void OnMouseMove(UIMouseEventArgs e)
            {
                //test
                //draw on this canvas
                if (!e.IsDragging)
                {
                    return;
                }
                this.lastX = e.X;
                this.lastY = e.Y;
                //temp fix here -> need converter
                var p = this.Painter;
                p.Clear(PixelFarm.Drawing.Color.White);
                pointList.Add(new Point(lastX, lastY));
                //clear and render again
                int j = pointList.Count;
                for (int i = 1; i < j; ++i)
                {
                    var p0 = pointList[i - 1];
                    var p1 = pointList[i];
                    p.Line(
                        p0.X, p0.Y,
                        p1.X, p1.Y);
                }

                this.InvalidateCanvasContent();
            }
            protected override void OnMouseUp(UIMouseEventArgs e)
            {
            }
        }

        static void SetupImageList()
        {
            if (!LayoutFarm.CustomWidgets.ResImageList.HasImages)
            {
                //set imagelists
                var imgdic = new Dictionary<CustomWidgets.ImageName, Image>();
                imgdic[CustomWidgets.ImageName.CheckBoxUnChecked] = LoadBitmap("../../Demo/arrow_close.png");
                imgdic[CustomWidgets.ImageName.CheckBoxChecked] = LoadBitmap("../../Demo/arrow_open.png");
                LayoutFarm.CustomWidgets.ResImageList.SetImageList(imgdic);
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