//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.DzBoardSample
{
    class AppModule
    {
        DesignBoardModule dzBoardModule;
        MenuBoardModule menuModule;
        AppHost _host;
        LayoutFarm.HtmlBoxes.HtmlHost htmlHost;
        string documentRootPath;
        public void StartModule(LayoutFarm.AppHost host)
        {
            this._host = host;
            dzBoardModule = new DesignBoardModule();
            menuModule = new MenuBoardModule();
            menuModule.menuItemClick += new EventHandler<MenuItemClickEventArgs>(menuModule_menuItemClick);
            var contentMx = new LayoutFarm.ContentManagers.ImageContentManager();
            contentMx.ImageLoadingRequest += contentMx_ImageLoadingRequest;
            //app specific here
            documentRootPath = System.Windows.Forms.Application.ExecutablePath;
            this.htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(_host,
                (s, e) => contentMx.AddRequestImage(e.ImageBinder),
                contentMx_LoadStyleSheet);
            //1. design board
            dzBoardModule.StartModule(htmlHost, this._host);
            ////2. canvas board
            //drawingBoard.StartModule(htmlHost, this.viewport);
            //3. menu
            menuModule.StartModule(htmlHost, this._host);
            //------------------------------------------------------
            //context knowledge*** 
            //------------------------------------------------------
        }
        static void SerializeCkAwareBox(DzBoxSerializer visitor, UIElement ui)
        {
            var holder = ui as UIHolderBox;
            //serialize content of this panel             
            holder.Walk(visitor);
        }
        static void SerializeDzImageBox(DzBoxSerializer writer, UIElement ui)
        {
            ui.Walk(writer);
            //writer.BeginElement("imgbox");
            //var holder = ui as UIHolderBox;
            //var imgBox = holder.TargetBox as LayoutFarm.CustomWidgets.ImageBox;

            //writer.AddAttribute("left", (holder.Left + holder.HolderBorder).ToString());
            //writer.AddAttribute("top", (holder.Top + holder.HolderBorder).ToString());
            //writer.AddAttribute("width", imgBox.Width.ToString());
            //writer.AddAttribute("height", imgBox.Height.ToString());

            ////info about image
            //writer.AddAttribute("imgsrc", imgBox.ImageBinder.ImageSource);

            //writer.EndElement();
        }
        void menuModule_menuItemClick(object sender, MenuItemClickEventArgs e)
        {
            switch (e.MenuName)
            {
                case "Rect":
                    {
                        //create rect from spec 

                        dzBoardModule.AddNewRect(0, 0, 50, 50);
                    }
                    break;
                case "Box":
                    {
                        dzBoardModule.AddNewBox(20, 20, 50, 50);
                    }
                    break;
                case "Text":
                    {
                    }
                    break;
                case "Image":
                    {
                        //load image
                        //Image img = viewport.P.CreatePlatformBitmap(
                        //dzBoardModule.AddNewImageBox(20, 20, 100, 50);
                        ImageBinder imgBinder = LoadImage("../../Demo/favorites32.png");
                        var holder = dzBoardModule.AddNewImageBox(20, 20, 50, 50, imgBinder);
                        holder.SetSerializeDelegate(SerializeDzImageBox);
                        holder.SetCloneDelegate(CloneImageBox);
                    }
                    break;
            }
        }
        static UIElement CloneImageBox(DesignBox dzBox)
        {
            UIHolderBox holderBox = dzBox as UIHolderBox;
            var originalImgBox = holderBox.TargetBox as LayoutFarm.CustomWidgets.ImageBox;
            var newClone = new UIHolderBox(holderBox.Width, holderBox.Height);
            newClone.BackColor = Color.White;
            var imgbox = new LayoutFarm.CustomWidgets.ImageBox(newClone.Width - 10, newClone.Height - 10);
            imgbox.ImageBinder = originalImgBox.ImageBinder;
            //clone content of text box 

            newClone.SetCloneDelegate(CloneImageBox);
            newClone.SetSerializeDelegate(SerializeDzImageBox);
            newClone.AddChild(imgbox);
            newClone.TargetBox = imgbox;
            return newClone;
        }
        static ImageBinder LoadImage(string filename)
        {
            ImageBinder binder = new ClientImageBinder(filename);
            binder.SetImage(DemoBase.LoadBitmap(filename));
            binder.State = ImageBinderState.Loaded;
            return binder;
        }
        void contentMx_ImageLoadingRequest(object sender, LayoutFarm.ContentManagers.ImageRequestEventArgs e)
        {
            //load resource -- sync or async? 
            string absolutePath = documentRootPath + "\\" + e.ImagSource;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }
            //load             
            e.SetResultImage(DemoBase.LoadBitmap(absolutePath));
        }
        void contentMx_LoadStyleSheet(object sender, LayoutFarm.ContentManagers.TextRequestEventArgs e)
        {
            string absolutePath = documentRootPath + "\\" + e.Src;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }
            //if found
            e.TextContent = System.IO.File.ReadAllText(absolutePath);
        }

    

    }
}