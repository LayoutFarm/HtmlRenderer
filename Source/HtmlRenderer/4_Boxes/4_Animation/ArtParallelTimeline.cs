//BSD 2014, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;

namespace HtmlRenderer.Drawing.Art
{   
    public class ArtParallelTimeline
    {  
        public List<ArtTimelineSeries> animationLines = new List<ArtTimelineSeries>();    
        public int endFrame; 
        public ArtParallelTimeline()
        { 
        } 
        public void Add(ArtTimelineSeries artAnimationline)
        {
            animationLines.Add(artAnimationline);
        } 
        public int FrameDuration
        {
            get
            {
                return endFrame + 1;
            }
            set
            {
                endFrame = value - 1;

            }
        }
        public int EndFrame
        {
            get
            {
                return endFrame;
            }

        } 
    }
}
