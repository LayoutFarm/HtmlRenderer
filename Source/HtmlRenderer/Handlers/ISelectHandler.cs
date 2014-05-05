using System;
using HtmlRenderer.Dom;

namespace HtmlRenderer
{
    

    public interface ISelectionHandler
    {
        int GetSelectingStartIndex(CssRect r);
        int GetSelectedEndIndexOffset(CssRect r);
        //----------
        float GetSelectedStartOffset(CssRect r);
        float GetSelectedEndOffset(CssRect r);
        //----------
        string GetSelectedText();
        string GetSelectedHtml();
        void Dispose(); 
    }
}