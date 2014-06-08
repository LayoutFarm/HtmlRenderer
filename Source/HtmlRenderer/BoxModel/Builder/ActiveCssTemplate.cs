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


        //------------------------------------------------------------------------------------------------------



        Dictionary<string, CssBoxTemplate> templatesForTagName = new Dictionary<string, CssBoxTemplate>();

        class CssBoxTemplate : CssBoxBase
        {

            public CssBoxTemplate(string name)
            {
                this.Name = name;
            }
            public string Name
            {
                get;
                set;
            }
            public override CssBoxBase GetParent()
            {
                return null;
            }
        }



        internal CssBoxBase GetExistingOrCreatePreparedBoxTemplateForTagName(string tagName)
        {
            CssBoxTemplate found;
            if (!templatesForTagName.TryGetValue(tagName, out found))
            {
                found = new CssBoxTemplate(tagName);
                this.templatesForTagName.Add(tagName, found);
            }
            return found;
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