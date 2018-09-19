﻿//MIT, 2014-present, WinterDev

using System.Collections.Generic;

using PixelFarm.Drawing;
using PaintLab.Svg;
using LayoutFarm.Svg;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("9.2 ShapeControls")]
    class DemoShapeControl : App
    {
        LayoutFarm.CustomWidgets.PolygonController polygonController = new CustomWidgets.PolygonController();
        LayoutFarm.CustomWidgets.RectBoxController rectBoxController = new CustomWidgets.RectBoxController();

        protected override void OnStart(AppHost host)
        {

            var spec = new Svg.SvgPathSpec() { FillColor = Color.Red };
            SvgRenderRootElement renderRoot = new SvgRenderRootElement();
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);
            using (VxsContext.Temp(out VertexStore vxs))
            {
                //red-triangle ***
                vxs.AddMoveTo(100, 20);
                vxs.AddLineTo(150, 50);
                vxs.AddLineTo(110, 80);
                vxs.AddCloseFigure();
                renderE._vxsPath = vxs.CreateTrim();
            }

            svgRenderVx.DisableBackingImage = true;
            var uiSprite = new UISprite(10, 10); //init size = (10,10), location=(0,0) 
            uiSprite.DisableBmpCache = true;
            uiSprite.LoadSvg(svgRenderVx);//
            host.AddChild(uiSprite);

            var spriteEvListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(spriteEvListener);



            //box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            //box1.BackColor = Color.Red;
            //box1.SetLocation(10, 10);
            ////box1.dbugTag = 1;
            //SetupActiveBoxProperties(box1);
            //viewport.AddContent(box1);
            //-------- 
            rectBoxController.Init();
            //polygonController.Visible = false;
            host.AddChild(polygonController);
            //-------------------------------------------
            host.AddChild(rectBoxController);

            //foreach (var ui in rectBoxController.GetControllerIter())
            //{
            //    viewport.AddContent(ui);
            //}

            spriteEvListener.MouseDown += e1 =>
            {
                //mousedown on ui sprite
                polygonController.SetPosition((int)uiSprite.Left, (int)uiSprite.Top);
                polygonController.SetTargetUISprite(uiSprite);
                polygonController.UpdateControlPoints(renderE._vxsPath);

            };
            spriteEvListener.MouseMove += e1 =>
            {
                if (e1.IsDragging)
                {
                    //drag event on uisprite

                    int left = (int)uiSprite.Left;
                    int top = (int)uiSprite.Top;

                    int new_left = left + e1.DiffCapturedX;
                    int new_top = top + e1.DiffCapturedY;
                    uiSprite.SetLocation(new_left, new_top);
                    //-----
                    //also update controller position
                    polygonController.SetPosition(new_left, new_top);
                }
            };

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



    [DemoNote("9.2.1 ShapeControls")]
    class DemoShapeControl2 : App
    {
        LayoutFarm.CustomWidgets.PolygonController polygonController = new CustomWidgets.PolygonController();
        LayoutFarm.CustomWidgets.RectBoxController rectBoxController = new CustomWidgets.RectBoxController();


        VgRenderVx CreateTestRenderVx()
        {
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/cat_simple.svg";
            string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/tiger.svg";
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/freepik/dog1.svg";
            //string svgfile = "1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f30b.svg";
            //string svgfile = "../Data/1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f370.svg";
            return ReadSvgFile(svgfile);
        }
        VgRenderVx CreateTestRenderVx2()
        {
            var spec = new Svg.SvgPathSpec() { FillColor = Color.Red };
            SvgRenderRootElement renderRoot = new SvgRenderRootElement();
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);

            using (VxsContext.Temp(out VertexStore vxs))
            {
                //red-triangle ***
                vxs.AddMoveTo(100, 20);
                vxs.AddLineTo(150, 50);
                vxs.AddLineTo(110, 80);
                vxs.AddCloseFigure();
                renderE._vxsPath = vxs.CreateTrim();
            }

            return svgRenderVx;
        }
        protected override void OnStart(AppHost host)
        {
            VgRenderVx svgRenderVx = CreateTestRenderVx();

            svgRenderVx.DisableBackingImage = true;
            var uiSprite = new UISprite(10, 10); //init size = (10,10), location=(0,0) 
            uiSprite.DisableBmpCache = true;
            uiSprite.LoadSvg(svgRenderVx);//
            host.AddChild(uiSprite);

            var spriteEvListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(spriteEvListener);



            //box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            //box1.BackColor = Color.Red;
            //box1.SetLocation(10, 10);
            ////box1.dbugTag = 1;
            //SetupActiveBoxProperties(box1);
            //viewport.AddContent(box1);
            //-------- 
            rectBoxController.Init();
            //polygonController.Visible = false;
            host.AddChild(polygonController);
            //-------------------------------------------
            host.AddChild(rectBoxController);

            //foreach (var ui in rectBoxController.GetControllerIter())
            //{
            //    viewport.AddContent(ui);
            //}

            spriteEvListener.MouseDown += e1 =>
            {
                //mousedown on ui sprite 
                //find exact part ... 
                SvgHitInfo hitInfo = uiSprite.FindRenderElementAtPos(e1.X, e1.Y, true);
                if (hitInfo.svg != null &&
                    hitInfo.svg._vxsPath != null)
                {

                    polygonController.UpdateControlPoints(hitInfo.copyOfVxs);
                }
                //polygonController.SetPosition((int)uiSprite.Left, (int)uiSprite.Top);
                //polygonController.SetTargetUISprite(uiSprite);
                //polygonController.UpdateControlPoints(svgRenderVx._renderE._vxsPath);

            };
            spriteEvListener.MouseMove += e1 =>
            {
                if (e1.IsDragging)
                {
                    //drag event on uisprite

                    int left = (int)uiSprite.Left;
                    int top = (int)uiSprite.Top;

                    int new_left = left + e1.DiffCapturedX;
                    int new_top = top + e1.DiffCapturedY;
                    uiSprite.SetLocation(new_left, new_top);
                    //-----
                    //also update controller position
                    polygonController.SetPosition(new_left, new_top);
                }
            };

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

        VgRenderVx ReadSvgFile(string filename)
        {

            string svgContent = System.IO.File.ReadAllText(filename);
            SvgDocBuilder docBuidler = new SvgDocBuilder();
            SvgParser parser = new SvgParser(docBuidler);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //TODO: review this step again
            SvgRenderVxDocBuilder builder = new SvgRenderVxDocBuilder();
            return builder.CreateRenderVx(docBuidler.ResultDocument, svgElem =>
            {

            });
        }
    }
}