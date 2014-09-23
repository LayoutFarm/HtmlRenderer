//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace LayoutFarm
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



    public abstract class ArtColorBrush
    {
        public ArtColorPalette ownerPalette;
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

    public class ArtSolidBrush : ArtColorBrush
    {
        Color color;
        public ArtSolidBrush(Color color)
        {
            this.color = color;
            this.myBrush = CurrentGraphicPlatform.CreateSolidBrushFromColor(color);
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

        public LinearGradientBrush CreateLinearGradientBrush()
        {
            if (colors.Count == 2)
            {
                return CurrentGraphicPlatform.P.CreateLinearGradientBrush(
                     positions[0], positions[1], colors[0], colors[1]);
            }
            else if (colors.Count > 2)
            {
                return CurrentGraphicPlatform.P.CreateLinearGradientBrush(
                    positions[0], positions[1], colors[0], colors[1]);
            }
            else
            {
                return null;
            }
        }
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

        public void RefreshGradient()
        {
            if (myBrush != null)
            {
                myBrush.Dispose();
                myBrush = null;
            }
            myBrush = gradientColorInfo.CreateLinearGradientBrush();

        }
    }
}