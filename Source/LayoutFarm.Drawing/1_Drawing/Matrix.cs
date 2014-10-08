 namespace LayoutFarm.Drawing
{
    public abstract class Matrix
    {
        public abstract void Translate(float dx, float dy);
        public abstract void Rotate(float angle);
        public abstract void Scale(float sx, float sy);
        public abstract float[] Elements { get; }
        public abstract void Shear(float sx, float sy);
    }


}