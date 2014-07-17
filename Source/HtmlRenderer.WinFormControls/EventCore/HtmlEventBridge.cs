//BSD 2014,WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes;
using System.Threading;

namespace HtmlRenderer.WebDom
{

    public class HtmlEventBridge
    {
        //-----------------------------------------------
        HtmlContainer _container;
        BoxHitChain _latestMouseDownHitChain = null;
        int _mousedownX;
        int _mousedownY;
        bool _isMouseDown;
        //-----------------------------------------------

        SelectionRange _currentSelectionRange = null;


        WinGraphics simpleWinGfx;
        Bitmap tempBmp = new Bitmap(1, 1);
        
        bool _isBinded;

        public HtmlEventBridge()
        {
        }
        public void Bind(HtmlContainer container)
        {
            if (container != null)
            {
                this._container = container;
            }
            simpleWinGfx = new WinGraphics(Graphics.FromImage(tempBmp), false);
            _isBinded = true;
        }
        public void Unbind()
        {
            this._container = null;
            this._isBinded = false;
        }
       
        public void MouseDown(int x, int y, int button)
        {
            if (!_isBinded)
            {
                return;
            }
            //---------------------------------------------------- 
            ClearPreviousSelection();
            //----------------------------------------------------

            _mousedownX = x;
            _mousedownY = y;
            this._isMouseDown = true;

            BoxHitChain hitChain = new BoxHitChain();
            hitChain.SetRootGlobalPosition(x, y);
            //1. prob hit chain only
            BoxUtils.HitTest(_container.GetRootCssBox(), x, y, hitChain);
            _latestMouseDownHitChain = hitChain;

            //2. invoke css event and script event 
            var hitInfo = hitChain.GetLastHit();

          
        }
        public void MouseMove(int x, int y, int button)
        {
            if (!_isBinded)
            {
                return;
            }
            if (this._isMouseDown)
            {
                //dragging
                if (this._mousedownX != x || this._mousedownY != y)
                {
                    //handle mouse drag
                    BoxHitChain hitChain = new BoxHitChain();
                    hitChain.SetRootGlobalPosition(x, y);

                    BoxUtils.HitTest(this._container.GetRootCssBox(), x, y, hitChain);

                    ClearPreviousSelection();
                    _currentSelectionRange = new SelectionRange(_latestMouseDownHitChain, hitChain, simpleWinGfx); 
                    
                }
            }
            else
            {

                //mouse move 
            }
        }
        public void MouseUp(int x, int y, int button)
        {
            if (!_isBinded)
            {
                return;
            }
            this._isMouseDown = false;

            BoxHitChain hitChain = new BoxHitChain();
            hitChain.SetRootGlobalPosition(x, y);
            //1. prob hit chain only
            BoxUtils.HitTest(_container.GetRootCssBox(), x, y, hitChain);

            //2. invoke css event and script event 
            var hitInfo = hitChain.GetLastHit();




        }
        public void MouseLeave()
        {
        }
        public void MouseDoubleClick(int x, int y, int button)
        {
        }
        public void MouseWheel(int x, int y, int button, int delta)
        {

        }
        void ClearPreviousSelection()
        { 
            //TODO: check mouse botton
            if (_currentSelectionRange != null)
            {
                _currentSelectionRange.ClearSelectionStatus();
                _currentSelectionRange = null;
            }
        }
        //-----------------------------------
    }
}