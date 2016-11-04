//MIT, 2015-2016, WinterDev

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