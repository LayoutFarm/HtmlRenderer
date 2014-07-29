using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;






namespace LayoutFarm.Presentation.Text
{
    public class VisualTextEditContentProducer
    {

        BoxStyle[] allRoles;
        public VisualTextEditContentProducer()
        {
            allRoles = CreateRoleSet(
              FontManager.GetTextFontInfo("Tahoma", 10),
              Color.Black, Color.Green, Color.Brown, Color.Red, Color.Blue, Color.Black);
        }



        static BoxStyle[] CreateRoleSet(TextFontInfo fontInfo, params Color[] colors)
        {
            int j = colors.Length;
            BoxStyle[] roleSet = new BoxStyle[j];
            for (int i = 0; i < j; ++i)
            {
                roleSet[i] = CreateSimpleTextRole(fontInfo, colors[i]);
            }
            return roleSet;
        }
        static BoxStyle CreateSimpleTextRole(TextFontInfo textFontInfo, Color textColor)
        {

            BoxStyle beh = new BoxStyle();
            beh.FontColor = textColor;
            return beh;
        }


    }
}