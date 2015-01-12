// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{
    public class TreeView : UIBox
    {
        //composite          
        CustomRenderBox primElement;//background
        Color backColor = Color.LightGray;
        int viewportX, viewportY;
        List<UICollection> layers = new List<UICollection>(1);
        int latestItemY;

        Panel panel; //panel 
        public TreeView(int width, int height)
            : base(width, height)
        {
            UICollection plainLayer = new UICollection(this);
            //panel for listview items
            this.panel = new Panel(width, height);
            panel.BackColor = Color.LightGray;
            plainLayer.AddUI(panel);
            this.layers.Add(plainLayer);
        }

        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
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
                renderE.Layers = new VisualLayerCollection();
                int layerCount = this.layers.Count;
                for (int m = 0; m < layerCount; ++m)
                {
                    UICollection plain = (UICollection)this.layers[m];
                    var groundLayer = new PlainLayer(renderE);
                    renderE.Layers.AddLayer(groundLayer);
                    renderE.SetViewport(this.viewportX, this.viewportY);
                    //---------------------------------
                    int j = plain.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        groundLayer.AddUI(plain.GetElement(i));
                    }
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
            panel.AddChildBox(treeNode);
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        }
        protected override void OnDragBegin(UIMouseEventArgs e)
        {
            if (this.DragStart != null)
            {
                this.DragStart(this, e);
            }
            base.OnDragBegin(e);
        }
        protected override void OnDragEnd(UIMouseEventArgs e)
        {
            if (this.DragStop != null)
            {
                this.DragStop(this, e);
            }
            base.OnDragEnd(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }
        protected override void OnDragging(UIMouseEventArgs e)
        {
            if (this.Dragging != null)
            {
                Dragging(this, e);
            }
            base.OnDragging(e);
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

        public event EventHandler<UIMouseEventArgs> Dragging;
        public event EventHandler<UIMouseEventArgs> DragStart;
        public event EventHandler<UIMouseEventArgs> DragStop;
        //----------------------------------------------------  
        public override void PerformContentLayout()
        {
            //manually perform layout of its content 
            //here: arrange item in panel
            this.panel.PerformContentLayout();
        }
        //----------------------------------------------------   
    }

    public class TreeNode : UIBox
    {
        const int NODE_DEFAULT_HEIGHT = 17;
        CustomRenderBox primElement;//bg primary render element
        Color backColor;
        bool isOpen = true;//test, open by default
        int newChildNodeY = NODE_DEFAULT_HEIGHT;
        int indentWidth = 17;
        int desiredHeight = 0; //after layout
        List<TreeNode> childNodes;
        TreeNode parentNode;
        TreeView ownerTreeView;
        //-------------------------- 
        Image nodeIcon;
        ImageBox uiNodeIcon;
        //--------------------------
        public TreeNode(int width, int height)
            : base(width, height)
        {

        }
        public Image NodeIconImage
        {
            get { return this.nodeIcon; }
            set
            {
                this.nodeIcon = value;
                if (uiNodeIcon != null)
                {
                    uiNodeIcon.Image = value;
                }
            }
        }
        protected override RenderElement CurrentPrimaryRenderElement
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
                PlainLayer plainLayer = null;
                if (element.Layers == null)
                {
                    element.Layers = new VisualLayerCollection();
                    plainLayer = new PlainLayer(element);
                    element.Layers.AddLayer(plainLayer);
                }
                else
                {
                    plainLayer = (PlainLayer)element.Layers.GetLayer(0);
                }
                //-----------------------------
                uiNodeIcon = new ImageBox(16, 16);//create with default size 
                SetupNodeIconBehaviour(uiNodeIcon);
                plainLayer.AddChild(uiNodeIcon.GetPrimaryRenderElement(rootgfx));
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
                    PlainLayer plainLayer = null;
                    if (primElement.Layers == null)
                    {
                        primElement.Layers = new VisualLayerCollection();
                        plainLayer = new PlainLayer(primElement);
                        primElement.Layers.AddLayer(plainLayer);
                    }
                    else
                    {
                        plainLayer = (PlainLayer)primElement.Layers.GetLayer(0);
                    }
                    //-----------------
                    //add to layer
                    var tnRenderElement = treeNode.GetPrimaryRenderElement(primElement.Root);
                    tnRenderElement.SetLocation(indentWidth, newChildNodeY);
                    plainLayer.AddChild(tnRenderElement);
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
                        childNode.PerformContentLayout();
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

    }





}