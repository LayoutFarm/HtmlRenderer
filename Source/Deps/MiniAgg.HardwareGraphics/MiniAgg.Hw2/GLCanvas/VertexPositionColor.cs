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
using OpenTK.Graphics.ES20;
namespace PixelFarm.Agg
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexV2S1Cvr
    {
        public int x;
        public int y;
        public int alpha;
        public VertexV2S1Cvr(int x, int y, int alpha)
        {
            this.x = x;
            this.y = y;
            this.alpha = alpha;
        }
        public override string ToString()
        {
            return x + "," + y + " alpha=" + alpha;
        }
        public const int VX_OFFSET = 0;
        public const int N_COORDS = 2;
    }


    [StructLayout(LayoutKind.Sequential)]
    struct VertexC4V3f
    {
        public uint color;
        public float x;
        public float y;
        float z;
        public VertexC4V3f(uint color, float x, float y)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            z = 0;
        }

        public override string ToString()
        {
            return x + "," + y;
        }

        public const int SIZE_IN_BYTES = sizeof(uint) + sizeof(float) * 3;
        public const int VX_OFFSET = sizeof(uint);
        public const OpenTK.Graphics.OpenGL.VertexPointerType VX_PTR_TYPE = OpenTK.Graphics.OpenGL.VertexPointerType.Float;
        public const int N_COORDS = 3;
    }



    /// <summary>
    /// vertex buffer object
    /// </summary>
    public struct VboC4V2S
    {
        public int VboID;
        public void Dispose()
        {
            GL.DeleteBuffers(1, ref this.VboID);
        }
        public void BindBuffer()
        {
            throw new NotSupportedException();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VboID);
            //GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexC4V2S.SIZE_IN_BYTES, (IntPtr)0);
            //GL.VertexPointer(VertexC4V2S.N_COORDS, VertexC4V2S.VX_PTR_TYPE, VertexC4V2S.SIZE_IN_BYTES, VertexC4V2S.VX_OFFSET);
        }
        public void UnbindBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
    public struct VboC4V3f
    {
        public int VboID;
        public void Dispose()
        {
            GL.DeleteBuffers(1, ref this.VboID);
        }
        public void BindBuffer()
        {
            throw new NotSupportedException();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VboID);
            //GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexC4V3f.SIZE_IN_BYTES, (IntPtr)0);
            //GL.VertexPointer(VertexC4V3f.N_COORDS, VertexC4V3f.VX_PTR_TYPE, VertexC4V3f.SIZE_IN_BYTES, VertexC4V3f.VX_OFFSET);
        }
        public void UnbindBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}