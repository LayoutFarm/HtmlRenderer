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
                    if (this.IsOpened)
                    {
                        this.Close();
                    }
                    else
                    {
                        this.Open();
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
            floatPart = new MenuBox(400, 200);
            if (childItems != null)
            {
                int j = childItems.Count;
                for (int i = 0; i < j; ++i)
                {
                    floatPart.AddChildBox(childItems[i]);
                }
            }
            return pnode;
        }
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

            switch (floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        if (this.ownerMenuBox == null) return;
                        var topRenderBox = ownerMenuBox.TopWindowRenderBox;
                        if (topRenderBox == null) return;
                        //------------------------------------------------ 
                        if (floatPart != null)
                        {
                            //show float part
                            //position relative to this menuItem
                            var htmlPNode = pnode as HtmlElement;
                            HtmlBoxes.CssBox nodePrincipalBox = htmlPNode.GetPrincipalBox();
                            //find global position of nodePrincipalBox
                            Point p = nodePrincipalBox.GetElementGlobalLocation();

                            floatPart.ShowMenu(this.ownerMenuBox.RootGfx, this.ownerMenuBox.HtmlHost);
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

            switch (floatPartStyle)
            {
                default:
                    {
                    } break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (this.ownerMenuBox == null) return;
                        //------------------------------------------------ 
                        var topRenderBox = ownerMenuBox.TopWindowRenderBox;
                        if (topRenderBox == null) return;
                        //------------------------------------------------ 
                        HtmlElement htmlPNode = pnode as HtmlElement;
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
        internal void SetOwnerMenuBox(MenuBox menuBox)
        {
            this.ownerMenuBox = menuBox;
        }
        public void MaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = true;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public void UnmaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = false;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public bool MaintenceOpenState
        {
            get;
            private set;
        }
        public void CloseRecursiveUp()
        {
            this.Close();

            if (this.ParentMenuItem != null &&
               !this.ParentMenuItem.MaintenceOpenState)
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

    public class MenuBox : LightHtmlWidgetBase
    {
        bool showing;
        TopWindowRenderBox topWindow;
        UIElement primaryUI;
        List<MenuItem> menuItems;
        DomElement pnode;

        public MenuBox(int w, int h)
            : base(w, h)
        {
        }
        public override UIElement GetPrimaryUIElement(HtmlBoxes.HtmlHost htmlhost)
        {
            return this.primaryUI = base.GetPrimaryUIElement(htmlhost);
        }
        protected override DomElement GetPresentationDomNode(DomElement hostNode)
        {
            if (pnode != null) return pnode;
            //------------------
            var doc = hostNode.OwnerDocument;
            pnode = doc.CreateElement("div");
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
            mnuItem.SetOwnerMenuBox(this);
        }
        internal TopWindowRenderBox TopWindowRenderBox
        {
            get { return this.topWindow; }
        }

        public void SetTopWindowRenderBox(TopWindowRenderBox topwin)
        {
            this.topWindow = topwin;
        }
        public void ShowMenu2(RootGraphic rootgfx, HtmlBoxes.HtmlHost htmlHost)
        {
            //add to top most

        }
        public void ShowMenu(RootGraphic rootgfx, HtmlBoxes.HtmlHost htmlHost)
        {
            //add to topmost box 
            if (!showing)
            {
                this.topWindow = rootgfx.TopWindowRenderBox;
                if (topWindow != null)
                {
                    if (pnode == null)
                    {
                        var primUI = this.GetPrimaryUIElement(htmlHost) as LightHtmlBox;
                        this.topWindow.AddChild(primaryUI.GetPrimaryRenderElement(rootgfx));
                    }
                    else
                    {
                        var parent = pnode.ParentNode as HtmlElement;
                        if (parent == null)
                        {
                            var primUI = this.GetPrimaryUIElement(htmlHost) as LightHtmlBox;
                            var htmldoc = primUI.HtmlContainer.WebDocument as HtmlDocument;
                            htmldoc.RootNode.AddChild(pnode);
                        }
                        this.topWindow.AddChild(primaryUI.GetPrimaryRenderElement(rootgfx));
                    }
                }
                showing = true;
            }
        }

        public void HideMenu()
        {
            if (showing)
            {
                //remove from parent
                if (this.pnode != null)
                {
                    var parent = pnode.ParentNode as HtmlElement;
                    if (parent != null)
                    {
                        var renderE = this.GetPrimaryRenderElement2(this.RootGfx);
                        parent.RemoveChild(pnode);
                        if (renderE != null)
                        {
                            this.topWindow.RemoveChild(renderE);
                        }
                    }
                }
                showing = false;
            }
        }


        internal RootGraphic RootGfx
        {
            get { return this.topWindow.Root; }
        }
        internal RenderElement GetPrimaryRenderElement2(RootGraphic rootgfx)
        {
            if (this.primaryUI != null)
            {
                return primaryUI.GetPrimaryRenderElement(rootgfx);
            }
            return null;
        }

    }
}