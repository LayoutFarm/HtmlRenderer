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
    }
#endif
}