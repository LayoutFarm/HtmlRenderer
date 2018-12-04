//Apache2, 2014-present, WinterDev

using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using System.Collections.Generic;
namespace LayoutFarm.HtmlWidgets
{
    public class MenuItem
    {

        DomElement _pnode;
        DomElement _menuIcon;
        MenuBox _ownerMenuBox;
        bool _thisMenuOpened;
        //2. float part   
        MenuBox _floatPart;
        HingeFloatPartStyle _floatPartStyle;
        List<MenuItem> _childItems;

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
        //
        public int Width => _width;
        public int Height => _height;
        //
        public DomElement GetPresentationDomNode(DomElement hostNode)
        {
            if (_pnode != null) return _pnode;
            //-----------------------------------
            var doc = hostNode.OwnerDocument;
            this._pnode = doc.CreateElement("div");

            _pnode.AddChild("img", item_icon =>
            {
                _menuIcon = item_icon;
                _menuIcon.AttachMouseDownEvent(e =>
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
                _menuIcon.AttachMouseUpEvent(e =>
                {
                    this.UnmaintenanceParentOpenState();
                    e.StopPropagation();
                });
                _menuIcon.AttachEventOnMouseLostFocus(e =>
                {
                    if (!this.MaintainOpenState)
                    {
                        this.CloseRecursiveUp();
                    }
                });
            });
            _pnode.AddChild("span", content =>
            {
                if (MenuItemText != null)
                {
                    _pnode.AddTextContent(this.MenuItemText);
                }
            });
            //--------------------------------------------------------
            //create simple menu item box 

            if (_childItems != null)
            {
                _floatPart = new MenuBox(200, 200);
                int j = _childItems.Count;
                for (int i = 0; i < j; ++i)
                {
                    _floatPart.AddChildBox(_childItems[i]);
                }
            }
            return _pnode;
        }
        //
        public DomElement CurrentDomElement => _pnode;
        //
        public string MenuItemText { get; set; }
        //
        public bool IsOpened => _thisMenuOpened;
        //
        public void Open()
        {
            if (_thisMenuOpened) return;
            _thisMenuOpened = true;
            //-----------------------------------
            if (_pnode == null) return;
            if (_floatPart == null) return;
            _ownerMenuBox.CurrentActiveMenuItem = this;
            switch (_floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        if (_ownerMenuBox == null) return;
                        if (_floatPart != null)
                        {
                            _floatPart.ShowMenu(this);
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
            if (!_thisMenuOpened) return;
            //
            _thisMenuOpened = false;
            if (_pnode == null) return;
            if (_floatPart == null) return;
            _ownerMenuBox.CurrentActiveMenuItem = null;
            switch (_floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (_ownerMenuBox == null) return;
                        if (_floatPart != null)
                        {
                            _floatPart.HideMenu();
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
            get => _ownerMenuBox;
            set => _ownerMenuBox = value;
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
            get => _floatPartStyle;
            set => _floatPartStyle = value;
        }
        public void AddSubMenuItem(MenuItem childItem)
        {
            if (_childItems == null)
            {
                _childItems = new List<MenuItem>();
            }
            _childItems.Add(childItem);
            childItem.ParentMenuItem = this;
            if (_floatPart != null)
            {
                _floatPart.AddChildBox(childItem);
            }
        }
    }

    public class MenuBox : HtmlWidgetBase
    {
        bool _showing;
        List<MenuItem> _menuItems;
        DomElement _presentation;
        MenuItem _currentActiveMenuItem;
        WebDom.Impl.HtmlDocument _htmldoc;

#if DEBUG
        static int s_dbugTotalId;
        public readonly int dbug_id = s_dbugTotalId++;
#endif
        public MenuBox(int w, int h)
            : base(w, h)
        {

        }
        public bool IsLandPart { get; set; }

        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {

            if (_presentation != null) return _presentation;
            _htmldoc = htmldoc;
            //presentation main node
            _presentation = htmldoc.CreateElement("div");

            //TODO: review IsLandPart again, this is temp fixed 
            if (!this.IsLandPart)
            {
                _presentation.SetAttribute("style", "position:absolute;width:" + this.Width + "px;height:" + this.Height + "px");
            }

            if (_menuItems != null)
            {
                int j = _menuItems.Count;
                for (int i = 0; i < j; ++i)
                {
                    _presentation.AddChild(_menuItems[i].GetPresentationDomNode(_presentation));
                }
            }
            return _presentation;
        }
        //
        internal WebDom.Impl.HtmlDocument HtmlDoc => _htmldoc;
        //
        internal MenuItem CurrentActiveMenuItem
        {
            get => _currentActiveMenuItem;
            set => _currentActiveMenuItem = value;
        }
        //
        public void AddChildBox(MenuItem mnuItem)
        {
            if (_menuItems == null)
            {
                _menuItems = new List<MenuItem>();
            }
            _menuItems.Add(mnuItem);
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
            if (!_showing)
            {
                //create presentation node
                if (_presentation == null)
                {
                    _htmldoc = relativeToMenuItem.OwnerMenuBox.HtmlDoc;
                    _presentation = this.GetPresentationDomNode(_htmldoc);
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
                _htmldoc.RootNode.AddChild(_presentation);

                _showing = true;
            }
        }

        public void HideMenu()
        {
#if DEBUG
            //if (this.dbug_id == 1)
            //{

            //}
#endif

            if (_showing)
            {


                _currentActiveMenuItem?.Close();

                //remove from parent
                if (_presentation != null)
                {
                    var parent = _presentation.ParentNode as IHtmlElement;
                    if (parent != null)
                    {
                        parent.removeChild(_presentation);
                    }
                }
                _showing = false;
            }
        }

    }
}