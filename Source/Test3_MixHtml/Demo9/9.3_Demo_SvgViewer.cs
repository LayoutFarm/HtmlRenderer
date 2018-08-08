//MIT, 2018-present, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using LayoutFarm.Svg;
using PaintLab.Svg;
namespace LayoutFarm.ColorBlenderSample
{
    [DemoNote("9.3 SvgViewer")]
    class DemoSvgViewer : App
    {
        AppHost host;

        ListView _lstvw_svgFiles;
        BackDrawBoardUI _backBoard;
        SvgParser parser;



        protected override void OnStart(AppHost host)
        {
            this.host = host;
            base.OnStart(host);


            {
                _backBoard = new BackDrawBoardUI(800, 600);
                _backBoard.SetLocation(100, 100);

                _backBoard.BackColor = PixelFarm.Drawing.Color.White;
                host.AddChild(_backBoard);
            }
            {
                _lstvw_svgFiles = new ListView(200, 400);
                _lstvw_svgFiles.SetLocation(500, 20);
                host.AddChild(_lstvw_svgFiles);
                _lstvw_svgFiles.ListItemMouseEvent += (s, e) =>
                {
                    if (_lstvw_svgFiles.SelectedIndex > -1)
                    {
                        if (_lstvw_svgFiles.SelectedIndex > -1)
                        {
                            string filename = _lstvw_svgFiles.GetItem(_lstvw_svgFiles.SelectedIndex).Tag as string;
                            if (filename != null)
                            {
                                ParseAndRenderSvgFile(filename);
                            }
                        }
                    }
                };

                foreach (string file in System.IO.Directory.GetFiles("../Test8_HtmlRenderer.Demo/Samples/Svg/others"))
                {
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = System.IO.Path.GetFileName(file);
                    listItem.Tag = file;
                    _lstvw_svgFiles.AddItem(listItem);
                }
                foreach (string file in System.IO.Directory.GetFiles("../Test8_HtmlRenderer.Demo/Samples/Svg/freepik"))
                {
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = System.IO.Path.GetFileName(file);
                    listItem.Tag = file;
                    _lstvw_svgFiles.AddItem(listItem);
                }
                foreach (string file in System.IO.Directory.GetFiles("../Test8_HtmlRenderer.Demo/Samples/Svg/twemoji"))
                {
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = System.IO.Path.GetFileName(file);
                    listItem.Tag = file;
                    _lstvw_svgFiles.AddItem(listItem);
                }

            }
        }
        void ParseAndRenderSvgFile(string svgFile)
        {
            var docBuilder = new SvgDocBuilder();
            parser = new SvgParser(docBuilder);

            string svgContent = System.IO.File.ReadAllText(svgFile);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //
            SvgRenderVxDocBuilder builder = new SvgRenderVxDocBuilder();
            VgRenderVx svgRenderVx = builder.CreateRenderVx(docBuilder.ResultDocument);

            var uiSprite = new UISprite(10, 10);
            var evListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(evListener);
            evListener.MouseDown += (e) =>
            {
                //hit on svg color- area

            };

             
            uiSprite.LoadSvg(svgRenderVx);

            _backBoard.ClearChildren();
            _backBoard.AddChild(uiSprite);
        }
    }
}