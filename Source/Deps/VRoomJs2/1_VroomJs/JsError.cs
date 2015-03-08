using System;
using System.Runtime.InteropServices;

namespace VroomJs
{
    [StructLayout(LayoutKind.Sequential)]
    struct JsError
    {
        public JsValue Type;
        public int Line;
        public int Column;
        public JsValue Resource;
        public JsValue Message;
        public JsValue Exception;
    }
}
