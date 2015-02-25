using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NativeV8;

namespace Test5_Ease
{
    public partial class FormTestV8 : Form
    {
        public FormTestV8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //where is my dll...
            NativeV8JsInterOp.LoadV8("..\\..\\dll\\MiniJsBridge.dll");
            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                Console.WriteLine("test-b");
            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));
            //===============================================================


            JsContext context = NativeV8JsInterOp.CreateNewJsContext(); 
            AboutMe ab = new AboutMe();//pure managed 
            context.EnterContext();
            context.RegisterTypeDefinition(jstypedef);
            //-------------------------------------------------------------
            context.SetParameter("a", 10);
            context.SetParameter("b", 20);
            //------------------------------------------------------------- 
            NativeJsInstanceProxy prox_ab = context.CreateWrapper(ab, jstypedef);

            context.SetParameter("x", prox_ab);
            //  run code 
            //context.Run("a+b");
            //context.Run("(function(){return a+b;})()");
            context.Run("if(x.C()){x.B();}");
            context.ExitContext();
            context.Close();
            context = null;
            //----------------------------------------------------------------

            //NativeV8JsInterOp.RegisterTypeDef();


            //AboutMe ab = new AboutMe();
            //NativeObjectProxy wrapObject = proxyStore.CreateProxyForObject(ab); 

            //NativeV8JsInterOp.RegisterNativePart(wrapObject); 
            ////int result = NativeV8JsInterOp.GetManagedIndexFromNativePart(wrapObject); 
            //NativeV8JsInterOp.TestCallFromNative(wrapObject);

            //NativeV8JsInterOp.UnRegisterNativePart(wrapObject);
            //-------------------------------------------------------------------

        }
        class AboutMe
        {
            public AboutMe()
            {

            }
            public void SayHello()
            {

            }
        }
        public class MyConsole
        {
            string myname;
            public MyConsole(string myname)
            {
                this.myname = myname;
            }
            public string MyName
            {
                get
                {
                    return this.myname;
                }
                set
                {
                    this.myname = value;
                }
            }
            /// <summary>
            /// expose to javascript
            /// </summary>
            /// <param name="str"></param>
            public void Log(string str)
            {
                Console.WriteLine(str);
            }
        }
        delegate void TestMeDel();

        void TestMe001()
        {

        }
 
         

    }
}
