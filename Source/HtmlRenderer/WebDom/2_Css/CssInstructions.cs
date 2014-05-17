//BSD  2014 ,WinterCore


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace HtmlRenderer.WebDom
{

     
    public class CssCodeInstruction
    {

        bool markedAsInherite;
        /// <summary>
        /// property name , not resolved
        /// </summary>
        public string propertyName;
        
        List<CssInstructionValue> valueCollection = new List<CssInstructionValue>();

        public CssCodeInstruction(string propertyName)
        {
            this.propertyName = propertyName;
        }
        public void AddCssInstructionValue(CssInstructionValue value)
        {
            markedAsInherite = (valueCollection.Count == 0 && value.propertyValue == "inherit"); 
             
            this.valueCollection.Add(value);
        }

        public void CollectValues(StringBuilder stBuilder)
        {
            int j = valueCollection.Count;
            for (int i = 0; i < j; ++i)
            {
                CssInstructionValue propV = valueCollection[i];
                stBuilder.Append(propV.ToString());
                if (i < j - 1)
                {
                    stBuilder.Append(' ');
                }
            }
        }
        public override string ToString()
        {

            StringBuilder stBuilder = new StringBuilder();
            CollectValues(stBuilder);
            return stBuilder.ToString();
        }

        public bool MarkedAsInherit
        {
            get
            {
                return this.markedAsInherite;
            }
        }
    }

   
    public class CssInstructionValue
    {
        
        public string propertyValue;
        public string unit;

        public CssInstructionValue(string propertyValue)
        {
            this.propertyValue = propertyValue;

        }
        public override string ToString()
        {
            if (unit != null)
            {
                return propertyValue + unit;
            }
            else
            {
                return propertyValue;
            }
        } 
    }

}