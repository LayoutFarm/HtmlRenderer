//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace LayoutFarm.Text
{

    partial class EditableTextFlowLayer
    {
        public override IEnumerable<RenderElement> GetVisualElementReverseIter()
        {
            if (lineCollection != null)
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    List<EditableVisualElementLine> lines = (List<EditableVisualElementLine>)lineCollection;
                    int j = lines.Count;
                    for (int i = lines.Count; i > -1; --i)
                    {
                        EditableVisualElementLine ln = lines[i];
                        LinkedListNode<EditableTextSpan> veNode = ln.Last;
                        while (veNode != null)
                        {
                            yield return veNode.Value;
                            veNode = veNode.Previous;
                        }
                    }

                }
                else
                {

                    EditableVisualElementLine ln = (EditableVisualElementLine)lineCollection;
                    LinkedListNode<EditableTextSpan> veNode = ln.Last;
                    while (veNode != null)
                    {
                        yield return veNode.Value;
                        veNode = veNode.Previous;
                    }
                }
            }
        }
        public override IEnumerable<RenderElement> GetVisualElementIter()
        {
            if (lineCollection != null)
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    List<EditableVisualElementLine> lines = (List<EditableVisualElementLine>)lineCollection;
                    int j = lines.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        EditableVisualElementLine ln = lines[i];
                        LinkedListNode<EditableTextSpan> veNode = ln.First;

                        while (veNode != null)
                        {
                            yield return veNode.Value;
                            veNode = veNode.Next;
                        }
                    }

                }
                else
                {
                    EditableVisualElementLine ln = (EditableVisualElementLine)lineCollection;
                    LinkedListNode<EditableTextSpan> veNode = ln.First;

                    while (veNode != null)
                    {
                        yield return veNode.Value;
                        veNode = veNode.Next;
                    }
                }
            }
        }
        public void AddTop(EditableTextSpan visualElement)
        {
#if DEBUG
            if (visualElement.ParentLink != null)
            {

            }
#endif

            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableVisualElementLine> lines = (List<EditableVisualElementLine>)lineCollection;
                lines[lines.Count - 1].AddLast(visualElement);
            }
            else
            {
                ((EditableVisualElementLine)lineCollection).AddLast(visualElement);
            }



        }
        public void AddBefore(EditableTextSpan beforeVisualElement, EditableTextSpan visualElement)
        {
            EditableVisualElementLine targetLine = beforeVisualElement.OwnerEditableLine;
            if (targetLine != null)
            {
                targetLine.AddBefore(beforeVisualElement, visualElement);
            }
            else
            {
                throw new NotSupportedException();
            }


        }

        public void AddAfter(EditableTextSpan afterVisualElement, EditableTextSpan visualElement)
        {


            EditableVisualElementLine targetLine = afterVisualElement.OwnerEditableLine;
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
                List<EditableVisualElementLine> lines = (List<EditableVisualElementLine>)lineCollection;
                for (int i = lines.Count - 1; i > -1; --i)
                {
                    lines[i].editableFlowLayer = null;
                    lines[i].Clear();
                }
                lines.Clear();

                lineCollection = new EditableVisualElementLine(this);
                FlowLayerHasMultiLines = false;
            }
            else
            {
                ((EditableVisualElementLine)lineCollection).Clear();
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
            List<EditableVisualElementLine> lines = (List<EditableVisualElementLine>)lineCollection;
            if (lines.Count < 2)
            {
                return;
            }

            EditableVisualElementLine tobeRemovedLine = lines[lineId];
            tobeRemovedLine.editableFlowLayer = null;
            int cy = tobeRemovedLine.Top;
            lines.RemoveAt(lineId); int j = lines.Count;
            for (int i = lineId; i < j; ++i)
            {
                EditableVisualElementLine line = lines[i];
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