//BSD Jan 2010,  April 2010, 2014 WinterDev 

using System;
using System.Collections.Generic;
using System.Text; 
using LayoutFarm.Drawing;

namespace LayoutFarm.Drawing
{
    public class ArtColorPalette
    {
        string name;
        List<ArtColorBrush> colorBrushes;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public void AddColor(ArtColorBrush colorBrush)
        {
            if (colorBrushes == null)
            {
                colorBrushes = new List<ArtColorBrush>();
            }
            colorBrush.ownerPalette = this;
            colorBrushes.Add(colorBrush);
        }
        public ArtColorBrush GetColorBrush(int index)
        {
            return colorBrushes[index];
        }
        public ArtColorBrush GetColorBrush(string name)
        {
            foreach (ArtColorBrush brush in colorBrushes)
            {
                if (brush.Name == name)
                {
                    return brush;
                }
            }
            return null;
        }
        public IEnumerable<ArtColorBrush> ColorBrushIter
        {
            get
            {
                if (colorBrushes != null)
                {
                    foreach (ArtColorBrush colorBrush in colorBrushes)
                    {
                        yield return colorBrush;
                    }
                }
            }
        }
        public int Count
        {
            get
            {
                if (colorBrushes != null)
                {
                    return colorBrushes.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
   


}