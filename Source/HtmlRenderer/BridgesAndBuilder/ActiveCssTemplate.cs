//BSD 2014, WinterDev

using System;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Dom
<<<<<<< HEAD
{   
=======
{

>>>>>>> 1.7.2105.1


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

        Dictionary<TemplateKey, BoxSpec> templatesForTagName = new Dictionary<TemplateKey, BoxSpec>();
        UniqueStringTable ustrTable = new UniqueStringTable();




        static readonly char[] _whiteSplitter = new[] { ' ' };

<<<<<<< HEAD
        //internal void ApplyActiveTemplateForElement(CssBox parentBox, CssBox box)
        //{

        //    //1. tag name key
        //    int tagNameKey = ustrTable.AddStringIfNotExist(box.HtmlElement.Name);

        //    //2. class name key
        //    int classNameKey = 0;
        //    var class_value = box.HtmlElement.TryGetAttribute("class", null);

        //    if (class_value != null)
        //    {
        //        classNameKey = ustrTable.AddStringIfNotExist(class_value);
        //    }

        //    //-----------------
        //    TemplateKey key = new TemplateKey(tagNameKey, classNameKey, parentBox.cssClassVersion);
        //    BoxSpec boxTemplate;

        //    if (!templatesForTagName.TryGetValue(key, out boxTemplate))
        //    {

        //        //create template for specific key  
        //        boxTemplate = new BoxSpec(box.WellknownTagName);
        //        boxTemplate.CloneAllStylesFrom(box.BoxSpec);

        //        //*** 
        //        //----------------------------
        //        //1. tag name
        //        CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(box.HtmlElement.Name);
        //        if (ruleGroup != null)
        //        {
        //            box.cssClassVersion++;
        //            foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
        //            {
        //                CssPropSetter.AssignPropertyValue(boxTemplate, parentBox.BoxSpec, decl);
        //            }
        //        }
        //        //----------------------------
        //        //2. series of class
        //        if (class_value != null)
        //        {
        //            box.cssClassVersion++;
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
        //                            CssPropSetter.AssignPropertyValue(boxTemplate, parentBox.BoxSpec, propDecl);
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
        //        templatesForTagName.Add(key, boxTemplate);
        //    }
        //    //***********
        //    box.CloneAllStyles(boxTemplate);
        //    //*********** 
        //}

        internal void ApplyActiveTemplateForElement2(BridgeHtmlElement parent, BridgeHtmlElement box)
=======
        
        internal void ApplyActiveTemplate(string elemName, string class_value, BoxSpec currentBoxSpec, BoxSpec parentSpec)
>>>>>>> 1.7.2105.1
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
<<<<<<< HEAD
            if (parent != null)
            {
                parentSpec = parent.Spec;
                parentSpecVersion = parentSpec.cssClassVersion;
            }
            TemplateKey key = new TemplateKey(tagNameKey, classNameKey, parentSpecVersion);
            //------------------------
            BoxSpec currentBoxSpec = box.Spec;
            if (currentBoxSpec == null)
            {
                box.Spec = currentBoxSpec = new BoxSpec();
            }
            //------------------------ 

             
=======

            if (parentSpec != null)
            {
                parentSpecVersion = parentSpec.VersionNumber;
            }
            TemplateKey key = new TemplateKey(tagNameKey, classNameKey, parentSpecVersion);
>>>>>>> 1.7.2105.1
            BoxSpec boxTemplate;
            if (!templatesForTagName.TryGetValue(key, out boxTemplate))
            {
                //create template for specific key  
<<<<<<< HEAD
                boxTemplate = new BoxSpec();
=======
                boxTemplate = new BoxSpec(currentBoxSpec.WellknownTagName);
>>>>>>> 1.7.2105.1
                boxTemplate.CloneAllStylesFrom(currentBoxSpec);

                //*** 
                //----------------------------
                //1. tag name
                CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(elemName);
                if (ruleGroup != null)
                {
                    currentBoxSpec.VersionNumber++;
                    foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
                    {
                        CssPropSetter.AssignPropertyValue(boxTemplate, parentSpec, decl);
                    }
                }
                //----------------------------
                //2. series of class
                if (class_value != null)
                {
                    currentBoxSpec.VersionNumber++;
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
                                    CssPropSetter.AssignPropertyValue(boxTemplate, parentSpec, propDecl);
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
            //***********
            currentBoxSpec.CloneAllStylesFrom(boxTemplate);
            //*********** 
            
        }
<<<<<<< HEAD


=======
>>>>>>> 1.7.2105.1
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