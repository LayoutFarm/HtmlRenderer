using System.Drawing;
namespace LayoutFarm.Drawing 
{
    class MyMatrix:Matrix 
    {

        System.Drawing.Drawing2D.Matrix mat;

        public MyMatrix(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            mat = new System.Drawing.Drawing2D.Matrix(
                m11, m12, m21, m22, dx, dy);
        }
        public MyMatrix()
        {
            mat = new System.Drawing.Drawing2D.Matrix();
        }
        public override void Translate(float dx, float dy)
        {
            mat.Translate(dx, dy);
        }
        public override void Rotate(float angle)
        {
            mat.Rotate(angle);
        }
        public override void Scale(float sx, float sy)
        {
            mat.Scale(sx, sy);

        }
        public override float[] Elements
        {
            get { return mat.Elements; }
        }
        public override void Shear(float sx, float sy)
        {
            mat.Shear(sx, sy);
        }
    }
}