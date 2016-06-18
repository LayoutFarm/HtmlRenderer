//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using VroomJs;
using System.Windows.Forms;
using LayoutFarm.Scripting;
namespace Test5_Ease
{
    /// <summary>
    /// simple 
    /// </summary>
    class MyWebConsole
    {
        TextBox outputTextBox;
        public MyWebConsole(TextBox outputTextBox)
        {
            this.outputTextBox = outputTextBox;
        }
        [JsMethod]
        public void log(object obj)
        {
            outputTextBox.Text += obj.ToString();
        }
    }
}