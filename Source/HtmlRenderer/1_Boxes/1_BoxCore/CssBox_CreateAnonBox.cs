//BSD 2014, WinterDev

namespace HtmlRenderer.Dom
{


    partial class CssBox
    {


        internal static CssBox CreateAnonBlock(CssBox parent, int insertAt = -1)
        {
            var newBox = new CssBox(parent, null, parent._myspec.GetAnonVersion());  
            ChangeDisplayType(newBox, CssDisplay.Block);             

            if (insertAt > -1)
            {
                newBox.ChangeSiblingOrder(insertAt);
            }
            //-------------------------------------------
#if DEBUG
            if (parent.HtmlElement != null)
            {
                newBox.dbugAnonCreator = (BridgeHtmlElement)parent.HtmlElement;
            }
            else if (parent.dbugAnonCreator != null)
            {
                newBox.dbugAnonCreator = parent.dbugAnonCreator;
            }
#endif

            return newBox;
        }
        internal static CssBox CreateAnonInline(CssBox parent)
        {
            var newBox = new CssBox(parent, null, parent._myspec.GetAnonVersion()); 
            CssBox.ChangeDisplayType(newBox, CssDisplay.Inline);
            return newBox;
        }

    }
}