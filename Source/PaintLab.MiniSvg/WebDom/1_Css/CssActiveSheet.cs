//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
namespace LayoutFarm.WebDom
{
    public class CssActiveSheet
    {
        //major groups...  
        Dictionary<string, CssRuleSetGroup> rulesForTagName = new Dictionary<string, CssRuleSetGroup>();
        Dictionary<string, CssRuleSetGroup> rulesForClassName = new Dictionary<string, CssRuleSetGroup>();
        Dictionary<string, CssRuleSetGroup> rulesForElementId = new Dictionary<string, CssRuleSetGroup>();
        Dictionary<string, CssRuleSetGroup> rulesForPsedoClass = new Dictionary<string, CssRuleSetGroup>();
        Dictionary<string, CssRuleSetGroup> rulesForAll = new Dictionary<string, CssRuleSetGroup>();
#if DEBUG
        CssActiveSheet dbugOriginal;
#endif

        public CssActiveSheet()
        {
        }
        public void LoadCssDoc(WebDom.CssDocument cssDoc)
        {
            foreach (WebDom.CssDocMember cssDocMember in cssDoc.GetCssDocMemberIter())
            {
                switch (cssDocMember.MemberKind)
                {
                    case WebDom.CssDocMemberKind.RuleSet:
                        this.AddRuleSet((WebDom.CssRuleSet)cssDocMember);
                        break;
                    case WebDom.CssDocMemberKind.Media:
                        this.AddMedia((WebDom.CssAtMedia)cssDocMember);
                        break;
                    default:
                    case WebDom.CssDocMemberKind.Page:
                        throw new NotSupportedException();
                }
            }
        }

        void AddRuleSet(WebDom.CssRuleSet ruleset)
        {
            List<CssRuleSetGroup> relatedRuleSets = new List<CssRuleSetGroup>();
            ExpandSelector(relatedRuleSets, ruleset.GetSelector());
            CssPropertyAssignmentCollection assignmentCollection = new CssPropertyAssignmentCollection(null);
            assignmentCollection.LoadRuleSet(ruleset);
            foreach (var ruleSetGroup in relatedRuleSets)
            {
                //start with share*** rule set

                ruleSetGroup.AddRuleSet(assignmentCollection);
            }
        }


        static CssRuleSetGroup GetGroupOrCreateIfNotExists(Dictionary<string, CssRuleSetGroup> dic,
            WebDom.CssSimpleElementSelector simpleSelector)
        {
            CssRuleSetGroup ruleSetGroup;
            if (!dic.TryGetValue(simpleSelector.Name, out ruleSetGroup))
            {
                ruleSetGroup = new CssRuleSetGroup(simpleSelector.Name);
                dic.Add(simpleSelector.Name, ruleSetGroup);
            }
            //-------------
            if (simpleSelector.Parent != null)
            {
                //get or create subgroup
                return ruleSetGroup.GetOrCreateSubgroup(simpleSelector);
            }
            //-------------  
            return ruleSetGroup;
        }

        void ExpandSelector(List<CssRuleSetGroup> relatedRuleSets, WebDom.CssElementSelector selector)
        {
            //recursive
            //create active element set                
            if (selector.IsSimpleSelector)
            {
                WebDom.CssSimpleElementSelector simpleSelector = (WebDom.CssSimpleElementSelector)selector;
                switch (simpleSelector.selectorType)
                {
                    default:
                        {
                            throw new NotSupportedException();
                        }
                    case WebDom.SimpleElementSelectorKind.ClassName:
                        {
                            //any element with specific class name 
                            relatedRuleSets.Add(
                                GetGroupOrCreateIfNotExists(
                                    rulesForClassName,
                                    simpleSelector));
                        }
                        break;
                    case WebDom.SimpleElementSelectorKind.Extend:
                        {
                        }
                        break;
                    case WebDom.SimpleElementSelectorKind.Id:
                        {
                            //element with id                              
                            relatedRuleSets.Add(
                                GetGroupOrCreateIfNotExists(
                                    rulesForElementId,
                                    simpleSelector));
                        }
                        break;
                    case WebDom.SimpleElementSelectorKind.PseudoClass:
                        {
                            relatedRuleSets.Add(
                               GetGroupOrCreateIfNotExists(
                                   rulesForPsedoClass,
                                   simpleSelector));
                        }
                        break;
                    case WebDom.SimpleElementSelectorKind.TagName:
                        {
                            relatedRuleSets.Add(
                                GetGroupOrCreateIfNotExists(
                                    rulesForTagName,
                                    simpleSelector));
                        }
                        break;
                    case WebDom.SimpleElementSelectorKind.All:
                        {
                            relatedRuleSets.Add(
                                GetGroupOrCreateIfNotExists(
                                    rulesForAll,
                                    simpleSelector));
                        }
                        break;
                }
            }
            else
            {
                WebDom.CssCompundElementSelector combiSelector = (WebDom.CssCompundElementSelector)selector;
                switch (combiSelector.OperatorName)
                {
                    case WebDom.CssCombinatorOperator.List:
                        {
                            ExpandSelector(relatedRuleSets, combiSelector.LeftSelector);
                            ExpandSelector(relatedRuleSets, combiSelector.RightSelector);
                        }
                        break;
                    case WebDom.CssCombinatorOperator.Descendant:
                        {
                            //focus on right side?
                            ExpandSelector(relatedRuleSets, combiSelector.RightSelector);
                        }
                        break;
                    default:
                        {
                            throw new NotSupportedException();
                        }
                }
            }
            //-----------------------------------------------------------------------------
        }

        void AddMedia(WebDom.CssAtMedia atMedia)
        {
            if (!atMedia.HasMediaName)
            {
                //global media
                foreach (WebDom.CssRuleSet ruleSet in atMedia.GetRuleSetIter())
                {
                    this.AddRuleSet(ruleSet);
                }
            }
            else
            {
                //has media name
            }
        }

        public CssRuleSetGroup GetRuleSetForTagName(string tagName)
        {
            CssRuleSetGroup found;
            rulesForTagName.TryGetValue(tagName, out found);
            return found;
        }
        public CssRuleSetGroup GetRuleSetForClassName(string className)
        {
            CssRuleSetGroup found;
            this.rulesForClassName.TryGetValue(className, out found);
            return found;
        }
        public CssRuleSetGroup GetRuleSetForId(string elementId)
        {
            CssRuleSetGroup found;
            this.rulesForElementId.TryGetValue(elementId, out found);
            return found;
        }
        public CssActiveSheet Clone()
        {
            CssActiveSheet newclone = new CssActiveSheet();
            newclone.rulesForTagName = CloneNew(this.rulesForTagName);
            newclone.rulesForClassName = CloneNew(this.rulesForClassName);
            newclone.rulesForElementId = CloneNew(this.rulesForElementId);
            newclone.rulesForPsedoClass = CloneNew(this.rulesForPsedoClass);
            newclone.rulesForAll = CloneNew(this.rulesForAll);
#if DEBUG
            newclone.dbugOriginal = this;
#endif
            return newclone;
        }

        /// <summary>
        ///  consume 
        /// </summary>
        /// <param name="another"></param>
        public void Combine(CssActiveSheet another)
        {
            MergeContent(this.rulesForClassName, another.rulesForClassName);
            MergeContent(this.rulesForAll, another.rulesForAll);
            MergeContent(this.rulesForElementId, another.rulesForElementId);
            MergeContent(this.rulesForPsedoClass, another.rulesForPsedoClass);
            MergeContent(this.rulesForTagName, another.rulesForTagName);
        }
        static Dictionary<string, CssRuleSetGroup> CloneNew(Dictionary<string, CssRuleSetGroup> source)
        {
            Dictionary<string, CssRuleSetGroup> newdic = new Dictionary<string, CssRuleSetGroup>();
            foreach (var kp in source)
            {
                newdic[kp.Key] = kp.Value.Clone();
            }
            return newdic;
        }

        static void MergeContent(Dictionary<string, CssRuleSetGroup> a, Dictionary<string, CssRuleSetGroup> b)
        {
            foreach (CssRuleSetGroup b_ruleSet in b.Values)
            {
                CssRuleSetGroup a_ruleset;
                if (!a.TryGetValue(b_ruleSet.Name, out a_ruleset))
                {
                    //not found
                    a.Add(b_ruleSet.Name, b_ruleSet);
                }
                else
                {
                    //if found then merge
                    a_ruleset.Merge(b_ruleSet);
                }
            }
        }
#if DEBUG
        public bool dbugIsClone
        {
            get
            {
                return this.dbugOriginal != null;
            }
        }
#endif


#if DEBUG
        static int dbugTotalId = 0;
        public readonly int dbugId = dbugTotalId++;
#endif
    }

    /// <summary>
    /// ruleset and its subgroups
    /// </summary>
    public class CssRuleSetGroup
    {
        CssPropertyAssignmentCollection _assignments;
        WebDom.CssSimpleElementSelector _originalSelector;
        CssRuleSetGroup _parent;
        List<CssRuleSetGroup> _subGroups;
#if DEBUG
        static int dbugTotalId = 0;
        public readonly int dbugId = dbugTotalId++;
#endif

        public CssRuleSetGroup(string name)
        {
            //if (dbugId == 170)
            //{
            //}
            this.Name = name;
        }
        private CssRuleSetGroup(CssRuleSetGroup parent, string name, WebDom.CssSimpleElementSelector simpleSelector)
        {
            //if (dbugId == 170)
            //{
            //}
            this.Name = name;
            this._parent = parent;
            this._originalSelector = simpleSelector;
        }
        public CssRuleSetGroup GetOrCreateSubgroup(WebDom.CssSimpleElementSelector simpleSelector)
        {
            if (_subGroups == null)
            {
                _subGroups = new List<CssRuleSetGroup>();
            }
            int j = _subGroups.Count;
            for (int i = 0; i < j; ++i)
            {
                //find sub group for specific selector 
                WebDom.CssSimpleElementSelector selector = _subGroups[i]._originalSelector;
                if (WebDom.CssSimpleElementSelector.IsCompatible(selector, simpleSelector))
                {
                    //found
                    return _subGroups[i];
                }
            }
            //if not found then create new one
            CssRuleSetGroup newSubgroup = new CssRuleSetGroup(this, this.Name, simpleSelector);
            this._subGroups.Add(newSubgroup);
            return newSubgroup;
        }
        public string Name
        {
            get;
            private set;
        }
        public void AddRuleSet(CssPropertyAssignmentCollection otherAssignments)
        {
            //assignment in this ruleset    
            //if (dbugId == 170)
            //{
            //}
            if (this._assignments == null)
            {
                //share
                this._assignments = otherAssignments;
            }
            else if (this._assignments != otherAssignments)
            {
                //then copy each css property assignment 
                //from other Assignment and add to this assignment
                if (this._assignments.OriginalOwner != this)
                {
                    this._assignments = this._assignments.Clone(this);
                }
                this._assignments.MergeProperties(otherAssignments);
            }
            else
            {
            }
        }

        public CssRuleSetGroup Clone()
        {
            CssRuleSetGroup newGroup = new CssRuleSetGroup(this.Name);
            newGroup._originalSelector = this._originalSelector;
            if (this._assignments != null)
            {
                newGroup._assignments = this._assignments.Clone(newGroup);
            }
            if (this._subGroups != null)
            {
                foreach (var subgroup in this._subGroups)
                {
                    var subclone = subgroup.Clone();
                    subclone._parent = newGroup;
                }
            }
            return newGroup;
        }

        public IEnumerable<WebDom.CssPropertyDeclaration> GetPropertyDeclIter()
        {
            if (_assignments != null)
            {
                var decls = this._assignments.GetDeclarations();
                foreach (var decl in decls.Values)
                {
                    yield return decl;
                }
            }
        }
        public WebDom.CssPropertyDeclaration GetPropertyDeclaration(WebDom.WellknownCssPropertyName wellknownPropName)
        {
            if (_assignments != null)
            {
                WebDom.CssPropertyDeclaration decl;
                _assignments.GetDeclarations().TryGetValue(wellknownPropName, out decl);
                return decl;
            }
            return null;
        }
        public void Merge(CssRuleSetGroup another)
        {
            //merge 
            //------------  
            if (another._assignments != null)
            {
                if (this._assignments == null)
                {
                    this._assignments = new CssPropertyAssignmentCollection(this);
                }
                //merge decl 
                this._assignments.MergeProperties(another._assignments);
            }

            //find subgroup
            if (another._subGroups != null)
            {
                if (this._subGroups == null)
                {
                    this._subGroups = new List<CssRuleSetGroup>();
                }
                foreach (CssRuleSetGroup ruleSetGroup in another._subGroups)
                {
                    //merge to this group
                    CssRuleSetGroup exiting = GetOrCreateSubgroup(ruleSetGroup._originalSelector);
                    exiting.Merge(ruleSetGroup);
                }
            }
        }
        public int SubGroupCount
        {
            get
            {
                if (this._subGroups == null)
                {
                    return 0;
                }
                return this._subGroups.Count;
            }
        }
        public CssRuleSetGroup GetSubGroup(int index)
        {
            return this._subGroups[index];
        }
        public WebDom.CssSimpleElementSelector OriginalSelector
        {
            get
            {
                return this._originalSelector;
            }
        }
    }


    public class CssPropertyAssignmentCollection
    {
        Dictionary<LayoutFarm.WebDom.WellknownCssPropertyName, WebDom.CssPropertyDeclaration> _myAssignments = new Dictionary<WebDom.WellknownCssPropertyName, WebDom.CssPropertyDeclaration>();
        object owner;
#if DEBUG
        static int s_dbugTotalId = 0;
        public static readonly int _dbugId = s_dbugTotalId++;
#endif
        public CssPropertyAssignmentCollection(object owner)
        {
            this.owner = owner;
        }
        internal void LoadRuleSet(CssRuleSet ruleSet)
        {
            foreach (CssPropertyDeclaration otherAssignment in ruleSet.GetAssignmentIter())
            {
                if (otherAssignment.WellknownPropertyName == WebDom.WellknownCssPropertyName.Unknown)
                {
                    continue;
                }
                _myAssignments[otherAssignment.WellknownPropertyName] = otherAssignment;
            }
        }


        public object OriginalOwner
        {
            get
            {
                return this.owner;
            }
        }
        public CssPropertyAssignmentCollection Clone(object newOwner)
        {
            CssPropertyAssignmentCollection newclone = new CssPropertyAssignmentCollection(newOwner);
            Dictionary<WellknownCssPropertyName, WebDom.CssPropertyDeclaration> newCloneDic = newclone._myAssignments;
            foreach (var kp in this._myAssignments)
            {
                newCloneDic.Add(kp.Key, kp.Value);
            }
            return newclone;
        }
        public void MergeProperties(CssPropertyAssignmentCollection sourceCollection)
        {
            Dictionary<WellknownCssPropertyName, CssPropertyDeclaration> fromDic = sourceCollection._myAssignments;
            Dictionary<WellknownCssPropertyName, CssPropertyDeclaration> targetDic = this._myAssignments;
            foreach (CssPropertyDeclaration sourceAssignment in fromDic.Values)
            {
                //add or replace
                targetDic[sourceAssignment.WellknownPropertyName] = sourceAssignment;
            }
        }

        internal Dictionary<WellknownCssPropertyName, CssPropertyDeclaration> GetDeclarations()
        {
            return this._myAssignments;
        }
    }
}
