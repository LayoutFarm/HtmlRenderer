#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library, except where noted.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Runtime.InteropServices;

namespace PixelFarm.Agg
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexC4V2S
    {
        public uint color;
        public short x;
        public short y;
        //int z;
        public VertexC4V2S(uint color, int x, int y)
        {
            this.color = color;
            this.x = (short)x;
            this.y = (short)y;
            //z = 0; 
        }
        //--------------------------------------------

        public override string ToString()
        {
            return x + "," + y;
        }

        public const int SIZE_IN_BYTES = sizeof(uint) + sizeof(short) * 2;
        public const int VX_OFFSET = sizeof(uint);
        public const OpenTK.Graphics.OpenGL.VertexPointerType VX_PTR_TYPE = OpenTK.Graphics.OpenGL.VertexPointerType.Short;
        public const int N_COORDS = 2;
    }


    [StructLayout(LayoutKind.Sequential)]
    struct VertexC4V2f
    {
        public uint color;
        public float x;
        public float y;
        //float z;
        public VertexC4V2f(uint color, float x, float y)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            //z = 0;
        }

        public override string ToString()
        {
            return x + "," + y;
        }

        public const int SIZE_IN_BYTES = sizeof(uint) + sizeof(float) * 2;

        public const int COLOR_OFFSET = 0;
        public const int VX_OFFSET = sizeof(uint);

        public const OpenTK.Graphics.OpenGL.VertexPointerType VX_PTR_TYPE = OpenTK.Graphics.OpenGL.VertexPointerType.Float;
        public const int N_COORDS = 2;

    }



}