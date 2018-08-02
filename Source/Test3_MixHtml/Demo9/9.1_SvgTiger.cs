//MIT, 2014-present, WinterDev

using PixelFarm.Drawing;
using PaintLab.Svg;
using LayoutFarm.UI;
using LayoutFarm.Svg;
 

namespace LayoutFarm
{
    [DemoNote("9.1 DemoSvgTiger")]
    class Demo_SvgTiger : App
    {
        LayoutFarm.CustomWidgets.RectBoxController rectBoxController = new CustomWidgets.RectBoxController();
        LayoutFarm.CustomWidgets.Box box1;
        BackDrawBoardUI _backBoard;

        protected override void OnStart(AppHost host)
        {

            _backBoard = new BackDrawBoardUI(800, 600);
            _backBoard.BackColor = Color.White;
            host.AddChild(_backBoard);
            box1 = new LayoutFarm.CustomWidgets.Box(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(box1);
            _backBoard.AddChild(box1);

            //----------------------

            //load lion svg

            string svgfile = "tiger.svg";
            //string svgfile = "1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f30b.svg";
            //string svgfile = "../Data/1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f370.svg";
            VgRenderVx svgRenderVx = ReadSvgFile(svgfile);
            var uiSprite = new UISprite(10, 10);
            uiSprite.LoadSvg(svgRenderVx);
            _backBoard.AddChild(uiSprite);

            //-------- 
            rectBoxController.Init();
            //------------
            host.AddChild(rectBoxController);

            //foreach (var ui in rectBoxController.GetControllerIter())
            //{
            //    viewport.AddContent(ui);
            //}

            //--------
            var evListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(evListener);


            evListener.MouseDown += (e) =>
            {

                //e.MouseCursorStyle = MouseCursorStyle.Pointer;
                ////--------------------------------------------
                //e.SetMouseCapture(rectBoxController.ControllerBoxMain);
                rectBoxController.UpdateControllerBoxes(box1);
                rectBoxController.Focus();
                //System.Console.WriteLine("click :" + (count++));
            };
            rectBoxController.ControllerBoxMain.KeyDown += (s1, e1) =>
            {
                if (e1.Ctrl && e1.KeyCode == UIKeys.X)
                {
                    //test copy back image buffer from current rect area

#if DEBUG
                    //test save some area
                    //int w = rectBoxController.ControllerBoxMain.Width;
                    //int h = rectBoxController.ControllerBoxMain.Height;

                    //using (DrawBoard gdiDrawBoard = DrawBoardCreator.CreateNewDrawBoard(1, w, h))
                    //{
                    //    gdiDrawBoard.OffsetCanvasOrigin(rectBoxController.ControllerBoxMain.Left, rectBoxController.ControllerBoxMain.Top);
                    //    _backBoard.CurrentPrimaryRenderElement.CustomDrawToThisCanvas(gdiDrawBoard, new Rectangle(0, 0, w, h));
                    //    var img2 = new ActualBitmap(w, h);
                    //    //copy content from drawboard to target image and save
                    //    gdiDrawBoard.RenderTo(img2, 0, 0, w, h);

                    //    img2.dbugSaveToPngFile("d:\\WImageTest\\ddd001.png");
                    //}
#endif                    

                }
            };
        }
        VgRenderVx ReadSvgFile(string filename)
        {

            string svgContent = System.IO.File.ReadAllText(filename);
            SvgDocBuilder docBuidler = new SvgDocBuilder();
            PaintLab.Svg.SvgParser parser = new SvgParser(docBuidler);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //
            return docBuidler.ResultDocument.CreateRenderVx();
        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.Box box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                e.SetMouseCapture(rectBoxController.ControllerBoxMain);
                rectBoxController.UpdateControllerBoxes(box);

            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                //controllerBox1.Visible = false;
                //controllerBox1.TargetBox = null;
            };
        }
    }




}