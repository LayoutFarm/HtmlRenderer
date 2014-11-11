//MS-PL, Apache2 
//2014, WinterDev

using System;
using LayoutFarm.Drawing;
using System.Collections.Generic;

using HtmlRenderer;
using HtmlRenderer.Css;
using LayoutFarm.SvgDom;

namespace LayoutFarm.SvgDom
{

    public abstract class SvgNode
    {

    }

    public struct ReEvaluateArgs
    {
        public readonly float containerW;
        public readonly float containerH;
        public readonly float emHeight;
        public ReEvaluateArgs(float containerW, float containerH, float emHeight)
        {
            this.containerW = containerW;
            this.containerH = containerH;
            this.emHeight = emHeight;
        }
    }
    public abstract class SvgElement : SvgNode
    {
        LinkedListNode<SvgElement> linkedNode = null;
        LinkedList<SvgElement> children;
        SvgElement parent;
        object controller;

        public SvgElement(object controller)
        {
            this.controller = controller;
        }


        public SvgElement Parent
        {
            get
            {
                return parent;
            }
        }
        public void AddChild(SvgElement child)
        {
            if (this.children == null)
            {
                this.children = new LinkedList<SvgElement>();
            }
            child.linkedNode = this.children.AddLast(child);
            child.parent = this;
        }
        public int Count
        {
            get
            {
                if (this.children == null)
                {
                    return 0;
                }
                else
                {
                    return this.children.Count;
                }
            }
        }

        public LinkedListNode<SvgElement> GetFirstNode()
        {
            if (children == null)
            {
                return null;
            }
            else
            {
                return this.children.First;
            }
        }
        public virtual void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {

        }
        public static float ConvertToPx(CssLength length, ref ReEvaluateArgs args)
        {
            //Return zero if no length specified, zero specified      
            switch (length.UnitOrNames)
            {
                case CssUnitOrNames.EmptyValue:
                    return 0;
                case CssUnitOrNames.Percent:
                    return (length.Number / 100f) * args.containerW;
                case CssUnitOrNames.Ems:
                    return length.Number * args.emHeight;
                case CssUnitOrNames.Ex:
                    return length.Number * (args.emHeight / 2);
                case CssUnitOrNames.Pixels:
                    //atodo: check support for hi dpi
                    return length.Number;
                case CssUnitOrNames.Milimeters:
                    return length.Number * 3.779527559f; //3 pixels per millimeter      
                case CssUnitOrNames.Centimeters:
                    return length.Number * 37.795275591f; //37 pixels per centimeter 
                case CssUnitOrNames.Inches:
                    return length.Number * 96f; //96 pixels per inch 
                case CssUnitOrNames.Points:
                    return length.Number * (96f / 72f); // 1 point = 1/72 of inch   
                case CssUnitOrNames.Picas:
                    return length.Number * 16f; // 1 pica = 12 points 
                default:
                    return 0;
            }
        }
        /// <summary>
        /// get length in pixel
        /// </summary>
        /// <param name="length"></param>
        /// <param name="hundredPercent"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static float ConvertToPx(CssLength length, float hundredPercent, float emHeight)
        {
            //Return zero if no length specified, zero specified      
            switch (length.UnitOrNames)
            {
                case CssUnitOrNames.EmptyValue:
                    return 0;
                case CssUnitOrNames.Percent:
                    return (length.Number / 100f) * hundredPercent;
                case CssUnitOrNames.Ems:
                    return length.Number * emHeight;
                case CssUnitOrNames.Ex:
                    return length.Number * (emHeight / 2);
                case CssUnitOrNames.Pixels:
                    //atodo: check support for hi dpi
                    return length.Number;
                case CssUnitOrNames.Milimeters:
                    return length.Number * 3.779527559f; //3 pixels per millimeter      
                case CssUnitOrNames.Centimeters:
                    return length.Number * 37.795275591f; //37 pixels per centimeter 
                case CssUnitOrNames.Inches:
                    return length.Number * 96f; //96 pixels per inch 
                case CssUnitOrNames.Points:
                    return length.Number * (96f / 72f); // 1 point = 1/72 of inch   
                case CssUnitOrNames.Picas:
                    return length.Number * 16f; // 1 pica = 12 points 
                default:
                    return 0;
            }
        }

        public virtual void Paint(Painter p)
        {

        }
        public virtual bool HitTestCore(SvgHitChain svgChain, float x, float y)
        {

            return false;
        }


        //--------------------------------
        public static object UnsafeGetController(SvgElement elem)
        {
            return elem.controller;
        }

    }

    public class SvgDefinitionList : SvgElement
    {
        public SvgDefinitionList(object controller)
            : base(controller)
        {

        }

    }
    public class SvgFragment : SvgElement
    {
        public SvgFragment()
            : base(null)
        {
        }
    }


    public class SvgLinearGradient : SvgElement
    {
        public SvgLinearGradient(object controller)
            : base(controller)
        {

        }
        public List<StopColorPoint> StopList { get; set; }
        public CssLength X1 { get; set; }
        public CssLength Y1 { get; set; }
        public CssLength X2 { get; set; }
        public CssLength Y2 { get; set; }
    }


    public class SvgUse : SvgNode
    {

    }




}