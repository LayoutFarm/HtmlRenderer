//MIT, 2015-2017, WinterDev

using System.Windows.Forms; 
namespace LayoutFarm.Scripting
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