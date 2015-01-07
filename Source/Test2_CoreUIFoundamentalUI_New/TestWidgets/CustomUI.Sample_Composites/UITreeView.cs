//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public class UITreeView : UIBox
    {
        //composite          
        CustomRenderBox primElement;//background
        Color backColor = Color.LightGray;
        int viewportX, viewportY;
        List<LayerElement> layers = new List<LayerElement>(1); 
        int latestItemY;

        UIPanel panel; //panel 
        public UITreeView(int width, int height)
            : base(width, height)
        {
            PlainLayerElement plainLayer = new PlainLayerElement();
            //panel for listview items
            this.panel = new UIPanel(width, height);
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
                RenderElement.DirectSetVisualElementLocation(renderE, this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.SetController(this);
                renderE.HasSpecificSize = true;
                //------------------------------------------------
                //create visual layer
                renderE.Layers = new VisualLayerCollection();
                int layerCount = this.layers.Count;
                for (int m = 0; m < layerCount; ++m)
                {
                    PlainLayerElement plain = (PlainLayerElement)this.layers[m];
                    var groundLayer = new VisualPlainLayer(renderE);
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
        public void AddItem(UITreeNode treeNode)
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

    public class UITreeNode : UIBox
    {
        const int NODE_DEFAULT_HEIGHT = 17;
        CustomRenderBox primElement;//bg primary render element
        Color backColor;
        bool isOpen = true;//test, open by default
        int newChildNodeY = NODE_DEFAULT_HEIGHT;
        int indentWidth = 17;
        int desiredHeight = 0; //after layout
        List<UITreeNode> childNodes;
        UITreeNode parentNode;
        UITreeView ownerTreeView;
        //-------------------------- 
        Image nodeIcon;
        UIImageBox uiNodeIcon;
        //--------------------------
        public UITreeNode(int width, int height)
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
                VisualPlainLayer plainLayer = null;
                if (element.Layers == null)
                {
                    element.Layers = new VisualLayerCollection();
                    plainLayer = new VisualPlainLayer(element);
                    element.Layers.AddLayer(plainLayer);
                }
                else
                {
                    plainLayer = (VisualPlainLayer)element.Layers.GetLayer(0);
                }
                //-----------------------------
                uiNodeIcon = new UIImageBox(16, 16);//create with default size 
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
        public UITreeNode ParentNode
        {
            get { return this.parentNode; }
        }
        public UITreeView TreeView
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

        internal void SetOwnerTreeView(UITreeView ownerTreeView)
        {
            this.ownerTreeView = ownerTreeView;
        }
        public void AddChildNode(UITreeNode treeNode)
        {

            if (childNodes == null)
            {
                childNodes = new List<UITreeNode>();
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
                    VisualPlainLayer plainLayer = null;
                    if (primElement.Layers == null)
                    {
                        primElement.Layers = new VisualLayerCollection();
                        plainLayer = new VisualPlainLayer(primElement);
                        primElement.Layers.AddLayer(plainLayer);
                    }
                    else
                    {
                        plainLayer = (VisualPlainLayer)primElement.Layers.GetLayer(0);
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
                        childNode.SetBound(indentWidth,
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
        void SetupNodeIconBehaviour(UIImageBox uiNodeIcon)
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