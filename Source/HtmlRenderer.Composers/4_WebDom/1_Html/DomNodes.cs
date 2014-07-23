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



    public abstract class DomNode
    {

        WebDocument ownerDoc;
        DomNode parentNode;

        HtmlNodeType nodeType;

#if DEBUG
        static int dbugTotalId;
        public int dbugId;
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


    public abstract class HtmlTextNode : DomNode
    {

        char[] copyBuffer;
        public HtmlTextNode(WebDocument ownerDoc, char[] copyBuffer)
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

        internal DomAttribute(WebDocument ownerDoc,
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

    public abstract class DomElement : DomNode
    {

        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex;
        List<DomAttribute> myAttributes;
        List<DomNode> myChildrenNodes;
        //------------------------------------------- 
        DomAttribute attrElemId;
        DomAttribute attrClass;
        //-------------------------------------------

        HtmlEventHandler evhMouseDown;
        HtmlEventHandler evhMouseUp;


        public DomElement(WebDocument ownerDoc, int nodePrefixNameIndex, int nodeLocalNameIndex)
            : base(ownerDoc)
        {

            this.nodePrefixNameIndex = nodePrefixNameIndex;
            this.nodeLocalNameIndex = nodeLocalNameIndex;
            SetNodeType(HtmlNodeType.OpenElement);
        }

        public static bool EqualNames(DomElement node1, DomElement node2)
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
        public IEnumerable<DomAttribute> GetAttributeIterForward()
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
        public IEnumerable<DomNode> GetChildNodeIterForward()
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

        public DomNode GetChildNode(int index)
        {
            return this.myChildrenNodes[index];
        }

        public void AddAttribute(DomAttribute attr)
        {
            if (myAttributes == null)
            {
                myAttributes = new List<DomAttribute>();
            }
            //-----------
            //some wellknownattr 
            switch (attr.LocalNameIndex)
            {
                case (int)WellknownElementName.Id:
                    {
                        this.attrElemId = attr;
                        this.OwnerDocument.RegisterElementById(this);
                    } break;
                case (int)WellknownElementName.Class:
                    {
                        this.attrClass = attr;
                    } break;
            }
            myAttributes.Add(attr);
            attr.SetParent(this);
        }
        public void AddChild(DomNode childNode)
        {
            switch (childNode.NodeType)
            {
                case HtmlNodeType.Attribute:
                    {
                        AddAttribute((DomAttribute)childNode);
                    } break;
                default:
                    {
                        if (myChildrenNodes == null)
                        {
                            myChildrenNodes = new List<DomNode>();
                        }
                        myChildrenNodes.Add((DomNode)childNode);
                        childNode.SetParent(this);

                        NotifyChange(ElementChangeKind.AddChild);

                    } break;
            }
        }
        public bool RemoveChild(DomNode childNode)
        {
            switch (childNode.NodeType)
            {
                case HtmlNodeType.Attribute:
                    {
                        //TODO: support remove attribute
                        return false;
                    }
                default:
                    if (myChildrenNodes != null)
                    {
                        bool result = myChildrenNodes.Remove(childNode);
                        if (result)
                        {
                            NotifyChange(ElementChangeKind.RemoveChild);
                        }
                        return result;
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
                NotifyChange(ElementChangeKind.ClearAllChildren);
            }            
          
        } 
        void NotifyChange(ElementChangeKind changeKind)
        {
            switch (this.DocState)
            {
                case DocumentState.ChangedAfterIdle:
                case DocumentState.Idle:
                    {
                        //notify parent 
                        OnChangeInIdleState(changeKind);
                    } break;
            }
        }
        protected virtual void OnChangeInIdleState(ElementChangeKind changeKind)
        {

        }
        //------------------------------------------
        public DomAttribute FindAttribute(int attrLocalNameIndex)
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
        public DomAttribute FindAttribute(string attrname)
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
            //TODO:
            get { return this.nodeLocalNameIndex; }
        }

        public bool HasAttributeElementId
        {
            get
            {
                return this.attrElemId != null;
            }
        }
        public bool HasAttributeClass
        {
            get
            {
                return this.attrClass != null;
            }
        }
        public string AttrClassValue
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
        public string AttrElementId
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