// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm.HtmlWidgets
{

    public class MenuItem
    {
        string menuItemText;

        DomElement pnode;
        DomElement menuIcon;

        MenuBox ownerMenuBox;

        bool thisMenuOpened;

        //2. float part   
        MenuBox floatPart;
        HingeFloatPartStyle floatPartStyle;
        List<MenuItem> childItems;

        public MenuItem(int width, int height)
        {

        }
        public DomElement GetPresentationDomNode(DomElement hostNode)
        {
            if (pnode != null) return pnode;
            //-----------------------------------
            var doc = hostNode.OwnerDocument;
            this.pnode = doc.CreateElement("div");
            pnode.AddChild("img", item_icon =>
            {
                menuIcon = item_icon;
                menuIcon.AttachMouseDownEvent(e =>
                {
                    //****
                    this.MaintenanceParentOpenState();

                    if (this.IsOpened)
                    {
                        this.Close();
                    }
                    else
                    {
                        this.Open();
                    }
                    e.StopPropagation();
                });
                menuIcon.AttachMouseUpEvent(e =>
                {
                    this.UnmaintenanceParentOpenState();
                    e.StopPropagation();
                });
                menuIcon.AttachEventOnMouseLostFocus(e =>
                {
                    if (!this.MaintenaceOpenState)
                    {
                        this.CloseRecursiveUp();
                    }
                });
            });
            pnode.AddChild("span", content =>
            {
                if (menuItemText != null)
                {
                    pnode.AddTextContent(this.menuItemText);
                }
            });

            //--------------------------------------------------------
            //create simple menu item box 

            if (childItems != null)
            {
                floatPart = new MenuBox(400, 200);
                int j = childItems.Count;
                for (int i = 0; i < j; ++i)
                {
                    floatPart.AddChildBox(childItems[i]);
                }
            }
            return pnode;
        }
        public DomElement CurrentDomElement { get { return this.pnode; } }
        public string MenuItemText
        {
            get { return this.menuItemText; }
            set
            {
                this.menuItemText = value;
            }
        }

        public bool IsOpened
        {
            get { return this.thisMenuOpened; }
        }
        public void Open()
        {
            if (thisMenuOpened) return;
            this.thisMenuOpened = true;

            //-----------------------------------
            if (pnode == null) return;
            if (floatPart == null) return;

            this.ownerMenuBox.CurrentActiveMenuItem = this;

            switch (floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        if (this.ownerMenuBox == null) return;
                        if (floatPart != null)
                        {
                            floatPart.ShowMenu(this);
                        }


                    } break;
                case HingeFloatPartStyle.Embeded:
                    {

                    } break;
            }

        }
        public void Close()
        {
            if (!thisMenuOpened) return;
            this.thisMenuOpened = false;

            if (pnode == null) return;
            if (floatPart == null) return;

            this.ownerMenuBox.CurrentActiveMenuItem = null;

            switch (floatPartStyle)
            {
                default:
                    {
                    } break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (this.ownerMenuBox == null) return;

                        if (floatPart != null)
                        {
                            floatPart.HideMenu();
                        }

                    } break;
                case HingeFloatPartStyle.Embeded:
                    {
                    } break;

            }
        }

        internal MenuBox OwnerMenuBox
        {
            get { return this.ownerMenuBox; }
            set { this.ownerMenuBox = value; }
        }
        public void MaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenaceOpenState = true;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public void UnmaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenaceOpenState = false;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public bool MaintenaceOpenState
        {
            get;
            private set;
        }
        public void CloseRecursiveUp()
        {
            this.Close();

            if (this.ParentMenuItem != null &&
               !this.ParentMenuItem.MaintenaceOpenState)
            {
                this.ParentMenuItem.CloseRecursiveUp();
            }
        }
        public MenuItem ParentMenuItem
        {
            get;
            private set;
        }
        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this.floatPartStyle; }
            set
            {
                this.floatPartStyle = value;
            }
        }
        public void AddSubMenuItem(MenuItem childItem)
        {
            if (childItems == null)
            {
                childItems = new List<MenuItem>();
            }
            this.childItems.Add(childItem);
            childItem.ParentMenuItem = this;
            if (floatPart != null)
            {
                floatPart.AddChildBox(childItem);
            }

        }
    }

    public class MenuBox : HtmlWidgetBase
    {
        bool showing;
        List<MenuItem> menuItems;
        DomElement pnode;
        MenuItem currentActiveMenuItem;
        WebDom.Impl.HtmlDocument htmldoc;
        public MenuBox(int w, int h)
            : base(w, h)
        {
        }
        public bool IsLandPart
        {
            get;
            set;
        }

        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {

            if (pnode != null) return pnode;
            this.htmldoc = htmldoc;

            pnode = htmldoc.CreateElement("div");
            //TODO: review IsLandPart again, this is temp fixed 
            if (!this.IsLandPart)
            {
                pnode.SetAttribute("style", "position:absolute;width:100px;height:200px");
            }

            if (menuItems != null)
            {
                int j = menuItems.Count;
                for (int i = 0; i < j; ++i)
                {
                    pnode.AddChild(menuItems[i].GetPresentationDomNode(pnode));
                }
            }
            return pnode;
        }
        internal WebDom.Impl.HtmlDocument HtmlDoc
        {
            get { return this.htmldoc; }
        }
        public void AddChildBox(MenuItem mnuItem)
        {
            if (menuItems == null)
            {
                menuItems = new List<MenuItem>();
            }
            this.menuItems.Add(mnuItem);
            if (pnode != null)
            {
                pnode.AddChild(mnuItem.GetPresentationDomNode(pnode));
            }
            mnuItem.OwnerMenuBox = this;
        }



        public void ShowMenu(MenuItem relativeToMenuItem)
        {
            //add to topmost box 
            if (!showing)
            {

                //create presentation node
                if (this.pnode == null)
                {
                    this.htmldoc = relativeToMenuItem.OwnerMenuBox.HtmlDoc;
                    this.pnode = this.GetPresentationDomNode(htmldoc);
                }


                var relativeMenuItemElement = relativeToMenuItem.CurrentDomElement as IHtmlElement;
                int x, y;

                relativeMenuItemElement.getGlobalLocation(out x, out y);
                var pHtmlNode = pnode as WebDom.Impl.HtmlElement;
                //TODO: review InnerContentWidth again
                pHtmlNode.SetLocation(x + relativeToMenuItem.OwnerMenuBox.Width, y);


                htmldoc.RootNode.AddChild(pnode);
                showing = true;
            }
        }
        public override int Width
        {
            get
            {
                if (this.pnode != null)
                {
                    return (int)((WebDom.Impl.HtmlElement)pnode).ActualWidth;
                }
                return base.Width;
            }
        }
        public void HideMenu()
        {
            if (showing)
            {
                if (this.currentActiveMenuItem != null)
                {
                    this.currentActiveMenuItem.Close();
                }
                //remove from parent
                if (this.pnode != null)
                {
                    var parent = pnode.ParentNode as IHtmlElement;
                    if (parent != null)
                    {
                        parent.removeChild(pnode);
                    }
                }
                showing = false;
            }
        }

        internal MenuItem CurrentActiveMenuItem
        {
            get { return this.currentActiveMenuItem; }
            set { this.currentActiveMenuItem = value; }
        }



    }
}