// 2015,2014 ,BSD, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo 

namespace LayoutFarm.WebDom.Impl
{
    public class HtmlTextNode : DomTextNode
    {
        //---------------------------------
        //this node may be simple text node  
        bool freeze;
        bool hasSomeChar;
        public HtmlTextNode(WebDocument ownerDoc, char[] buffer)
            : base(ownerDoc, buffer)
        {
        }
        public bool IsWhiteSpace
        {
            get
            {
                return !this.hasSomeChar;
            }
        }

        public bool IsFreeze
        {
            get { return this.freeze; }
        }
#if DEBUG
        public override string ToString()
        {
            return new string(base.GetOriginalBuffer());
        }
#endif
        public void WriteTextNode(DomTextWriter writer)
        {
            //write inner run
            writer.InnerStringBuilder.Append(this.GetOriginalBuffer());
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