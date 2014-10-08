//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm;

namespace LayoutFarm.UI
{
    public delegate void UIDragEventHandler(object sender, UIDragEventArgs e);

    public class UIDragEventArgs : UIEventArgs
    {
        public UIMouseButtons Button;

        int lastestLogicalViewportMouseDownX, lastestLogicalViewportMouseDownY, currentLogicalX, currentLogicalY;
        int lastestXDiff, lastestYDiff;

        public UIMouseEventType EventType;
        public RenderElement DragingElement;
        public UIMouseEventArgs MouseInfo;

        
        public UIDragEventArgs()
        {
            EventType = UIMouseEventType.Drag;
        }
        
        public void SetEventInfo(Point loca, UIMouseButtons button, int lastestLogicalViewportMouseDownX,
            int lastestLogicalViewportMouseDownY,
            int currentLogicalX,
            int currentLogicalY,
            int lastestXDiff,
            int lastestYDiff)
        {
             
            Button = button;
            this.Location = loca;

            this.currentLogicalX = currentLogicalX;
            this.currentLogicalY = currentLogicalY;
            this.lastestLogicalViewportMouseDownY = lastestLogicalViewportMouseDownY;
            this.lastestLogicalViewportMouseDownX = lastestLogicalViewportMouseDownX;
            this.lastestXDiff = lastestXDiff;
            this.lastestYDiff = lastestYDiff;
        }
        public int XDiffFromMouseDownPos
        {
            get
            {
                return this.currentLogicalX - this.lastestLogicalViewportMouseDownX;
            }
        }
        public int YDiffFromMouseDownPos
        {
            get
            {
                return this.currentLogicalY - this.lastestLogicalViewportMouseDownY;
            }
        }
        public override void Clear()
        {
            Button = UIMouseButtons.Left;
            base.Clear();
        }
     
        public int XDiff
        {
            get
            {
                return lastestXDiff;
            }
        }
        public int YDiff
        {
            get
            {
                return lastestYDiff;
            }
        }
        //public bool IsDragOut
        //{
        //    get
        //    {
        //        return SourceRenderElement.ContainPoint(this.X, this.Y);
        //    }
        //}

       
       

    }





}