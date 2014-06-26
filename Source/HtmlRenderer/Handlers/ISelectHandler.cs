//BSD 2014,WinterCore

using System;
using HtmlRenderer.Dom;

namespace HtmlRenderer
{
    

    public interface ISelectionHandler
    {
        int GetSelectingStartIndex(CssRun r);
        int GetSelectedEndIndexOffset(CssRun r);
        //----------
        float GetSelectedStartOffset(CssRun r);
        float GetSelectedEndOffset(CssRun r);
        //----------
        string GetSelectedText();
        string GetSelectedHtml();
        void Dispose(); 
    }
}