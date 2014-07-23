//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Collections.Generic;
using HtmlRenderer.Css;

namespace HtmlRenderer.SvgDom
{

    public abstract class SvgNode
    {

    }


    public abstract class SvgElement : SvgNode
    {
        LinkedListNode<SvgElement> linkedNode = null;
        LinkedList<SvgElement> children;
        SvgElement parent;
        public SvgElement()
        {

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

        internal LinkedListNode<SvgElement> GetFirstNode()
        {
            return this.children.First;
        }
        public virtual void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
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


        public virtual void Paint(HtmlRenderer.Drawing.IGraphics g)
        {

        }
    }

    public class SvgRect : SvgElement
    {
         

        public SvgRect()
        {
        }
        public CssLength X
        {
            get;
            set;
        }
        public CssLength Y
        {
            get;
            set;
        }
        public CssLength Width
        {
            get;
            set;
        }
        public CssLength Height
        {
            get;
            set;
        }
        //----------------------------
        public float ActualX
        {
            get;
            set;
        }
        public float ActualY
        {
            get;
            set;
        }
        public float ActualWidth
        {
            get;
            set;
        }
        public float ActualHeight
        {
            get;
            set;
        }

        //----------------------------
        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            this.ActualX = ConvertToPx(this.X, containerW, emHeight);
            this.ActualY = ConvertToPx(this.Y, containerW, emHeight);

            this.ActualWidth = ConvertToPx(this.Width, containerW, emHeight);
            this.ActualHeight = ConvertToPx(this.Height, containerW, emHeight);

        }

        public override void Paint(Drawing.IGraphics g)
        {
           
            g.FillRectangle(
                System.Drawing.Brushes.Black,
                this.ActualX,
                this.ActualY,
                this.ActualWidth,
                this.ActualHeight);
                  

        }
    }

    public class SvgFragment : SvgElement
    {

    }



}