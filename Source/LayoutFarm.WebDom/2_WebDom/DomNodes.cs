//BSD  2015,2014 ,WinterDev

using System;
using System.Text;
using System.Collections.Generic;

namespace LayoutFarm.WebDom
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



    public abstract class DomNode
    {

        WebDocument ownerDoc;
        DomNode parentNode; 
        HtmlNodeType nodeType;

#if DEBUG
        static int dbugTotalId;
        public int dbugId;
        public int dbugMark;
#endif

        internal DomNode(WebDocument ownerDoc)
        {
            this.ownerDoc = ownerDoc; 
#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;
#endif

        }
        public DocumentState DocState
        {
            get
            {
                return this.ownerDoc.DocumentState;
            }
        }
        public DomNode ParentNode
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

        public WebDocument OwnerDocument
        {
            get
            {
                return ownerDoc;
            }
        }

        internal void SetParent(DomNode parentNode)
        {
            this.parentNode = parentNode;
        }

    }


    public abstract class DomTextNode : DomNode
    {

        char[] copyBuffer;
        public DomTextNode(WebDocument ownerDoc, char[] copyBuffer)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeType.TextNode);
            this.copyBuffer = copyBuffer;
        }
        public void AppendTextContent(char[] newCopyBuffer)
        {
            if (copyBuffer != null)
            {
                char[] newbuffer = new char[copyBuffer.Length + newCopyBuffer.Length];
                Array.Copy(copyBuffer, newbuffer, copyBuffer.Length);
                Array.Copy(newCopyBuffer, 0, newbuffer, copyBuffer.Length, newCopyBuffer.Length);
                this.copyBuffer = newbuffer;
            }
            else
            {
                this.copyBuffer = newCopyBuffer;
            }
        }
        public char[] GetOriginalBuffer()
        {
            return copyBuffer;
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

    public class DomComment : DomNode
    {
        internal DomComment(WebDocument ownerDoc)
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
    public class DomProcessInstructionNode : DomNode
    {
        int procName;
        internal DomProcessInstructionNode(WebDocument ownerDoc, int procName)
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
    public class DomCDataNode : DomNode
    {
        internal DomCDataNode(WebDocument ownerDoc)
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
    public class DomAttribute : DomNode
    {

        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex;
        string attrValue;

        public DomAttribute(WebDocument ownerDoc,
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

    

}