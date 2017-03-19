//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
#if DEBUG
    public class dbugVisitorMessage
    {
        public static dbugVisitorMessage WITH_0 = new dbugVisitorMessage(">>WITH ");
        public static dbugVisitorMessage WITH_1 = new dbugVisitorMessage("<<WITH ");
        public static dbugVisitorMessage TOPDOWN_RECAL_FROM_VISITOR_0 = new dbugVisitorMessage(">> TOPDOWN_RECAL_FROM_VISITOR");
        public static dbugVisitorMessage TOPDOWN_RECAL_FROM_VISITOR_1 = new dbugVisitorMessage("<< TOPDOWN_RECAL_FROM_VISITOR");
        public static dbugVisitorMessage SKIP = new dbugVisitorMessage("SKIP ");
        public static dbugVisitorMessage SKIP_CAL = new dbugVisitorMessage("SKIP_CAL ");
        public static dbugVisitorMessage NOT_NEED_RECAL = new dbugVisitorMessage(">>not need recal");
        public static dbugVisitorMessage HAS_CALCULATED_SIZE_EXIT = new dbugVisitorMessage(" HAS CALCULATED SIZE :EXIT");
        public static dbugVisitorMessage E_RECAL_BUB_EARLY_EXIT = new dbugVisitorMessage("E_RECAL_BUB_EARLY_EXIT");
        public static dbugVisitorMessage E_RECAL_BUB_0 = new dbugVisitorMessage(">>E_RECAL_BUB: ");
        public static dbugVisitorMessage E_RECAL_BUB_1 = new dbugVisitorMessage("<<E_RECAL_BUB: ");
        public static dbugVisitorMessage OWNER_LAYER_SUSPEND_SO_EARLY_EXIT = new dbugVisitorMessage("OWNER_LAYER_SUSPEND_SO_EARLY_EXIT");
        public static dbugVisitorMessage NO_OWNER_LAY = new dbugVisitorMessage("NO-OWNER-LAY ");
        public static dbugVisitorMessage NOT_NEED_ARR = new dbugVisitorMessage("not need arrangement");
        public static dbugVisitorMessage SCROLLABLE_BASE_ACCEPT_EXTERNAL_WINDOW_SIZE = new dbugVisitorMessage("SCROLLABLE_BASE_ACCEPT_EXTERNAL_WINDOW_SIZE");
        public static dbugVisitorMessage EVAL_SCR = new dbugVisitorMessage("EVAL-SCR");
        public static dbugVisitorMessage PASS_FIRST_ARR = new dbugVisitorMessage("PASS_FIRST_ARR");
        public string text;
        public dbugVisitorMessage(string text)
        {
            this.text = text;
        }
    }
#endif
}