//BSD  2014 ,WinterFarm

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HtmlRenderer.WebDom
{
    public enum HtmlNodeType
    {
        OpenElement,
        CloseElement,
        ShortElement,
        Attribute,
        TextNode,
        CData,
        ProcessInstruction,
        Comment
    }



    public abstract class HtmlNode
    {

        HtmlDocument ownerDoc;
        HtmlNode parentNode;

        HtmlNodeType nodeType;

#if DEBUG
        static int dbugTotalId;
        public int dbugId;
#endif

        internal HtmlNode(HtmlDocument ownerDoc)
        {
            this.ownerDoc = ownerDoc;

#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;
           
#endif

        }
        public HtmlNode ParentNode
        {
            get
            {
                return this.parentNode;
            }
        }
        protected void SetNodeType(HtmlNodeType nodeType)
        {
            this.nodeType = nodeType;
        }
        public HtmlNodeType NodeType
        {
            get
            {
                return nodeType;
            }
        }
        internal void SetParent(HtmlNode parentNode)
        {
            this.parentNode = parentNode;
        }
        public HtmlDocument OwnerDocument
        {
            get
            {
                return ownerDoc;
            }
        }
    }

    public class HtmlRootNode : HtmlElement
    {
        internal HtmlRootNode(HtmlDocument ownerDoc)
            : base(ownerDoc, 0, 0)
        {
        }
    }


    public class HtmlTextNode : HtmlNode
    {
        StringBuilder _sb = new StringBuilder();
        internal HtmlTextNode(HtmlDocument ownerDoc, char[] textBuffer)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeType.TextNode);
            this.AppendTextContent(textBuffer);
        }
        public void AppendTextContent(char[] buff)
        {
            this._sb.Append(buff);
        }
        public char[] CopyTextBuffer()
        {
            char[] copyBuffer = new char[this._sb.Length];
            _sb.CopyTo(0, copyBuffer, 0, copyBuffer.Length);
            return copyBuffer;
        }
#if DEBUG
        public override string ToString()
        {
            if (this._sb.Length == 0)
            {
                return "t-node" + string.Empty;
            }
            else
            {
                return "t-node " + _sb.ToString();
            }
        }
#endif
    }

    public class HtmlComment : HtmlNode
    {
        internal HtmlComment(HtmlDocument ownerDoc)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeType.Comment);
        }
        public StringBuilder Content
        {
            get;
            set;
        }

    }
    public class HtmlProcessInstructionNode : HtmlNode
    {
        int procName;
        internal HtmlProcessInstructionNode(HtmlDocument ownerDoc, int procName)
            : base(ownerDoc)
        {
            this.procName = procName;
            SetNodeType(HtmlNodeType.ProcessInstruction);
        }
        public StringBuilder Content
        {
            get;
            set;
        }

    }
    public class HtmlCDataNode : HtmlNode
    {
        internal HtmlCDataNode(HtmlDocument ownerDoc)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeType.CData);
        }
        public string Content
        {
            get;
            set;
        }

    }

    /// <summary>
    /// attribute node
    /// </summary>
    public class HtmlAttribute : HtmlNode
    {

        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex;
        string attrValue;

        internal HtmlAttribute(HtmlDocument ownerDoc,
            int nodePrefixNameIndex,
            int nodeLocalNameIndex)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeType.Attribute);
            this.nodePrefixNameIndex = nodePrefixNameIndex;
            this.nodeLocalNameIndex = nodeLocalNameIndex;
        }

        public string Value
        {
            get { return this.attrValue; }
            set { this.attrValue = value; }
        }

        public string Prefix
        {
            get
            {
                return OwnerDocument.GetString(this.nodePrefixNameIndex);
            }
        }
        public string LocalName
        {
            get
            {
                return OwnerDocument.GetString(this.nodeLocalNameIndex);
            }
        }
        public int LocalNameIndex
        {
            get
            {
                return this.nodeLocalNameIndex;
            }
        }
        public int PrefixNameIndex
        {
            get
            {
                return this.nodePrefixNameIndex;
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.nodePrefixNameIndex > 0)
            {
                sb.Append(Prefix);
                sb.Append(':');
                sb.Append(LocalName);
            }
            else
            {
                sb.Append(this.LocalName);
            }
            if (this.Value != null)
            {
                sb.Append('=');
                sb.Append(this.Value);
            }

            return sb.ToString();
        }
#endif
    }

    public class HtmlElement : HtmlNode
    {

        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex; 

        List<HtmlAttribute> myAttributes;
        List<HtmlNode> myChildrenNodes;

        HtmlElement closeNode; 

        internal HtmlElement(HtmlDocument ownerDoc, int nodePrefixNameIndex, int nodeLocalNameIndex)
            : base(ownerDoc)
        {

            this.nodePrefixNameIndex = nodePrefixNameIndex;
            this.nodeLocalNameIndex = nodeLocalNameIndex;
            SetNodeType(HtmlNodeType.OpenElement);
        }
       
        public static bool EqualNames(HtmlElement node1, HtmlElement node2)
        {
            return node1.nodeLocalNameIndex == node2.nodeLocalNameIndex
                && node1.nodePrefixNameIndex == node2.nodePrefixNameIndex;
        }
#if DEBUG
        public override string ToString()
        {
            return "e-node: " + this.LocalName;
        }
#endif
        public IEnumerable<HtmlAttribute> GetAttributeIterForward()
        {
            if (myAttributes != null)
            {
                int j = myAttributes.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return myAttributes[i];
                }
            }
        }
        public IEnumerable<HtmlNode> GetChildNodeIterForward()
        {
            if (myChildrenNodes != null)
            {
                int j = myChildrenNodes.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return myChildrenNodes[i];
                }
            }
        }

        public void MarkAsShortTagElement()
        {
            SetNodeType(HtmlNodeType.ShortElement);
        }
        public void MarkAsCloseTagElement()
        {
            SetNodeType(HtmlNodeType.CloseElement);
        }
        public int ChildNodeCount
        {
            get
            {
                if (this.myChildrenNodes != null)
                {
                    return this.myChildrenNodes.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        public HtmlElement CloseNode
        {
            get
            {
                return this.closeNode;
            }
            set
            {
                this.closeNode = value;
            }
        }

        public void AddAttribute(HtmlAttribute attr)
        {

            if (myAttributes == null)
            {
                myAttributes = new List<HtmlAttribute>();
            }
            myAttributes.Add(attr);
            attr.SetParent(this);
        }


        public HtmlAttribute FindAttribute(int attrLocalNameIndex)
        {
            if (myAttributes != null)
            {
                for (int i = myAttributes.Count - 1; i > -1; --i)
                {
                    if (myAttributes[i].nodeLocalNameIndex == attrLocalNameIndex)
                    {
                        return myAttributes[i];
                    }
                }
            }
            return null;
        }
        public HtmlAttribute FindAttribute(string attrname)
        {
            int nameIndex = this.OwnerDocument.FindStringIndex(attrname);
            if (nameIndex >= 0)
            {
                return this.FindAttribute(nameIndex);
            }
            else
            {
                return null;
            }
        }

        public int AttributeCount
        {
            get
            {
                return (this.myAttributes != null) ? this.myAttributes.Count : 0;
            }
        }
        public string Prefix
        {
            get
            {
                return OwnerDocument.GetString(this.nodePrefixNameIndex);
            }
        }
        public string LocalName
        {
            get
            {
                return OwnerDocument.GetString(this.nodeLocalNameIndex);
            }
        }
        public int LocalNameIndex
        {
            get
            {
                return this.nodeLocalNameIndex;
            }
        }
        public int PrefixNameIndex
        {
            get
            {
                return this.nodePrefixNameIndex;
            }
        }

        public void AddChild(HtmlNode childNode)
        {
            switch (childNode.NodeType)
            {
                case HtmlNodeType.Attribute:
                    {
                        AddAttribute((HtmlAttribute)childNode);

                    } break;
                default:
                    {
                        if (myChildrenNodes == null)
                        {
                            myChildrenNodes = new List<HtmlNode>();
                        }
                        myChildrenNodes.Add((HtmlNode)childNode);
                        childNode.SetParent(this);
                    } break;
            }
        }
        public HtmlNode GetFirstNode()
        {
            return this.myChildrenNodes[0];
        }
    }

}