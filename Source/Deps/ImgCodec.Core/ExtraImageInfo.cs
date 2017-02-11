//MIT, 2016-2017, WinterDev

using System;
using ImageTools.Helpers;

namespace ImageTools
{

    public class ExtraImageInfo
    {

        /// <summary>
        /// The default animation speed, when the image is animated.
        /// </summary>
        public const int DefaultDelayTime = 10;
         
        private int _delayTime;
        /// <summary>
        /// If not 0, this field specifies the number of hundredths (1/100) of a second to 
        /// wait before continuing with the processing of the Data Stream. 
        /// The clock starts ticking immediately after the graphic is rendered. 
        /// This field may be used in conjunction with the User Input Flag field. 
        /// </summary>

        public int DelayTime
        {
            get
            {
                int delayTime = _delayTime;

                if (delayTime <= 0)
                {
                    delayTime = DefaultDelayTime;
                }

                return delayTime;
            }
            set { _delayTime = value; }
        }
    }
}