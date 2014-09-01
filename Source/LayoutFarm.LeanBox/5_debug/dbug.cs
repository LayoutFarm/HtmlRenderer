//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;



namespace LayoutFarm.Presentation
{
#if DEBUG

    public enum dbugLayoutMsgOwnerKind
    {
        Unknown,
        VisualElement,
        Layer,
        Line,
    }


    public class dbugLayoutMsgWriter
    {
        public List<dbugLayoutMsg> allMessages = new List<dbugLayoutMsg>();
        int currentIndentLevel;
        public dbugLayoutMsgWriter()
        {

        }
        public void Add(dbugLayoutMsg msg)
        {
            msg.indentLevel = this.currentIndentLevel;
            allMessages.Add(msg);
        }
        public void Add(ArtVisualElement ve, string msg)
        {
            this.Add(new dbugLayoutMsg(ve, msg));

        }
        public void EnterNewLevel()
        {
            this.currentIndentLevel++;
        }
        public void LeaveCurrentLevel()
        {
            this.currentIndentLevel--;
        }


    }
    public class dbugLayoutMsg
    {
        public dbugLayoutMsgOwnerKind msgOwnerKind;
        public object owner;
        public string msg;
        public int indentLevel;

        public dbugLayoutMsg(ArtVisualElement ve, string msg)
        {
            this.owner = ve;
            this.msg = msg;
            if (ve == null)
            {
                this.msgOwnerKind = dbugLayoutMsgOwnerKind.Unknown;
            }
            else
            {
                this.msgOwnerKind = dbugLayoutMsgOwnerKind.VisualElement;
            }
        }
        public dbugLayoutMsg(VisualLayer layer, string msg)
        {
            this.owner = layer;
            this.msg = msg;
            this.msgOwnerKind = dbugLayoutMsgOwnerKind.Layer;
        }
        public override string ToString()
        {
            return new string('.', indentLevel) + "[" + indentLevel +
                "]" + this.msg;
        }
    }


#endif


}