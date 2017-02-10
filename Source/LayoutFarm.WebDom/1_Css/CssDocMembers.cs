//BSD, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.WebDom
{
    public enum CssDocMemberKind
    {
        RuleSet,
        Media,
        Page,
    }
    public abstract class CssDocMember
    {
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId;
#endif
        public CssDocMember()
        {
#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;
#endif
        }
        public abstract CssDocMemberKind MemberKind { get; }
    }


    public class CssRuleSet : CssDocMember
    {
        CssElementSelector elementSelector;
        List<CssPropertyDeclaration> _decls = new List<CssPropertyDeclaration>();
        public CssRuleSet()
        {
        }
#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.elementSelector.ToString());
            sb.Append('{');
            int j = _decls.Count;
            for (int i = 0; i < j; ++i)
            {
                sb.Append(_decls[i].ToString());
                if (i < j - 1)
                {
                    sb.Append(';');
                }
            }

            sb.Append('}');
            return sb.ToString();
        }
#endif
        public void PrepareExpression(CssCombinatorOperator combinator)
        {
            switch (combinator)
            {
                default:
                    {
                    }
                    break;
                case CssCombinatorOperator.AdjacentSibling:
                    {
                        throw new NotSupportedException();
                    }
                    break;
                case CssCombinatorOperator.Child:
                    {
                        throw new NotSupportedException();
                    }
                    break;
                case CssCombinatorOperator.Descendant:
                    {
                        throw new NotSupportedException();
                    }
                    break;
                case CssCombinatorOperator.GeneralSibling:
                    {
                        throw new NotSupportedException();
                    }
                    break;
                case CssCombinatorOperator.List:
                    {
                        CssCompundElementSelector combinatorExpr = new CssCompundElementSelector(combinator);
                        combinatorExpr.LeftSelector = this.elementSelector;
                        this.elementSelector = combinatorExpr;
                    }
                    break;
            }
        }
        public void AddSelector(CssSimpleElementSelector primExpr)
        {
#if DEBUG
            if (primExpr == null)
            {
            }
#endif
            if (elementSelector == null)
            {
                elementSelector = primExpr;
            }
            else
            {
                CssCompundElementSelector combinatorExpr = this.elementSelector as CssCompundElementSelector;
                if (combinatorExpr != null)
                {
                    combinatorExpr.RightSelector = primExpr;
                }
                else
                {
                    CssSimpleElementSelector currentPrimExpr = this.elementSelector as CssSimpleElementSelector;
                    if (currentPrimExpr != null)
                    {
                        combinatorExpr = new CssCompundElementSelector(CssCombinatorOperator.Descendant);
                        combinatorExpr.LeftSelector = this.elementSelector;
                        combinatorExpr.RightSelector = primExpr;
                        this.elementSelector = combinatorExpr;
                    }
                    else
                    {
                    }
                }
            }
        }
        public void AddCssCodeProperty(CssPropertyDeclaration property)
        {
            this._decls.Add(property);
        }
        public void RemoveCssProperty(WellknownCssPropertyName wellknownName)
        {
            if (wellknownName == WellknownCssPropertyName.Unknown)
            {
                //can't delete
                return;
            }
            for (int i = _decls.Count - 1; i >= 0; --i)
            {
                if (_decls[i].WellknownPropertyName == wellknownName)
                {
                    _decls.RemoveAt(i);
                }
            }
        }
        public CssElementSelector GetSelector()
        {
            return this.elementSelector;
        }
        public IEnumerable<CssPropertyDeclaration> GetAssignmentIter()
        {
            foreach (var assignment in this._decls)
            {
                yield return assignment;
            }
        }
        public override CssDocMemberKind MemberKind
        {
            get { return CssDocMemberKind.RuleSet; }
        }
    }


    public class CssAtMedia : CssDocMember
    {
        List<string> _mediaList = new List<string>();
        List<CssRuleSet> _ruleSets = new List<CssRuleSet>();
        public void AddMedia(string mediaName)
        {
            this._mediaList.Add(mediaName);
        }
        public void AddRuleSet(CssRuleSet ruleSet)
        {
            this._ruleSets.Add(ruleSet);
        }
        public override CssDocMemberKind MemberKind
        {
            get { return CssDocMemberKind.Media; }
        }
        public bool HasMediaName
        {
            get
            {
                return this._mediaList.Count > 0;
            }
        }
        public IEnumerable<string> GetMediaNameIter()
        {
            foreach (string mediaName in this._mediaList)
            {
                yield return mediaName;
            }
        }
        public IEnumerable<CssRuleSet> GetRuleSetIter()
        {
            foreach (CssRuleSet ruleSet in this._ruleSets)
            {
                yield return ruleSet;
            }
        }
    }

    public class CssAtPage : CssDocMember
    {
        public string PseudoPage;
        List<CssPropertyDeclaration> _decls = new List<CssPropertyDeclaration>();
        public void AddCssCodeProperty(CssPropertyDeclaration property)
        {
            this._decls.Add(property);
        }
        public override CssDocMemberKind MemberKind
        {
            get { return CssDocMemberKind.Page; }
        }
    }
}