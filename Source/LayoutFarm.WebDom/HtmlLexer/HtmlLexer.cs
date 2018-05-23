//BSD, 2014-2018, WinterDev

using LayoutFarm.WebLexer;
namespace LayoutFarm.WebDom.Parser
{
    
    public abstract partial class HtmlLexer
    {
        public event XmlLexerEventHandler LexStateChanged;
        protected void RaiseStateChanged(XmlLexerEvent lexEvent, int startIndex, int len)
        {
            LexStateChanged(lexEvent, startIndex, len);
        }
        public virtual void Analyze(TextSnapshot textSnapshot) { }
        public virtual void BeginLex()
        {
        }
        public virtual void EndLex()
        {
        }
        public static HtmlLexer CreateLexer()
        {
            return new MyHtmlLexer();
        }
    }
}
