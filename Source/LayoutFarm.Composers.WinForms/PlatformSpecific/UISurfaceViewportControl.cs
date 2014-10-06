//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms; 
using LayoutFarm.Drawing;

namespace LayoutFarm
{

    public partial class UISurfaceViewportControl : UserControl
    {

        CanvasViewport canvasViewport;
        WinSurfaceViewportControl winBridge;
        MyTopWindowRenderBox wintop;


        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(int width, int height)
        {
            var myRootGraphic = new MyRootGraphic(width, height);
            this.wintop = new MyTopWindowRenderBox(myRootGraphic, width, height);

            this.winBridge = new WinSurfaceViewportControl();
            this.winBridge.BindWindowControl(this);

            this.canvasViewport = new CanvasViewport(winBridge, wintop, this.Size.ToSize(), 4);

            this.winBridge.BindCanvasViewPort(this.canvasViewport);
            wintop.CanvasForcePaint += canvasViewport.PaintMe;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (canvasViewport != null)
            {
                canvasViewport.UpdateCanvasViewportSize(this.Width, this.Height);
            }
            base.OnSizeChanged(e);
        }

        public void TopDownRecalculateContent()
        {
            wintop.TopDownReCalculateContentSize();
        }

        public void AddContent(RenderElement vi)
        {
            var layer0 = wintop.Layers.Layer0 as VisualPlainLayer;
            if (layer0 != null)
            {
                layer0.AddChild(vi);
                vi.InvalidateGraphic();
            }
        }
        public void AddContent(LayoutFarm.UI.UIElement ui)
        {
            AddContent(ui.GetPrimaryRenderElement(wintop.Root));
        }
        public void Close()
        {
            canvasViewport.Close();
        }
        public void PaintMe()
        {
            canvasViewport.PaintMe(this, EventArgs.Empty);
        }
        
        //-----------------------------------------------------------------------------
        protected override void OnGotFocus(EventArgs e)
        {
            this.winBridge.OnGotFocus(e);
            base.OnGotFocus(e);

        }
        protected override void OnLostFocus(EventArgs e)
        {
            this.winBridge.OnGotFocus(e);
            base.OnLostFocus(e);
        }
        protected override void OnDoubleClick(EventArgs e)
        {
            this.winBridge.OnDoubleClick(e);
            base.OnDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.winBridge.OnMouseDown(e);
            base.OnMouseDown(e);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.winBridge.OnMouseMove(e);
            base.OnMouseMove(e);

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.winBridge.OnMouseUp(e);
            base.OnMouseUp(e);

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.winBridge.OnPaint(e);
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.winBridge.OnMouseWheel(e);
            base.OnMouseWheel(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.winBridge.OnKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            this.winBridge.OnKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.winBridge.OnKeyPress(e);
            return;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.winBridge.ProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

#if DEBUG
        public event EventHandler dbug_VisualRootDrawMsg;
        public event EventHandler dbug_VisualRootHitChainMsg;
        List<dbugLayoutMsg> dbugrootDocDebugMsgs = new List<dbugLayoutMsg>();
        List<dbugLayoutMsg> dbugrootDocHitChainMsgs = new List<dbugLayoutMsg>();
        public List<dbugLayoutMsg> dbug_rootDocDebugMsgs
        {
            get
            {
                return this.dbugrootDocDebugMsgs;
            }
        }
        public List<dbugLayoutMsg> dbug_rootDocHitChainMsgs
        {
            get
            {
                return this.dbugrootDocHitChainMsgs;
            }
        }
        public void dbug_HighlightMeNow(Rectangle rect)
        {

            using (System.Drawing.Pen mpen = new System.Drawing.Pen(System.Drawing.Brushes.White, 2))
            using (System.Drawing.Graphics g = this.CreateGraphics())
            {

                System.Drawing.Rectangle r = rect.ToRect();
                g.DrawRectangle(mpen, r);
                g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Y), new System.Drawing.Point(r.Right, r.Bottom));
                g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Bottom), new System.Drawing.Point(r.Right, r.Y));
            }

        }
        public void dbug_InvokeVisualRootDrawMsg()
        {
            if (dbug_VisualRootDrawMsg != null)
            {
                dbug_VisualRootDrawMsg(this, EventArgs.Empty);
            }
        }
        public void dbug_InvokeHitChainMsg()
        {
            if (dbug_VisualRootHitChainMsg != null)
            {
                dbug_VisualRootHitChainMsg(this, EventArgs.Empty);
            }
        }
        public void dbug_BeginLayoutTraceSession(string beginMsg)
        {
            this.wintop.dbugVisualRoot.dbug_BeginLayoutTraceSession(beginMsg);
        }
        public void dbug_DisableAllDebugInfo()
        {
            this.wintop.dbugVisualRoot.dbug_DisableAllDebugInfo();
        }
        public void dbug_EnableAllDebugInfo()
        {
            this.wintop.dbugVisualRoot.dbug_EnableAllDebugInfo();
        }
        public void dbug_ReArrangeWithBreakOnSelectedNode()
        {

            vinv_dbugBreakOnSelectedVisuallElement = true;
            this.wintop.TopDownReArrangeContentIfNeed();

        }
        protected bool vinv_dbugBreakOnSelectedVisuallElement
        {
            get;
            set;
        }
#endif

    }



}
