//Apache2, 2014-2017, WinterDev

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
        List<TreeNode> treeNodes = new List<TreeNode>();
        DomElement pnode;
        //content 
        public TreeView(int width, int height)
            : base(width, height)
        {
        }
        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (pnode != null) return pnode;
            //create primary presentation node

            pnode = htmldoc.CreateElement("div");
            pnode.SetAttribute("style", "font:10pt tahoma");
            int j = treeNodes.Count;
            for (int i = 0; i < j; ++i)
            {
                pnode.AddChild(treeNodes[i].GetPrimaryPresentationNode(pnode));
            }
            return pnode;
        }
        public void AddItem(TreeNode treeNode)
        {
            treeNodes.Add(treeNode);
            if (pnode != null)
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
        Color backColor;
        bool isOpen = true;//test, open by default
        int newChildNodeY = NODE_DEFAULT_HEIGHT;
        int indentWidth = 17;
        int desiredHeight = 0; //after layout
        List<TreeNode> childNodes;
        TreeNode parentNode;
        TreeView ownerTreeView;
        //-------------------------- 

        DomElement pnode;
        DomElement nodeBar;
        DomElement nodeIcon;
        DomElement nodeSpan;
        DomElement nodeBody;
        DomElement nodeContent;
        string nodeString;
        int width;
        int height;
        public TreeNode(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public string NodeText
        {
            get { return this.nodeString; }
            set
            {
                this.nodeString = value;
            }
        }

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
            if (this.pnode != null) return pnode;
            //---------------------------------
            var ownerdoc = hostNode.OwnerDocument;
            pnode = ownerdoc.CreateElement("div");
            //---------------------------------
            //bar part
            pnode.AddChild("div", node_bar =>
            {
                this.nodeBar = node_bar;
                node_bar.AddChild("img", node_icon =>
                {
                    this.nodeIcon = node_icon;
                    SetupNodeIconBehaviour(node_icon);
                });
                node_bar.AddChild("span", node_span =>
                {
                    this.nodeSpan = node_span;
                    if (this.nodeString != null)
                    {
                        node_span.AddTextContent(this.nodeString);
                    }
                });
            });
            //---------------------------------
            //content part
            //indent  
            pnode.AddChild("div", node_body =>
            {
                this.nodeBody = node_body;
                node_body.SetAttribute("style", "padding-left:17px");
                node_body.AddChild("div", node_content =>
                {
                    this.nodeContent = node_content;
                    if (childNodes != null)
                    {
                        nodeContent.SetAttribute("style", "padding-left:0px");
                        int j = childNodes.Count;
                        for (int i = 0; i < j; ++i)
                        {
                            var childnode = childNodes[i].GetPrimaryPresentationNode(nodeContent);
                            node_content.AddChild(childnode);
                        }
                    }
                });
            });
            return pnode;
        }
        DomElement GetPrimaryPresentationNode2(DomElement hostNode)
        {
            //implement wth table

            if (this.pnode != null) return pnode;
            //---------------------------------
            var ownerdoc = hostNode.OwnerDocument;
            pnode = ownerdoc.CreateElement("div");
            //---------------------------------
            //bar part
            pnode.AddChild("div", node_bar =>
            {
                this.nodeBar = node_bar;
                node_bar.AddChild("img", node_icon =>
                {
                    this.nodeIcon = node_icon;
                    SetupNodeIconBehaviour(node_icon);
                });
                node_bar.AddChild("span", node_span =>
                {
                    this.nodeSpan = node_span;
                    if (this.nodeString != null)
                    {
                        node_span.AddTextContent(this.nodeString);
                    }
                });
            });
            //---------------------------------
            //content part
            //indent  
            pnode.AddChild("div", node_body =>
            {
                this.nodeBody = node_body;
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
                            this.nodeContent = td1;
                            if (childNodes != null)
                            {
                                int j = childNodes.Count;
                                for (int i = 0; i < j; ++i)
                                {
                                    var childnode = childNodes[i].GetPrimaryPresentationNode(nodeContent);
                                    nodeContent.AddChild(childnode);
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
            return pnode;
        }

        public bool IsOpen
        {
            get { return this.isOpen; }
        }
        public int ChildCount
        {
            get
            {
                if (childNodes == null) return 0;
                return childNodes.Count;
            }
        }
        public TreeNode ParentNode
        {
            get { return this.parentNode; }
        }
        public TreeView TreeView
        {
            get
            {
                if (this.ownerTreeView != null)
                {
                    //top node
                    return this.ownerTreeView;
                }
                else
                {
                    if (this.parentNode != null)
                    {
                        //recursive
                        return this.parentNode.TreeView;
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
            this.ownerTreeView = ownerTreeView;
        }
        public void AddChildNode(TreeNode treeNode)
        {
            if (childNodes == null)
            {
                childNodes = new List<TreeNode>();
            }
            this.childNodes.Add(treeNode);
            treeNode.parentNode = this;
            //---------------------------
            //add treenode presentaion
            if (this.isOpen)
            {
                if (this.nodeContent != null)
                {
                    //add child presentation 
                    nodeContent.AddChild(
                        treeNode.GetPrimaryPresentationNode(nodeContent));
                }
            }
            //---------------------------
        }
        public void Expand()
        {
            if (this.isOpen) return;
            this.isOpen = true;
            if (this.nodeBody != null)
            {
                if (nodeBody.ParentNode == null)
                {
                    pnode.AddChild(nodeBody);
                }
            }

            //this.TreeView.PerformContentLayout();
        }
        public void Collapse()
        {
            if (!this.isOpen) return;
            this.isOpen = false;
            if (this.nodeBody != null)
            {
                var htmlPNode = this.pnode as IHtmlElement;
                if (htmlPNode != null)
                {
                    htmlPNode.removeChild(this.nodeBody);
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