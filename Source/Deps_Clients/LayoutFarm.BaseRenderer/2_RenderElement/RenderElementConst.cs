//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
    static class RenderElementConst
    {
        public const int IS_TRANSLUCENT_BG = 1 << (1 - 1);
        public const int SCROLLABLE_FULL_MODE = 1 << (2 - 1);
        public const int TRANSPARENT_FOR_ALL_EVENTS = 1 << (3 - 1);
        public const int HIDDEN = 1 << (4 - 1);
        public const int IS_GRAPHIC_VALID = 1 << (5 - 1);
        public const int IS_IN_ANIMATION_MODE = 1 << (7 - 1);
        public const int IS_TOP_RENDERBOX = 1 << (8 - 1);
        public const int FIRST_ARR_PASS = 1 << (9 - 1);
        public const int ANIMATION_WAITING_FOR_NORMAL_MODE = 1 << (10 - 1);
        public const int IS_BLOCK_ELEMENT = 1 << (11 - 1);
        public const int IS_LINE_BREAK = 1 << (12 - 1);
        public const int HAS_TRANSPARENT_BG = 1 << (13 - 1);
        public const int HAS_DOUBLE_SCROLL_SURFACE = 1 << (14 - 1);
        public const int IS_IN_RENDER_CHAIN = 1 << (15 - 1);
        public const int MAY_HAS_CHILD = 1 << (16 - 1);
        public const int MAY_HAS_VIEWPORT = 1 << (17 - 1);
        //===============================================

        internal const int LY_HAS_SPC_WIDTH = 1 << (1 - 1);
        internal const int LY_HAS_SPC_HEIGHT = 1 << (2 - 1);
        internal const int LY_HAS_SPC_SIZE = LY_HAS_SPC_WIDTH | LY_HAS_SPC_HEIGHT;
        internal const int LY_REACH_MIN_WIDTH = 1 << (3 - 1);
        internal const int LY_REACH_MAX_WIDTH = 1 << (4 - 1);
        internal const int LY_REACH_MIN_HEIGHT = 1 << (5 - 1);
        internal const int LY_REACH_MAX_HEIGHT = 1 << (6 - 1);
        internal const int LY_HAS_ARRANGED_CONTENT = 1 << (7 - 1);
        internal const int LAY_HAS_CALCULATED_SIZE = 1 << (8 - 1);
        internal const int LY_SUSPEND = 1 << (9 - 1);
        internal const int LY_SUSPEND_GRAPHIC = 1 << (12 - 1);
        internal const int LY_IN_LAYOUT_QUEUE = 1 << (13 - 1);
    }
}
