//2014 MIT,WinterDev   

using System;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using OpenTK;
using OpenTK.Graphics.ES20;
using LayoutFarm.DrawingGL;

namespace PixelFarm.Agg
{
    class AggCoordList3f
    {
        ArrayList<float> data = new ArrayList<float>();
        int coordCount = 0;
        int lineY;
       
        
        int lineCount;
        int lastX = 0;
        int lastA = 0;

        public AggCoordList3f()
        {

        }
        public void BeginNewLine(int lineY)
        {             
            lineCount++;
            this.lastA = 0;
            this.lineY = lineY;
        }
        public void CloseLine()
        { 
        }
        public void AddCoord(int x, int alpha)
        { 
            this.data.AddVertex(this.lastX = x);
            this.data.AddVertex(this.lineY);
            this.data.AddVertex(this.lastA = alpha); 

            this.coordCount++;
        }
        public void Clear()
        {
            this.coordCount = 0;
            this.data.Clear();
        }
        public int Count
        {
            get { return this.coordCount; }
        }

        public float[] GetInternalArray()
        {
            return this.data.Array;
        }

    }







    class CoordList2f
    {
        ArrayList<float> data = new ArrayList<float>();
        int coordCount = 0;
        public CoordList2f()
        {
        }
        public void AddCoord(float x, float y)
        {
            this.data.AddVertex(x);
            this.data.AddVertex(y);
            this.coordCount++;
        }
        public void Clear()
        {
            this.coordCount = 0;
            this.data.Clear();
        }
        public int Count
        {
            get { return this.coordCount; }
        }

        public float[] GetInternalArray()
        {
            return this.data.Array;
        }

    }
}