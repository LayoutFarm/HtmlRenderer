//MIT, 2015-present, WinterDev  
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Text;
namespace LayoutFarm.WebDom
{
    public class DomTextWriter
    {
        StringBuilder _stbuilder;
        public DomTextWriter(StringBuilder stbuilder)
        {
            _stbuilder = stbuilder;
        }
        public StringBuilder InnerStringBuilder
        {
            get { return _stbuilder; }
        }
        public void Write(string s1)
        {
            _stbuilder.Append(s1);
        }
        public void Write(char c1)
        {
            _stbuilder.Append(c1);
        }
        public void Write(string s1, string s2)
        {
            _stbuilder.Append(s1);
            _stbuilder.Append(s2);
        }
        public void Write(string s1, string s2, string s3)
        {
            _stbuilder.Append(s1);
            _stbuilder.Append(s2);
            _stbuilder.Append(s3);
        }
        public void NewLine()
        {
            _stbuilder.AppendLine();
        }
    }
}