using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutFarm.Ease;

namespace Test5_Ease
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            EaseHost.StartGraphicsHost();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            VroomJs.JsBridge.LoadV8(@"D:\projects\V8Net\build\Debug\VRoomJsNative.dll");
            Application.Run(new Form1());
        }
    }
}
