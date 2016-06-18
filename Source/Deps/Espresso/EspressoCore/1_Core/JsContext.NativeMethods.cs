//2013 MIT, Federico Di Gregorio<fog@initd.org>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace VroomJs
{
    partial class JsContext
    {
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int getVersion();
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr jscontext_new(int id, HandleRef engine);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jscontext_dispose(HandleRef engine);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jscontext_force_gc();
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern JsValue jscontext_execute(HandleRef context,
            [MarshalAs(UnmanagedType.LPWStr)] string str,
            [MarshalAs(UnmanagedType.LPWStr)] string name);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern JsValue jscontext_execute_script(HandleRef context, HandleRef script);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern JsValue jscontext_get_global(HandleRef engine);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern JsValue jscontext_get_variable(HandleRef engine, [MarshalAs(UnmanagedType.LPWStr)] string name);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern JsValue jscontext_set_variable(HandleRef engine, [MarshalAs(UnmanagedType.LPWStr)] string name, JsValue value);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static internal extern JsValue jsvalue_alloc_string([MarshalAs(UnmanagedType.LPWStr)] string str);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static internal extern JsValue jsvalue_alloc_array(int length);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static internal extern void jsvalue_dispose(JsValue value);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static internal extern JsValue jscontext_invoke(HandleRef engine, IntPtr funcPtr, IntPtr thisPtr, JsValue args);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern JsValue jscontext_get_property_names(HandleRef engine, IntPtr ptr);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern JsValue jscontext_get_property_value(HandleRef engine, IntPtr ptr, [MarshalAs(UnmanagedType.LPWStr)] string name);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern JsValue jscontext_set_property_value(HandleRef engine, IntPtr ptr, [MarshalAs(UnmanagedType.LPWStr)] string name, JsValue value);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern JsValue jscontext_invoke_property(HandleRef engine, IntPtr ptr, [MarshalAs(UnmanagedType.LPWStr)] string name, JsValue args);
    }
}
