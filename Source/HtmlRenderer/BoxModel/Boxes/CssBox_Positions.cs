using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace HtmlRenderer.Dom
{
    partial class CssBox
    {
        float _globalX;
        float _globalY;
        float _localX;
        float _localY;

        public bool IsAbsolutePosition
        {
            get
            {
                return this.Position == CssPosition.Absolute;
            }
        }
        public float GlobalX
        {
            get { return this._globalX; }
        }
        public float GlobalY
        {
            get { return this._globalY; }
        }
        public float LocalX
        {
            get { return this._localX; }
        }
        public float LocalY
        {
            get { return this._localY; }
        }
        //void Offset(float dx, float dy)
        //{
        //    this._globalX += dx;
        //    this._globalY += dy;
        //    this._localX += dx;
        //    this._localY += dy;

        //    this._boxCompactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
        //}
        void OffsetOnlyGlobal(float dx, float dy)
        {
            this._globalX += dx;
            this._globalY += dy;

            this._boxCompactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
        }
        public void SetGlobalLocation(float globalX, float globalY, float container_globalX, float container_globalY)
        {
            this._globalX = globalX;
            this._globalY = globalY; 

            this._localX = globalX - container_globalX;
            this._localY = globalY - container_globalY;


            this._boxCompactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
        }

        /// <summary>
        /// Gets the bounds of the box
        /// </summary>
        public RectangleF GlobalBound
        {
            get { return new RectangleF(new PointF(this.GlobalX, this.GlobalY), Size); }
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

        /// <summary>
        /// Gets the right of the box. When setting, it will affect only the width of the box.
        /// </summary>
        public float GlobalActualRight
        {
            get { return GlobalX + this.SizeWidth; }
        }
        public void SetGlobalActualRight(float value)
        {
            this.SetSize(value - GlobalX, this.SizeHeight);
        }
        /// <summary>
        /// Gets or sets the bottom of the box. 
        /// (When setting, alters only the Size.Height of the box)
        /// </summary>
        public float GlobalActualBottom
        {
            get { return this.GlobalY + this.SizeHeight; }
        }
        public void SetGlobalActualBottom(float value)
        {
            this.SetSize(this.SizeWidth, value - this._globalY);
        }



        /// <summary>
        /// Gets the left of the client rectangle (Where content starts rendering)
        /// </summary>
        public float GlobalClientLeft
        {
            get { return this.GlobalX + ActualBorderLeftWidth + ActualPaddingLeft; }
        }

        /// <summary>
        /// Gets the right of the client rectangle
        /// </summary>
        public float GlobalClientRight
        {
            get { return GlobalActualRight - ActualPaddingRight - ActualBorderRightWidth; }
        }

        public float GlobalClientTop
        {
            get { return this.GlobalY + this.ClientTop; }
        }

        public float ClientTop
        {
            get { return ActualBorderTopWidth + ActualPaddingTop; }
        }
        /// <summary>
        /// Gets the bottom of the client rectangle
        /// </summary>
        public float GlobalClientBottom
        {
            get { return this.GlobalActualBottom - ActualPaddingBottom - ActualBorderBottomWidth; }
        }

        /// <summary>
        /// Gets the client rectangle
        /// </summary>
        public RectangleF GlobalClientRectangle
        {
            get { return RectangleF.FromLTRB(GlobalClientLeft, GlobalClientTop, GlobalClientRight, GlobalClientBottom); }
        }

        public float ClientWidth
        {
            get { return this.GlobalClientRight - this.GlobalClientLeft; }
        }
    }
}
