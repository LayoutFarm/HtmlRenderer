//BSD Jan 2010,  April 2010, 2014 WinterDev 

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO; 

namespace HtmlRenderer.Drawing.Art
{
    
    public class ArtGfxInstructionInfo
    {   
        public const int ROLE_BEGIN = 1;
        public const int ROLE_END = 2; 
        public const int ROLE_BEH_COUNT = 3;

        public const int BEH_BEGIN = 4;
        public const int BEH_END = 5; 
        public const int STATE_COUNT = 6;

       
        public const int STATE_INSTRUCTION_BEGIN = 7;
        public const int STATE_INSTRUCTION_END = 8;
        public const int EVENT_ID = 9;
        //---------------------------  
        public const int BEH_STATE_ANIMATION_COUNT = 10;
        //--------------------------- 
       
        public const int DT_BYTE = 1;
        public const int DT_INT16 = 2;
        public const int DT_INT32 = 3;
        public const int DT_INT64 = 4;
        public const int DT_DATETIME = 5;
        public const int DT_STRING = 6;
        public const int DT_COLOR = 7;
        public const int DT_OBJECT = 8;
        public const int DT_CURSOR = 9;
        public const int DT_BRUSH = 11; 

        public const int DT_CSS = 13;

         
        public readonly int instructionId;
         
        public readonly int moduleId;
       
        public readonly GfxProcessorModule module;
        
        public readonly bool isAnimatable;

        public ArtGfxInstructionInfo(GfxProcessorModule module, int instructionId, bool isAnimatable)
        {
            this.module = module;
            this.moduleId = module.ModuleId;
            this.instructionId = instructionId;
            this.isAnimatable = isAnimatable;
        }

    
        public static ArtGfxInstructionInfo RegisterGfxInfo(GfxProcessorModule module, int instructionId, bool isAnimatable, ArtGfxInstructionInfo[] table)
        {
            table[instructionId] = new ArtGfxInstructionInfo(module, instructionId, isAnimatable);
            return table[instructionId];
        } 
    } 
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
    public class ArtSolidBrush : ArtColorBrush
    {
        Color color;
        public ArtSolidBrush(Color color)
        {
            this.color = color;
            this.myBrush = new SolidBrush(color);
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

        internal LinearGradientBrush CreateLinearGradientBrush()
        {
 
            if (colors.Count == 2)
            {
                return new System.Drawing.Drawing2D.LinearGradientBrush(
                     positions[0], positions[1], colors[0], colors[1]);
            }
            else if (colors.Count > 2)
            {
               
                return new System.Drawing.Drawing2D.LinearGradientBrush(
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