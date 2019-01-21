//MIT, 2018-present, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using PaintLab.Svg;
namespace LayoutFarm.ColorBlenderSample
{
    [DemoNote("9.3 SvgViewer")]
    class DemoSvgViewer : App
    {
        AppHost _host;

        ListView _lstvw_svgFiles;
        BackDrawBoardUI _backBoard;
        VgVisualDocHost _vgDocHost;
        protected override void OnStart(AppHost host)
        {
            _host = host;
            base.OnStart(host);

            _vgDocHost = new VgVisualDocHost();
            _vgDocHost.SetImgRequestDelgate(ImgBinderLoadImg);
            _vgDocHost.SetInvalidateDelegate(vgElem =>
            {
                //TODO

            });
            //
            {
                _backBoard = new BackDrawBoardUI(800, 600);
                _backBoard.SetLocation(0, 0);
                _backBoard.BackColor = PixelFarm.Drawing.Color.White;
                host.AddChild(_backBoard);
            }
            {
                _lstvw_svgFiles = new ListView(200, 400);
                _lstvw_svgFiles.SetLocation(500, 20);
                host.AddChild(_lstvw_svgFiles);
                //
                _lstvw_svgFiles.ListItemMouseEvent += (s, e) =>
                {
                    if (_lstvw_svgFiles.SelectedIndex > -1)
                    {
                        string filename = _lstvw_svgFiles.GetItem(_lstvw_svgFiles.SelectedIndex).Tag as string;
                        if (filename != null)
                        {
                            ParseAndRenderSvgFile(filename);
                        }
                    }
                };

                //foreach (string file in System.IO.Directory.GetFiles("../Test8_HtmlRenderer.Demo/Samples/Svg/others", "*.svg"))
                //{
                //    ListItem listItem = new ListItem(200, 20);
                //    listItem.Text = System.IO.Path.GetFileName(file);
                //    listItem.Tag = file;
                //    _lstvw_svgFiles.AddItem(listItem);
                //}
                //foreach (string file in System.IO.Directory.GetFiles("../Test8_HtmlRenderer.Demo/Samples/Svg/freepik", "*.svg"))
                //{
                //    ListItem listItem = new ListItem(200, 20);
                //    listItem.Text = System.IO.Path.GetFileName(file);
                //    listItem.Tag = file;
                //    _lstvw_svgFiles.AddItem(listItem);
                //}
                //foreach (string file in System.IO.Directory.GetFiles("../Test8_HtmlRenderer.Demo/Samples/Svg/twemoji", "*.svg"))
                //{
                //    ListItem listItem = new ListItem(200, 20);
                //    listItem.Text = System.IO.Path.GetFileName(file);
                //    listItem.Tag = file;
                //    _lstvw_svgFiles.AddItem(listItem);
                //}
                foreach (string file in System.IO.Directory.GetFiles("../Test8_HtmlRenderer.Demo/Samples/Svg/noto_emoji", "*.svg"))
                {
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = System.IO.Path.GetFileName(file);
                    listItem.Tag = file;
                    _lstvw_svgFiles.AddItem(listItem);
                }
            }
        }
        void ImgBinderLoadImg(ImageBinder reqImgBinder, VgVisualElement vgVisualE, object o)
        {
            PixelFarm.Drawing.Image img = _host.LoadImage(reqImgBinder.ImageSource);
            reqImgBinder.SetLocalImage(img);
            reqImgBinder.State = BinderState.Loaded;
        }
        void ParseAndRenderSvgFile(string svgFile)
        {
            var docBuilder = new SvgDocBuilder();
            var parser = new SvgParser(docBuilder);

            string svgContent = System.IO.File.ReadAllText(svgFile);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //

            VgVisualDocBuilder builder = new VgVisualDocBuilder();
            VgVisualElement vgVisElem = builder.CreateVgVisualDoc(docBuilder.ResultDocument, _vgDocHost).VgRootElem;

            var uiSprite = new UISprite(10, 10);
            var evListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(evListener);
            evListener.MouseDown += (e) =>
            {
                //hit on svg color- area
                VgHitInfo hitInfo = uiSprite.FindRenderElementAtPos(e.X, e.Y, false);
#if DEBUG
                if (hitInfo.hitElem != null)
                {

                    SvgElement domElem = hitInfo.hitElem.DomElem;
                    if (domElem != null && domElem.ElemId != null)
                    {

                        Console.WriteLine(domElem.ElemId);
                    }

                }
#endif

            };


            uiSprite.LoadVg(vgVisElem);

            _backBoard.ClearChildren();
            _backBoard.AddChild(uiSprite);
        } 
    }
}