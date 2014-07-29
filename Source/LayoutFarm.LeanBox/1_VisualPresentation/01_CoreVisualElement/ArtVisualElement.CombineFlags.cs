using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
 
namespace LayoutFarm.Presentation
{


    partial class ArtVisualElement
    {   
        int uiCombineFlags; 
        public VisualElementNature ElementNature
        {
            get
            {
                return (VisualElementNature)(uiCombineFlags & 0xF);
            }
        } 
        static void SetVisualElementNature(ArtVisualElement target, VisualElementNature visualNature)
        {
            target.uiCombineFlags = (target.uiCombineFlags & ~0xF) | (int)visualNature;
        }
         
    }
}