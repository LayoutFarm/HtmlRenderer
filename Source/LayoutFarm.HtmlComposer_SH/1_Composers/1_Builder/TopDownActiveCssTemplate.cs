//BSD, 2014-present, WinterDev 

using System;
using System.Collections.Generic;
using LayoutFarm.Css;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{
    //for RenderTreeBuilder ***
    //--------------------------------------------------------------------------------------------------       
    struct CssTemplateKey
    {
        public readonly int tagNameKey;
        public readonly int classNameKey;
        public CssTemplateKey(int tagNameKey, int classNameKey)
        {
            this.tagNameKey = tagNameKey;
            this.classNameKey = classNameKey;
        }
#if DEBUG
        public override string ToString()
        {
            return "t:" + this.tagNameKey + ",c:" + this.classNameKey;
        }
#endif
    }

    class TopDownActiveCssTemplate
    {
        CssActiveSheet _activeSheet;
        bool _isCloneOnce = false;
        UniqueStringTable _ustrTable = new UniqueStringTable();
        List<BoxSpecLevel> _specLevels = new List<BoxSpecLevel>();
        int _currentSpecLevel = 0;
        public TopDownActiveCssTemplate(CssActiveSheet activeSheet)
        {
            _activeSheet = activeSheet;
            _specLevels.Add(new BoxSpecLevel(0));
        }
        public void EnterLevel()
        {
            _currentSpecLevel++;
            _specLevels.Add(new BoxSpecLevel(_currentSpecLevel));
        }
        public void ExitLevel()
        {
            _currentSpecLevel--;
            _specLevels.RemoveAt(_specLevels.Count - 1);//remove last level
        }
        public CssActiveSheet ActiveSheet
        {
            get => _activeSheet;

            set => _activeSheet = value;

        }
        public void ClearCacheContent()
        {
            _specLevels.Clear();
            _currentSpecLevel = 0;
            _specLevels.Add(new BoxSpecLevel(0));
            _ustrTable = new UniqueStringTable();
        }
        void CloneActiveCssSheetOnce()
        {
            if (!_isCloneOnce)
            {
                //clone 
                _activeSheet = _activeSheet.Clone();
                _isCloneOnce = true;
            }
        }
        public void LoadRawStyleElementContent(string rawStyleElementContent)
        {
            CloneActiveCssSheetOnce();
            LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(_activeSheet, rawStyleElementContent);
        }
        public void LoadAnotherStylesheet(WebDom.CssActiveSheet anotherActiveSheet)
        {
            CloneActiveCssSheetOnce();
            _activeSheet.Combine(anotherActiveSheet);
        }


        class BoxSpecLevel
        {
            readonly int _level;
            Dictionary<CssTemplateKey, BoxSpec> _specCollections;
            public BoxSpecLevel(int level)
            {
                _level = level;
            }
            public void AddBoxSpec(CssTemplateKey key, BoxSpec spec)
            {
                //add box spec at this level
                if (_specCollections == null)
                {
                    _specCollections = new Dictionary<CssTemplateKey, BoxSpec>();
                }
                //add or replace if exists
                _specCollections[key] = spec;
            }
            public BoxSpec SearchUp(CssTemplateKey key)
            {
                //recursive search up
                BoxSpec found = null;
                if (_specCollections != null)
                {
                    if (_specCollections.TryGetValue(key, out found))
                    {
                        return found;
                    }
                }
                return null;
            }
        }
        //-----------------------------------------------------------------------------------------------

        static readonly char[] _whiteSplitter = new[] { ' ' };
        void CacheBoxSpec(CssTemplateKey key, BoxSpec spec)
        {
            //add at last(top) level
            _specLevels[_specLevels.Count - 1].AddBoxSpec(key, spec);
        }
        BoxSpec SearchUpBoxSpec(CssTemplateKey templateKey)
        {
            //bottom up 
            for (int i = _specLevels.Count - 1; i >= 0; --i)
            {
                BoxSpecLevel boxSpecLevel = _specLevels[i];
                BoxSpec found = boxSpecLevel.SearchUp(templateKey);
                if (found != null)
                {
                    return found;
                }
            }
            //not found
            return null;
        }

        internal void ApplyCacheTemplate(string elemName,
             string class_value,
             BoxSpec currentBoxSpec,
             BoxSpec parentSpec)
        {
            //1. tag name key
            int tagNameKey = _ustrTable.AddStringIfNotExist(elemName);
            //2. class name key
            int classNameKey = 0;
            if (class_value != null)
            {
                classNameKey = _ustrTable.AddStringIfNotExist(class_value);
            }
            //find cache in the same level
            var templateKey = new CssTemplateKey(tagNameKey, classNameKey);
            //BoxSpec boxTemplate = SearchUpBoxSpec(templateKey);
            BoxSpec boxTemplate = null;
            if (boxTemplate != null)
            {
                BoxSpec.CloneAllStyles(currentBoxSpec, boxTemplate);
            }
            else
            {
                //create template for specific key  
                boxTemplate = new BoxSpec();
                //copy current spec to boxTemplate
                BoxSpec.CloneAllStyles(boxTemplate, currentBoxSpec);
                //*** 
                //----------------------------
                //1. tag name
                CssRuleSetGroup ruleGroup = _activeSheet.GetRuleSetForTagName(elemName);
                if (ruleGroup != null)
                {
                    //currentBoxSpec.VersionNumber++;
                    foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
                    {
                        SpecSetter.AssignPropertyValue(boxTemplate, parentSpec, decl);
                    }
                }
                //----------------------------
                //2. series of class
                if (class_value != null)
                {
                    //currentBoxSpec.VersionNumber++;
                    string[] classNames = class_value.Split(_whiteSplitter, StringSplitOptions.RemoveEmptyEntries);
                    int j = classNames.Length;
                    if (j > 0)
                    {
                        for (int i = 0; i < j; ++i)
                        {
                            CssRuleSetGroup ruleSetGroup = _activeSheet.GetRuleSetForClassName(classNames[i]);
                            if (ruleSetGroup != null)
                            {
                                foreach (var propDecl in ruleSetGroup.GetPropertyDeclIter())
                                {
                                    SpecSetter.AssignPropertyValue(boxTemplate, parentSpec, propDecl);
                                }
                                //---------------------------------------------------------
                                //find subgroup for more specific conditions
                                int subgroupCount = ruleSetGroup.SubGroupCount;
                                for (int m = 0; m < subgroupCount; ++m)
                                {
                                    //find if selector condition match with this box
                                    CssRuleSetGroup ruleSetSubGroup = ruleSetGroup.GetSubGroup(m);
                                    var selector = ruleSetSubGroup.OriginalSelector;
                                }
                            }
                        }
                    }
                }

                boxTemplate.Freeze();
                //this.CacheBoxSpec(templateKey, boxTemplate); 
                boxTemplate.IsTemplate = true;
                //copy back from template to currentBoxSpec
                BoxSpec.CloneAllStyles(currentBoxSpec, boxTemplate);
            }
        }


        //internal void oldApplyActiveTemplate(string elemName, string class_value, BoxSpec currentBoxSpec, BoxSpec parentSpec)
        //{

        //    //1. tag name key
        //    int tagNameKey = ustrTable.AddStringIfNotExist(elemName);
        //    //2. class name key
        //    int classNameKey = 0;
        //    if (class_value != null)
        //    {
        //        classNameKey = ustrTable.AddStringIfNotExist(class_value);
        //    } 
        //    var templateKey = new TemplateKey2(tagNameKey, classNameKey);
        //    BoxSpec boxTemplate = SearchUpBoxSpec(templateKey);
        //    if (boxTemplate == null)
        //    {
        //        //create template for specific key  
        //        boxTemplate = new BoxSpec();
        //        //if (boxTemplate.__aa_dbugId == 30)
        //        //{
        //        //} 
        //        //copy current spec to boxTemplate
        //        BoxSpec.CloneAllStyles(boxTemplate, currentBoxSpec); 
        //        //*** 
        //        //----------------------------
        //        //1. tag name
        //        CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(elemName);
        //        if (ruleGroup != null)
        //        {
        //            //currentBoxSpec.VersionNumber++;
        //            foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
        //            {
        //                SpecSetter.AssignPropertyValue(boxTemplate, parentSpec, decl);
        //            }
        //        }
        //        //----------------------------
        //        //2. series of class
        //        if (class_value != null)
        //        {
        //            //currentBoxSpec.VersionNumber++;
        //            string[] classNames = class_value.Split(_whiteSplitter, StringSplitOptions.RemoveEmptyEntries);
        //            int j = classNames.Length;
        //            if (j > 0)
        //            {
        //                for (int i = 0; i < j; ++i)
        //                {

        //                    CssRuleSetGroup ruleSetGroup = activeSheet.GetRuleSetForClassName(classNames[i]);
        //                    if (ruleSetGroup != null)
        //                    {
        //                        foreach (var propDecl in ruleSetGroup.GetPropertyDeclIter())
        //                        {
        //                            SpecSetter.AssignPropertyValue(boxTemplate, parentSpec, propDecl);
        //                        }
        //                        //---------------------------------------------------------
        //                        //find subgroup for more specific conditions
        //                        int subgroupCount = ruleSetGroup.SubGroupCount;
        //                        for (int m = 0; m < subgroupCount; ++m)
        //                        {
        //                            //find if selector condition match with this box
        //                            CssRuleSetGroup ruleSetSubGroup = ruleSetGroup.GetSubGroup(m);
        //                            var selector = ruleSetSubGroup.OriginalSelector;
        //                        }
        //                    }
        //                }
        //            }
        //        } 
        //        if (!currentBoxSpec.DoNotCache)
        //        {
        //            CacheBoxSpec(templateKey, boxTemplate);
        //        }
        //        boxTemplate.Freeze();
        //    }

        //    BoxSpec.CloneAllStyles(currentBoxSpec, boxTemplate);
        //}


        internal void ApplyActiveTemplateForSpecificElementId(DomElement element)
        {
            var ruleset = _activeSheet.GetRuleSetForId(element.AttrElementId);
            if (ruleset != null)
            {
                //TODO:  implement this
                throw new NotSupportedException();
            }
        }



        enum AssignPropertySource
        {
            Inherit,
            TagName,
            ClassName,
            StyleAttribute,
            HtmlAttribute,
            Id,
        }
    }
}