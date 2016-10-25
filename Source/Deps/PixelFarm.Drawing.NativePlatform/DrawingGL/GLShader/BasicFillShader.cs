//MIT 2016, WinterDev

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class BasicFillShader : ShaderBase
    {
        ShaderVtxAttrib2f a_position;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar4 u_solidColor;
        public BasicFillShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position; 
            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;              
            varying vec4 v_color;
 
            void main()
            {
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1); 
                v_color= u_solidColor;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color; 
                void main()
                {
                    gl_FragColor = v_color;
                }
            ";
            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }

            a_position = shaderProgram.GetAttrV2f("a_position");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
        }
        public void FillTriangleStripWithVertexBuffer(float[] linesBuffer, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadPureV2f(linesBuffer);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, nelements);
        }
        //--------------------------------------------
        int orthoviewVersion = -1;
        void CheckViewMatrix()
        {
            int version = 0;
            if (orthoviewVersion != (version = _canvasShareResource.OrthoViewVersion))
            {
                orthoviewVersion = version;
                u_matrix.SetData(_canvasShareResource.OrthoView.data);
            }
        }
        //--------------------------------------------
        public unsafe void FillTriangles(float[] polygon2dVertices, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------  

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public unsafe void DrawLineLoopWithVertexBuffer(float* polygon2dVertices, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.LineLoop, 0, nelements);
        }
        public unsafe void FillTriangleFan(float* polygon2dVertices, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.TriangleFan, 0, nelements);
        }
        public void DrawLine(float x1, float y1, float x2, float y2, PixelFarm.Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            unsafe
            {
                float* vtx = stackalloc float[4];
                vtx[0] = x1; vtx[1] = y1;
                vtx[2] = x2; vtx[3] = y2;
                a_position.UnsafeLoadPureV2f(vtx);
            }
            GL.DrawArrays(BeginMode.Lines, 0, 2);
        }
    }
}