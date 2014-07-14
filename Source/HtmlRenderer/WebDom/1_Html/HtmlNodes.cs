//BSD  2014 ,WinterDev

using System;
using System.Text;
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

    
    public abstract class HtmlTextNode : HtmlNode
    {

        char[] copyBuffer;
        internal HtmlTextNode(HtmlDocument ownerDoc, char[] copyBuffer)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeType.TextNode);
            this.copyBuffer = copyBuffer;
        }
        internal void AppendTextContent(char[] newCopyBuffer)
        {
            if (copyBuffer != null)
            {
                char[] newbuffer = new char[copyBuffer.Length + newCopyBuffer.Length];
                Array.Copy(copyBuffer, newbuffer, copyBuffer.Length);
                Array.Copy(newCopyBuffer, 0, newbuffer, copyBuffer.Length + 1, newCopyBuffer.Length);
                this.copyBuffer = newbuffer;
            }
            else
            {
                this.copyBuffer = newCopyBuffer;
            }
        }
        internal char[] GetOriginalBuffer()
        {
            return copyBuffer;
            //if(copyBuffer
            //char[] copyBuffer = new char[this._sb.Length];
            //_sb.CopyTo(0, copyBuffer, 0, copyBuffer.Length);
            //return copyBuffer;
        }
#if DEBUG
        public override string ToString()
        {
            if (copyBuffer != null)
            {
                return "t-node" + string.Empty;
            }
            else
            {
                return "t-node " + new string(this.copyBuffer);
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
    public class HtmlAttribute : HtmlNode, IHtmlAttribute
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
        //-------------------------------

        public string Name
        {
            get
            {
                return this.LocalName;
            }
        }
    }

    public abstract class HtmlElement : HtmlNode, IHtmlElement
    {

        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex;

        List<HtmlAttribute> myAttributes;
        List<HtmlNode> myChildrenNodes;

        HtmlElement closeNode;

        string _elementId;
        string _className;
        string _style;

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
        public IEnumerable<IHtmlAttribute> GetAttributeIter()
        {
            if (this.myAttributes != null)
            {
                foreach (var atttr in this.myAttributes)
                {
                    yield return atttr;
                }
            }
        } 
        
        public int ChildrenCount
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

        public HtmlNode GetChildNode(int index)
        {
            return this.myChildrenNodes[index];
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
                for (int i = myAttributes.Count - 1; i >=0; --i)
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
       

        //------------------------------------------
        //temp fix
        public string Id
        {//temp fix
            get
            {
                throw new NotSupportedException();
                return this._elementId;
            }
            set
            {
                this._elementId = value;
            }
        }
        public string ClassName
        {//temp fix
            get
            {
                throw new NotSupportedException();
                return this._className;
            }
            set { this._className = value; }
        }
        public string Style
        {//temp fix
            get
            {
                throw new NotSupportedException();
                return this._style;
            }
            set { this._style = value; }
        }
        
        
        public bool HasAttributes()
        {
            return this.AttributeCount > 0;
        }
        public string Name
        {
            get { return this.LocalName; }
        } 
        
    }
 
}