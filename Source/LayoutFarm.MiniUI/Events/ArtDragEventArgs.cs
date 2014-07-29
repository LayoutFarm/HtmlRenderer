using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;



using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{
                        public delegate void ArtDragEventHandler(object sender, ArtDragEventArgs e);

                public class ArtDragEventArgs : ArtEventArgs
    {
        public ArtMouseButtons Button;

        int lastestLogicalViewportMouseDownX, lastestLogicalViewportMouseDownY, currentLogicalX, currentLogicalY;
        int lastestXDiff, lastestYDiff;

        public ArtVisualMouseEventType EventType;
        public ArtVisualElement DragingElement;
        public ArtMouseEventArgs MouseInfo;

        static Stack<ArtDragEventArgs> dragEventArgsPool = new Stack<ArtDragEventArgs>();

        private ArtDragEventArgs()
        {
            EventType = ArtVisualMouseEventType.Drag;
        }
        public static ArtDragEventArgs GetFreeDragEventArgs()
        {
            if (dragEventArgsPool.Count > 0)
            {
                return dragEventArgsPool.Pop();
            }
            else
            {
                return new ArtDragEventArgs();
            }
        }
        public static void ReleaseEventArgs(ArtDragEventArgs e)
        {
            e.Clear();
            dragEventArgsPool.Push(e);
        }
        public void SetEventInfo(Point loca, ArtMouseButtons button, int lastestLogicalViewportMouseDownX,
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
                        Button = ArtMouseButtons.Left;
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

                                public object DraggingUI
        {
            get
            {
                if (DragingElement.HasScriptElement)
                {
                    return DragingElement.GetScriptUI();
                }
                else
                {
                    return null;
                }
            }
        }
                                        public void SwapCurrentDragElement(ArtVisualElement withThisElement)
        {
                                                ArtVisualRootWindow winroot = this.WinRoot;            if (winroot != null)
            {
                                winroot.CurrentDraggingElement = withThisElement;
            }
                    }

    }





}