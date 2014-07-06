//BSD 2014, WinterDev 
using System;

namespace HtmlRenderer.Dom
{
#if DEBUG
    partial class BoxModelBuilder
    {
        static int dbugS01 = 0;
        static void dbugCompareSpecDiff(string prefix, CssBox box)
        {
            BridgeHtmlElement element = box.HtmlElement as BridgeHtmlElement;
            BoxSpec curSpec = CssBox.UnsafeGetBoxSpec(box);

            dbugPropCheckReport rep = new dbugPropCheckReport();
            if (element != null)
            {
                if (!BoxSpec.dbugCompare(
                    rep,
                    element.Spec,
                    curSpec))
                {
                    foreach (string s in rep.GetList())
                    {
                        Console.WriteLine(prefix + s);
                    }

                }
            }
            else
            {
                if (box.dbugAnonCreator != null)
                {
                    if (!BoxSpec.dbugCompare(
                    rep,
                    box.dbugAnonCreator.Spec.GetAnonVersion(),
                    curSpec))
                    {
                        foreach (string s in rep.GetList())
                        {
                            Console.WriteLine(prefix + s);
                        }
                    }
                }
                else
                {

                }
            }

        }


        /// <summary>
        /// Go over all the text boxes (boxes that have some text that will be rendered) and
        /// remove all boxes that have only white-spaces but are not 'preformatted' so they do not effect
        /// the rendered html.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        static void dbugCorrectTextBoxes(CssBox box)
        {
            return;

            CssBoxCollection boxes = CssBox.UnsafeGetChildren(box);
            for (int i = boxes.Count - 1; i >= 0; i--)
            {
                var childBox = boxes[i];
                if (childBox.MayHasSomeTextContent)
                {
                    // is the box has text
                    // or is the box is pre-formatted
                    // or is the box is only one in the parent 
                    bool keepBox = childBox.HtmlElement != null;
                    if (!keepBox)
                    {
                        keepBox = !childBox.TextContentIsWhitespaceOrEmptyText ||
                         childBox.WhiteSpace == CssWhiteSpace.Pre ||
                         childBox.WhiteSpace == CssWhiteSpace.PreWrap ||
                         boxes.Count == 1;
                    }

                    if (!keepBox && box.ChildCount > 0)
                    {
                        if (i == 0)
                        {
                            //first
                            // is first/last box where is in inline box and it's next/previous box is inline
                            keepBox = box.IsInline && boxes[1].IsInline;
                        }
                        else if (i == box.ChildCount - 1)
                        {
                            //last
                            // is first/last box where is in inline box and it's next/previous box is inline
                            keepBox = box.IsInline && boxes[i - 1].IsInline;
                        }
                        else
                        {
                            //between
                            // is it a whitespace between two inline boxes
                            keepBox = boxes[i - 1].IsInline && boxes[i + 1].IsInline;
                        }
                    }

                    if (keepBox)
                    {
                        // valid text box, parse it to words                            
                        childBox.UpdateRunList();
                    }
                    else
                    {

                        boxes.RemoveAt(i);
                    }
                }
                else
                {
                    // recursive 
                    dbugCorrectTextBoxes(childBox);
                }
            }
        }

    }


#endif
}