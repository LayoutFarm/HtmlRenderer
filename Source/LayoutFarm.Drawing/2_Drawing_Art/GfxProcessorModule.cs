using System;
using System.Collections.Generic;
using System.Text; 
namespace LayoutFarm.Drawing
{

     
    public abstract class GfxProcessorModule
    {


#if DEBUG
        public string moduleName;
#endif
        public GfxProcessorModule()
        {
        }
 
        public abstract int ModuleId
        {
            get;
        }
 
        
        public abstract ArtGfxInstructionInfo GetGfxInfo(int instructionId);

#if DEBUG
        public override string ToString()
        {
            return moduleName;
        }
#endif

    }

    public static class GfxColor
    {
        public const int COLOR_ALPHA_COMPO = 2;
        public const int COLOR_RED_COMPO = 3;
        public const int COLOR_GREEN_COMPO = 4;
        public const int COLOR_BLUE_COMPO = 5;

        public const int COLOR_SOLID = 6;
        public const int COLOR_GRADIENT_LINEAR = 7;
        public const int COLOR_GRADIENT_CIRCULAR = 8;
    }

}