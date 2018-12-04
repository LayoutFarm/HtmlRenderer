//Apache2, 2014-present, WinterDev

using System;

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
namespace LayoutFarm.HtmlWidgets
{
    public class TreeView : HtmlWidgetBase
    {
        //composite          
        List<TreeNode> _treeNodes = new List<TreeNode>();
        DomElement _pnode;
        //content 
        public TreeView(int width, int height)
            : base(width, height)
        {
        }
        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (_pnode != null) return _pnode;
            //create primary presentation node

            _pnode = htmldoc.CreateElement("div");
            _pnode.SetAttribute("style", "font:10pt tahoma");
            int j = _treeNodes.Count;
            for (int i = 0; i < j; ++i)
            {
                _pnode.AddChild(_treeNodes[i].GetPrimaryPresentationNode(_pnode));
            }
            return _pnode;
        }
        public void AddItem(TreeNode treeNode)
        {
            _treeNodes.Add(treeNode);
            if (_pnode != null)
            {
            }
        }
        public void PerformContentLayout()
        {
        }
    }

    public class TreeNode
    {
        const int NODE_DEFAULT_HEIGHT = 17;
        Color _backColor;
        bool _isOpen = true;//test, open by default
        int _newChildNodeY = NODE_DEFAULT_HEIGHT;
        int _indentWidth = 17;
        int _desiredHeight = 0; //after layout
        List<TreeNode> _childNodes;
        TreeNode _parentNode;
        TreeView _ownerTreeView;
        //-------------------------- 

        DomElement _pnode;
        DomElement _nodeBar;
        DomElement _nodeIcon;
        DomElement _nodeSpan;
        DomElement _nodeBody;
        DomElement _nodeContent;

        int _width;
        int _height;
        public TreeNode(int width, int height)
        {
            _width = width;
            _height = height;
        }
        public string NodeText { get; set; }

        void SetupNodeIconBehaviour(DomElement uiNodeIcon)
        {
            uiNodeIcon.AttachMouseDownEvent(e =>
            {
                if (this.IsOpen)
                {
                    //then close
                    this.Collapse();
                }
                else
                {
                    this.Expand();
                }
            });
        }
        public DomElement GetPrimaryPresentationNode(DomElement hostNode)
        {
            if (this._pnode != null) return _pnode;
            //---------------------------------
            var ownerdoc = hostNode.OwnerDocument;
            _pnode = ownerdoc.CreateElement("div");
            //---------------------------------
            //bar part
            _pnode.AddChild("div", node_bar =>
            {
                this._nodeBar = node_bar;
                node_bar.AddChild("img", node_icon =>
                {
                    this._nodeIcon = node_icon;
                    SetupNodeIconBehaviour(node_icon);
                });
                node_bar.AddChild("span", node_span =>
                {
                    this._nodeSpan = node_span;
                    if (NodeText != null)
                    {
                        node_span.AddTextContent(NodeText);
                    }
                });
            });
            //---------------------------------
            //content part
            //indent  
            _pnode.AddChild("div", node_body =>
            {
                _nodeBody = node_body;
                node_body.SetAttribute("style", "padding-left:17px");
                node_body.AddChild("div", node_content =>
                {
                    _nodeContent = node_content;
                    if (_childNodes != null)
                    {
                        _nodeContent.SetAttribute("style", "padding-left:0px");
                        int j = _childNodes.Count;
                        for (int i = 0; i < j; ++i)
                        {
                            var childnode = _childNodes[i].GetPrimaryPresentationNode(_nodeContent);
                            node_content.AddChild(childnode);
                        }
                    }
                });
            });
            return _pnode;
        }
        DomElement GetPrimaryPresentationNode2(DomElement hostNode)
        {
            //implement wth table

            if (_pnode != null) return _pnode;
            //---------------------------------
            var ownerdoc = hostNode.OwnerDocument;
            _pnode = ownerdoc.CreateElement("div");
            //---------------------------------
            //bar part
            _pnode.AddChild("div", node_bar =>
            {
                _nodeBar = node_bar;
                node_bar.AddChild("img", node_icon =>
                {
                    _nodeIcon = node_icon;
                    SetupNodeIconBehaviour(node_icon);
                });
                node_bar.AddChild("span", node_span =>
                {
                    _nodeSpan = node_span;
                    if (NodeText != null)
                    {
                        node_span.AddTextContent(NodeText);
                    }
                });
            });
            //---------------------------------
            //content part
            //indent  
            _pnode.AddChild("div", node_body =>
            {
                _nodeBody = node_body;
                //implement with table
                //plan: => implement with inline div***

                node_body.AddChild("table", table =>
                {
                    table.AddChild("tr", tr =>
                    {
                        tr.AddChild("td", td1 =>
                        {
                            //indent
                            td1.SetAttribute("style", "width:20px");
                        });
                        tr.AddChild("td", td1 =>
                        {
                            _nodeContent = td1;
                            if (_childNodes != null)
                            {
                                int j = _childNodes.Count;
                                for (int i = 0; i < j; ++i)
                                {
                                    var childnode = _childNodes[i].GetPrimaryPresentationNode(_nodeContent);
                                    _nodeContent.AddChild(childnode);
                                }
                            }
                        });
                    });
                });
                //node_body.SetAttribute("style", "background-color:yellow");
                //node_body.AddChild("div", node_indent =>
                //{
                //    node_indent.SetAttribute("style", "width:32px;display:inline");
                //    node_indent.AddChild("img", img2 => { });

                //});
                //node_body.AddChild("div", node_content =>
                //{
                //    node_content.SetAttribute("style", "display:inline");
                //    //start with blank content
                //    this.nodeContent = node_content;
                //    if (childNodes != null)
                //    {
                //        int j = childNodes.Count;
                //        for (int i = 0; i < j; ++i)
                //        {
                //            var childnode = childNodes[i].GetPrimaryPresentationNode(node_content);
                //            node_content.AddChild(childnode);
                //        }
                //    }

                //});
            });
            //---------------------
            return _pnode;
        }
        //
        public bool IsOpen => _isOpen;
        //
        public int ChildCount
        {
            get
            {
                if (_childNodes == null) return 0;
                return _childNodes.Count;
            }
        }
        //
        public TreeNode ParentNode => _parentNode;
        //
        public TreeView TreeView
        {
            get
            {
                if (_ownerTreeView != null)
                {
                    //top node
                    return _ownerTreeView;
                }
                else
                {
                    if (_parentNode != null)
                    {
                        //recursive
                        return _parentNode.TreeView;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        internal void SetOwnerTreeView(TreeView ownerTreeView)
        {
            _ownerTreeView = ownerTreeView;
        }
        public void AddChildNode(TreeNode treeNode)
        {
            if (_childNodes == null)
            {
                _childNodes = new List<TreeNode>();
            }
            _childNodes.Add(treeNode);
            treeNode._parentNode = this;
            //---------------------------
            //add treenode presentaion
            if (_isOpen)
            {
                if (_nodeContent != null)
                {
                    //add child presentation 
                    _nodeContent.AddChild(
                        treeNode.GetPrimaryPresentationNode(_nodeContent));
                }
            }
            //---------------------------
        }
        public void Expand()
        {
            if (_isOpen) return;
            _isOpen = true;
            if (_nodeBody != null)
            {
                if (_nodeBody.ParentNode == null)
                {
                    _pnode.AddChild(_nodeBody);
                }
            }

            //this.TreeView.PerformContentLayout();
        }
        public void Collapse()
        {
            if (!_isOpen) return;
            _isOpen = false;
            if (_nodeBody != null)
            {
                var htmlPNode = this._pnode as IHtmlElement;
                if (htmlPNode != null)
                {
                    htmlPNode.removeChild(_nodeBody);
                }
            }
            //this.TreeView.PerformContentLayout();
        }

        void SetupNodeIconBehaviour(ImageBox uiNodeIcon)
        {
            uiNodeIcon.MouseDown += (s, e) =>
            {
                if (this.IsOpen)
                {
                    //then close
                    this.Collapse();
                }
                else
                {
                    this.Expand();
                }
            };
        }
    }
}