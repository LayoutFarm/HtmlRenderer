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
        public DocumentState DocState
        {
            get
            {
                return this.ownerDoc.DocumentState;
            }
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

        public HtmlDocument OwnerDocument
        {
            get
            {
                return ownerDoc;
            }
        }

        internal void SetParent(HtmlNode parentNode)
        {
            this.parentNode = parentNode;
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
                Array.Copy(newCopyBuffer, 0, newbuffer, copyBuffer.Length, newCopyBuffer.Length);
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
        //-------------------------------

        public string Name
        {
            get
            {
                return this.LocalName;
            }
        }
    }

    public abstract class HtmlElement : HtmlNode
    {

        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex;
        List<HtmlAttribute> myAttributes;
        List<HtmlNode> myChildrenNodes;
        //------------------------------------------- 
        HtmlAttribute attrElemId;
        HtmlAttribute attrClass;
        //-------------------------------------------

        HtmlEventHandler evhMouseDown;
        HtmlEventHandler evhMouseUp;


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

        public void AddAttribute(HtmlAttribute attr)
        {
            if (myAttributes == null)
            {
                myAttributes = new List<HtmlAttribute>();
            }
            //-----------
            //some wellknownattr 
            switch (attr.LocalNameIndex)
            {
                case (int)WellknownHtmlName.Id:
                    {
                        this.attrElemId = attr;
                        this.OwnerDocument.RegisterElementById(this);
                    } break;
                case (int)WellknownHtmlName.Class:
                    {
                        this.attrClass = attr;
                    } break;
            }

            myAttributes.Add(attr);
            attr.SetParent(this);



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
        public bool RemoveChild(HtmlNode childNode)
        {
            switch (childNode.NodeType)
            {
                case HtmlNodeType.Attribute:
                    {


                        return false;
                    }
                default:
                    if (myChildrenNodes != null)
                    {
                        return myChildrenNodes.Remove(childNode);
                    }
                    return false;
            }
        }

        /// <summary>
        /// clear all children elements
        /// </summary>
        public void ClearAllElements()
        {
            if (this.myChildrenNodes != null)
            {
                this.myChildrenNodes.Clear();
            }
            switch (this.DocState)
            {
                case DocumentState.Idle:
                    {
                        //change 
                        this.OwnerDocument.SetDocumentState(DocumentState.ChangedAfterIdle);
                    } break; 
            }
        }

        //------------------------------------------
        public HtmlAttribute FindAttribute(int attrLocalNameIndex)
        {
            if (myAttributes != null)
            {
                for (int i = myAttributes.Count - 1; i >= 0; --i)
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


        internal bool HasAttributeElementId
        {
            get
            {
                return this.attrElemId != null;
            }
        }
        internal bool HasAttributeClass
        {
            get
            {
                return this.attrClass != null;
            }
        }
        internal string AttrClassValue
        {
            get
            {
                if (this.attrClass != null)
                {
                    return this.attrClass.Value;
                }
                return null;
            }
        }
        internal string AttrElementId
        {
            get
            {
                if (attrElemId != null)
                {
                    return this.attrElemId.Value;
                }

                return null;
            }
        }

        public virtual void DispatchEvent(HtmlEventArgs eventArgs)
        {

            switch (eventArgs.EventName)
            {
                //--------------------------
                //primary mouse event
                case EventName.MouseDown:
                    {
                        OnMouseDown(eventArgs);
                    } break;
                case EventName.MouseUp:
                    {
                        OnMouseUp(eventArgs);
                    } break;
                case EventName.MouseMove:
                    {


                    } break;
                //-----------------------------
                //secondary mouse event
                case EventName.MouseOver:
                    {
                    } break;
                case EventName.MouseLeave:
                    {
                    } break;
                //-----------------------------
            }
        }
        public string Name
        {
            get { return this.LocalName; }
        }
        //------------------------------------------------------------------------ 
        protected virtual void OnMouseDown(HtmlEventArgs e)
        {
            //some element has intrinsic reponse to event 
            //eg. click on link  
            if (this.evhMouseDown != null)
            {
                this.evhMouseDown(e);
            }
        }
        protected virtual void OnMouseUp(HtmlEventArgs e)
        {
            if (this.evhMouseUp != null)
            {
                this.evhMouseUp(e);
            }
        }
        public void AttachEvent(EventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case EventName.MouseDown:
                    {
                        this.evhMouseDown += handler;
                    } break;
                case EventName.MouseUp:
                    {
                        this.evhMouseUp += handler;
                    } break;
            }
        }
        public void DetachEvent(EventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case EventName.MouseDown:
                    {
                        this.evhMouseDown -= handler;
                    } break;
                case EventName.MouseUp:
                    {
                        this.evhMouseUp -= handler;
                    } break;
            }

        }
        //-------------------------------------------------------
    }

}