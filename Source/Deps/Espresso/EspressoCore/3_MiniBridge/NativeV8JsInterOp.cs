//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace Espresso
{


    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate void ManagedListenerDel(int mIndex,
      [MarshalAs(UnmanagedType.LPWStr)]string methodName,
      IntPtr args);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate void JsEngineSetupCallbackDel(IntPtr nativeJsEngine, IntPtr currentNativeJsContext);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate void ManagedMethodCallDel(int mIndex, int hint, IntPtr metArgs);

    class NativeRef : INativeRef
    {
        /// <summary>
        /// managed obj
        /// </summary>
        object wrapObject;
        /// <summary>
        /// manged side indeex
        /// </summary>
        int mIndex;
        /// <summary>
        /// unmanaged side 
        /// </summary>
        IntPtr unmanagedObjectPtr;

        public NativeRef(int mIndex, object wrapObject)
        {
            this.mIndex = mIndex;
            this.wrapObject = wrapObject;
        }
        public int ManagedIndex
        {
            get
            {
                return this.mIndex;
            }
        }
        public object WrapObject
        {
            get
            {
                return this.wrapObject;
            }
        }

        public bool HasNativeSide
        {
            get
            {
                return this.unmanagedObjectPtr != IntPtr.Zero;
            }
        }
        public void SetUnmanagedPtr(IntPtr unmanagedObjectPtr)
        {
            this.unmanagedObjectPtr = unmanagedObjectPtr;
        }
        public IntPtr UnmanagedPtr
        {
            get
            {
                return this.unmanagedObjectPtr;
            }
        }

    }


    /// <summary>
    /// instance of object with type definition
    /// </summary>
    class NativeJsInstanceProxy : NativeRef, INativeScriptable
    {
        JsTypeDefinition jsTypeDef;
        public NativeJsInstanceProxy(int mIndex, object wrapObject, JsTypeDefinition jsTypeDef)
            : base(mIndex, wrapObject)
        {
            this.jsTypeDef = jsTypeDef;
        }
        public IntPtr UnmanagedTypeDefinitionPtr
        {
            get { return this.jsTypeDef.nativeProxy.UnmanagedPtr; }
        }
    }

    class NativeObjectProxyStore
    {
        List<INativeRef> nativeRefList = new List<INativeRef>();
        JsContext ownerContext;

        Dictionary<object, NativeJsInstanceProxy> createdWrappers = new Dictionary<object, NativeJsInstanceProxy>();

        public NativeObjectProxyStore(JsContext ownerContext)
        {
            this.ownerContext = ownerContext;
        }


        public NativeJsInstanceProxy CreateProxyForObject(object o, JsTypeDefinition jsTypeDefinition)
        {
            NativeJsInstanceProxy found;
            if (this.createdWrappers.TryGetValue(o, out found))
            {
                return found;
            }

            var proxyObject = new NativeJsInstanceProxy(
                nativeRefList.Count,
                o,
                jsTypeDefinition);

            nativeRefList.Add(proxyObject);
            this.createdWrappers.Add(o, proxyObject);

            //register
            NativeV8JsInterOp.CreateNativePart(ownerContext, proxyObject);
            return proxyObject;
        }
        public INativeRef CreateProxyForTypeDefinition(JsTypeDefinition jsTypeDefinition)
        {

            var proxyObject = new NativeRef(nativeRefList.Count, jsTypeDefinition);
            //store data this side too
            jsTypeDefinition.nativeProxy = proxyObject;
            //store in exported list
            nativeRefList.Add(proxyObject);
            //register type definition
            NativeV8JsInterOp.RegisterTypeDef(ownerContext, jsTypeDefinition);
            return proxyObject;
        }
        public void Dispose()
        {

            int j = nativeRefList.Count;
            for (int i = nativeRefList.Count - 1; i > -1; --i)
            {
                NativeV8JsInterOp.UnRegisterNativePart(nativeRefList[i]);
            }
            nativeRefList.Clear();
        }
        public INativeRef GetProxyObject(int index)
        {
            return this.nativeRefList[index];
        }
    }


    enum ManagedCallbackKind
    {
        Listener,
        MethodCall,
    }


    static class NativeV8JsInterOp
    {
        //basic 

        static ManagedListenerDel engineListenerDel;
       

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TestCallBack();

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void V8Init();

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr CreateWrapperForManagedObject(IntPtr unmanagedEnginePtr, int mIndex, IntPtr rtTypeDefinition);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetManagedIndex(IntPtr unmanagedPtr);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern int RelaseWrapper(IntPtr unmanagedPtr);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterManagedCallback(IntPtr funcPointer, int callBackKind);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void ContextRegisterManagedCallback(IntPtr contextPtr, IntPtr funcPointer, int callBackKind);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void ContextRegisterManagedCallback2(IntPtr contextSetupArgs, IntPtr funcPointer, int callBackKind);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe IntPtr ContextRegisterTypeDefinition(
            IntPtr nativeJsContextPtr, int mIndex,
            void* stream, int length);

        //---------------------------------------------------------------------------------
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArgCount(IntPtr callingArgsPtr);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern JsValue ArgGetThis(IntPtr callingArgsPtr);



        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern JsValue ArgGetObject(IntPtr callingArgsPtr, int index);
        //---------------------------------------------------------------------------------

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResultSetString(IntPtr callingArgsPtr, [MarshalAs(UnmanagedType.LPWStr)] string value);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResultSetBool(IntPtr callingArgsPtr, bool value);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResultSetInt32(IntPtr callingArgsPtr, int value);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResultSetDouble(IntPtr callingArgsPtr, double value);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResultSetFloat(IntPtr callingArgsPtr, float value);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ResultSetJsValue(IntPtr callingArgsPtr, JsValue jsvalue);


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseWrapper(IntPtr externalManagedHandler);


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExecNode(string[] args);


        static NativeV8JsInterOp()
        {
            //prepare 
            engineListenerDel = new ManagedListenerDel(EngineListener_Listen);
           
        }

        static void RegisterManagedListener(ManagedListenerDel mListenerDel)
        {
            RegisterManagedCallback(
                 System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(mListenerDel),
                (int)ManagedCallbackKind.Listener);
        }
       
        internal static void CtxRegisterManagedMethodCall(JsContext jsContext, ManagedMethodCallDel mMethodCall)
        {
            //register managed method to js context
            ContextRegisterManagedCallback(
                jsContext.Handle.Handle,
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(mMethodCall),
                (int)ManagedCallbackKind.MethodCall);
        }

        public static void RegisterCallBacks()
        {
            //------------------
            //built in listener
            //------------------
            NativeV8JsInterOp.RegisterManagedListener(engineListenerDel);

        }
        static void EngineListener_Listen(int mIndex, string methodName, IntPtr args)
        {

        }

        public static unsafe void RegisterTypeDef(JsContext context, JsTypeDefinition jsTypeDefinition)
        {

            INativeRef proxObject = jsTypeDefinition.nativeProxy;
            byte[] finalBuffer = null;
            using (MemoryStream ms = new MemoryStream())
            {
                //serialize with our custom protocol
                //plan change to json ?

                //utf16
                BinaryWriter binWriter = new BinaryWriter(ms, System.Text.Encoding.Unicode);
                //binay format 
                //1. typename
                //2. fields
                //3. method
                //4. indexer get/set   
                binWriter.Write((short)1);//start marker


                context.CollectionTypeMembers(jsTypeDefinition);
                //------------------------------------------------

                jsTypeDefinition.WriteDefinitionToStream(binWriter);
                //------------------------------------------------
                finalBuffer = ms.ToArray();

                fixed (byte* tt = &finalBuffer[0])
                {
                    proxObject.SetUnmanagedPtr(
                        ContextRegisterTypeDefinition(
                        context.Handle.Handle,
                        0, tt, finalBuffer.Length));
                }

                //ms.Close();
            }
        }
        public static void CreateNativePart(JsContext context, INativeScriptable proxyObj)
        {
            if (!proxyObj.HasNativeSide)
            {
                proxyObj.SetUnmanagedPtr(
                    CreateWrapperForManagedObject(
                        context.Handle.Handle,
                        proxyObj.ManagedIndex,
                        proxyObj.UnmanagedTypeDefinitionPtr));
            }
        }
        public static void UnRegisterNativePart(INativeRef proxyObj)
        {
            if (proxyObj.HasNativeSide)
            {
                ReleaseWrapper(proxyObj.UnmanagedPtr);
                proxyObj.SetUnmanagedPtr(IntPtr.Zero);
            }
        }
        //public static int GetManagedIndexFromNativePart(INativeRef proxyObj)
        //{
        //    return GetManagedIndex(proxyObj.UnmanagedPtr);
        //}
    }


}