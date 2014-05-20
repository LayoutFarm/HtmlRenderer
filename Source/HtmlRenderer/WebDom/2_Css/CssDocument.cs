//BSD  2014 ,WinterCore

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
 
namespace HtmlRenderer.WebDom
{

     
    public class CssDocument
    {   
        List<CssCodeItemBase> cssItemCollection = new List<CssCodeItemBase>();         
        public CssDocument()
        { 

        }
        public void Add(CssCodeItemBase cssitem)
        {
            cssItemCollection.Add(cssitem);
        }
        public List<CssCodeItemBase> AllCssItems
        {
            get
            {
                return this.cssItemCollection;
            }
        }
        public IEnumerable<CssAtRule> GetCssAtRuleIter()
        {
            int j = cssItemCollection.Count;
            for (int i = 0; i < j; ++i)
            {
                CssCodeItemBase itm = cssItemCollection[i];
                if (itm is CssAtRule)
                {
                    yield return (CssAtRule)itm;
                }
            }
        }
        public IEnumerable<CssCodeBlockItem> GetCssBlockIter()
        {
            int j = cssItemCollection.Count;
            for (int i = 0; i < j; ++i)
            {
                CssCodeItemBase itm = cssItemCollection[i];
                if (itm is CssCodeBlockItem)
                {
                    yield return (CssCodeBlockItem)itm;
                }
            }
        }
        public CssCodeBlockItem GetFirstCssBlock()
        {
            foreach (CssCodeItemBase cssItem in this.cssItemCollection)
            {
                if (cssItem is CssCodeBlockItem)
                {
                    return (CssCodeBlockItem)cssItem;
                }
            }
            return null;
        }
#if DEBUG
        public override string ToString()
        {
            StringBuilder stBuilder = new StringBuilder();
            int j = cssItemCollection.Count;
            for (int i = 0; i < j; ++i)
            {
                stBuilder.Append(cssItemCollection[i].ToString());
            }                

            return stBuilder.ToString();
        }
#endif

    }
    
}