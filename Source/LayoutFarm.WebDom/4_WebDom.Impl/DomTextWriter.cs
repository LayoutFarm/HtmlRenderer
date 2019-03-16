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
        public void Clear() => _stbuilder.Length = 0;
        public StringBuilder InnerStringBuilder => _stbuilder;
        public override string ToString() => _stbuilder.ToString();

        public void Write(char[] buffer)
        {
            _stbuilder.Append(buffer);
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