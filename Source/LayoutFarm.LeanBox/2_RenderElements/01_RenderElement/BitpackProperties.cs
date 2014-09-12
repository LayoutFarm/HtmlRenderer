using System;
using System.Runtime.InteropServices;

namespace LayoutFarm.Presentation
{

    struct BitpackProperties  
    {
        int innerBitFlags;
        public bool this[int index]
        {
            get
            {

                return ((innerBitFlags >> (index - 1)) & 1) != 0;
            }
            set
            {
                if (value)
                {
                    innerBitFlags |= (1 << (index - 1));
                }
                else
                {
                    innerBitFlags &= ~(1 << (index - 1));
                }
            }
        }
    }
}