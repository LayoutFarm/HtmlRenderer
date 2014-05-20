//BSD  2014 ,WinterCore

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO; 

namespace HtmlRenderer.WebDom
{
    public class CssAtRule : CssCodeItemBase
    {
        public string atRuleName;
        public string mediaType;
        public CssDocument innerDoc;

        public string AtRuleName
        {
            get
            {
                return this.atRuleName;
            }
        }
        public string MediaType
        {
            get
            {
                return this.mediaType;
            }
        }
        public CssDocument InnerDoc
        {
            get
            {
                return this.innerDoc;
            }
        }
 
        public override string ToString()
        {
            return "@"+atRuleName;
        }
 
    }
}