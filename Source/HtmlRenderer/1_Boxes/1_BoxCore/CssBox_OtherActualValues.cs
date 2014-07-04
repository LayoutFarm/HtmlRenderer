using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using HtmlRenderer.Parse;


namespace HtmlRenderer.Dom
{
    partial class CssBox
    {
<<<<<<< HEAD:Source/HtmlRenderer/1_Boxes/0_BoxCore/CssBox_OtherActualValues.cs

=======
>>>>>>> 1.7.2105.1:Source/HtmlRenderer/1_Boxes/1_BoxCore/CssBox_OtherActualValues.cs
        Font _actualFont;
        float _actualLineHeight;
        float _actualWordSpacing;
        float _actualTextIndent;

        float _actualBorderSpacingHorizontal;
        float _actualBorderSpacingVertical;

        /// <summary>
        /// Gets the line height
        /// </summary>
        public float ActualLineHeight
        {
            get
            {
                return _actualLineHeight;

                //if ((this._prop_pass_eval & CssBoxAssignments.LINE_HEIGHT) == 0)
                //{
                //    this._prop_pass_eval |= CssBoxAssignments.LINE_HEIGHT;
                //    _actualLineHeight = .9f * CssValueParser.ParseLength(LineHeight, this.SizeHeight, this);
                //}
                //return _actualLineHeight;
            }
        }

        /// <summary>
        /// Gets the text indentation (on first line only)
        /// </summary>
        public float ActualTextIndent
        {
            get
            {
<<<<<<< HEAD:Source/HtmlRenderer/1_Boxes/0_BoxCore/CssBox_OtherActualValues.cs
                if ((this._prop_pass_eval & BoxAssignmentStates.TEXT_INDENT) == 0)
                {
                    this._prop_pass_eval |= BoxAssignmentStates.TEXT_INDENT;
                    _actualTextIndent = CssValueParser.ParseLength(
                        this.BoxSpec.TextIndent, this.SizeWidth, this);
=======
                if ((this._prop_pass_eval & CssBoxAssignments.TEXT_INDENT) == 0)
                {
                    this._prop_pass_eval |= CssBoxAssignments.TEXT_INDENT;
                    _actualTextIndent = CssValueParser.ConvertToPx(TextIndent, this.SizeWidth, this);
>>>>>>> 1.7.2105.1:Source/HtmlRenderer/1_Boxes/1_BoxCore/CssBox_OtherActualValues.cs
                }
                return _actualTextIndent;
            }
        }

        /// <summary>
        /// Gets the actual width of whitespace between words.
        /// </summary>
        public float ActualWordSpacing
        {
            get { return _actualWordSpacing; }
        }
        protected float MeasureWordSpacing(LayoutVisitor lay)
        {
<<<<<<< HEAD:Source/HtmlRenderer/1_Boxes/0_BoxCore/CssBox_OtherActualValues.cs
            if ((this._prop_pass_eval & BoxAssignmentStates.WORD_SPACING) == 0)
            {
                this._prop_pass_eval |= BoxAssignmentStates.WORD_SPACING;
=======
            if ((this._prop_pass_eval & CssBoxAssignments.WORD_SPACING) == 0)
            {
                this._prop_pass_eval |= CssBoxAssignments.WORD_SPACING;
>>>>>>> 1.7.2105.1:Source/HtmlRenderer/1_Boxes/1_BoxCore/CssBox_OtherActualValues.cs

                _actualWordSpacing = lay.MeasureWhiteSpace(this);

                if (!this.BoxSpec.WordSpacing.IsNormalWordSpacing)
                {
<<<<<<< HEAD:Source/HtmlRenderer/1_Boxes/0_BoxCore/CssBox_OtherActualValues.cs
                    _actualWordSpacing += CssValueParser.ParseLength(this.BoxSpec.WordSpacing, 1, this);
=======
                    _actualWordSpacing += CssValueParser.ConvertToPx(this.WordSpacing, 1, this);
>>>>>>> 1.7.2105.1:Source/HtmlRenderer/1_Boxes/1_BoxCore/CssBox_OtherActualValues.cs
                }
            }
            return this._actualWordSpacing;
        }
        /// <summary>
        /// Gets the actual horizontal border spacing for tables
        /// </summary>
        public float ActualBorderSpacingHorizontal
        {
            get
            {
<<<<<<< HEAD:Source/HtmlRenderer/1_Boxes/0_BoxCore/CssBox_OtherActualValues.cs
                if ((this._prop_pass_eval & BoxAssignmentStates.BORDER_SPACING_H) == 0)
                {
                    this._prop_pass_eval |= BoxAssignmentStates.BORDER_SPACING_H;
                    _actualBorderSpacingHorizontal = this.BoxSpec.BorderSpacingHorizontal.Number;
=======
                if ((this._prop_pass_eval & CssBoxAssignments.BORDER_SPACING_H) == 0)
                {
                    this._prop_pass_eval |= CssBoxAssignments.BORDER_SPACING_H;
                    _actualBorderSpacingHorizontal = this.BorderSpacingHorizontal.Number;
>>>>>>> 1.7.2105.1:Source/HtmlRenderer/1_Boxes/1_BoxCore/CssBox_OtherActualValues.cs
                }
                return _actualBorderSpacingHorizontal;
            }
        }

        /// <summary>
        /// Gets the actual vertical border spacing for tables
        /// </summary>
        public float ActualBorderSpacingVertical
        {
            get
            {
<<<<<<< HEAD:Source/HtmlRenderer/1_Boxes/0_BoxCore/CssBox_OtherActualValues.cs
                if ((this._prop_pass_eval & BoxAssignmentStates.BORDER_SPACING_V) == 0)
                {
                    this._prop_pass_eval |= BoxAssignmentStates.BORDER_SPACING_V;
                    _actualBorderSpacingVertical = this.BoxSpec.BorderSpacingVertical.Number;
=======
                if ((this._prop_pass_eval & CssBoxAssignments.BORDER_SPACING_V) == 0)
                {
                    this._prop_pass_eval |= CssBoxAssignments.BORDER_SPACING_V;
                    _actualBorderSpacingVertical = this.BorderSpacingVertical.Number;
>>>>>>> 1.7.2105.1:Source/HtmlRenderer/1_Boxes/1_BoxCore/CssBox_OtherActualValues.cs
                }
                 
                return _actualBorderSpacingVertical;
            }
        }
        ///// <summary>
        ///// Ensures that the specified length is converted to pixels if necessary
        ///// </summary>
        ///// <param name="length"></param>
        //public CssLength NoEms(CssLength length)
        //{
        //    if (length.UnitOrNames == CssUnitOrNames.Ems)
        //    {
        //        return length.ConvertEmToPixels(GetEmHeight());
        //    }
        //    return length;
        //}
    }
}