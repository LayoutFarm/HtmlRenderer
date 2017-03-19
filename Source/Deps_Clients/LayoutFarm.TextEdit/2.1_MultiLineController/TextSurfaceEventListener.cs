//Apache2, 2014-2017, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    public class TextDomEventArgs : EventArgs
    {
        public bool updateJustCurrentLine;
        public readonly UIKeys key;
        public readonly char c;
        public bool PreventDefault;
        public TextDomEventArgs(char c)
        {
            this.c = c;
        }
        public TextDomEventArgs(UIKeys key)
        {
            this.key = key;
        }
        public TextDomEventArgs(bool updateJustCurrentLine)
        {
            this.updateJustCurrentLine = updateJustCurrentLine;
        }
    }

    public sealed class TextSurfaceEventListener
    {
        TextEditRenderBox targetTextSurface;
        char[] previewKeyDownRegisterChars;
        public event EventHandler<TextDomEventArgs> PreviewArrowKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewEnterKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewBackSpaceKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewRegisteredKeyDown;
        public event EventHandler<TextDomEventArgs> CharacterAdded;
        public event EventHandler<TextDomEventArgs> CharacterRemoved;
        public event EventHandler<TextDomEventArgs> CharacterReplaced;
        public event EventHandler<TextDomEventArgs> ReplacedAll;
        public event EventHandler<TextDomEventArgs> ArrowKeyCaretPosChanged;
        public event EventHandler<TextDomEventArgs> KeyDown;
        public event EventHandler<UIKeyEventArgs> SpecialKeyInserted;
        public event EventHandler<UIKeyEventArgs> SplitedNewLine;
        public TextSurfaceEventListener()
        {
        }

        public char[] PreviewKeydownRegisterChars
        {
            get
            {
                return previewKeyDownRegisterChars;
            }
            set
            {
                previewKeyDownRegisterChars = value;
            }
        }
        public TextEditRenderBox TextSurfaceElement
        {
            get
            {
                return targetTextSurface;
            }
        }
        public void SetMonitoringTextSurface(TextEditRenderBox textSurfaceElement)
        {
            this.targetTextSurface = textSurfaceElement;
        }
        internal static bool NotifyPreviewEnter(TextSurfaceEventListener listener)
        {
            if (listener.PreviewEnterKeyDown != null)
            {
                TextDomEventArgs e = new TextDomEventArgs(UIKeys.Enter);
                listener.PreviewEnterKeyDown(listener, e);
                return e.PreventDefault;
            }
            return false;
        }
        internal static bool NotifyPreviewBackSpace(TextSurfaceEventListener listener)
        {
            if (listener.PreviewBackSpaceKeyDown != null)
            {
                TextDomEventArgs e = new TextDomEventArgs(UIKeys.Back);
                listener.PreviewBackSpaceKeyDown(listener, e);
                return e.PreventDefault;
            }
            return false;
        }
        internal static bool NotifyPreviewArrow(TextSurfaceEventListener listener, UIKeys key)
        {
            if (listener.PreviewArrowKeyDown != null)
            {
                TextDomEventArgs e = new TextDomEventArgs(key);
                listener.PreviewArrowKeyDown(listener, e);
                return e.PreventDefault;
            }
            return false;
        }
        internal static bool NotifyPreviewKeydown(TextSurfaceEventListener listener, char c)
        {
            if (listener.IsRegisterPreviewKeyDownChar(c))
            {
                if (listener.PreviewRegisteredKeyDown != null)
                {
                    TextDomEventArgs e = new TextDomEventArgs(c);
                    listener.PreviewRegisteredKeyDown(listener, e);
                    return e.PreventDefault;
                }
            }
            return false;
        }
        internal static void NotifyArrowKeyCaretPosChanged(TextSurfaceEventListener listener, UIKeys key)
        {
            if (listener.ArrowKeyCaretPosChanged != null)
            {
                listener.ArrowKeyCaretPosChanged(listener, new TextDomEventArgs(key));
            }
        }
        bool IsRegisterPreviewKeyDownChar(char c)
        {
            if (previewKeyDownRegisterChars != null)
            {
                for (int i = previewKeyDownRegisterChars.Length - 1; i > -1; --i)
                {
                    if (previewKeyDownRegisterChars[i] == c)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static void NotifyCharacterAdded(TextSurfaceEventListener listener, char c)
        {
            if (listener.CharacterAdded != null)
            {
                listener.CharacterAdded(listener, new TextDomEventArgs(c));
            }
        }

        internal static void NotifyCharactersReplaced(TextSurfaceEventListener listener, char c)
        {
            if (listener.CharacterReplaced != null)
            {
                listener.CharacterReplaced(listener, new TextDomEventArgs(c));
            }
        }
        internal static void NotifyCharactersRemoved(TextSurfaceEventListener listener, TextDomEventArgs e)
        {
            if (listener.CharacterRemoved != null)
            {
                listener.CharacterRemoved(listener, e);
            }
        }
        internal static void NotifyKeyDown(TextSurfaceEventListener listener, UIKeys key)
        {
            if (listener.KeyDown != null)
            {
                listener.KeyDown(listener, new TextDomEventArgs(key));
            }
        }
        internal static void NofitySplitNewLine(TextSurfaceEventListener listener, UIKeyEventArgs e)
        {
            if (listener.SplitedNewLine != null)
            {
                listener.SplitedNewLine(listener, e);
            }
        }
        internal static void NotifyReplaceAll(TextSurfaceEventListener listener, TextDomEventArgs e)
        {
            if (listener.ReplacedAll != null)
            {
                listener.ReplacedAll(listener, e);
            }
        }

        internal static void NotifyFunctionKeyDown(TextSurfaceEventListener listener, UIKeys key)
        {
        }
    }
}