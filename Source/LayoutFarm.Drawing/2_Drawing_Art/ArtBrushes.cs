//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;



namespace LayoutFarm
{
    public abstract class ArtColorBrush
    {
        internal ArtColorPalette ownerPalette;
        string name;

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

        public string FullName
        {
            get
            {
                return ownerPalette.Name + ":" + name;
            }
        }

        //-----------------------------------
        public Brush myBrush;
        public ArtColorBrush()
        {
        }

        public void Dispose()
        {
            if (myBrush != null)
            {
                myBrush.Dispose();
                myBrush = null;
            }
        }
    }

    public class ArtGradientColorInfo
    {
        List<Color> colors = new List<Color>();
        List<Point> positions = new List<Point>();
        public int gradientType;
        public ArtGradientColorInfo()
        {
        }
        public Color GetColor(int index)
        {
            return colors[index];
        }
        public Point GetPosition(int index)
        {
            return positions[index];
        }
        public int ColorCount
        {
            get
            {
                return colors.Count;
            }
        }
        public void AddColor(Color color, Point position)
        {
            if (colors == null)
            {
                colors = new List<Color>();
                positions = new List<Point>();
            }
            colors.Add(color);
            positions.Add(position);
        }

        //internal LinearGradientBrush CreateLinearGradientBrush()
        //{

        //    if (colors.Count == 2)
        //    {
        //        return new LinearGradientBrush(
        //             positions[0],
        //             positions[1],
        //             colors[0],
        //             colors[1]);
        //    }
        //    else if (colors.Count > 2)
        //    {

        //        return new LinearGradientBrush(
        //            positions[0], positions[1], colors[0], colors[1]);

        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
    public class ArtGradientBrush : ArtColorBrush
    {

        ArtGradientColorInfo gradientColorInfo;
        public ArtGradientBrush()
        {
        }
        public ArtGradientBrush(LinearGradientBrush linearGradient)
        {

            this.myBrush = linearGradient;

        }
        public ArtGradientBrush(ArtGradientColorInfo gradientColorInfo)
        {
            this.gradientColorInfo = gradientColorInfo;
        }
        public ArtGradientColorInfo GradientColorInfo
        {
            get
            {
                return this.gradientColorInfo;
            }
            set
            {
                this.gradientColorInfo = value;
            }
        }


    }





    public class ArtSolidBrush : ArtColorBrush
    {
        Color color;
        public ArtSolidBrush(Color color)
        {
            this.color = color;
            this.myBrush = CurrentGraphicPlatform.CreateSolidBrush(color);
        }
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                if (this.myBrush != null)
                {
                    SolidBrush solidBrush = (SolidBrush)myBrush;
                    solidBrush.Color = value;
                }
            }
        }
    }
    public class ArtImageBrush : ArtColorBrush
    {

        Image myImage;
        string imageBrushName;

        public ArtImageBrush(string imageBrushName)
        {
            this.imageBrushName = imageBrushName;

        }
        public Image MyImage
        {
            get
            {
                return this.myImage;
            }
            set
            {
                this.myImage = value;
            }
        }
    }
}