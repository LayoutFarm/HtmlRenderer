//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;



using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{
    public delegate void UIDragEventHandler(object sender, UIDragEventArgs e);

    public class UIDragEventArgs : UIEventArgs
    {
        public UIMouseButtons Button;

        int lastestLogicalViewportMouseDownX, lastestLogicalViewportMouseDownY, currentLogicalX, currentLogicalY;
        int lastestXDiff, lastestYDiff;

        public UIMouseEventType EventType;
        public ArtVisualElement DragingElement;
        public UIMouseEventArgs MouseInfo;

        static Stack<UIDragEventArgs> dragEventArgsPool = new Stack<UIDragEventArgs>();

        private UIDragEventArgs()
        {
            EventType = UIMouseEventType.Drag;
        }
        public static UIDragEventArgs GetFreeDragEventArgs()
        {
            if (dragEventArgsPool.Count > 0)
            {
                return dragEventArgsPool.Pop();
            }
            else
            {
                return new UIDragEventArgs();
            }
        }
        public static void ReleaseEventArgs(UIDragEventArgs e)
        {
            e.Clear();
            dragEventArgsPool.Push(e);
        }
        public void SetEventInfo(Point loca, UIMouseButtons button, int lastestLogicalViewportMouseDownX,
            int lastestLogicalViewportMouseDownY,
            int currentLogicalX,
            int currentLogicalY,
            int lastestXDiff,
            int lastestYDiff)
        {
            Location = loca;
            Button = button;

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
        public bool IsDragOut
        {
            get
            {
                return SourceVisualElement.ContainPoint(this.X, this.Y);
            }
        }

       
        public void SwapCurrentDragElement(ArtVisualElement withThisElement)
        {
            VisualRootWindow winroot = this.WinRoot; if (winroot != null)
            {
                winroot.CurrentDraggingElement = withThisElement;
            }
        }

    }





}