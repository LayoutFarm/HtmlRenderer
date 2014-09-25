//BSD  2014 ,WinterDev

using System;
using System.Text;
using System.Collections.Generic;

namespace HtmlRenderer.WebDom
{   
    public abstract partial class DomElement : DomNode
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
                case (int)WellknownName.Id:
                    {
                        this.attrElemId = attr;
                        this.OwnerDocument.RegisterElementById(this);
                    } break;
                case (int)WellknownName.Class:
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
         
        public string Name
        {
            get { return this.LocalName; }
        }
        
       
    }


 
}