//MIT, 2014-present, WinterDev

using PixelFarm.Drawing;
using PaintLab.Svg;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("9.1 DemoSvgTiger")]
    class Demo_SvgTiger : App
    {
        LayoutFarm.CustomWidgets.RectBoxController _rectBoxController;
        LayoutFarm.CustomWidgets.Box _redBox;
        BackDrawBoardUI _backBoard;
        VgVisualDocHost _vgVisualDocHost;

        protected override void OnStart(AppHost host)
        {
            _vgVisualDocHost = new VgVisualDocHost();
            _vgVisualDocHost.SetInvalidateDelegate(vgElem =>
            {
                //invalidate graphic here
            });

            _backBoard = new BackDrawBoardUI(800, 600);
            _backBoard.BackColor = Color.White;
            host.AddChild(_backBoard);

            //load lion svg

            string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/tiger.svg";
            //string svgfile = "1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f30b.svg";
            //string svgfile = "../Data/1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f370.svg";

            VgVisualDoc vgDocRoot = ReadSvgFile(svgfile);
            VgVisualElement vgVisElem = vgDocRoot.VgRootElem;
            var uiSprite = new UISprite(10, 10);
            uiSprite.LoadVg(vgVisElem);
            _backBoard.Add(uiSprite);


            //
            _redBox = new LayoutFarm.CustomWidgets.Box(50, 50); //visual rect box
            _redBox.BackColor = Color.Red;
            _redBox.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(_redBox);
            _backBoard.Add(_redBox);
            //----------------------


            //-------- 
            _rectBoxController = new CustomWidgets.RectBoxController();
            _rectBoxController.Init();
            _backBoard.Add(_rectBoxController);

            //foreach (var ui in rectBoxController.GetControllerIter())
            //{
            //    viewport.AddContent(ui);
            //}

            //--------
            var svgEvListener = new GeneralEventListener();
            //uiSprite.AttachExternalEventListener(svgEvListener);
            svgEvListener.MouseDown += (e) =>
            {

                //e.MouseCursorStyle = MouseCursorStyle.Pointer;
                ////--------------------------------------------
                //e.SetMouseCapture(rectBoxController.ControllerBoxMain);
                _rectBoxController.UpdateControllerBoxes(_redBox);
                _rectBoxController.Focus();
                //System.Console.WriteLine("click :" + (count++));
            };
            _rectBoxController.ControllerBoxMain.KeyDown += (s1, e1) =>
            {
                if (e1.KeyCode == UIKeys.C && e1.Ctrl)
                {
                    //test copy back image buffer from current rect area

#if DEBUG

                    //int left = rectBoxController.ControllerBoxMain.Left;
                    //int top = rectBoxController.ControllerBoxMain.Top;
                    //int width = rectBoxController.ControllerBoxMain.Width;
                    //int height = rectBoxController.ControllerBoxMain.Height;

                    //using (DrawBoard drawBoard = DrawBoardCreator.CreateNewDrawBoard(1, width, height))
                    //{

                    //    //create new draw board
                    //    drawBoard.OffsetCanvasOrigin(left, top);
                    //    _backBoard.CurrentPrimaryRenderElement.CustomDrawToThisCanvas(drawBoard, new Rectangle(0, 0, width, height));
                    //    using (var img2 = new PixelFarm.CpuBlit.ActualBitmap(width, height))
                    //    {
                    //        //copy content from drawboard to target image and save
                    //        drawBoard.RenderTo(img2, 0, 0, width, height);

                    //        PixelFarm.CpuBlit.Imaging.PngImageWriter.SaveImgBufferToPngFile(
                    //            PixelFarm.CpuBlit.ActualBitmap.GetBufferPtr(img2),
                    //            img2.Stride,
                    //            img2.Width,
                    //            img2.Height,
                    //            "d:\\WImageTest\\tiger.png");
                    //    }
                    //    //copy content from drawboard to target image and save
                    //}

#endif

                }
            };
        }
        VgVisualDoc ReadSvgFile(string filename)
        {

            string svgContent = System.IO.File.ReadAllText(filename);
            VgDocBuilder docBuidler = new VgDocBuilder();
            SvgParser parser = new SvgParser(docBuidler);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //TODO: review this step again
            VgVisualDocBuilder builder = new VgVisualDocBuilder();
            return builder.CreateVgVisualDoc(docBuidler.ResultDocument, _vgVisualDocHost);

        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.Box box)
        {

            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                e.SetMouseCaptureElement(_rectBoxController.ControllerBoxMain);
                _rectBoxController.UpdateControllerBoxes(box);
                _rectBoxController.Focus();
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