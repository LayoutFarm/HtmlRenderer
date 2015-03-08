//2013 MIT, Federico Di Gregorio <fog@initd.org>
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace VroomJs
{
    partial class JsEngine : IDisposable
    {

        delegate void KeepaliveRemoveDelegate(int context, int slot);
        delegate JsValue KeepAliveGetPropertyValueDelegate(int context, int slot, [MarshalAs(UnmanagedType.LPWStr)] string name);
        delegate JsValue KeepAliveSetPropertyValueDelegate(int context, int slot, [MarshalAs(UnmanagedType.LPWStr)] string name, JsValue value);
        delegate JsValue KeepAliveValueOfDelegate(int context, int slot);
        delegate JsValue KeepAliveInvokeDelegate(int context, int slot, JsValue args);
        delegate JsValue KeepAliveDeletePropertyDelegate(int context, int slot, [MarshalAs(UnmanagedType.LPWStr)] string name);
        delegate JsValue KeepAliveEnumeratePropertiesDelegate(int context, int slot);


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void js_set_object_marshal_type(JsObjectMarshalType objectMarshalType);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void js_dump_allocated_items();

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr jsengine_new(
            KeepaliveRemoveDelegate keepaliveRemove,
            KeepAliveGetPropertyValueDelegate keepaliveGetPropertyValue,
            KeepAliveSetPropertyValueDelegate keepaliveSetPropertyValue,
            KeepAliveValueOfDelegate keepaliveValueOf,
            KeepAliveInvokeDelegate keepaliveInvoke,
            KeepAliveDeletePropertyDelegate keepaliveDeleteProperty,
            KeepAliveEnumeratePropertiesDelegate keepaliveEnumerateProperties,
            int maxYoungSpace,
            int maxOldSpace
        );
       
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jsengine_terminate_execution(HandleRef engine);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jsengine_dump_heap_stats(HandleRef engine);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jsengine_dispose(HandleRef engine);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jsengine_dispose_object(HandleRef engine, IntPtr obj);
    }
}