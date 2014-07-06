//BSD 2014, WinterDev

namespace HtmlRenderer.Dom
{
     
    
    partial class CssBox
    {
        //=============================================================
        static class CssBoxFlagsConst
        {
            public const int HAS_ASSIGNED_LOCATION = 1 << (2 - 1);
            public const int EVAL_ROWSPAN = 1 << (3 - 1);
            public const int EVAL_COLSPAN = 1 << (4 - 1);
            public const int HAS_EVAL_WHITESPACE = 1 << (5 - 1);
            public const int TEXT_IS_ALL_WHITESPACE = 1 << (6 - 1);
            public const int TEXT_IS_EMPTY = 1 << (7 - 1);
            //-----------------------------------------------
            //layout state
            public const int LAY_RUNSIZE_MEASURE = 1 << (8 - 1);
            public const int LAY_EVAL_COMPUTE_VALUES = 1 << (9 - 1);             
            public const int LAY_TABLE_FIXED = 1 << (10 - 1);            
            public const int LAY_WIDTH_FREEZE = 1 << (11 - 1);
            //-----------------------------------------------
            public const int HAS_ROUND_CORNER = 1 << (12 - 1);
            //-----------------------------------------------
            public const int IS_INLINE_BOX = 1 << (13 - 1);
            //-----------------------------------------------
            public const int HAS_CONTAINER_PROP = 1 << (14 - 1);

        } 
    }
}