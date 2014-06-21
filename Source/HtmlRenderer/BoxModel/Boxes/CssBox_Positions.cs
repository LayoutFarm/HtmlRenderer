using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace HtmlRenderer.Dom
{
    partial class CssBox
    {
        float _localX;
        float _localY;
      

        public float LocalX
        {
            get { return this._localX; }
        }
        public float LocalY
        {
            get { return this._localY; }
        }

        /// <summary>
        /// set location relative to container box
        /// </summary>
        /// <param name="localX"></param>
        /// <param name="localY"></param>
        public void SetLocation(float localX, float localY)
        {
            this._localX = localX;
            this._localY = localY;
            this._boxCompactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
        } 

        public RectangleF LocalBound
        {
            get { return new RectangleF(new PointF(this.LocalX, this.LocalY), Size); }
        }
        /// <summary>
        /// Gets the width available on the box, counting padding and margin.
        /// </summary>
        public float AvailableWidth
        {
            get { return this.SizeWidth - ActualBorderLeftWidth - ActualPaddingLeft - ActualPaddingRight - ActualBorderRightWidth; }
        }
        public float LocalActualRight
        {
            get { return this.LocalX + this.SizeWidth; }
        }
        public float LocalActualBottom
        {
            get { return this.LocalY + this.SizeHeight; }
        } 
        public float LocalClientLeft
        {
            get { return ActualBorderLeftWidth + ActualPaddingLeft; }
        }

        public float LocalClientRight
        {
            get { return this.LocalActualRight - ActualPaddingRight - ActualBorderRightWidth; }
        }


        public float LocalClientTop
        {
            get { return ActualBorderTopWidth + ActualPaddingTop; }
        }

        public float LocalClientBottom
        {
            get { return this.LocalActualBottom - ActualPaddingBottom - ActualBorderBottomWidth; }
        }


        public RectangleF LocalClientRectangle
        {
            get { return RectangleF.FromLTRB(this.LocalClientLeft, LocalClientTop, LocalClientRight, LocalClientBottom); }
        }

        public float ClientWidth
        {
            get { return this.SizeWidth - (ActualPaddingLeft + ActualBorderLeftWidth + ActualPaddingRight + ActualBorderRightWidth); }
        }
        public float ClientHeight
        {
            get { return this.SizeHeight - (ActualPaddingTop + ActualBorderTopWidth + ActualPaddingBottom + ActualBorderBottomWidth); }
        }
    }
}
