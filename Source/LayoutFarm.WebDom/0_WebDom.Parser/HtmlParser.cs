//BSD  2015,2014 ,WinterDev


using System.Collections.Generic;
namespace LayoutFarm.WebDom.Parser
{
    public enum ParseEngineKind
    {
        MyHtmlParser,
        HtmlKitParser
    }
    public abstract class HtmlParser
    {
        public abstract void ResetParser();
        public abstract void Parse(TextSource textSnapshot, WebDocument htmldoc, DomElement currentNode);
        public static HtmlParser CreateHtmlParser(ParseEngineKind engineKind)
        {
            switch (engineKind)
            {
                case ParseEngineKind.HtmlKitParser:
                    return new HtmlKitParser();
                default:
                    return new HtmlKitParser();
                    //return new MyHtmlParser();
            }
        }
    }


    class HtmlStack
    {

        List<DomElement> nodes = new List<DomElement>();
        int count;

        public HtmlStack()
        {
        }
        public void Push(DomElement node)
        {
            count++;
            this.nodes.Add(node);
        }
        public DomElement Pop()
        {
            DomElement node = this.nodes[count - 1];
            this.nodes.RemoveAt(count - 1);
            count--;
            return node;
        }
        public DomElement Peek()
        {
            if (count > 1)
            {
                return nodes[count - 1];
            }
            else
            {
                return null;
            }
        }
        public int Count
        {
            get
            {
                return this.count;
            }
        }
        public void Clear()
        {
            this.nodes.Clear();
            this.count = 0;
        }
    }

}