
using System.Windows.Forms;
namespace Mini
{
    public partial class FormTestBed : Form
    {
        MyMiniGLES2Control miniGLControl;
        public FormTestBed()
        {
            InitializeComponent();
        }
        public MyMiniGLES2Control InitMiniGLControl(int w, int h)
        {
            if (miniGLControl == null)
            {
                miniGLControl = new MyMiniGLES2Control();
                miniGLControl.Width = w;
                miniGLControl.Height = h;
                miniGLControl.ClearColor = PixelFarm.Drawing.Color.Blue;
                this.Controls.Add(miniGLControl);
            }
            return miniGLControl;
        }
        public MyMiniGLES2Control MiniGLControl
        {
            get { return this.miniGLControl; }
        }
    }
}
