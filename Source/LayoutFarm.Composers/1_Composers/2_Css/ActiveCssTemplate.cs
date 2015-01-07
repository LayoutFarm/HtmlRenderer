// 2015,2014 ,BSD, WinterDev

using System;
using PixelFarm.Drawing;
using System.Collections.Generic;
using LayoutFarm.Css;
using LayoutFarm.WebDom;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.Composers
{

    class ActiveCssTemplate
    {

        CssActiveSheet activeSheet;
        bool isCloneOnce = false;
        public ActiveCssTemplate(CssActiveSheet activeSheet)
        {
            this.activeSheet = activeSheet;
        }
        public CssActiveSheet ActiveSheet
        {
            get
            {
                return this.activeSheet;
            }
            set
            {
                this.activeSheet = value;
            }
        }

        void CloneActiveCssSheetOnce()
        {
            if (!isCloneOnce)
            {
                //clone 
                activeSheet = activeSheet.Clone(new object());
                isCloneOnce = true;
            }
        }
        public void LoadRawStyleElementContent(string rawStyleElementContent)
        {
            CloneActiveCssSheetOnce();
            CssParserHelper.ParseStyleSheet(activeSheet, rawStyleElementContent);
        }
        public void LoadAnotherStylesheet(WebDom.CssActiveSheet anotherActiveSheet)
        {
            CloneActiveCssSheetOnce();
            activeSheet.Combine(anotherActiveSheet);
        }
        //--------------------------------------------------------------------------------------------------       


        struct TemplateKey
        {
            public readonly int tagNameKey;
            public readonly int classNameKey;
            public readonly int version;
            public TemplateKey(int tagNameKey, int classNameKey, int version)
            {
                this.tagNameKey = tagNameKey;
                this.classNameKey = classNameKey;
                this.version = version;
            }
        }

        Dictionary<TemplateKey, BoxSpec> templatesForTagName = new Dictionary<TemplateKey, BoxSpec>();
        UniqueStringTable ustrTable = new UniqueStringTable();




        static readonly char[] _whiteSplitter = new[] { ' ' };


        internal void ApplyActiveTemplate(string elemName, string class_value, BoxSpec currentBoxSpec, BoxSpec parentSpec)
        {

            //1. tag name key
            int tagNameKey = ustrTable.AddStringIfNotExist(elemName);

            //2. class name key
            int classNameKey = 0;
            if (class_value != null)
            {
                classNameKey = ustrTable.AddStringIfNotExist(class_value);
            }

            int parentSpecVersion = 0;

            if (parentSpec != null)
            {
                parentSpecVersion = parentSpec.VersionNumber;
            }

            TemplateKey key = new TemplateKey(tagNameKey, classNameKey, parentSpecVersion);
            BoxSpec boxTemplate;
            if (!templatesForTagName.TryGetValue(key, out boxTemplate))
            {
                //create template for specific key  
                boxTemplate = new BoxSpec();
                BoxSpec.CloneAllStyles(boxTemplate, currentBoxSpec);
                BoxSpec.SetVersionNumber(currentBoxSpec, parentSpec.VersionNumber + 1);

                //*** 
                //----------------------------
                //1. tag name
                CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(elemName);
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

                            CssRuleSetGroup ruleSetGroup = activeSheet.GetRuleSetForClassName(classNames[i]);
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
                templatesForTagName.Add(key, boxTemplate);
                boxTemplate.Freeze(); 
               
            }
            BoxSpec.CloneAllStyles(currentBoxSpec, boxTemplate);

        }


        internal void ApplyActiveTemplateForSpecificElementId(DomElement element)
        {
            var ruleset = activeSheet.GetRuleSetForId(element.AttrElementId);
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