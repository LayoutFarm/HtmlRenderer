//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public static class Clipboard
    {
        static UIPlatform currentUIPlatform;
        static string textdata;
        public static void Clear()
        {
            //textdata = null;

        }
        public static void SetText(string text)
        {
            //textdata = text;
            currentUIPlatform.SetClipboardData(text);
        }
        public static bool ContainUnicodeText()
        {
            return textdata != null;
        }
        public static string GetUnicodeText()
        {
            return currentUIPlatform.GetClipboardData();
        }

        public static void SetUIPlatform(UIPlatform uiPlatform)
        {
            currentUIPlatform = uiPlatform;
        }
    }
}