//BSD 2014, WinterCore

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
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;

using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;
using HtmlRenderer.Parse;

using HtmlRenderer.WebDom;


namespace HtmlRenderer.Dom
{
    partial class BoxModelBuilder
    {
        static void ApplyStyleSheet01(CssBox element, ActiveCssTemplate activeCssTemplate)
        {


#if DEBUG
            dbugPropCheckReport rep = new dbugPropCheckReport();
#endif

            BoxSpec currentElementSpec = element.Spec;
            if (element.ParentBox != null && element.ParentBox.Spec != null)
            {
                currentElementSpec.InheritStylesFrom(element.ParentBox.Spec);
            }
            else
            {
                //only for root 
            }

            if (element.HtmlElement != null)
            {
                //------------------------------------------------------------------- 
                //1. element tag
                //2. css class 
                // try assign style using the html element tag    
                activeCssTemplate.ApplyActiveTemplate(element.ParentBox.Spec, element);
                //3.
                // try assign style using the "id" attribute of the html element
                if (element.HtmlElement.HasAttribute("id"))
                {
                    throw new NotSupportedException();
                    // var id = element.HtmlElement.TryGetAttribute("id");
                    // AssignStylesForElementId(element, activeCssTemplate, "#" + id);
                }
                //-------------------------------------------------------------------
                //4. 
                //element attribute
                AssignStylesFromTranslatedAttributesHTML5(element, activeCssTemplate);
                //AssignStylesFromTranslatedAttributes_Old(box, activeCssTemplate);
                //------------------------------------------------------------------- 
                //5.
                //style attribute value of element
                if (element.HtmlElement.HasAttribute("style"))
                {
                    var ruleset = activeCssTemplate.ParseCssBlock(element.HtmlElement.Name, element.HtmlElement.TryGetAttribute("style"));
                    foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                    {                        
                        CssPropSetter.AssignPropertyValue(
                            element.Spec,
                            element.ParentBox.Spec,
                            propDecl);
                    }
                }
            }


            //===================================================================
            //parent style assignment is complete before step down into child ***
            foreach (var childBox in element.GetChildBoxIter())
            {
                //recursive
                ApplyStyleSheet01(childBox, activeCssTemplate);
            }



        }
    }
}