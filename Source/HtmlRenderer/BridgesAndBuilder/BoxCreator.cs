//BSD 2014, WinterDev


using System.Collections.Generic;


namespace HtmlRenderer.Dom
{

    public abstract class CustomCssBoxGenerator
    {
        public abstract CssBox CreateCssBox(IHtmlElement tag, CssBox parentBox, BoxSpec spec);

    }

    public static class BoxCreator
    {
        static List<CustomCssBoxGenerator> generators = new List<CustomCssBoxGenerator>();
        public static void RegisterCustomCssBoxGenerator(CustomCssBoxGenerator generator)
        {
            generators.Add(generator);
        }
        static CssBox CreateCustomCssBox(IHtmlElement tag, CssBox parentBox, BoxSpec spec)
        {
            int j = generators.Count;
            if (j > 0)
            {
                for (int i = j - 1; i >= 0; --i)
                {
                    var box = generators[i].CreateCssBox(tag, parentBox, spec);
                    if (box != null)
                    {
                        return box;
                    }
                }
            }
            return null;
        }


        internal static CssBox CreateBox(CssBox parentBox, BridgeHtmlElement childElement)
        {
            CssBox newBox = null;
            //----------------------------------------- 
            //1. create new box
            //----------------------------------------- 
            switch (childElement.WellknownTagName)
            {
                case WellknownHtmlTagName.br: 
                    //special treatment for br
                    newBox = new CssBox(parentBox, childElement, childElement.Spec);
                    CssBox.ChangeDisplayType(newBox, CssDisplay.BlockInsideInlineAfterCorrection); 
                    break;
                case WellknownHtmlTagName.img:
                    newBox = new CssBoxImage(parentBox, childElement, childElement.Spec);
                    break;
                case WellknownHtmlTagName.hr:
                    newBox = new CssBoxHr(parentBox, childElement, childElement.Spec);
                    break;
                //test extension box
                case WellknownHtmlTagName.X: 
                    newBox = CreateCustomBox(parentBox, childElement, childElement.Spec);

                    if (newBox == null)
                    {
                        newBox = new CssBox(parentBox, childElement, childElement.Spec);
                    }
                    break;
                default:
                    newBox = new CssBox(parentBox, childElement, childElement.Spec);
                    break;
            }

            return newBox;
        }

        static CssBox CreateCustomBox(CssBox parent, IHtmlElement tag, BoxSpec boxspec)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent, boxspec);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }

        /// <summary>
        /// Create new css block box.
        /// </summary>
        /// <returns>the new block box</returns>
        internal static CssBox CreateRootBlock()
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();

            var box = new CssBox(null, null, spec);
            //------------------------------------
            box.ReEvaluateFont(10);
            //------------------------------------
            return box;
        }



    }
}