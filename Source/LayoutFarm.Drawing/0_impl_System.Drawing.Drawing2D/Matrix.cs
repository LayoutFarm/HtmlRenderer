using System.Drawing;
using System.Drawing.Drawing2D;
namespace LayoutFarm.Drawing
{
    public class Matrix
    {

        System.Drawing.Drawing2D.Matrix mat;

        public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            mat = new System.Drawing.Drawing2D.Matrix(
                m11, m12, m21, m22, dx,dy);
        }
        public Matrix()
        {
            mat = new System.Drawing.Drawing2D.Matrix();
        }
        public void Translate(float dx,float dy)
        {
            mat.Translate(dx,dy);
        }
        public void Rotate(float angle)
        {
            mat.Rotate(angle);
        }
        public void Scale(float sx, float sy)
        {
            mat.Scale(sx, sy);
             
        }
        public float[] Elements
        {
            get { return mat.Elements; }
        }
        public void Shear(float sx,float sy)
        {
            mat.Shear(sx, sy);
        }
    }


}