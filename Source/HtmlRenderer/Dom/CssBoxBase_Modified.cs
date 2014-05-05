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
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// css property set
    /// </summary>
    public class CssPropSet
    {
        [AttrName(HtmlConstants.Display)]
        public CssBoxDisplayType CssDisplayType { get; set; }

        [AttrName(HtmlConstants.Dir)]
        public CssDirection CssDiretion { get; set; }
    }



    partial class CssBoxBase
    {
        public CssBoxDisplayType CssDisplay
        {
            get { return this._myCssDisplay; }
            set
            {
                this._myCssDisplay = value;

                this.PassTestInlineOnlyDeep = this.PassTestInlineOnly = false; 
            } 
        }
        public CssDirection CssDirection
        {
            get { return this._myDirection; }
            set { this._myDirection = value; }
        }
        //----------------------------------------------------
        internal bool PassTestInlineOnly
        {
            get;
            set;
        }

        internal bool InlineOnlyResult
        {
            get;
            private set;
        }
        internal bool PassTestInlineOnlyDeep
        {
            get;
            set;
        }
        internal bool InlineOnlyDeepResult
        {
            get;
            private set;
        }
        internal void SetInlineOnlyTestResult(bool hasInlineOnly)
        {
            this.PassTestInlineOnly = true;
            this.InlineOnlyResult = hasInlineOnly;
        }
        internal void SetInlineOnlyDeepResult(bool inlineOnlyDeepResult)
        {
            this.PassTestInlineOnlyDeep = true;
            this.InlineOnlyDeepResult = inlineOnlyDeepResult;
        }

    }

}