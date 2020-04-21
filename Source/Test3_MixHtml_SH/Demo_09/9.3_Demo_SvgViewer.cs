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

        ListBox _lstbox_svgFiles;
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
                _lstbox_svgFiles = new ListBox(200, 400);
                _lstbox_svgFiles.SetLocation(500, 20);
                host.AddChild(_lstbox_svgFiles);
                //
                _lstbox_svgFiles.ListItemMouseEvent += (s, e) =>
                {
                    if (_lstbox_svgFiles.SelectedIndex > -1)
                    {
                        string filename = _lstbox_svgFiles.GetItem(_lstbox_svgFiles.SelectedIndex).Tag as string;
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
                //foreach (string file in System.IO.Directory.GetFiles("../../../HtmlRenderer.SomeTestResources/Svg/twemoji", "*.svg"))
                //{
                //    ListItem listItem = new ListItem(200, 20);
                //    listItem.Text = System.IO.Path.GetFileName(file);
                //    listItem.Tag = file;
                //    _lstvw_svgFiles.AddItem(listItem);
                //}


                //string[] allFiles = System.IO.Directory.GetFiles("../../../HtmlRenderer.SomeTestResources/Svg/noto_emoji", "*.svg");

                string rootSampleFolder = "..\\Test8_HtmlRenderer.Demo\\Samples\\Svg\\others";

                string[] allFiles = System.IO.Directory.GetFiles(rootSampleFolder, "*.svg");
                int i = 0;
                int lim = Math.Min(allFiles.Length, 150);

                for (; i < lim; ++i)
                {
                    string file = allFiles[i];
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = System.IO.Path.GetFileName(file);
                    listItem.Tag = file;
                    _lstbox_svgFiles.AddItem(listItem);
                }

            }
        }
        void ImgBinderLoadImg(PixelFarm.Drawing.ImageBinder reqImgBinder, VgVisualElement vgVisualE, object o)
        {
            PixelFarm.Drawing.Image img = _host.LoadImage(reqImgBinder.ImageSource);
            reqImgBinder.SetLocalImage(img);
            reqImgBinder.State = PixelFarm.Drawing.BinderState.Loaded;
        }
        void ParseAndRenderSvgFile(string svgFile)
        {

            var docBuilder = new VgDocBuilder();
            var svgParser = new SvgParser(docBuilder);

            //TODO: ask file content from host,

            string svgContent = System.IO.File.ReadAllText(svgFile);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            svgParser.ParseDocument(textSnapshot);
            //
#if DEBUG
            System.Diagnostics.Debug.WriteLine(svgFile);
#endif
            var vgDocBuilder = new VgVisualDocBuilder();
            VgVisualElement vgVisElem = vgDocBuilder.CreateVgVisualDoc(docBuilder.ResultDocument, _vgDocHost).VgRootElem;

            var uiSprite = new UISprite(10, 10);
            var evListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(evListener);
            evListener.MouseDown += (s, e) =>
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
            _backBoard.Add(uiSprite);
        }
    }
}