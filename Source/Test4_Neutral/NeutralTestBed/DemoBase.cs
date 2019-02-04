//MIT, 2014-present, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit;
using PixelFarm.Drawing;
using PixelFarm.DrawingGL;

namespace Mini
{

    public delegate void GLSwapBufferDelegate();
    public delegate IntPtr GetGLControlDisplay();
    public delegate IntPtr GetGLSurface();

    public abstract class DemoBase
    {
        public DemoBase()
        {
            this.Width = 900;
            this.Height = 700;

        }

        //when we use with opengl
        GLSwapBufferDelegate _swapBufferDelegate;
        GetGLControlDisplay _getGLControlDisplay;
        GetGLSurface _getGLSurface;

        public event EventHandler RequestGraphicRefresh;
        protected void InvalidateGraphics()
        {
            RequestGraphicRefresh?.Invoke(this, EventArgs.Empty);
        }
        public virtual void Draw(Painter p)
        {
            OnGLRender(this, EventArgs.Empty);
        }
        public void CloseDemo()
        {
            DemoClosing();
        }


        public virtual void Init() { }
        public virtual void KeyDown(int keycode) { }
        public virtual void MouseDrag(int x, int y) { }
        public virtual void MouseDown(int x, int y, bool isRightButton) { }
        public virtual void MouseUp(int x, int y) { }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }



        protected virtual void DemoClosing()
        {
        }
        protected virtual void OnTimerTick(object sender, EventArgs e)
        {
        }
        protected virtual bool EnableAnimationTimer
        {
            get { return false; }
            set { }
        }
        public static void InvokeGLPainterReady(DemoBase demo, GLPainterContext pcx, GLPainter painter)
        {
            demo.OnGLPainterReady(painter);
            demo.OnReadyForInitGLShaderProgram();
        }
        public static void InvokePainterReady(DemoBase demo, Painter painter)
        {
            demo.OnPainterReady(painter);
        }
        protected virtual void OnGLPainterReady(GLPainter painter)
        {

        }
        protected virtual void OnPainterReady(Painter painter)
        {

        }
        protected virtual void OnReadyForInitGLShaderProgram()
        {
            //this method is called when the demo is ready for create GLES shader program
        }
        protected virtual void OnGLRender(object sender, EventArgs args)
        {

        }
        public void InvokeGLPaint()
        {
            OnGLRender(this, EventArgs.Empty);
        }

        protected void SwapBuffers()
        {
            //manual swap buffer
            if (_swapBufferDelegate != null)
            {
                _swapBufferDelegate();
            }
        }
        public void SetEssentialGLHandlers(
            GLSwapBufferDelegate swapBufferDelegate,
            GetGLControlDisplay getGLControlDisplay,
            GetGLSurface getGLSurface)
        {
            _swapBufferDelegate = swapBufferDelegate;
            _getGLControlDisplay = getGLControlDisplay;
            _getGLSurface = getGLSurface;
        }
        protected IntPtr getGLControlDisplay()
        {
            return _getGLControlDisplay();
        }
        protected IntPtr getGLSurface()
        {
            return _getGLSurface();
        }
    }

    [Flags]
    public enum DemoBackend
    {
        All,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DemoConfigGroupAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InfoAttribute : Attribute
    {
        public InfoAttribute()
        {

        }
        public InfoAttribute(DemoCategory catg)
        {
            this.Category = catg;
        }
        public InfoAttribute(string desc)
        {
            this.Description = desc;
        }
        public InfoAttribute(DemoCategory catg, string desc)
        {
            this.Category = catg;
            this.Description = desc;
        }
        public string Description { get; }
        public DemoCategory Category { get; }
        public string OrderCode { get; set; }
        public AvailableOn AvailableOn { get; set; }
    }

    public enum DemoCategory
    {
        Vector,
        Bitmap
    }

    [Flags]
    public enum AvailableOn
    {
        Empty = 0,
        GdiPlus = 1 << 1, //software
        Agg = 1 << 2, //software
        GLES = 1 << 3, //hardware
        BothHardwareAndSoftware = GdiPlus | Agg | GLES
    }
    public static class RootDemoPath
    {
        public static string Path = "";
    }

    public enum DemoConfigPresentaionHint
    {
        TextBox,
        CheckBox,
        OptionBoxes,
        SlideBarDiscrete,
        SlideBarContinuous_R4,
        SlideBarContinuous_R8,
        ConfigGroup,
    }
    public class DemoConfigAttribute : Attribute
    {
        public DemoConfigAttribute()
        {
        }
        public DemoConfigAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
    public class DemoActionAttribute : Attribute
    {
        public DemoActionAttribute()
        {
        }
        public DemoActionAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }


    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NoteAttribute : Attribute
    {
        public NoteAttribute(string desc)
        {
            this.Desc = desc;
        }
        public string Desc { get; set; }
    }


    public class ExampleAction
    {
        public ExampleAction(DemoActionAttribute config, System.Reflection.MethodInfo method)
        {
            Method = method;
            if (!string.IsNullOrEmpty(config.Name))
            {
                this.Name = config.Name;
            }
            else
            {
                this.Name = method.Name;
            }

        }
        public System.Reflection.MethodInfo Method { get; }
        public string Name { get; }
        public void InvokeMethod(object target)
        {
            Method.Invoke(target, null);
        }
    }
    public class ExampleGroupConfigDesc
    {
        public List<ExampleConfigDesc> _configList = new List<ExampleConfigDesc>();
        public List<ExampleAction> _configActionList = new List<ExampleAction>();

        public ExampleGroupConfigDesc(DemoConfigGroupAttribute config, Type configClassOrStruct)
        {
            ConfigClassOrStruct = configClassOrStruct;
            OriginalGroupConfifAttribute = config;
            Name = config.Name;
        }
        public System.Reflection.PropertyInfo OwnerProperty { get; internal set; }
        public string Name { get; }
        public DemoConfigGroupAttribute OriginalGroupConfifAttribute { get; }
        public Type ConfigClassOrStruct { get; }

    }

    public class ExampleConfigDesc
    {
        System.Reflection.PropertyInfo _property;
        List<ExampleConfigValue> _optionFields;
        public ExampleConfigDesc(DemoConfigAttribute config, System.Reflection.PropertyInfo property)
        {
            _property = property;
            this.OriginalConfigAttribute = config;
            if (!string.IsNullOrEmpty(config.Name))
            {
                this.Name = config.Name;
            }
            else
            {
                this.Name = property.Name;
            }


            Type propType = property.PropertyType;
            if (propType == typeof(bool))
            {
                this.PresentaionHint = DemoConfigPresentaionHint.CheckBox;
            }
            else if (propType.IsEnum)
            {
                this.PresentaionHint = Mini.DemoConfigPresentaionHint.OptionBoxes;
                //find option
                var enumFields = propType.GetFields();
                int j = enumFields.Length;
                _optionFields = new List<ExampleConfigValue>(j);
                for (int i = 0; i < j; ++i)
                {
                    var enumField = enumFields[i];
                    if (enumField.IsStatic)
                    {
                        //use field name or note that assoc with its field name

                        string fieldNameOrNote = enumField.Name;
                        var foundNotAttr = enumField.GetCustomAttributes(typeof(NoteAttribute), false);
                        if (foundNotAttr.Length > 0)
                        {
                            fieldNameOrNote = ((NoteAttribute)foundNotAttr[0]).Desc;
                        }
                        _optionFields.Add(new ExampleConfigValue(property, enumField, fieldNameOrNote));
                    }
                }
            }
            else if (propType == typeof(Int32))
            {
                this.PresentaionHint = DemoConfigPresentaionHint.SlideBarDiscrete;
            }
            else if (propType == typeof(double))
            {
                this.PresentaionHint = DemoConfigPresentaionHint.SlideBarContinuous_R8;
            }
            else if (propType == typeof(float))
            {
                this.PresentaionHint = DemoConfigPresentaionHint.SlideBarContinuous_R4;
            }
            else
            {
                //config with custom attribute group

                var foundAttrs = propType.GetCustomAttributes(typeof(DemoConfigGroupAttribute), false);
                if (foundAttrs.Length == 1)
                {
                    this.PresentaionHint = DemoConfigPresentaionHint.ConfigGroup;
                }
                else
                {
                    this.PresentaionHint = DemoConfigPresentaionHint.TextBox;
                }
            }
        }
        public string Name { get; }
        public DemoConfigAttribute OriginalConfigAttribute { get; }
        public DemoConfigPresentaionHint PresentaionHint { get; }
        public Type DataType => _property.PropertyType;
        public void InvokeSet(object target, object value)
        {
            _property.GetSetMethod().Invoke(target, new object[] { value });
        }
        public object InvokeGet(object target)
        {
            return _property.GetGetMethod().Invoke(target, null);
        }
        public List<ExampleConfigValue> GetOptionFields() => _optionFields;

        EventHandler _updatePresentationValueHandler;
        public void SetUpdatePresentationValueHandler(EventHandler updatePresentationValue)
        {
            _updatePresentationValueHandler = updatePresentationValue;
        }
        public void InvokeUpdatePresentationValue()
        {
            _updatePresentationValueHandler?.Invoke(this, EventArgs.Empty);
        }


    }
    public class ExampleAndDesc
    {
        static Type s_demoConfigAttrType = typeof(DemoConfigAttribute);
        static Type s_infoAttrType = typeof(InfoAttribute);
        static Type s_demoAction = typeof(DemoActionAttribute);

        List<ExampleConfigDesc> _configList = new List<ExampleConfigDesc>();
        List<ExampleAction> _actionList = new List<ExampleAction>();
        List<ExampleGroupConfigDesc> _groupConfigs = new List<ExampleGroupConfigDesc>();

        public ExampleAndDesc(Type t, string name)
        {
            this.Type = t;
            this.Name = name;
            this.OrderCode = "";
            var p1 = t.GetProperties();
            InfoAttribute[] exInfoList = t.GetCustomAttributes(s_infoAttrType, false) as InfoAttribute[];
            int m = exInfoList.Length;
            if (m > 0)
            {
                for (int n = 0; n < m; ++n)
                {
                    InfoAttribute info = exInfoList[n];
                    if (!string.IsNullOrEmpty(info.OrderCode))
                    {
                        this.OrderCode = info.OrderCode;
                    }
                    if (!string.IsNullOrEmpty(info.Description))
                    {
                        this.Description += " " + info.Description;
                    }
                    AvailableOn |= info.AvailableOn;
                }
            }
            if (AvailableOn == AvailableOn.Empty)
            {
                //if user dose not specific then
                //assume available on All
                AvailableOn |= AvailableOn.Agg | AvailableOn.GLES | AvailableOn.GdiPlus;
            }

            if (string.IsNullOrEmpty(this.Description))
            {
                this.Description = this.Name;
            }


            foreach (var property in t.GetProperties())
            {
                //1.
                var foundAttrs = property.GetCustomAttributes(s_demoConfigAttrType, true);
                if (foundAttrs.Length > 0)
                {
                    //this is configurable attrs
                    Type propertyType = property.PropertyType;
                    if (propertyType.IsClass &&
                        CreateExampleConfigGroup((DemoConfigAttribute)foundAttrs[0], property))
                    {
                        //check if config group or not
                        //if yes=> the the config group is created with CreateExampleConfigGroup()
                        //nothing to do more here

                        //else => go the another else block
                    }
                    else
                    {
                        _configList.Add(new ExampleConfigDesc((DemoConfigAttribute)foundAttrs[0], property));
                    }
                }
            }
            foreach (var met in t.GetMethods())
            {
                //only public and instance method
                if (met.IsStatic) continue;
                if (met.DeclaringType == t)
                {
                    if (met.GetParameters().Length == 0)
                    {
                        var foundAttrs = met.GetCustomAttributes(s_demoAction, false);
                        if (foundAttrs.Length > 0)
                        {
                            //this is configurable attrs
                            _actionList.Add(new ExampleAction((DemoActionAttribute)foundAttrs[0], met));
                        }
                    }
                }
            }
            //------
            //some config is group config
            //... so extract more from that
            //------
        }
        bool CreateExampleConfigGroup(DemoConfigAttribute configAttr, System.Reflection.PropertyInfo prop)
        {
            Type configGroup = prop.PropertyType;
            var configGroupAttrs = configGroup.GetCustomAttributes(typeof(DemoConfigGroupAttribute), false);
            if (configGroupAttrs.Length > 0)
            {
                //reflection this type
                ExampleGroupConfigDesc groupConfigDesc = new ExampleGroupConfigDesc((DemoConfigGroupAttribute)configGroupAttrs[0], configGroup);
                groupConfigDesc.OwnerProperty = prop;

                List<ExampleConfigDesc> configList = groupConfigDesc._configList;
                List<ExampleAction> exActionList = groupConfigDesc._configActionList;
                //
                foreach (var property in configGroup.GetProperties())
                {
                    //1.
                    var foundAttrs = property.GetCustomAttributes(s_demoConfigAttrType, true);
                    if (foundAttrs.Length > 0)
                    {
                        //in this version: no further subgroup
                        configList.Add(new ExampleConfigDesc((DemoConfigAttribute)foundAttrs[0], property));
                    }
                }
                foreach (var met in configGroup.GetMethods())
                {
                    //only public and instance method
                    if (met.IsStatic) continue;
                    if (met.DeclaringType == configGroup)
                    {
                        if (met.GetParameters().Length == 0)
                        {
                            var foundAttrs = met.GetCustomAttributes(s_demoAction, false);
                            if (foundAttrs.Length > 0)
                            {
                                //this is configurable attrs
                                exActionList.Add(new ExampleAction((DemoActionAttribute)foundAttrs[0], met));
                            }
                        }
                    }
                }
                //------------ 
                _groupConfigs.Add(groupConfigDesc);
                return true;
            }
            return false;
        }

        public AvailableOn AvailableOn { get; }
        public Type Type { get; }
        public string Name { get; }
        public override string ToString()
        {
            return this.OrderCode + " : " + this.Name;
        }
        public List<ExampleConfigDesc> GetConfigList() => _configList;
        public List<ExampleAction> GetActionList() => _actionList;
        public List<ExampleGroupConfigDesc> GetGroupConfigList() => _groupConfigs;

        public string Description { get; }
        public string OrderCode { get; }

        public bool IsAvailableOn(AvailableOn availablePlatform)
        {
            return ((int)AvailableOn & (int)availablePlatform) != 0;
        }
    }
    public class ExampleConfigValue
    {
        System.Reflection.FieldInfo _fieldInfo;
        System.Reflection.PropertyInfo _property;
        public ExampleConfigValue(System.Reflection.PropertyInfo property, System.Reflection.FieldInfo fieldInfo, string name)
        {
            _property = property;
            _fieldInfo = fieldInfo;
            this.Name = name;
            this.ValueAsInt32 = (int)fieldInfo.GetValue(null);
        }
        public string Name { get; }
        public int ValueAsInt32 { get; }
        public void InvokeSet(object target)
        {
            _property.GetSetMethod().Invoke(target, new object[] { ValueAsInt32 });
        }
    }

}