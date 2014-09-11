//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LayoutFarm.Presentation.Text
{

    partial class EditableVisualTextRun
    {
        internal EditableVisualTextRun Remove(int startIndex, int length, bool withFreeRun, VisualElementArgs vinv)
        {
            EditableVisualTextRun freeRun = null;
            if (startIndex > -1 && length > 0)
            {

                int oldLexLength = mybuffer.Length; char[] newBuff = new char[oldLexLength - length];
                if (withFreeRun)
                {
                    freeRun = MakeTextRun(startIndex, length);
                }
                if (startIndex > 0)
                {
                    Array.Copy(mybuffer, 0, newBuff, 0, startIndex);
                }

                Array.Copy(mybuffer, startIndex + length, newBuff, startIndex, oldLexLength - startIndex - length);

                this.mybuffer = newBuff;
                UpdateRunWidth(vinv);
            }

            if (withFreeRun)
            {
                return freeRun;
            }
            else
            {
                return null;
            }
        }
        public static EditableVisualTextRun InnerRemove(EditableVisualTextRun tt, int startIndex, int length, bool withFreeRun, VisualElementArgs vinv)
        {
            return tt.Remove(startIndex, length, withFreeRun, vinv);
        }
        public static EditableVisualTextRun InnerRemove(EditableVisualTextRun tt, int startIndex, bool withFreeRun, VisualElementArgs vinv)
        {

            return tt.Remove(startIndex, tt.CharacterCount - (startIndex), withFreeRun, vinv);
        }
        public static VisualLocationInfo InnerGetCharacterFromPixelOffset(EditableVisualTextRun tt, int pixelOffset)
        {
            return tt.GetCharacterFromPixelOffset(pixelOffset);
        }


        public int GetCharWidth(int index)
        {
            return GetCharacterWidth(mybuffer[index]);
        }
        int GetCharacterWidth(char c)
        {
            return GetTextFontInfo().GetCharWidth(c);
        }
        public VisualLocationInfo GetCharacterFromPixelOffset(int pixelOffset)
        {
            if (pixelOffset < Width)
            {
                char[] myBuffer = this.mybuffer;
                int j = myBuffer.Length;
                int accWidth = 0; for (int i = 0; i < j; i++)
                {
                    char c = myBuffer[i];

                    int charW = GetCharacterWidth(c);
                    if (accWidth + charW > pixelOffset)
                    {
                        if (pixelOffset - accWidth > 3)
                        {
                            return new VisualLocationInfo(accWidth + charW, i);
                        }
                        else
                        {
                            return new VisualLocationInfo(accWidth, i - 1);
                        }
                    }
                    else
                    {
                        accWidth += charW;
                    }
                }
                return new VisualLocationInfo(accWidth, j - 1);
            }
            else
            {
                return new VisualLocationInfo(0, -1);
            }

        }
        public EditableVisualTextRun LeftCopy(int index)
        {

            if (index > -1)
            {
                return MakeTextRun(0, index + 1);
            }
            else
            {
                return null;
            }
        }
        public void InsertAfter(int index, char c, VisualElementArgs vinv)
        {
            int oldLexLength = mybuffer.Length; char[] newBuff = new char[oldLexLength + 1];
            if (index > -1 && index < mybuffer.Length - 1)
            {
                Array.Copy(mybuffer, newBuff, index + 1); newBuff[index + 1] = c;
                Array.Copy(mybuffer, index + 1, newBuff, index + 2, oldLexLength - index - 1);
            }
            else if (index == -1)
            {
                newBuff[0] = c;
                Array.Copy(mybuffer, 0, newBuff, 1, mybuffer.Length);

            }
            else if (index == oldLexLength - 1)
            {
                Array.Copy(mybuffer, newBuff, oldLexLength);
                newBuff[oldLexLength] = c;

            }
            else
            {
                throw new NotSupportedException();
            }
            this.mybuffer = newBuff;
            UpdateRunWidth(vinv);

        }

    }
}