//BSD  2014 ,WinterCore

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HtmlRenderer.WebDom
{
 
    public abstract class CssCodeItemBase
    {
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId;
#endif
        public CssCodeItemBase()
        {

#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;
#endif
        }
    }

     
    public class CssCodeBlockItem : CssCodeItemBase
    {
        /// <summary>
        /// css selector expression
        /// </summary>
        public List<CssSelectorExpressionBase> selectorExpressions = new List<CssSelectorExpressionBase>();

       
        public List<CssCodeInstruction> cssCodeInstruction = new List<CssCodeInstruction>();

        public CssCodeBlockItem()
        {
                
        }
#if DEBUG
        public override string ToString()
        {

            StringBuilder stBuilder = new StringBuilder();
            int j = selectorExpressions.Count;
            for (int i = 0; i < j; ++i)
            {
                stBuilder.Append(selectorExpressions[i].ToString());
                if (i < j - 1)
                {
                    stBuilder.Append(',');
                }
            }
            j = cssCodeInstruction.Count;
            stBuilder.Append('{');
            for (int i = 0; i < j; ++i)
            {
                stBuilder.Append(cssCodeInstruction[i].ToString());
            }

            stBuilder.Append('}');
            return stBuilder.ToString();
        }
#endif

    }








}