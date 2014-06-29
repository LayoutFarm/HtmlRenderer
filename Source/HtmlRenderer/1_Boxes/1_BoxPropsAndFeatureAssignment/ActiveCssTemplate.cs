//BSD 2014, WinterCore
using System;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Dom
{
    class CssBoxTemplate : CssBoxBase
    {
        public CssBoxTemplate(WellknownHtmlTagName wellknownTagName)
        {
            this.WellknownTagName = wellknownTagName;
        }
       
        public override CssBoxBase GetParent()
        {
            return null;
        }
        public void InheritStylesFrom(CssBoxBase source)
        {
            base.InheritStyles(source, false);
        }
        public void CloneAllStylesFrom(CssBoxBase source)
        {
            base.InheritStyles(source, true);
        }
    }


    class ActiveCssTemplate
    {


        CssActiveSheet activeSheet;
        WebDom.Parser.CssParser miniCssParser;
        bool isCloneOnce = false;
        public ActiveCssTemplate(CssActiveSheet activeSheet)
        {
            this.activeSheet = activeSheet;
            miniCssParser = new WebDom.Parser.CssParser();

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
            CssParser.ParseStyleSheet(activeSheet, rawStyleElementContent);
        }
        public void LoadAnotherStylesheet(CssActiveSheet anotherActiveSheet)
        {
            CloneActiveCssSheetOnce();
            activeSheet.Combine(anotherActiveSheet);
        }
        //--------------------------------------------------------------------------------------------------       
        public WebDom.CssRuleSet ParseCssBlock(string className, string blockSource)
        {
            return miniCssParser.ParseCssPropertyDeclarationList(blockSource.ToCharArray());
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

        Dictionary<TemplateKey, CssBoxTemplate> templatesForTagName = new Dictionary<TemplateKey, CssBoxTemplate>();
        UniqueStringTable ustrTable = new UniqueStringTable();




        static readonly char[] _whiteSplitter = new[] { ' ' };

        internal void ApplyActiveTemplateForElement(CssBox parentBox, CssBox box)
        {

            //1. tag name key
            int tagNameKey = ustrTable.AddStringIfNotExist(box.HtmlElement.Name);

            //2. class name key
            int classNameKey = 0;
            var class_value = box.HtmlElement.TryGetAttribute("class", null);

            if (class_value != null)
            {
                classNameKey = ustrTable.AddStringIfNotExist(class_value);
            }

            //-----------------
            TemplateKey key = new TemplateKey(tagNameKey, classNameKey, parentBox.cssClassVersion);
            CssBoxTemplate boxTemplate;

            if (!templatesForTagName.TryGetValue(key, out boxTemplate))
            {

                //create template for specific key  
                boxTemplate = new CssBoxTemplate(box.WellknownTagName);
                boxTemplate.CloneAllStylesFrom(box);

                //*** 
                //----------------------------
                //1. tag name
                CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(box.HtmlElement.Name);
                if (ruleGroup != null)
                {
                    box.cssClassVersion++;
                    foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
                    {
                        BoxModelBuilder.AssignPropertyValue(boxTemplate, parentBox, decl);
                    }
                }
                //----------------------------
                //2. series of class
                if (class_value != null)
                {
                    box.cssClassVersion++;
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
                                    BoxModelBuilder.AssignPropertyValue(boxTemplate, parentBox, propDecl);
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
            }
            //***********
            box.CloneAllStyles(boxTemplate);
            //*********** 
        }
        //----------------------------------------------------------------------------
        internal void ApplyAbsoluteStyles(BridgeHtmlNode targetBox)
        {
            return;
            //1. element name 
            int tagNameKey = ustrTable.AddStringIfNotExist(targetBox.Name);  //element's name
            int classNameKey = 0;
            var class_value = targetBox.ClassName;
            if (class_value != null)
            {
                classNameKey = ustrTable.AddStringIfNotExist(class_value);
            }

            TemplateKey key = new TemplateKey(tagNameKey, classNameKey, 0);
            CssBoxTemplate boxTemplate;
            if (!templatesForTagName.TryGetValue(key, out boxTemplate))
            {
                //create box template with init value

                boxTemplate = new CssBoxTemplate(targetBox.WellknownTagName);
                CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(targetBox.Name);

            }


            //CssBoxTemplate boxTemplate;
            //BoxSpec currentSpec = targetBox.Spec;

            //if (!templatesForTagName.TryGetValue(key, out boxTemplate))
            //{
            //    //create template for specific key   
            //    boxTemplate = new CssBoxTemplate(targetBox.WellknownTagName);
            //    boxTemplate.CloneAllStylesFrom(currentSpec);
            //    //*** 
            //    //----------------------------
            //    //1. tag name
            //    CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(targetBox.Name);
            //    if (ruleGroup != null)
            //    {
            //        //notify that this spec 
            //        currentSpec.cssClassVersion++;
            //        foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
            //        {
            //            BoxModelBuilder.AssignPropertyValue(boxTemplate, parentSpec, decl);
            //        }
            //    }
            //    //----------------------------
            //    //2. series of class
            //    if (class_value != null)
            //    {
            //        currentSpec.cssClassVersion++;

            //        string[] classNames = class_value.Split(_whiteSplitter, StringSplitOptions.RemoveEmptyEntries);

            //        int j = classNames.Length;
            //        if (j > 0)
            //        {
            //            for (int i = 0; i < j; ++i)
            //            {

            //                CssRuleSetGroup ruleSetGroup = activeSheet.GetRuleSetForClassName(classNames[i]);
            //                if (ruleSetGroup != null)
            //                {
            //                    foreach (var propDecl in ruleSetGroup.GetPropertyDeclIter())
            //                    {
            //                        BoxModelBuilder.AssignPropertyValue(boxTemplate, parentSpec, propDecl);
            //                    }
            //                    //---------------------------------------------------------
            //                    //find subgroup for more specific conditions
            //                    int subgroupCount = ruleSetGroup.SubGroupCount;
            //                    for (int m = 0; m < subgroupCount; ++m)
            //                    {
            //                        //find if selector condition match with this box
            //                        CssRuleSetGroup ruleSetSubGroup = ruleSetGroup.GetSubGroup(m);
            //                        var selector = ruleSetSubGroup.OriginalSelector;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    templatesForTagName.Add(key, boxTemplate);
            //}
            ////***********
            //currentSpec.CloneAllStyles(boxTemplate);
            ////*********** 
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