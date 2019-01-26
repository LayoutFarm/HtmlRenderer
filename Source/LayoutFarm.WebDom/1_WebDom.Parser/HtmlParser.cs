//BSD, 2014-present, WinterDev 


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
                    //return new HtmlKitParser();
                    return new MyHtmlParser();
            }
        }
    }


    class HtmlStack
    {
        List<DomElement> _nodes = new List<DomElement>();
        int _count;
        public HtmlStack()
        {
        }
        public void Push(DomElement node)
        {
            _count++;
            _nodes.Add(node);
        }
        public DomElement Pop()
        {
            DomElement node = _nodes[_count - 1];
            _nodes.RemoveAt(_count - 1);
            _count--;
            return node;
        }
        public DomElement Peek()
        {
            if (_count > 1)
            {
                return _nodes[_count - 1];
            }
            else
            {
                return null;
            }
        }
        //
        public int Count => _count;


        public void Clear()
        {
            _nodes.Clear();
            _count = 0;
        }
    }
}