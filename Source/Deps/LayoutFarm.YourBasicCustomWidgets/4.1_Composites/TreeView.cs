//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class TreeView : UIBox
    {
        //composite          
        CustomRenderBox primElement;//background
        Color backColor = Color.LightGray;
        int viewportX, viewportY;
        UICollection uiList;
        int latestItemY;
        SimpleBox panel; //panel 
        public TreeView(int width, int height)
            : base(width, height)
        {
            //panel for listview items
            this.panel = new SimpleBox(width, height);
            panel.ContentLayoutKind = BoxContentLayoutKind.VerticalStack;
            panel.BackColor = Color.LightGray;
            uiList = new UICollection(this);
            uiList.AddUI(panel);
        }

        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.SetController(this);
                renderE.HasSpecificSize = true;
                //------------------------------------------------
                //create visual layer 
                int n = this.uiList.Count;
                for (int m = 0; m < n; ++m)
                {
                    renderE.AddChild(uiList.GetElement(m));
                }

                //---------------------------------
                primElement = renderE;
            }
            return primElement;
        }
        public void AddItem(TreeNode treeNode)
        {
            treeNode.SetLocation(0, latestItemY);
            latestItemY += treeNode.Height;
            treeNode.SetOwnerTreeView(this);
            panel.AddChild(treeNode);
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        }

        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }


        public override int ViewportX
        {
            get { return this.viewportX; }
        }
        public override int ViewportY
        {
            get { return this.viewportY; }
        }
        public override void SetViewport(int x, int y)
        {
            this.viewportX = x;
            this.viewportY = y;
            if (this.HasReadyRenderElement)
            {
                this.panel.SetViewport(x, y);
            }
        }
        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        //----------------------------------------------------  
        public override void PerformContentLayout()
        {
            //manually perform layout of its content 
            //here: arrange item in panel
            this.panel.PerformContentLayout();
        }
        //----------------------------------------------------   
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "treeview");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }

    public class TreeNode : UIBox
    {
        const int NODE_DEFAULT_HEIGHT = 17;
        CustomRenderBox primElement;//bg primary render element
        CustomTextRun myTextRun;
        Color backColor;
        bool isOpen = true;//test, open by default
        int newChildNodeY = NODE_DEFAULT_HEIGHT;
        int indentWidth = 17;
        int desiredHeight = 0; //after layout
        List<TreeNode> childNodes;
        TreeNode parentNode;
        TreeView ownerTreeView;
        //-------------------------- 
        ImageBinder nodeIcon;
        ImageBox uiNodeIcon;
        //--------------------------
        public TreeNode(int width, int height)
            : base(width, height)
        {
        }
        public ImageBinder NodeIconImage
        {
            get { return this.nodeIcon; }
            set
            {
                this.nodeIcon = value;
                if (uiNodeIcon != null)
                {
                    uiNodeIcon.ImageBinder = value;
                }
            }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return primElement != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                //first time
                var element = new CustomRenderBox(rootgfx, this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = this.backColor;
                element.HasSpecificSize = true;
                //-----------------------------
                // create default layer for node content


                //-----------------------------
                uiNodeIcon = new ImageBox(16, 16);//create with default size 
                SetupNodeIconBehaviour(uiNodeIcon);
                element.AddChild(uiNodeIcon);
                //-----------------------------
                myTextRun = new CustomTextRun(rootgfx, 10, 17);
                myTextRun.SetLocation(16, 0);
                myTextRun.Text = "Test01";
                element.AddChild(myTextRun);
                //-----------------------------
                this.primElement = element;
            }
            return primElement;
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }
        //------------------------------------------------
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
                if (this.primElement != null)
                {
                    //add child presentation 
                    //below here
                    //create layers                    

                    //add to layer

                    var tnRenderElement = treeNode.GetPrimaryRenderElement(primElement.Root);
                    tnRenderElement.SetLocation(indentWidth, newChildNodeY);
                    primElement.AddChild(tnRenderElement);
                    newChildNodeY += tnRenderElement.Height;
                    //-----------------
                }
            }
            //---------------------------
        }
        public void Expand()
        {
            if (this.isOpen) return;
            this.isOpen = true;
            this.TreeView.PerformContentLayout();
        }
        public void Collapse()
        {
            if (!this.isOpen) return;
            this.isOpen = false;
            this.TreeView.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.InvalidateGraphics();
            //if this has child
            //reset
            this.desiredHeight = NODE_DEFAULT_HEIGHT;
            this.newChildNodeY = NODE_DEFAULT_HEIGHT;
            if (this.isOpen)
            {
                if (childNodes != null)
                {
                    int j = childNodes.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        var childNode = childNodes[i];
                        childNode.PerformContentLayout();//manaul?
                        //set new size 
                        childNode.SetBounds(indentWidth,
                            newChildNodeY,
                            childNode.Width,
                            childNode.DesiredHeight);
                        newChildNodeY += childNode.DesiredHeight;
                    }
                }
            }
            this.desiredHeight = newChildNodeY;
        }
        public override int DesiredHeight
        {
            get
            {
                return this.desiredHeight;
            }
        }
        //------------------------------------------------
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
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "treenode");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}