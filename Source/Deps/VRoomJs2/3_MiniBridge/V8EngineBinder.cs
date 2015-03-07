//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using VroomJs;

namespace NativeV8
{


    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void ManagedListenerDel(int mIndex,
        [MarshalAs(UnmanagedType.LPWStr)]string methodName,
        IntPtr args);


    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void ManagedMethodCallDel(int mIndex, int hint, IntPtr metArgs);



    public class JsTypeDefinition : JsTypeMemberDefinition
    {
        //store definition for js
        List<JsFieldDefinition> fields = new List<JsFieldDefinition>();
        List<JsMethodDefinition> methods = new List<JsMethodDefinition>();
        List<JsPropertyDefinition> props = new List<JsPropertyDefinition>();

        public JsTypeDefinition(string typename)
            : base(typename, JsMemberKind.Type)
        {

        }
        public void AddMember(JsFieldDefinition fieldDef)
        {
            fieldDef.SetOwner(this);
            fields.Add(fieldDef);
        }
        public void AddMember(JsMethodDefinition methodDef)
        {
            methodDef.SetOwner(this);
            methods.Add(methodDef);
        }
        public void AddMember(JsPropertyDefinition propDef)
        {
            propDef.SetOwner(this);
            props.Add(propDef);

        }
        /// <summary>
        /// serialization this typedefinition to binary format and 
        /// send to native side
        /// </summary>
        /// <param name="writer"></param>
        internal void WriteDefinitionToStream(BinaryWriter writer)
        {
            //----------------------
            //this is our custom protocol/convention with the MiniJsBridge            
            //we may change this in the future
            //eg. use json serialization/deserialization 
            //----------------------

            //1. kind/flags
            writer.Write((short)this.MemberId);
            //2. member id
            writer.Write((short)0);
            //3. typename                         
            WriteUtf16String(this.MemberName, writer);

            //4. num of field
            int j = fields.Count;
            writer.Write((short)j);
            for (int i = 0; i < j; ++i)
            {
                JsFieldDefinition fielddef = fields[i];
                //field flags
                writer.Write((short)0);

                //*** field id -- unique field id within one type
                writer.Write((short)fielddef.MemberId);

                //field name
                WriteUtf16String(fielddef.MemberName, writer);
            }
            //------------------
            j = methods.Count;
            writer.Write((short)j);
            for (int i = 0; i < j; ++i)
            {
                JsMethodDefinition methoddef = methods[i];
                //method flags
                writer.Write((short)0);
                //id
                writer.Write((short)methoddef.MemberId);
                //method name
                WriteUtf16String(methoddef.MemberName, writer);
            }

            //property
            j = props.Count;
            writer.Write((short)j);
            for (int i = 0; i < j; ++i)
            {
                JsPropertyDefinition property = this.props[i];
                //flags
                writer.Write((short)0);
                //id
                writer.Write((short)property.MemberId);
                //name
                WriteUtf16String(property.MemberName, writer);
            }

        }

        internal List<JsFieldDefinition> GetFields()
        {
            return this.fields;
        }
        internal List<JsMethodDefinition> GetMethods()
        {
            return this.methods;
        }
        internal List<JsPropertyDefinition> GetProperties()
        {
            return this.props;
        }
    }
    public abstract class JsTypeMemberDefinition
    {
        string mbname;
        JsMemberKind memberKind;
        JsTypeDefinition owner;
        int memberId;
        internal NativeObjectProxy nativeProxy;
        public JsTypeMemberDefinition(string mbname, JsMemberKind memberKind)
        {
            this.mbname = mbname;
            this.memberKind = memberKind;
        }
        public bool IsRegisterd
        {
            get
            {
                return this.nativeProxy != null;
            }
        }

        public string MemberName
        {
            get
            {
                return this.mbname;
            }
        }
        public JsMemberKind MemberKind
        {
            get
            {
                return this.memberKind;
            }
        }
        public void SetOwner(JsTypeDefinition owner)
        {
            this.owner = owner;
        }
        protected static void WriteUtf16String(string str, BinaryWriter writer)
        {
            char[] charBuff = str.ToCharArray();
            writer.Write((short)charBuff.Length);
            writer.Write(charBuff);
        }

        public int MemberId
        {
            get
            {
                return this.memberId;
            }
        }
        public void SetMemberId(int memberId)
        {
            this.memberId = memberId;
        }

    }
    public enum JsMemberKind
    {
        Field,
        Method,
        Event,
        Property,
        Indexer,
        PropertyGet,
        PropertySet,
        IndexerGet,
        IndexerSet,
        Type
    }

    public class JsFieldDefinition : JsTypeMemberDefinition
    {
        public JsFieldDefinition(string fieldname)
            : base(fieldname, JsMemberKind.Field)
        {

        }
    }

    public class JsPropertyDefinition : JsTypeMemberDefinition
    {
        JsPropertyGetDefinition getter;
        JsPropertyGetDefinition setter;
        System.Reflection.PropertyInfo propInfo;

        public JsPropertyDefinition(string name)
            : base(name, JsMemberKind.Property)
        {

        }
        public JsPropertyDefinition(string name, JsMethodCallDel getter, JsMethodCallDel setter)
            : base(name, JsMemberKind.Property)
        {

            if (getter != null)
            {
                this.GetterMethod = new JsPropertyGetDefinition(name, getter);
            }
            if (setter != null)
            {
                this.SetterMethod = new JsPropertySetDefinition(name, setter);
            }
        }
        public JsPropertyDefinition(string name, System.Reflection.PropertyInfo propInfo)
            : base(name, JsMemberKind.Property)
        {

            this.propInfo = propInfo;
            var getter = propInfo.GetGetMethod();
            if (getter != null)
            {
                this.GetterMethod = new JsPropertyGetDefinition(name, getter);
            }
            var setter = propInfo.GetSetMethod();
            if (setter != null)
            {
                this.SetterMethod = new JsPropertySetDefinition(name, setter);
            }
        }
        public JsPropertyGetDefinition GetterMethod
        {
            get;
            set;
        }
        public JsPropertySetDefinition SetterMethod
        {
            get;
            set;
        }
        public bool IsIndexer { get; set; }
    }

    public class JsPropertyGetDefinition : JsMethodDefinition
    {
        public JsPropertyGetDefinition(string name)
            : base(name)
        {
        }
        public JsPropertyGetDefinition(string name, JsMethodCallDel getter)
            : base(name, getter)
        {
        }
        public JsPropertyGetDefinition(string name, System.Reflection.MethodInfo getterMethod)
            : base(name, getterMethod)
        {
        }
    }

    public class JsPropertySetDefinition : JsMethodDefinition
    {
        public JsPropertySetDefinition(string name)
            : base(name)
        {
        }
        public JsPropertySetDefinition(string name, JsMethodCallDel setter)
            : base(name, setter)
        {
        }
        public JsPropertySetDefinition(string name, System.Reflection.MethodInfo setterMethod)
            : base(name, setterMethod)
        {
        }
    }



    public delegate void JsMethodCallDel(ManagedMethodArgs args);

    public struct ManagedMethodArgs
    {
        IntPtr metArgsPtr;
        JsContext context;
        public ManagedMethodArgs(JsContext context, IntPtr metArgsPtr)
        {
            this.context = context;
            this.metArgsPtr = metArgsPtr;
        }
        public int ArgCount
        {
            get
            {
                return NativeV8JsInterOp.ArgCount(this.metArgsPtr);
            }
        }

        public object GetThisArg()
        {
            var value = NativeV8JsInterOp.ArgGetThis(this.metArgsPtr);
            return this.context.Converter.FromJsValue(value);

            //var value = NativeV8JsInterOp.ArgGetThis(this.metArgsPtr);
            //switch (value.Type)
            //{
            //    case JsValueType.Managed:
            //        return this.context.KeepAliveGet(value.Index);
            //    case JsValueType.JsTypeWrap:
            //        return this.context.GetObjectProxy(value.Index);
            //}

            //return null;
        }
        public object GetArgAsObject(int index)
        {
            var value = NativeV8JsInterOp.ArgGetObject(this.metArgsPtr, index);
            return this.context.Converter.FromJsValue(value);
        }
         
        //--------------------------------------------------------------------
        public void SetResult(bool value)
        {
            NativeV8JsInterOp.ResultSetBool(metArgsPtr, value);
        }
        public void SetResult(int value)
        {
            NativeV8JsInterOp.ResultSetInt32(metArgsPtr, value);
        }
        public void SetResult(string value)
        {
            NativeV8JsInterOp.ResultSetString(metArgsPtr, value);
        }
        public void SetResult(double value)
        {
            NativeV8JsInterOp.ResultSetDouble(metArgsPtr, value);
        }
        public void SetResult(float value)
        {
            NativeV8JsInterOp.ResultSetFloat(metArgsPtr, value);
        }
        public void SetResultNull()
        {
            NativeV8JsInterOp.ResultSetJsValue(metArgsPtr,
                this.context.Converter.ToJsValueNull());
        }
        public void SetResultObj(object result)
        {
            NativeV8JsInterOp.ResultSetJsValue(metArgsPtr,
                this.context.Converter.AnyToJsValue(result));
        }

        public void SetResultObj(object result, JsTypeDefinition jsTypeDef)
        {
            if (!jsTypeDef.IsRegisterd)
            {
                this.context.RegisterTypeDefinition(jsTypeDef);
            }

            var proxy = this.context.CreateWrapper(result, jsTypeDef);

            NativeV8JsInterOp.ResultSetJsValue(metArgsPtr,
               this.context.Converter.ToJsValue(proxy));
        }
        public void SetResultAutoWrap<T>(T result)
            where T : class,new()
        {

            var jsTypeDef = this.context.GetJsTypeDefinition<T>(result);
            var proxy = this.context.CreateWrapper(result, jsTypeDef);

            NativeV8JsInterOp.ResultSetJsValue(metArgsPtr,
               this.context.Converter.ToJsValue(proxy));
        }
    }


    public class JsMethodDefinition : JsTypeMemberDefinition
    {

        JsMethodCallDel methodCallDel;
        System.Reflection.MethodInfo method;
        public JsMethodDefinition(string methodName)
            : base(methodName, JsMemberKind.Method)
        {
        }
        public JsMethodDefinition(string methodName, JsMethodCallDel methodCallDel)
            : base(methodName, JsMemberKind.Method)
        {
            this.methodCallDel = methodCallDel;
        }
        public JsMethodDefinition(string methodName, System.Reflection.MethodInfo method)
            : base(methodName, JsMemberKind.Method)
        {
            this.method = method;
        }
        public void InvokeMethod(ManagedMethodArgs args)
        {
            if (method != null)
            {
                //invoke method
                var thisArg = args.GetThisArg() as NativeJsInstanceProxy;
                int argCount = args.ArgCount;
                object[] parameters = new object[argCount];
                for (int i = 0; i < argCount; ++i)
                {
                    parameters[i] = args.GetArgAsObject(i);
                }
                object result = this.method.Invoke(thisArg.WrapObject, parameters);
                args.SetResultObj(result);
            }
            else
            {
                methodCallDel(args);
            }
        }
    }






    public abstract class NativeObjectProxy
    {
        object wrapObject;
        int mIndex;//managed index
        IntPtr unmanagedObjectPtr;
        public NativeObjectProxy(int mIndex, object wrapObject)
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

        public bool HasNativeWrapperPart
        {
            get
            {
                return this.unmanagedObjectPtr != IntPtr.Zero;
            }
        }
        public void SetUnmanagedObjectPointer(IntPtr unmanagedObjectPtr)
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


    class NativeObjectProxy<T> : NativeObjectProxy
        where T : class
    {
        public NativeObjectProxy(int mIndex, T wrapObject)
            : base(mIndex, wrapObject)
        {

        }
    }

    /// <summary>
    /// instance of object with type definition
    /// </summary>
    public class NativeJsInstanceProxy : NativeObjectProxy
    {
        JsTypeDefinition jsTypeDef;
        public NativeJsInstanceProxy(int mIndex, object wrapObject, JsTypeDefinition jsTypeDef)
            : base(mIndex, wrapObject)
        {
            this.jsTypeDef = jsTypeDef;

        }
        public JsTypeDefinition JsTypeDefinition
        {
            get
            {
                return this.jsTypeDef;
            }
        }
    }

    class NativeObjectProxyStore
    {
        List<NativeObjectProxy> exportList = new List<NativeObjectProxy>();
        JsContext ownerContext;
        public NativeObjectProxyStore(JsContext ownerContext)
        {
            this.ownerContext = ownerContext;
        }
        public NativeJsInstanceProxy CreateProxyForObject(object o, JsTypeDefinition jsTypeDefinition)
        {
            NativeJsInstanceProxy proxyObject = new NativeJsInstanceProxy(
                exportList.Count,
                o,
                jsTypeDefinition);

            exportList.Add(proxyObject);

            //register
            NativeV8JsInterOp.CreateNativePart(ownerContext, proxyObject);
            return proxyObject;
        }
        public NativeObjectProxy CreateProxyForTypeDefinition(JsTypeDefinition jsTypeDefinition)
        {

            NativeObjectProxy<JsTypeDefinition> proxyObject = new NativeObjectProxy<JsTypeDefinition>(exportList.Count, jsTypeDefinition);
            //store data this side too
            jsTypeDefinition.nativeProxy = proxyObject;
            //store in exported list
            exportList.Add(proxyObject);
            //register type definition
            NativeV8JsInterOp.RegisterTypeDef(ownerContext, jsTypeDefinition);
            return proxyObject;
        }
        public void Dispose()
        {

            int j = exportList.Count;
            for (int i = exportList.Count - 1; i > -1; --i)
            {
                NativeV8JsInterOp.UnRegisterNativePart(exportList[i]);
            }
            exportList.Clear();
        }
        public NativeObjectProxy GetProxyObject(int index)
        {
            return this.exportList[index];
        }
    }





    public static class NativeV8JsInterOp
    {
        //basic 
        static IntPtr hModuleV8;
        static ManagedListenerDel engineListenerDel;




        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TestCallBack();


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr CreateWrapperForManagedObject(IntPtr unmanagedEnginePtr, int mIndex, IntPtr rtTypeDefinition);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetManagedIndex(IntPtr unmanagedPtr);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern int RelaseWrapper(IntPtr unmanagedPtr);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.StdCall)]
        static extern void RegisterManagedCallback(IntPtr funcPointer, int callBackKind);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.StdCall)]
        static extern void ContextRegisterManagedCallback(IntPtr contextPtr, IntPtr funcPointer, int callBackKind);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe IntPtr ContextRegisterTypeDefinition(
            IntPtr unmanagedEnginePtr, int mIndex,
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
            ContextRegisterManagedCallback(
                jsContext.Handle.Handle,
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(mMethodCall),
                (int)ManagedCallbackKind.MethodCall);
        }
        public static void LoadV8(string v8bridgeDll)
        {

            IntPtr v8mod = UnsafeNativeMethods.LoadLibrary(v8bridgeDll);
            hModuleV8 = v8mod;
            if (v8mod == IntPtr.Zero)
            {
                return;
            }
        }
        public static void RegisterCallBacks()
        {
            //------------------
            //built in listener
            //------------------
            NativeV8JsInterOp.RegisterManagedListener(engineListenerDel);
            //NativeV8JsInterOp.RegisterManagedMethodCall(engineMethodCallbackDel);
        }

        public static void UnloadV8()
        {
            if (hModuleV8 != IntPtr.Zero)
            {
                UnsafeNativeMethods.FreeLibrary(hModuleV8);
                hModuleV8 = IntPtr.Zero;
            }
        }

        static void EngineListener_Listen(int mIndex, string methodName, IntPtr args)
        {

        }

        public static unsafe void RegisterTypeDef(JsContext context, JsTypeDefinition jsTypeDefinition)
        {

            NativeObjectProxy proxObject = jsTypeDefinition.nativeProxy;
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
                    proxObject.SetUnmanagedObjectPointer(
                        ContextRegisterTypeDefinition(
                        context.Handle.Handle,
                        0, tt, finalBuffer.Length));
                }

                ms.Close();
            }
        }
        public static void CreateNativePart(JsContext context, NativeJsInstanceProxy proxyObj)
        {
            if (!proxyObj.HasNativeWrapperPart)
            {
                proxyObj.SetUnmanagedObjectPointer(
                    CreateWrapperForManagedObject(
                        context.Handle.Handle,
                        proxyObj.ManagedIndex,
                        proxyObj.JsTypeDefinition.nativeProxy.UnmanagedPtr));
            }
        }
        public static void UnRegisterNativePart(NativeObjectProxy proxyObj)
        {
            if (proxyObj.HasNativeWrapperPart)
            {
                ReleaseWrapper(proxyObj.UnmanagedPtr);
                proxyObj.SetUnmanagedObjectPointer(IntPtr.Zero);
            }
        }
        public static int GetManagedIndexFromNativePart(NativeObjectProxy proxyObj)
        {
            return GetManagedIndex(proxyObj.UnmanagedPtr);
        }
    }


    public enum ManagedCallbackKind : int
    {
        Listener,
        MethodCall,
    }






    static class UnsafeNativeMethods
    {

        [DllImport("Kernel32.dll")]
        public static extern IntPtr LoadLibrary(string libraryName);
        [DllImport("Kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("Kernel32.dll")]
        public static extern uint SetErrorMode(int uMode);
        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();
    }
}