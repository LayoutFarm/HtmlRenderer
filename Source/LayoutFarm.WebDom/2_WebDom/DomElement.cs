//BSD, 2014-2018, WinterDev

using System.Collections.Generic;
namespace LayoutFarm.WebDom
{
    public abstract partial class DomElement : DomNode
    {
        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex;
        Dictionary<int, DomAttribute> myAttributes;
        List<DomNode> myChildrenNodes;
        //------------------------------------------- 
        DomAttribute attrElemId;
        DomAttribute attrClass;
        //-------------------------------------------

        HtmlEventHandler evhMouseDown;
        HtmlEventHandler evhMouseUp;
        HtmlEventHandler evhMouseLostFocus;
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
                foreach (DomAttribute attr in myAttributes.Values)
                {
                    yield return attr;
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
        public virtual void SetAttribute(DomAttribute attr)
        {
            if (myAttributes == null)
            {
                myAttributes = new Dictionary<int, DomAttribute>();
            }
            //-----------
            //some wellknownattr 
            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Id:
                    {
                        this.attrElemId = attr;
                        this.OwnerDocument.RegisterElementById(this);
                    }
                    break;
                case WellknownName.Class:
                    {
                        this.attrClass = attr;
                    }
                    break;
            }
            //--------------------
            var attrNameIndex = this.OwnerDocument.AddStringIfNotExists(attr.LocalName);
            myAttributes[attrNameIndex] = attr;//update or replace 
            attr.SetParent(this);
            NotifyChange(ElementChangeKind.AddAttribute);
            //---------------------
        }
        public void SetAttribute(string attrName, string value)
        {
            DomAttribute domAttr = this.OwnerDocument.CreateAttribute(null, attrName);
            domAttr.Value = value;
            SetAttribute(domAttr);
        }

        public void AddAttribute(DomAttribute attr)
        {
            if (myAttributes == null)
            {
                myAttributes = new Dictionary<int, DomAttribute>();
            }
            //-----------
            //some wellknownattr 
            switch (attr.LocalNameIndex)
            {
                case (int)WellknownName.Id:
                    {
                        this.attrElemId = attr;
                        this.OwnerDocument.RegisterElementById(this);
                    }
                    break;
                case (int)WellknownName.Class:
                    {
                        this.attrClass = attr;
                    }
                    break;
            }
            myAttributes.Add(attr.LocalNameIndex, attr);
            attr.SetParent(this);
            NotifyChange(ElementChangeKind.AddAttribute);
        }

        public virtual void AddChild(DomNode childNode)
        {
            switch (childNode.NodeType)
            {
                case HtmlNodeType.Attribute:
                    {
                        AddAttribute((DomAttribute)childNode);
                    }
                    break;
                default:
                    {
                        if (myChildrenNodes == null)
                        {
                            myChildrenNodes = new List<DomNode>();
                        }
                        myChildrenNodes.Add((DomNode)childNode);
                        childNode.SetParent(this);
                        NotifyChange(ElementChangeKind.AddChild);
                    }
                    break;
            }
        }
        public virtual bool RemoveChild(DomNode childNode)
        {
            switch (childNode.NodeType)
            {
                case HtmlNodeType.Attribute:
                    {
                        //TODO: support remove attribute
                        return false;
                    }
                default:
                    {
                        if (myChildrenNodes != null)
                        {
                            bool result = myChildrenNodes.Remove(childNode);
                            if (result)
                            {
                                childNode.SetParent(null);
                                NotifyChange(ElementChangeKind.RemoveChild);
                            }
                            return result;
                        }
                        return false;
                    }
            }
        }

        /// <summary>
        /// clear all children elements
        /// </summary>
        public virtual void ClearAllElements()
        {
            if (this.myChildrenNodes != null)
            {
                for (int i = this.myChildrenNodes.Count - 1; i >= 0; --i)
                {
                    //clear content 
                    myChildrenNodes[i].SetParent(null);
                }
                this.myChildrenNodes.Clear();
                NotifyChange(ElementChangeKind.ClearAllChildren);
            }
        }

        public void NotifyChange(ElementChangeKind changeKind)
        {
            switch (this.DocState)
            {
                case DocumentState.ChangedAfterIdle:
                case DocumentState.Idle:
                    {
                        //notify parent 
                        OnElementChangedInIdleState(changeKind);
                    }
                    break;
            }
        }
        protected virtual void OnElementChangedInIdleState(ElementChangeKind changeKind)
        {
        }
        //------------------------------------------
        public DomAttribute FindAttribute(int attrLocalNameIndex)
        {
            if (myAttributes != null)
            {
                DomAttribute found;
                myAttributes.TryGetValue(attrLocalNameIndex, out found);
                return found;
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