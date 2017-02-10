//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
#if DEBUG
    partial class RootGraphic
    {
        public static readonly dbugVisualRootMsg dbugMsg_E_LAYOUT_INV_BUB_FIRST_enter = new dbugVisualRootMsg(">>E_LAYOUT_INV_BUB_FIRST* ");
        public static readonly dbugVisualRootMsg dbugMsg_E_LAYOUT_INV_BUB_FIRST_exit = new dbugVisualRootMsg("<<E_LAYOUT_INV_BUB_FIRST* ");
        public static readonly dbugVisualRootMsg dbugMsg_L_LAYOUT_INV_BUB_enter = new dbugVisualRootMsg(">>L_LAYOUT_INV_BUB: ");
        public static readonly dbugVisualRootMsg dbugMsg_L_LAYOUT_INV_BUB_exit = new dbugVisualRootMsg("<<L_LAYOUT_INV_BUB: ");
        public static readonly dbugVisualRootMsg dbugMsg_E_CHILD_LAYOUT_INV_BUB_enter = new dbugVisualRootMsg(">>E_CHILD_LAYOUT_INV_BUB: ");
        public static readonly dbugVisualRootMsg dbugMsg_E_CHILD_LAYOUT_INV_BUB_exit = new dbugVisualRootMsg("<<E_CHILD_LAYOUT_INV_BUB: ");
        public static readonly dbugVisualRootMsg dbugMsg_BGN_Line_AddNormalRunToLast = new dbugVisualRootMsg("BGN:Line.AddNormalRunToLast ");
        public static readonly dbugVisualRootMsg dbugMsg_FSH_Line_AddNormalRunToLast = new dbugVisualRootMsg("FSH:Line.AddNormalRunToLast ");
        public static readonly dbugVisualRootMsg dbugMsg_CLEAR_LAYOUT_enter = new dbugVisualRootMsg(">> CLEAR_LAYOUT ");
        public static readonly dbugVisualRootMsg dbugMsg_CLEAR_LAYOUT_exit = new dbugVisualRootMsg("<< CLEAR_LAYOUT ");
        public static readonly dbugVisualRootMsg dbugMsg_VisualElementLine_INVALIDATE_enter = new dbugVisualRootMsg("\t>>VisualElementLine::INVALID_LAY_LINE");
        public static readonly dbugVisualRootMsg dbugMsg_VisualElementLine_INVALIDATE_exit = new dbugVisualRootMsg("\t<<VisualElementLine::INVALID_LAY_LINE");
        public static readonly dbugVisualRootMsg dbugMsg_VisualElementLine_OwnerFlowElementIsIn_SUSPEND_MODE_enter = new dbugVisualRootMsg("\t>>VisualElementLine::OwnerFlowElementIsIn_SUSPEND_MODE");
        public static readonly dbugVisualRootMsg dbugMsg_LAYER_OWNER_ALREADY_IN_ARR_Q = new dbugVisualRootMsg(">< LAYER_s_OWNER_ALREADY_in_ARR_Q");
        public static readonly dbugVisualRootMsg dbugMsg_line_sep = new dbugVisualRootMsg("===");
        public static readonly dbugVisualRootMsg dbugMsg_NO_PARENT = new dbugVisualRootMsg("*** NO_PARENT");
        public static readonly dbugVisualRootMsg dbugMsg_NO_OWNER_LAY = new dbugVisualRootMsg(" NO_OWNER_LAY");
        public static readonly dbugVisualRootMsg dbugMsg_NOT_BUBBLE_UP_HAS_SPECIFIC_SIZE = new dbugVisualRootMsg("*** NOT_BUBBLE_UP_HAS_SPECIFIC_SIZE");
        public static readonly dbugVisualRootMsg dbugMsg_NOT_BUBBLE_UP_IS_SCROLLABLE = new dbugVisualRootMsg("*** NOT_BUBBLE_UP_IS_SCROLLABLE");
        public static readonly dbugVisualRootMsg dbugMsg_tto = new dbugVisualRootMsg("\tto ");
        public static readonly dbugVisualRootMsg dbugMsg_PARENT_NOTTIFY_BOUND_CHAGED = new dbugVisualRootMsg("PARENT_NOTIFY_BOUND_CHANGED :");
        public static readonly dbugVisualRootMsg dbugMsg_ADD_TO_LAYOUT_QUEUE = new dbugVisualRootMsg("ADD_TO_LAYOUT_QUEUE ");
        public static readonly dbugVisualRootMsg dbugMsg_BLOCKED = new dbugVisualRootMsg("BLOCKED1");
        public class dbugVisualRootMsg
        {
            public string msg;
            public dbugVisualRootMsg(string msg)
            {
                this.msg = msg;
            }
        }
    }
#endif
}