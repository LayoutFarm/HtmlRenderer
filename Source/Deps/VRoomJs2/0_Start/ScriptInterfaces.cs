//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace VroomJs
{
    public interface INativeRef
    {
        int ManagedIndex { get; }
        object WrapObject { get; }
        bool HasNativeSide { get; }
        void SetUnmanagedPtr(IntPtr unmanagedObjectPtr);
        IntPtr UnmanagedPtr { get; }
    }
    public interface INativeScriptable : INativeRef
    {
        IntPtr UnmanagedTypeDefinitionPtr { get; }
    }

    public abstract class JsTypeMemberDefinition
    {
        string mbname;
        JsMemberKind memberKind;
        JsTypeDefinition owner;
        int memberId;
        internal INativeRef nativeProxy;
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
                var thisArg = args.GetThisArg();
                int argCount = args.ArgCount;
                object[] parameters = new object[argCount];
                for (int i = 0; i < argCount; ++i)
                {
                    parameters[i] = args.GetArgAsObject(i);
                }
                object result = this.method.Invoke(thisArg, parameters);

                args.SetResultObj(result);
            }
            else
            {
                methodCallDel(args);
            }
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
}