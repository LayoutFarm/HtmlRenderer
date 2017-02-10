//BSD, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.WebDom
{
    public abstract class CssElementSelector
    {
        public CssElementSelector Parent { get; set; }
        public abstract string SelectorSignature { get; }
        public abstract bool IsSimpleSelector { get; }
        public abstract bool IsSameNameAndType(CssElementSelector anotherSelector);
    }

    public enum SimpleElementSelectorKind
    {
        TagName,
        ClassName,
        PseudoClass,
        Id,
        All,
        Extend
    }


    /// <summary>
    /// primitive selector expression
    /// </summary>
    public class CssSimpleElementSelector : CssElementSelector
    {
        string _name = "";
        List<CssAttributeSelectorExpression> attrs;
        public SimpleElementSelectorKind selectorType;
        public CssSimpleElementSelector()
        {
        }
        public CssSimpleElementSelector(SimpleElementSelectorKind selectorType)
        {
            this.selectorType = selectorType;
        }
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
        public override string SelectorSignature
        {
            get
            {
                switch (selectorType)
                {
                    case SimpleElementSelectorKind.ClassName:
                        return "." + Name;
                    case SimpleElementSelectorKind.Id:
                        return "#" + Name;
                    case SimpleElementSelectorKind.All:
                        return "*";
                    case SimpleElementSelectorKind.Extend:
                        return "::" + Name;
                    case SimpleElementSelectorKind.PseudoClass:
                        return ":" + Name;
                    default:
                        return Name;
                }
            }
        }

        public override bool IsSimpleSelector
        {
            get { return true; }
        }
        public override string ToString()
        {
            return SelectorSignature;
        }
        public void AddAttribute(CssAttributeSelectorExpression attr)
        {
            if (attrs == null)
            {
                attrs = new List<CssAttributeSelectorExpression>();
            }
            this.attrs.Add(attr);
        }

        public override bool IsSameNameAndType(CssElementSelector anotherSelector)
        {
            if (anotherSelector == null)
            {
                return false;
            }
            if (anotherSelector == this)
            {
                return true;
            }
            //------------------------------
            CssSimpleElementSelector another = anotherSelector as CssSimpleElementSelector;
            return another != null && (another.Name == this.Name) &&
                (another.selectorType == this.selectorType);
        }
        public static bool IsCompatible(CssSimpleElementSelector sel1, CssSimpleElementSelector sel2)
        {
            //walk top of both

            var parOrSelf1 = GetTopParentOrSelf(sel1);
            var parOrSelf2 = GetTopParentOrSelf(sel2);
            return parOrSelf1.IsSameNameAndType(parOrSelf2);
        }
        static CssElementSelector GetTopParentOrSelf(CssSimpleElementSelector selector)
        {
            var parent = selector.Parent;
            if (parent == null)
            {
                return selector;
            }
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }
            return parent;
        }
    }


    /// <summary>
    /// css combinator expression
    /// </summary>
    public class CssCompundElementSelector : CssElementSelector
    {
        CssElementSelector _left;
        CssElementSelector _right;
        public CssCompundElementSelector(CssCombinatorOperator opname)
        {
            this.OperatorName = opname;
        }

        public CssCombinatorOperator OperatorName { get; private set; }

        public CssElementSelector LeftSelector
        {
            get { return this._left; }
            set
            {
                this._left = value;
                if (this.OperatorName != CssCombinatorOperator.List)
                {
                    value.Parent = this;
                }
            }
        }
        public CssElementSelector RightSelector
        {
            get { return this._right; }
            set
            {
                this._right = value;
                if (this.OperatorName != CssCombinatorOperator.List)
                {
                    value.Parent = this;
                }
            }
        }

        public override bool IsSimpleSelector
        {
            get { return false; }
        }
        public override string SelectorSignature
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(LeftSelector.SelectorSignature);
                switch (OperatorName)
                {
                    case CssCombinatorOperator.Descendant:
                        {
                            sb.Append(' ');
                        }
                        break;
                    case CssCombinatorOperator.AdjacentSibling:
                        {
                            sb.Append('+');
                        }
                        break;
                    case CssCombinatorOperator.Child:
                        {
                            sb.Append('>');
                        }
                        break;
                    case CssCombinatorOperator.GeneralSibling:
                        {
                            sb.Append('~');
                        }
                        break;
                    case CssCombinatorOperator.List:
                        {
                            sb.Append(',');
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }

                sb.Append(RightSelector.SelectorSignature);
                return sb.ToString();
            }
        }
        public override string ToString()
        {
            return this.SelectorSignature;
        }
        public override bool IsSameNameAndType(CssElementSelector anotherSelector)
        {
            if (anotherSelector == null)
            {
                return false;
            }
            if (anotherSelector == this)
            {
                return true;
            }
            //------------------------------
            CssCompundElementSelector another = anotherSelector as CssCompundElementSelector;
            if (another != null)
            {
                return this.OperatorName == another.OperatorName &&
                    this.LeftSelector.IsSameNameAndType(another.LeftSelector) &&
                    this.RightSelector.IsSameNameAndType(another.RightSelector);
            }
            return false;
        }
    }



    public enum CssCombinatorOperator
    {
        //comma separate list
        List,
        /// <summary>
        /// Adjacent op Add
        /// </summary>
        AdjacentSibling, //+
        /// <summary>
        /// Child op GT
        /// </summary>
        Child, //>
        /// <summary>
        /// simple space
        /// </summary>
        Descendant,
        /// <summary>
        /// sibling operator tile 
        /// </summary> 
        GeneralSibling, //  ~
    }
}