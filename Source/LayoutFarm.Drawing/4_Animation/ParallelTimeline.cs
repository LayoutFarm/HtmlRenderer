//BSD 2014, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;

namespace LayoutFarm.Drawing.Animation
{   
    public class ParallelTimeline
    {  
        public List<TimelineSeriesBase> animationLines = new List<TimelineSeriesBase>();    
        public int endFrame; 
        public ParallelTimeline()
        { 
        } 
        public void Add(TimelineSeriesBase artAnimationline)
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
