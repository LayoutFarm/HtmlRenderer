//MIT, 2015-2018, WinterDev  
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Text;
namespace LayoutFarm.WebDom
{
    public class DomTextWriter
    {
        StringBuilder stbuilder;
        public DomTextWriter(StringBuilder stbuilder)
        {
            this.stbuilder = stbuilder;
        }
        public StringBuilder InnerStringBuilder
        {
            get { return this.stbuilder; }
        }
        public void Write(string s1)
        {
            stbuilder.Append(s1);
        }
        public void Write(char c1)
        {
            stbuilder.Append(c1);
        }
        public void Write(string s1, string s2)
        {
            stbuilder.Append(s1);
            stbuilder.Append(s2);
        }
        public void Write(string s1, string s2, string s3)
        {
            stbuilder.Append(s1);
            stbuilder.Append(s2);
            stbuilder.Append(s3);
        }
        public void NewLine()
        {
            stbuilder.AppendLine();
        }
    }
}