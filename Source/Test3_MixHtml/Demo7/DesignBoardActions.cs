//Apache2, 2014-2018, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.DzBoardSample
{
    abstract class DzBoardAction
    {
        public DzBoardAction()
        {
        }

        public abstract void InvokeUndo(DesignBoardModule dzBoard);
        public abstract void InvokeRedo(DesignBoardModule dzBoard);
    }

    class DzSetLocationAction : DzBoardAction
    {
        AbstractRect dzBox;
        Point oldPoint;
        Point newPoint;
        public DzSetLocationAction(AbstractRect box, Point oldPoint, Point newPoint)
        {
            this.dzBox = box;
            this.oldPoint = oldPoint;
            this.newPoint = newPoint;
        }
        public override void InvokeUndo(DesignBoardModule dzBoard)
        {
            //move to old point
            this.dzBox.SetLocation(oldPoint.X, oldPoint.Y);
        }
        public override void InvokeRedo(DesignBoardModule dzBoard)
        {
            //move to new point
            this.dzBox.SetLocation(newPoint.X, newPoint.Y);
        }
    }
    class DzSetBoundsAction : DzBoardAction
    {
        AbstractRect dzBox;
        Rectangle oldBounds;
        Rectangle newBounds;
        public DzSetBoundsAction(AbstractRect box, Rectangle oldBounds, Rectangle newBounds)
        {
            this.dzBox = box;
            this.oldBounds = oldBounds;
            this.newBounds = newBounds;
        }
        public override void InvokeUndo(DesignBoardModule dzBoard)
        {
            //move to old point
            this.dzBox.SetLocationAndSize(oldBounds.Left, oldBounds.Top, oldBounds.Width, oldBounds.Height);
        }
        public override void InvokeRedo(DesignBoardModule dzBoard)
        {
            this.dzBox.SetLocationAndSize(newBounds.Left, newBounds.Top, newBounds.Width, newBounds.Height);
        }
    }
    class DzMoveElementSetAction : DzBoardAction
    {
        List<DzSetLocationAction> dzBoxList;
        public DzMoveElementSetAction(List<DzSetLocationAction> dzBoxList)
        {
            this.dzBoxList = dzBoxList;
        }
        public override void InvokeUndo(DesignBoardModule dzBoard)
        {
            int j = dzBoxList.Count;
            for (int i = 0; i < j; ++i)
            {
                dzBoxList[i].InvokeUndo(dzBoard);
            }
        }
        public override void InvokeRedo(DesignBoardModule dzBoard)
        {
            int j = dzBoxList.Count;
            for (int i = 0; i < j; ++i)
            {
                dzBoxList[i].InvokeRedo(dzBoard);
            }
        }
    }
    class DzCommandCollection
    {
        LinkedList<DzBoardAction> undoList = new LinkedList<DzBoardAction>();
        Stack<DzBoardAction> reverseUndoAction = new Stack<DzBoardAction>();
        int maxCommandsCount = 20;
        DesignBoardModule boardModule;
        public DzCommandCollection(DesignBoardModule module)
        {
            this.boardModule = module;
        }

        public void Clear()
        {
            undoList.Clear();
            reverseUndoAction.Clear();
        }

        public int MaxCommandCount
        {
            get
            {
                return maxCommandsCount;
            }
            set
            {
                maxCommandsCount = value;
                if (undoList.Count > maxCommandsCount)
                {
                    int diff = undoList.Count - maxCommandsCount;
                    for (int i = 0; i < diff; i++)
                    {
                        undoList.RemoveFirst();
                    }
                }
            }
        }

        public void AddAction(DzBoardAction docAct)
        {
            if (boardModule.EnableUndoHistoryRecording)
            {
                undoList.AddLast(docAct);
                EnsureCapacity();
            }
        }

        void EnsureCapacity()
        {
            if (undoList.Count > maxCommandsCount)
            {
                undoList.RemoveFirst();
            }
        }
        public void UndoLastAction()
        {
            DzBoardAction docAction = PopUndoCommand();
            if (docAction != null)
            {
                boardModule.EnableUndoHistoryRecording = false;
                docAction.InvokeUndo(boardModule);
                boardModule.EnableUndoHistoryRecording = true;
                reverseUndoAction.Push(docAction);
            }
        }
        public void ReverseLastUndoAction()
        {
            if (reverseUndoAction.Count > 0)
            {
                boardModule.EnableUndoHistoryRecording = false;
                DzBoardAction docAction = reverseUndoAction.Pop();
                boardModule.EnableUndoHistoryRecording = true;
                docAction.InvokeRedo(boardModule);
                undoList.AddLast(docAction);
            }
        }
        public DzBoardAction PeekCommand
        {
            get
            {
                return undoList.Last.Value;
            }
        }
        public int Count
        {
            get
            {
                return undoList.Count;
            }
        }
        public DzBoardAction PopUndoCommand()
        {
            if (undoList.Count > 0)
            {
                DzBoardAction lastCmd = undoList.Last.Value;
                undoList.RemoveLast();
                return lastCmd;
            }
            else
            {
                return null;
            }
        }
    }
}