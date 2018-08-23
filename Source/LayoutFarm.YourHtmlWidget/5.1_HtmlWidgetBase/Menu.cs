//Apache2, 2014-present, WinterDev

using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using System.Collections.Generic;
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

        int _width;
        int _height;
#if DEBUG
        static int s_dbugTotalId;
        public readonly int dbug_id = s_dbugTotalId++;
#endif

        public MenuItem(int width, int height)
        {
            //size of each item
            _width = width;
            _height = height;
        }

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

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
                    this.MaintainParentOpenState();
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
                    if (!this.MaintainOpenState)
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
                floatPart = new MenuBox(200, 200);
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
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void Close()
        {
            if (!thisMenuOpened) return;
            //
            this.thisMenuOpened = false;
            if (pnode == null) return;
            if (floatPart == null) return;
            this.ownerMenuBox.CurrentActiveMenuItem = null;
            switch (floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (this.ownerMenuBox == null) return;
                        if (floatPart != null)
                        {
                            floatPart.HideMenu();
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }

        internal MenuBox OwnerMenuBox
        {
            get { return this.ownerMenuBox; }
            set { this.ownerMenuBox = value; }
        }
        public void MaintainParentOpenState()
        {
            //recursive


            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintainOpenState = true;
                //recursive
                this.ParentMenuItem.MaintainParentOpenState();
            }
        }
        public void UnmaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintainOpenState = false;
                this.ParentMenuItem.UnmaintenanceParentOpenState();
            }
        }
        public bool MaintainOpenState
        {
            get;
            private set;
        }
        public void CloseRecursiveUp()
        {
            this.Close();
            if (this.ParentMenuItem != null &&
               !this.ParentMenuItem.MaintainOpenState)
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
        DomElement _presentation;
        MenuItem currentActiveMenuItem;
        WebDom.Impl.HtmlDocument htmldoc;

#if DEBUG
        static int s_dbugTotalId;
        public readonly int dbug_id = s_dbugTotalId++;
#endif
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

            if (_presentation != null) return _presentation;
            this.htmldoc = htmldoc;
            //presentation main node
            _presentation = htmldoc.CreateElement("div");

            //TODO: review IsLandPart again, this is temp fixed 
            if (!this.IsLandPart)
            {
                _presentation.SetAttribute("style", "position:absolute;width:" + this.Width + "px;height:" + this.Height + "px");
            }

            if (menuItems != null)
            {
                int j = menuItems.Count;
                for (int i = 0; i < j; ++i)
                {
                    _presentation.AddChild(menuItems[i].GetPresentationDomNode(_presentation));
                }
            }



            return _presentation;
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
            if (_presentation != null)
            {
                _presentation.AddChild(mnuItem.GetPresentationDomNode(_presentation));
            }
            mnuItem.OwnerMenuBox = this;
        }



        public void ShowMenu(MenuItem relativeToMenuItem)
        {
#if DEBUG
            //if (this.dbug_id == 1)
            //{

            //}

#endif
            //add to topmost box 
            if (!showing)
            {
                //create presentation node
                if (this._presentation == null)
                {
                    this.htmldoc = relativeToMenuItem.OwnerMenuBox.HtmlDoc;
                    this._presentation = this.GetPresentationDomNode(htmldoc);
                }
                var relativeMenuItemElement = relativeToMenuItem.CurrentDomElement as IHtmlElement;
                int x, y;
                relativeMenuItemElement.getGlobalLocation(out x, out y);
                var pHtmlNode = _presentation as WebDom.Impl.HtmlElement;

                if (relativeToMenuItem.OwnerMenuBox.IsLandPart)
                {
                    pHtmlNode.SetLocation(x, y);
                }
                else
                {
                    pHtmlNode.SetLocation(x + relativeToMenuItem.OwnerMenuBox.Width, y);
                }


                //pHtmlNode.SetLocation(x, y);
                htmldoc.RootNode.AddChild(_presentation);

                showing = true;
            }
        }

        public void HideMenu()
        {
#if DEBUG
            //if (this.dbug_id == 1)
            //{

            //}
#endif

            if (showing)
            {

                if (this.currentActiveMenuItem != null)
                {
                    this.currentActiveMenuItem.Close();
                }
                //remove from parent
                if (this._presentation != null)
                {
                    var parent = _presentation.ParentNode as IHtmlElement;
                    if (parent != null)
                    {
                        parent.removeChild(_presentation);
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