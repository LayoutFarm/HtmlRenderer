//BSD 2014, WinterDev

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


        static void ApplyStyleSheetForBox(CssBox box, BoxSpec parentSpec, ActiveCssTemplate activeCssTemplate)
        {
           
            BridgeHtmlElement element = box.HtmlElement as BridgeHtmlElement;
            BoxSpec curSpec = box.Spec;

#if DEBUG
            //---------------------
            dbugS01++;
             
            //---------------------
            Console.WriteLine("A " + dbugS01 + "-------------");
            dbugCompareSpecDiff("", box);
#endif
            //---------------------
            //0.
            curSpec.InheritStylesFrom(parentSpec);

            if (element != null)
            {
                //1. apply style  
                activeCssTemplate.ApplyActiveTemplate(element.Name,
                   element.TryGetAttribute("class", null),
                   curSpec,
                   parentSpec);

                //2.
                // try assign style using the "id" attribute of the html element
                if (element.HasAttribute("id"))
                {
                    throw new NotSupportedException();
                    //string id = element.GetAttributeValue("id", null);
                    //if (id != null)
                    //{   
                    //    //AssignStylesForElementId(box, activeCssTemplate, "#" + id);
                    //}
                }
                //-------------------------------------------------------------------

                //3. some html translate attributes
                AssignStylesFromTranslatedAttributesHTML5(box, activeCssTemplate);
                //AssignStylesFromTranslatedAttributes_Old(box, activeCssTemplate);
                //------------------------------------------------------------------- 
                //4. a style attribute value
                string attrStyleValue;
                if (element.TryGetAttribute2("style", out attrStyleValue))
                {
                    var ruleset = activeCssTemplate.ParseCssBlock(element.Name, attrStyleValue);
                    foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                    {
                        CssPropSetter.AssignPropertyValue(
                            curSpec,
                            parentSpec,
                            propDecl);
                    }
                }
                curSpec.Freeze();
            }

#if DEBUG
            //------------------------
            //compare and write results
            Console.WriteLine("\tB " + dbugS01 + "-------------");
            dbugCompareSpecDiff("\t", box);
            //------------------------
#endif
            //===================================================================
            //5. children
            //parent style assignment is complete before step down into child ***
            foreach (var childBox in box.GetChildBoxIter())
            {
                //recursive
                ApplyStyleSheetForBox(childBox, curSpec, activeCssTemplate);
            }


        }
#if DEBUG
        static int dbugS01 = 0;
        static void dbugCompareSpecDiff(string prefix, CssBox box)
        {
            BridgeHtmlElement element = box.HtmlElement as BridgeHtmlElement;
            BoxSpec curSpec = box.Spec;
            dbugPropCheckReport rep = new dbugPropCheckReport();
            if (element != null)
            {
                if (!BoxSpec.dbugCompare(
                    rep,
                    element.Spec,
                    curSpec))
                {
                    foreach (string s in rep.GetList())
                    {
                        Console.WriteLine(prefix + s);
                    }

                }
            }
            else
            {
                if (box.dbugAnonCreator != null)
                {    
                    if (!BoxSpec.dbugCompare(
                    rep,
                    box.dbugAnonCreator.Spec.GetAnonVersion(),
                    curSpec))
                    {
                        foreach (string s in rep.GetList())
                        {
                            Console.WriteLine(prefix + s);
                        }
                    }
                }
                else
                {

                }
            }

        }
#endif
    }
}