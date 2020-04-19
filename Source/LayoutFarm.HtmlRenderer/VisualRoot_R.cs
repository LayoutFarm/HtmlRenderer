//BSD, 2014-present, WinterDev
//ArthurHub, Jose Manuel Menendez Poo 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    partial class HtmlVisualRoot
    {
        public RenderElement RootRenderElement { get; set; }

        public void PerformPaint(PaintVisitor p)
        {
            if (_rootBox == null)
            {
                return;
            }

            p.PushContaingBlock(_rootBox);
#if DEBUG
            p.dbugEnableLogRecord = false;
            p.dbugResetLogRecords();
            dbugPaintN++;
#endif
            CssBox.Paint(_rootBox, p);

            p.PopContainingBlock();
#if DEBUG

            if (p.dbugEnableLogRecord)
            {

            }
#endif

        }
    }

}