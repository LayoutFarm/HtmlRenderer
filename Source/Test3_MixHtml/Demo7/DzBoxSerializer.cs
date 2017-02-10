//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.UI;
using System.Xml;
namespace LayoutFarm.DzBoardSample
{
    delegate void UISerializeHandler(DzBoxSerializer writer, UIElement box);
    class DzBoxSerializer : UIVisitor
    {
        Stack<UIElement> uiStacks = new Stack<UIElement>();
        XmlDocument xmldoc = new XmlDocument();
        XmlElement rootElement;
        XmlElement currentElement;
        Stack<XmlElement> elementStack = new Stack<XmlElement>();
        public DzBoxSerializer(string rootNodeName)
        {
            rootElement = currentElement = xmldoc.CreateElement(rootNodeName);
            xmldoc.AppendChild(rootElement);
        }
        public override void BeginElement(UIElement ui, string uiname)
        {
            var newElement = xmldoc.CreateElement(uiname);
            elementStack.Push(currentElement);
            currentElement.AppendChild(newElement);
            currentElement = newElement;
            uiStacks.Push(ui);
        }
        public override void EndElement()
        {
            uiStacks.Pop();
            currentElement = elementStack.Pop();
        }
        public override void Attribute(string name, double value)
        {
            currentElement.SetAttribute(name, value.ToString());
        }
        public override void Attribute(string name, int value)
        {
            currentElement.SetAttribute(name, value.ToString());
        }
        public override void Attribute(string name, string value)
        {
            //create text node
            currentElement.SetAttribute(name, value);
        }
        public override void TextNode(string content)
        {
            currentElement.AppendChild(xmldoc.CreateTextNode(content));
        }
        public override void Comment(string content)
        {
            currentElement.AppendChild(xmldoc.CreateComment(content));
        }
        public void WriteToFile(string filename)
        {
            this.xmldoc.Save(filename);
        }
    }

    class DzBoxDeserializer
    {
        XmlDocument xmldoc;
        public void LoadFile(string filename)
        {
            xmldoc = new XmlDocument();
            xmldoc.Load(filename);
        }
        public void DrawContents(DesignBoardModule dzBoard)
        {
            //write content to the viewport
            var rootdoc = xmldoc.DocumentElement;
            foreach (var childnode in rootdoc.ChildNodes)
            {
                var elemNode = childnode as XmlElement;
                if (elemNode == null)
                {
                    continue;
                }
                //create element
                int left, top, width, height;
                switch (elemNode.Name)
                {
                    case "rectbox":
                        {
                            //create rect box and add to viewport
                            GetDimensionAttr(elemNode, out left, out top, out width, out height);
                            dzBoard.AddNewBox(left, top, width, height);
                        }
                        break;
                    case "shapebox":
                        {
                            GetDimensionAttr(elemNode, out left, out top, out width, out height);
                            dzBoard.AddNewRect(left, top, width, height);
                        }
                        break;
                    case "textbox":
                        {
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
            }
        }
        static void GetDimensionAttr(XmlElement elemNode, out int left, out int top, out int width, out int height)
        {
            var s_left = elemNode.GetAttribute("left");
            var s_top = elemNode.GetAttribute("top");
            var s_width = elemNode.GetAttribute("width");
            var s_height = elemNode.GetAttribute("height");
            left = int.Parse(s_left);
            top = int.Parse(s_top);
            width = int.Parse(s_width);
            height = int.Parse(s_height);
        }
    }


    static class DzBoxSerializerHelper
    {
        static Dictionary<Type, string> registerTypeNames = new Dictionary<Type, string>();
        static DzBoxSerializerHelper()
        {
            registerTypeNames.Add(typeof(LayoutFarm.CustomWidgets.SimpleBox), "panel");
        }

        public static void WriteElement(DzBoxSerializer writer, UIBox uiElement, string elemName)
        {
            uiElement.Walk(writer);
            //writer.BeginElement(elemName);
            ////collect bounds and attrs
            //writer.AddAttribute("left", uiElement.Left.ToString());
            //writer.AddAttribute("top", uiElement.Top.ToString());
            //writer.AddAttribute("width", uiElement.Width.ToString());
            //writer.AddAttribute("height", uiElement.Height.ToString());
            ////-------------------------------------------------------

            ////content
            //writer.EndElement();
        }
        public static void WriteCommon(DzBoxSerializer writer, UIBox uiElement)
        {
            //find element type
            //var elemType = uiElement.GetType();

            //writer.BeginElement(elemName);
            ////collect bounds and attrs
            //writer.AddAttribute("left", uiElement.Left.ToString());
            //writer.AddAttribute("top", uiElement.Top.ToString());
            //writer.AddAttribute("width", uiElement.Width.ToString());
            //writer.AddAttribute("height", uiElement.Height.ToString());
            ////-------------------------------------------------------

            ////content
            //writer.EndElement();
        }
    }
}