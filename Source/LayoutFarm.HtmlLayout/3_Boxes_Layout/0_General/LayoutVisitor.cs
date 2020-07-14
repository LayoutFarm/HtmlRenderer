//BSD, 2014-present, WinterDev 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using Typography.Text;
namespace LayoutFarm.HtmlBoxes
{
    public class LayoutVisitor : BoxVisitor
    {
        //---------
        //reusable
        //---------

        HtmlVisualRoot _htmlVisualRoot;
        float _totalMarginLeftAndRight;
        Queue<Dictionary<CssBox, PartialBoxStrip>> _dicStripPool;
        Queue<List<PartialBoxStrip>> _listStripPool;
        Dictionary<CssBox, PartialBoxStrip> _readyDicStrip = new Dictionary<CssBox, PartialBoxStrip>();
        List<PartialBoxStrip> _readyListStrip = new List<PartialBoxStrip>();
        FloatingContextStack _floatingContextStack = new FloatingContextStack();
        static int s_totalLayoutIdEpisode = 0;
        int _episodeId = 1;

        TextServiceClient _txtClient;
        internal LayoutVisitor(TextServiceClient txtClient)
        {
            _txtClient = txtClient;
        }
        internal void Bind(HtmlVisualRoot htmlVisualRoot)
        {
            _htmlVisualRoot = htmlVisualRoot;
            if (_episodeId == ushort.MaxValue - 1)
            {
                //reset
                s_totalLayoutIdEpisode = 1;
                _episodeId = s_totalLayoutIdEpisode++;
            }
        }
        internal void UnBind()
        {
            _htmlVisualRoot = null;
            if (_dicStripPool != null) _dicStripPool.Clear();
            if (_listStripPool != null) _listStripPool.Clear();
            _readyDicStrip.Clear();
            _readyListStrip.Clear();
            _totalMarginLeftAndRight = 0;
            _floatingContextStack.Reset();
            InAbsoluteLayerMode = false;

        }
        internal bool InAbsoluteLayerMode { get; set; }
        internal ITextService TextService => _txtClient;

        protected override void OnPushDifferentContainingBlock(CssBox box)
        {
            _totalMarginLeftAndRight += (box.ActualMarginLeft + box.ActualMarginRight);
        }
        //
        internal CssBox FloatingContextOwner => _floatingContextStack.CurrentTopOwner;


        protected override void OnPushContainingBlock(CssBox box)
        {
            _floatingContextStack.PushContainingBlock(box);
            base.OnPushContainingBlock(box);
        }

        protected override void OnPopContainingBlock()
        {
            _floatingContextStack.PopContainingBlock();
            base.OnPopContainingBlock();
        }

        protected override void OnPopDifferentContaingBlock(CssBox box)
        {
            _totalMarginLeftAndRight -= (box.ActualMarginLeft + box.ActualMarginRight);
        }
        internal CssBox LatestSiblingBox { get; set; }
        internal CssBox LatestLeftFloatBox => _floatingContextStack.LatestLeftFloatBox;

        internal CssBox LatestRightFloatBox => _floatingContextStack.LatestRightFloatBox;


        internal bool HasFloatBoxInContext => _floatingContextStack.HasFloatBoxInContext;


        internal FloatingContextStack GetFloatingContextStack() => _floatingContextStack;


        internal void AddFloatBox(CssBox floatBox)
        {
            _floatingContextStack.AddFloatBox(floatBox);
        }
        internal void UpdateRootSize(CssBox box)
        {
            float candidateRootWidth = Math.Max(
                box.CalculateMinimumWidth(_episodeId) + CalculateWidthMarginTotalUp(box),
                (box.VisualWidth + this.ContainerBlockGlobalX) < CssBoxConstConfig.BOX_MAX_RIGHT ? box.VisualWidth : 0);
            _htmlVisualRoot.UpdateSizeIfWiderOrHigher(
                this.ContainerBlockGlobalX + candidateRootWidth,
                this.ContainerBlockGlobalY + box.VisualHeight);
        }
        /// <summary>
        /// Get the total margin value (left and right) from the given box to the given end box.<br/>
        /// </summary>
        /// <param name="box">the box to start calculation from.</param>
        /// <returns>the total margin</returns>
        float CalculateWidthMarginTotalUp(CssBox box)
        {
            if ((box.VisualWidth + this.ContainerBlockGlobalX) > CssBoxConstConfig.BOX_MAX_RIGHT ||
                (box.ParentBox != null && (box.ParentBox.VisualWidth + this.ContainerBlockGlobalX) > CssBoxConstConfig.BOX_MAX_RIGHT))
            {
                return (box.ActualMarginLeft + box.ActualMarginRight) + _totalMarginLeftAndRight;
            }
            return 0;
        }


        internal void RequestImage(ImageBinder binder, CssBox requestFrom)
        {
            _htmlVisualRoot.RaiseImageRequest(binder,
                requestFrom,
                false);
        }
        internal void RequestScrollView(CssBox requestFrom)
        {
            _htmlVisualRoot.RequestScrollView(requestFrom);
        }
        internal float MeasureWhiteSpace(CssBox box)
        {
            //depends on Font of this box           
            float w = this.TextService.MeasureWhitespace(box.ResolvedFont);
            if (!(box.WordSpacing.IsEmpty || box.WordSpacing.IsNormalWordSpacing))
            {
                w += LayoutFarm.WebDom.Parser.CssLengthExt.ConvertToPxWithFontAdjust(box.WordSpacing, 0, box);
            }
            return w;
        }
        internal float MeasureStringWidth(char[] buffer, int startIndex, int len, RequestFont f)
        {
            var textSpan = new PixelFarm.Drawing.TextBufferSpan(buffer, startIndex, len);
            return this.TextService.MeasureString(textSpan, f).Width;
        }
        internal Size MeasureStringSize(char[] buffer, int startIndex, int len, RequestFont f)
        {
            var textSpan = new PixelFarm.Drawing.TextBufferSpan(buffer, startIndex, len);
            return this.TextService.MeasureString(textSpan, f);
        }
        internal Size MeasureStringSize(char[] buffer, int startIndex, int len, ResolvedFont f)
        {
            var textSpan = new Typography.Text.TextBufferSpan(buffer, startIndex, len);
            return _txtClient.MeasureString(textSpan, f);
            //return GlobalTextService.TextService2.MeasureString(textSpan, f);

            //return this.TextService.MeasureString(textSpan, f);

            //var textSpan = new TextBufferSpan(buffer, startIndex, len);
            //return this.TextService.MeasureString(textSpan, f);
        }
        //---------------------------------------------------------------
        internal Dictionary<CssBox, PartialBoxStrip> GetReadyStripDic()
        {
            if (_readyDicStrip == null)
            {
                if (_dicStripPool == null || _dicStripPool.Count == 0)
                {
                    return new Dictionary<CssBox, PartialBoxStrip>();
                }
                else
                {
                    return _dicStripPool.Dequeue();
                }
            }
            else
            {
                var tmpReadyStripDic = _readyDicStrip;
                _readyDicStrip = null;
                return tmpReadyStripDic;
            }
        }
        internal void ReleaseStripDic(Dictionary<CssBox, PartialBoxStrip> dic)
        {
            //clear before add to pool
            dic.Clear();
            if (_readyDicStrip == null)
            {
                _readyDicStrip = dic;
            }
            else
            {
                if (_dicStripPool == null)
                {
                    _dicStripPool = new Queue<Dictionary<CssBox, PartialBoxStrip>>();
                }


                _dicStripPool.Enqueue(dic);
            }
        }
        //---------------------------------------------------------------
        internal List<PartialBoxStrip> GetReadyStripList()
        {
            if (_readyListStrip == null)
            {
                if (_dicStripPool == null || _dicStripPool.Count == 0)
                {
                    return new List<PartialBoxStrip>();
                }
                else
                {
                    return _listStripPool.Dequeue();
                }
            }
            else
            {
                var tmpReadyListStrip = _readyListStrip;
                _readyListStrip = null;
                return tmpReadyListStrip;
            }
        }
        internal void ReleaseStripList(List<PartialBoxStrip> list)
        {
            //clear before add to pool
            list.Clear();
            if (_readyListStrip == null)
            {
                _readyListStrip = list;
            }
            else
            {
                if (_listStripPool == null)
                {
                    _listStripPool = new Queue<List<PartialBoxStrip>>();
                }
                _listStripPool.Enqueue(list);
            }
        }


        //--------------------------------------------------------------- 
        internal int EpisodeId => _episodeId;

        //---------------------------------------------------------------

#if DEBUG
        int dbugIndentLevel;
        internal bool dbugEnableLogRecord;
        internal List<string> dbuglogRecords = new List<string>();
        public enum dbugPaintVisitorContextName
        {
            Init
        }
        public void dbugResetLogRecords()
        {
            this.dbugIndentLevel = 0;
            dbuglogRecords.Clear();
        }
        public void dbugEnterNewContext(CssBox box, dbugPaintVisitorContextName contextName)
        {
            if (this.dbugEnableLogRecord)
            {
                var controller = CssBox.UnsafeGetController(box);
                //if (box.__aa_dbugId == 7)
                //{
                //}
                dbuglogRecords.Add(new string('>', dbugIndentLevel) + dbugIndentLevel.ToString() +
                    "x:" + box.Left + ",y:" + box.Top + ",w:" + box.VisualWidth + "h:" + box.VisualHeight +
                    " " + box.ToString() + ",id:" + box.__aa_dbugId);
                dbugIndentLevel++;
            }
        }
        public void dbugExitContext()
        {
            if (this.dbugEnableLogRecord)
            {
                dbuglogRecords.Add(new string('<', dbugIndentLevel) + dbugIndentLevel.ToString());
                dbugIndentLevel--;
                if (dbugIndentLevel < 0)
                {
                    throw new NotSupportedException();
                }
            }
        }
#endif
    }
}