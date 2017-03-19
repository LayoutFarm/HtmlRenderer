//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.Text
{
    partial class EditableTextFlowLayer
    {
        public override IEnumerable<RenderElement> GetRenderElementReverseIter()
        {
            if (lineCollection != null)
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                    int j = lines.Count;
                    for (int i = lines.Count; i > -1; --i)
                    {
                        EditableTextLine ln = lines[i];
                        LinkedListNode<EditableRun> veNode = ln.Last;
                        while (veNode != null)
                        {
                            yield return veNode.Value;
                            veNode = veNode.Previous;
                        }
                    }
                }
                else
                {
                    EditableTextLine ln = (EditableTextLine)lineCollection;
                    LinkedListNode<EditableRun> veNode = ln.Last;
                    while (veNode != null)
                    {
                        yield return veNode.Value;
                        veNode = veNode.Previous;
                    }
                }
            }
        }
        public override IEnumerable<RenderElement> GetRenderElementIter()
        {
            if (lineCollection != null)
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                    int j = lines.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        EditableTextLine ln = lines[i];
                        LinkedListNode<EditableRun> veNode = ln.First;
                        while (veNode != null)
                        {
                            yield return veNode.Value;
                            veNode = veNode.Next;
                        }
                    }
                }
                else
                {
                    EditableTextLine ln = (EditableTextLine)lineCollection;
                    LinkedListNode<EditableRun> veNode = ln.First;
                    while (veNode != null)
                    {
                        yield return veNode.Value;
                        veNode = veNode.Next;
                    }
                }
            }
        }
        public void AddTop(EditableRun visualElement)
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                lines[lines.Count - 1].AddLast(visualElement);
            }
            else
            {
                ((EditableTextLine)lineCollection).AddLast(visualElement);
            }
        }
        public void AddBefore(EditableRun beforeVisualElement, EditableRun visualElement)
        {
            EditableTextLine targetLine = beforeVisualElement.OwnerEditableLine;
            if (targetLine != null)
            {
                targetLine.AddBefore(beforeVisualElement, visualElement);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void AddAfter(EditableRun afterVisualElement, EditableRun visualElement)
        {
            EditableTextLine targetLine = afterVisualElement.OwnerEditableLine;
            if (targetLine != null)
            {
                targetLine.AddAfter(afterVisualElement, visualElement);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override void Clear()
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                for (int i = lines.Count - 1; i > -1; --i)
                {
                    lines[i].editableFlowLayer = null;
                    lines[i].Clear();
                }
                lines.Clear();
                lineCollection = new EditableTextLine(this);
                FlowLayerHasMultiLines = false;
            }
            else
            {
                ((EditableTextLine)lineCollection).Clear();
            }
        }

        internal void Remove(int lineId)
        {
#if DEBUG
            if (lineId < 0)
            {
                throw new NotSupportedException();
            }
#endif
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) == 0)
            {
                return;
            }
            List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
            if (lines.Count < 2)
            {
                return;
            }

            EditableTextLine tobeRemovedLine = lines[lineId];
            tobeRemovedLine.editableFlowLayer = null;
            int cy = tobeRemovedLine.Top;
            lines.RemoveAt(lineId); int j = lines.Count;
            for (int i = lineId; i < j; ++i)
            {
                EditableTextLine line = lines[i];
                line.SetTop(cy); line.SetLineNumber(i); cy += line.ActualLineHeight;
            }

            if (lines.Count == 1)
            {
                lineCollection = lines[0];
                FlowLayerHasMultiLines = false;
            }
        }
    }
}