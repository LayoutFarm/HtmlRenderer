//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.Text
{
    public struct EditableRunCharLocation
    {
        public readonly int pixelOffset;
        public readonly int charIndex;
        public static EditableRunCharLocation EmptyTextRunLocationInfo = new EditableRunCharLocation();
        public EditableRunCharLocation(int pixelOffset, int charIndex)
        {
            this.pixelOffset = pixelOffset;
            this.charIndex = charIndex;
        }
    }
}