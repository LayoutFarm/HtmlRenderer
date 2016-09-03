//2013 MIT, Federico Di Gregorio <fog@initd.org>
using System;
using System.Runtime.InteropServices;

namespace Espresso
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
