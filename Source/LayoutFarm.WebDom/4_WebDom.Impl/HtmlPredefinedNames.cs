//BSD, 2014-2016, WinterDev   

namespace LayoutFarm.WebDom.Impl
{
    public static class HtmlPredefineNames
    {
        static readonly ValueMap<WellknownName> _wellKnownHtmlNameMap =
            new ValueMap<WellknownName>();
        static UniqueStringTable htmlUniqueStringTableTemplate = new UniqueStringTable();
        static HtmlPredefineNames()
        {
            int j = _wellKnownHtmlNameMap.Count;
            for (int i = 0; i < j; ++i)
            {
                htmlUniqueStringTableTemplate.AddStringIfNotExist(_wellKnownHtmlNameMap.GetStringFromValue((WellknownName)(i + 1)));
            }
        }
        public static UniqueStringTable CreateUniqueStringTableClone()
        {
            return htmlUniqueStringTableTemplate.Clone();
        }
    }
}