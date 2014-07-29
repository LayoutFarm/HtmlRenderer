
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using LayoutFarm.Presentation;




namespace LayoutFarm.Presentation
{


    public abstract partial class VisualRoot
    {

                                static int screenWidth = 1024;
                                static int screenHeight = 800;

        public static int ScreenWidth
        {
            get
            {
                return screenWidth;
            }
        }
        public static int ScreenHeight
        {
            get
            {
                return screenHeight;
            }
        }
        public abstract void AddVisualRequest(VisualElementRequest req);

    }
}