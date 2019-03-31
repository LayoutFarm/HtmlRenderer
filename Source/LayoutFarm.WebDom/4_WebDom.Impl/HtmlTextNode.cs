//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo 

namespace LayoutFarm.WebDom.Impl
{
    public class HtmlTextNode : DomTextNode
    {
        //---------------------------------
        //this node may be simple text node  
        bool _freeze;
        bool _hasSomeChar;
        public HtmlTextNode(WebDocument ownerDoc, char[] buffer)
            : base(ownerDoc, buffer)
        {
        }
        //
        public bool IsWhiteSpace => !_hasSomeChar;
        public bool IsFreeze => _freeze;

#if DEBUG
        public override string ToString()
        {
            return new string(base.GetOriginalBuffer());
        }
#endif
        public void WriteTextNode(DomTextWriter writer)
        {
            //write inner run
            writer.Write(this.GetOriginalBuffer());
        }
    }
    public enum TextSplitPartKind : byte
    {
        Text = 1,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }
}