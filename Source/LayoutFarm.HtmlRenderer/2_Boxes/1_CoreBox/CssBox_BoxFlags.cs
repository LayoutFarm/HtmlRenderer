// 2015,2014 ,BSD, WinterDev

namespace LayoutFarm.HtmlBoxes
{

    partial class CssBox
    {
        static class BoxFlags
        {   
            public const int HAS_ASSIGNED_LOCATION = 1 << (2 - 1);
            public const int HAS_CONTAINER_PROP = 1 << (3 - 1);
            public const int HAS_EVAL_WHITESPACE = 1 << (4 - 1);
            public const int TEXT_IS_ALL_WHITESPACE = 1 << (5 - 1);
            public const int TEXT_IS_EMPTY = 1 << (6 - 1);
            //----------------------------------------------- 
            public const int DONT_CHANGE_DISPLAY_TYPE = 1 << (7 - 1);
            
            public const int LAY_RUNSIZE_MEASURE = 1 << (8 - 1);
            public const int LAY_EVAL_COMPUTE_VALUES = 1 << (9 - 1);
            public const int LAY_TABLE_FIXED = 1 << (10 - 1);
            public const int LAY_WIDTH_FREEZE = 1 << (11 - 1);
            //-----------------------------------------------
            public const int HAS_ROUND_CORNER = 1 << (12 - 1);
            public const int HAS_VISIBLE_BG = 1 << (13 - 1);
            public const int HAS_SOME_VISIBLE_BORDER = 1 << (14 - 1);
            //-----------------------------------------------
            public const int IS_INLINE_BOX = 1 << (15 - 1);
            public const int IS_BR_ELEM = 1 << (16 - 1);
           
            //-----------------------------------------------
            public const int IS_CUSTOM_CSSBOX = 1 << (18 - 1);
            //-----------------------------------------------
            public const int OVERFLOW_HIDDEN = 1 << (19 - 1);

            //-----------------------------------------------
            
            public const int HAS_CUSTOM_RENDER_TECHNIQUE = 1 << (20 - 1);
            public const int HAS_CUSTOM_HIT_TEST_TECHNIQUE = 1 << (21 - 1);
            public const int HAS_CUSTOM_LAYOUT_TECHNIQUE = 1 << (22 - 1);
            //-----------------------------------------------
        }
    }

}