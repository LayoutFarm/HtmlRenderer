//BSD 2014, WinterDev

namespace HtmlRenderer.Boxes
{


    partial class CssBox
    {


        internal static CssBox CreateAnonBlock(CssBox parent)
        {
            var newBox = new CssBox(parent, null, parent._myspec.GetAnonVersion());
            ChangeDisplayType(newBox, Css.CssDisplay.Block);  
            return newBox;
        }
        internal static CssBox CreateAnonBlock(CssBox parent,CssBox insertBefore)
        {
            var newBox = new CssBox(parent, null, parent._myspec.GetAnonVersion());
            ChangeDisplayType(newBox, Css.CssDisplay.Block);

            parent.Boxes.Remove(newBox);
            parent.Boxes.InsertBefore(insertBefore, newBox);
            //-------------------------------------------
#if DEBUG
            //if (parent.HtmlElement != null)
            //{
            //    //newBox.dbugAnonCreator = (BridgeHtmlElement)parent.HtmlElement;
            //}
            //else if (parent.dbugAnonCreator != null)
            //{
            //    //newBox.dbugAnonCreator = parent.dbugAnonCreator;
            //}
#endif

            return newBox;
        }
        internal static CssBox CreateAnonInline(CssBox parent)
        {
            var newBox = new CssBox(parent, null, parent._myspec.GetAnonVersion());
            CssBox.ChangeDisplayType(newBox, Css.CssDisplay.Inline);
            return newBox;
        }

    }
}