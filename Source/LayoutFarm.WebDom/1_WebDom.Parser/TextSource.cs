//BSD, 2014-present, WinterDev 

using System;
using LayoutFarm.WebLexer;
namespace LayoutFarm.WebDom.Parser
{
    public class TextSource
    {
        TextSnapshot _actualSnapshot;
        public TextSource(char[] textBuffer)
        {
            _actualSnapshot = new TextSnapshot(textBuffer);
        }
        internal TextSnapshot ActualSnapshot => _actualSnapshot;


        public static string GetInternalText(TextSource textsource)
        {
            return new string(TextSnapshot.UnsafeGetInternalBuffer(textsource._actualSnapshot));
        }
    }
}