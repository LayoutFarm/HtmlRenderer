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

    class ActiveCssTemplate
    {
        public readonly HtmlContainer htmlContainer;
        CssActiveSheet activeSheet;



        bool isCloneOnce = false;
        public ActiveCssTemplate(HtmlContainer htmlContainer, CssActiveSheet activeSheet)
        {
            this.activeSheet = activeSheet;
            this.htmlContainer = htmlContainer;
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
        public void LoadLinkStyleSheet(string href)
        {

            string stylesheet;
            CssActiveSheet stylesheetData;
            StylesheetLoadHandler.LoadStylesheet(this.htmlContainer,
                href,  //load style sheet from external ?
                out stylesheet, out stylesheetData);

            if (stylesheet != null)
            {
                CloneActiveCssSheetOnce();
                CssParser.ParseStyleSheet(activeSheet, stylesheet);
            }
            else if (stylesheetData != null)
            {
                CloneActiveCssSheetOnce();
                activeSheet.Combine(stylesheetData);
            }
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

        int dbugCount = 0;

        static readonly char[] _whiteSplitter = new[] { ' ' };

        internal void ApplyActiveTemplateForElement(CssBox parentBox, CssBox box)
        {

            //1. tag name key
            int tagNameKey = ustrTable.AddStringIfNotExist(box.HtmlTag.Name);

            //2. class name key
            int classNameKey = 0;
            var class_value = box.HtmlTag.TryGetAttribute("class", null);

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
                templatesForTagName.Add(key, boxTemplate);
                // Console.WriteLine((dbugCount++).ToString());

                //----------------------------
                //1. tag name
                CssRuleSetGroup ruleGroup = activeSheet.GetRuleSetForTagName(box.HtmlTag.Name);
                if (ruleGroup != null)
                {
                    box.cssClassVersion = 1;
                    foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
                    {
                        BoxModelBuilder.AssignPropertyValue(boxTemplate, parentBox, decl);
                    }
                }
                //----------------------------
                //2. series of class
                if (class_value != null)
                {
                    box.cssClassVersion |= (1 << 1);

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
            }

            //***********
            box.SpecialCloneStyles(boxTemplate);
            //***********

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