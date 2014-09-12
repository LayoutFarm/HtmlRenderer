//2014 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace LayoutFarm.Presentation
{
    public static class ArtVisualElementUtils
    {
        static void vinv_dbug_WriteInfo(dbugVisitorMessage msg, object o)
        {

        }


        public static TextRunStyle CreateSimpleRole(Color color)
        {
            TextRunStyle beh = new TextRunStyle();
            beh.SharedBgColorBrush = new ArtSolidBrush(color);
            return beh;
        }

        static Size ReCalculateContentSizeVerticalStack(LinkedList<RenderElement> velist)
        {
            int local_desiredWidth = 0;
            int local_desiredHeight = 0;

            LinkedListNode<RenderElement> curNode = velist.First;
            while (curNode != null)
            {

                RenderElement visualElement = curNode.Value;
                if (!visualElement.HasCalculatedSize)
                {
                    visualElement.TopDownReCalculateContentSize();
                }
#if DEBUG
                else
                {
                    vinv_dbug_WriteInfo(dbugVisitorMessage.SKIP_CAL, visualElement);
                }
#endif
                if (local_desiredWidth < visualElement.ElementDesiredWidth)
                {
                    local_desiredWidth = visualElement.ElementDesiredWidth;
                }
                local_desiredHeight += visualElement.ElementDesiredHeight;
                curNode = curNode.Next;
            }
            return new Size(local_desiredWidth, local_desiredHeight);
        }

        static Size ReCalculateContentSizeHorizontalStack(LinkedList<RenderElement> velist)
        {

            int local_desiredWidth = 0;
            int local_desiredHeight = 17;
            LinkedListNode<RenderElement> curNode = velist.First;
            while (curNode != null)
            {
                RenderElement visualElement = curNode.Value;
                if (!visualElement.HasCalculatedSize)
                {
                    visualElement.TopDownReCalculateContentSize();
                }
#if DEBUG
                else
                {

                    vinv_dbug_WriteInfo(dbugVisitorMessage.SKIP, visualElement);

                }
#endif

                if (local_desiredHeight < visualElement.ElementDesiredHeight)
                {
                    local_desiredHeight = visualElement.ElementDesiredHeight;
                }
                local_desiredWidth += visualElement.ElementDesiredWidth;
                curNode = curNode.Next;
            }

            return new Size(local_desiredWidth, local_desiredHeight);
        }

        public static Size GetFitWidthSize(Size contentSize, int containerWidth, bool maintainRatio)
        {
            if (maintainRatio)
            {
                if (contentSize == Size.Empty)
                {
                    contentSize = new Size(containerWidth, containerWidth);
                }


                double client_width = (double)contentSize.Width;
                double client_height = (double)contentSize.Height;
                double ratio = client_width / client_height;
                contentSize.Width = containerWidth; contentSize.Height = (int)(contentSize.Width / ratio); return contentSize;
            }
            else
            {
                contentSize.Width = containerWidth;
                return contentSize;
            }
        }
        public static Size GetFitHeightSize(Size contentSize, int containerHeight, bool maintainRatio)
        {
            if (maintainRatio)
            {
                if (contentSize == Size.Empty)
                {
                    contentSize = new Size(containerHeight, containerHeight);
                }


                double client_width = (double)contentSize.Width;
                double client_height = (double)contentSize.Height;
                double ratio = client_width / client_height;
                contentSize.Height = containerHeight; contentSize.Width = (int)(contentSize.Height * ratio);
                return contentSize;
            }
            else
            {
                contentSize.Height = containerHeight;
                return contentSize;
            }

        }
        public static Size GetFitRectSize(Size contentSize, Size containerSize, bool maintainRatio)
        {
            if (maintainRatio)
            {
                if (contentSize == Size.Empty)
                {
                    contentSize = containerSize;
                }
                double client_width = (double)contentSize.Width;
                double client_height = (double)contentSize.Height;
                double ratio = client_width / client_height;
                if (contentSize.Width > contentSize.Height)
                {
                    contentSize.Width = containerSize.Width; contentSize.Height = (int)(contentSize.Width / ratio);
                    if (contentSize.Height > containerSize.Height)
                    {
                        contentSize.Height = containerSize.Height;
                        contentSize.Width = (int)(ratio * contentSize.Height);
                    }
                    return contentSize;
                }
                else if (contentSize.Width < contentSize.Height)
                {
                    contentSize.Height = containerSize.Height; contentSize.Width = (int)(contentSize.Height * ratio);
                    if (contentSize.Width > containerSize.Width)
                    {
                        contentSize.Width = containerSize.Width;
                        contentSize.Height = (int)(contentSize.Width / ratio);
                    }
                    return contentSize;
                }
                else
                {
                    return containerSize;

                }
            }
            else
            {
                return containerSize;
            }
        }

    }
}