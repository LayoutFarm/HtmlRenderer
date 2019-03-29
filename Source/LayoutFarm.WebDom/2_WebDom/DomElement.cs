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
        DomAttribute _attrStyle;
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
            if (_attrElemId != null) yield return _attrElemId;
            if (_attrStyle != null) yield return _attrStyle;
            if (_attrClass != null) yield return _attrClass;

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
                case WellknownName.Style:
                    {
                        _attrStyle = attr;
                    }
                    break;
                default:
                    {
                        if (_myAttributes == null)
                        {
                            _myAttributes = new Dictionary<int, DomAttribute>();
                        }
                        //--------------------
                        int attrNameIndex = this.OwnerDocument.AddStringIfNotExists(attr.LocalName);
                        _myAttributes[attrNameIndex] = attr;//update or replace 
                    }
                    break;
            }
            attr.SetParent(this);
            NotifyChange(ElementChangeKind.SetAttribute, attr);
            //---------------------
        }
        public void SetAttribute(string attrName, string value)
        {
            SetAttribute(this.OwnerDocument.CreateAttribute(attrName, value));
        }

        public virtual void AddChild(DomNode childNode)
        {
            switch (childNode.NodeKind)
            {
                case HtmlNodeKind.Attribute:
                    {
                        SetAttribute((DomAttribute)childNode);
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
            if (_attrElemId != null && attrLocalNameIndex == _attrElemId.LocalNameIndex) return _attrElemId;
            if (_attrStyle != null && attrLocalNameIndex == _attrStyle.LocalNameIndex) return _attrStyle;
            if (_attrClass != null && attrLocalNameIndex == _attrClass.LocalNameIndex) return _attrClass;

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


        public bool HasSomeAttribute => _attrStyle != null || _attrClass != null || _attrElemId != null || _myAttributes != null;

        public int AttributeCount
        {
            get
            {
                int count = 0;
                if (_attrElemId != null) count++;
                if (_attrStyle != null) count++;
                if (_attrClass != null) count++;
                if (_myAttributes != null) count += _myAttributes.Count;

                return count;
            }
        }


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