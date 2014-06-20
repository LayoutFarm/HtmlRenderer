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
        List<CssDocMember> cssItemCollection = new List<CssDocMember>();
        public CssDocument()
        {

        }
        public void Add(CssDocMember cssitem)
        {
            cssItemCollection.Add(cssitem);
        }
        public IEnumerable<CssDocMember> GetCssDocMemberIter()
        {
            int j = cssItemCollection.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return cssItemCollection[i];                 
            }
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