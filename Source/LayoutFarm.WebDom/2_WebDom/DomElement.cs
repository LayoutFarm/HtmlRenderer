//BSD, 2014-present, WinterDev 

using System.Collections.Generic;
namespace LayoutFarm.WebDom
{
    public abstract partial class DomElement : DomNode
    {
        internal readonly int _nodePrefixNameIndex;
        internal readonly int _nodeLocalNameIndex;
        Dictionary<int, DomAttribute> _myAttributes;
        List<DomNode> _myChildrenNodes;
        //------------------------------------------- 
        DomAttribute _attrElemId;
        DomAttribute _attrClass;
        //-------------------------------------------

        HtmlEventHandler _evhMouseDown;
        HtmlEventHandler _evhMouseUp;
        HtmlEventHandler _evhMouseLostFocus;

        public DomElement(WebDocument ownerDoc, int nodePrefixNameIndex, int nodeLocalNameIndex)
            : base(ownerDoc)
        {
            _nodePrefixNameIndex = nodePrefixNameIndex;
            _nodeLocalNameIndex = nodeLocalNameIndex;
            SetNodeType(HtmlNodeKind.OpenElement);
        }

        public static bool EqualNames(DomElement node1, DomElement node2)
        {
            return node1._nodeLocalNameIndex == node2._nodeLocalNameIndex
                && node1._nodePrefixNameIndex == node2._nodePrefixNameIndex;
        }
#if DEBUG
        public override string ToString()
        {
            return "e-node: " + this.LocalName;
        }
#endif
        public IEnumerable<DomAttribute> GetAttributeIterForward()
        {
            if (_myAttributes != null)
            {
                foreach (DomAttribute attr in _myAttributes.Values)
                {
                    yield return attr;
                }
            }
        }
        public IEnumerable<DomNode> GetChildNodeIterForward()
        {
            if (_myChildrenNodes != null)
            {
                int j = _myChildrenNodes.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return _myChildrenNodes[i];
                }
            }
        }

        public int ChildrenCount => (_myChildrenNodes != null) ? _myChildrenNodes.Count : 0;

        public DomNode GetChildNode(int index) => _myChildrenNodes[index];

        public virtual void SetAttribute(DomAttribute attr)
        {
            SetDomAttribute(attr);
        }
        protected void SetDomAttribute(DomAttribute attr)
        {
            if (_myAttributes == null)
            {
                _myAttributes = new Dictionary<int, DomAttribute>();
            }
            //-----------
            //some wellknownattr 
            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Id:
                    {
                        _attrElemId = attr;
                        this.OwnerDocument.RegisterElementById(this);
                    }
                    break;
                case WellknownName.Class:
                    {
                        _attrClass = attr;
                    }
                    break;
            }
            //--------------------
            int attrNameIndex = this.OwnerDocument.AddStringIfNotExists(attr.LocalName);
            _myAttributes[attrNameIndex] = attr;//update or replace 
            attr.SetParent(this);
            NotifyChange(ElementChangeKind.SetAttribute, attr);
            //---------------------
        }
        public void SetAttribute(string attrName, string value)
        {
            SetAttribute(this.OwnerDocument.CreateAttribute(attrName, value));
        }
        public void AddAttribute(DomAttribute attr)
        {
            SetAttribute(attr);
        }
        public virtual void AddChild(DomNode childNode)
        {
            switch (childNode.NodeKind)
            {
                case HtmlNodeKind.Attribute:
                    {
                        AddAttribute((DomAttribute)childNode);
                    }
                    break;
                default:
                    {
                        if (_myChildrenNodes == null)
                        {
                            _myChildrenNodes = new List<DomNode>();
                        }
                        _myChildrenNodes.Add((DomNode)childNode);
                        childNode.SetParent(this);
                        NotifyChange(ElementChangeKind.AddChild, null);
                    }
                    break;
            }
        }
        public virtual bool RemoveChild(DomNode childNode)
        {
            switch (childNode.NodeKind)
            {
                case HtmlNodeKind.Attribute:
                    {
                        //TODO: support remove attribute
                        return false;
                    }
                default:
                    {
                        if (_myChildrenNodes != null)
                        {
                            bool result = _myChildrenNodes.Remove(childNode);
                            if (result)
                            {
                                childNode.SetParent(null);
                                NotifyChange(ElementChangeKind.RemoveChild, null);
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
            if (_myChildrenNodes != null)
            {
                for (int i = _myChildrenNodes.Count - 1; i >= 0; --i)
                {
                    //clear content 
                    _myChildrenNodes[i].SetParent(null);
                }
                _myChildrenNodes.Clear();
                NotifyChange(ElementChangeKind.ClearAllChildren, null);
            }
        }

        /// <summary>
        /// when we change dom element, the change may affect some part of dom/ or entire document
        /// </summary>
        /// <param name="changeKind"></param>
        /// <param name="attr"></param>
        protected void NotifyChange(ElementChangeKind changeKind, DomAttribute attr)
        {
            switch (this.DocState)
            {
                case DocumentState.ChangedAfterIdle:
                case DocumentState.Idle:
                    //notify parent 
                    OnElementChangedInIdleState(changeKind, attr);
                    break;
            }
        }
        protected virtual void OnElementChangedInIdleState(ElementChangeKind changeKind, DomAttribute attr)
        {
        }
        //------------------------------------------
        public DomAttribute FindAttribute(int attrLocalNameIndex)
        {
            if (_myAttributes != null)
            {
                DomAttribute found;
                _myAttributes.TryGetValue(attrLocalNameIndex, out found);
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

        public int AttributeCount => (_myAttributes != null) ? _myAttributes.Count : 0;

        public string Prefix => OwnerDocument.GetString(_nodePrefixNameIndex);

        public string LocalName => OwnerDocument.GetString(_nodeLocalNameIndex);

        public int LocalNameIndex => _nodeLocalNameIndex;

        public bool HasAttributeElementId => _attrElemId != null;

        public bool HasAttributeClass => _attrClass != null;

        public string AttrClassValue
        {
            get
            {
                if (_attrClass != null)
                {
                    return _attrClass.Value;
                }
                return null;
            }
        }
        public string AttrElementId
        {
            get
            {
                if (_attrElemId != null)
                {
                    return _attrElemId.Value;
                }

                return null;
            }
        }

        public string Name => this.LocalName;

        //public object Tag { get; set; }


    }
}