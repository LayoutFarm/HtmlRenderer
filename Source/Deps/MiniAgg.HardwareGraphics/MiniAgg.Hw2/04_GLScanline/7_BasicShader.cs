//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Agg;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using OpenTK;
using OpenTK.Graphics.ES20;
using LayoutFarm.DrawingGL;

namespace LayoutFarm.DrawingGL
{

    class BasicShader
    {
        bool isInited;
        MiniShaderProgram shaderProgram;
        ShaderVtxAttrib a_position;
        ShaderVtxAttrib a_color;
        ShaderVtxAttrib a_textureCoord;

        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 u_useSolidColor;
        ShaderUniformVar1 u_useAggColor;

        ShaderUniformVar4 u_solidColor;
        MyMat4 mymat4;

        public BasicShader()
        {
            shaderProgram = new MiniShaderProgram();

        }
        public void UnloadShader()
        {
            shaderProgram.DeleteMe();
            shaderProgram = null;
        }
        public void InitShader()
        {
            if (isInited) { return; }
            //----------------

            //vertex shader source
            string vs = @"        
            attribute vec3 a_position;
            attribute vec4 a_color; 
            attribute vec2 a_texcoord;
            
            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
            uniform int u_useSolidColor;            
            uniform int u_useAggColor;             

            varying vec4 v_color;
            varying vec2 v_texcoord;
             
            void main()
            {
                float a= a_position[2]; //before matrix op
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                if(u_useAggColor !=0)
                { 
                     
                    v_color= vec4(u_solidColor.r /255.0,u_solidColor.g /255.0,u_solidColor.b/255.0, a/255.0);
                }
                else if(u_useSolidColor !=0)
                {   
                    v_color= u_solidColor;                         
                }
                else
                {
                    v_color = a_color;
                } 
                v_texcoord= a_texcoord;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color; 
                varying vec2 v_texcoord;                 
                void main()
                {       
                    gl_FragColor= v_color;
                }
            ";

            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }

            a_position = shaderProgram.GetVtxAttrib("a_position");
            a_color = shaderProgram.GetVtxAttrib("a_color");
            a_textureCoord = shaderProgram.GetVtxAttrib("a_texcoord");

            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");

            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_useAggColor = shaderProgram.GetUniform1("u_useAggColor");

            u_solidColor = shaderProgram.GetUniform4("u_solidColor");


            shaderProgram.UseProgram();
            isInited = true;
        }

        public MyMat4 ViewMatrix
        {
            get { return this.mymat4; }
            set
            {
                this.mymat4 = value;
                u_matrix.SetData(this.ViewMatrix.data);
            }
        }

        public void AggDrawLines(AggCoordList3f linesBuffer, int nelements, LayoutFarm.Drawing.Color color)
        {
            u_useAggColor.SetValue(1); //***
            u_useSolidColor.SetValue(1);

            //put original color, to be normalize on server side
            u_solidColor.SetValue((float)color.R, (float)color.G, (float)color.B, (float)color.A);
            a_position.LoadV3f(linesBuffer.GetInternalArray(), 3, 0);
            GL.DrawArrays(BeginMode.LineStrip, 0, nelements);
        }
        //---------------------------------- 
        public void DrawLineStripsWithVertexBuffer(CoordList2f linesBuffer, int nelements, LayoutFarm.Drawing.Color color)
        {
            u_useAggColor.SetValue(0);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(linesBuffer.GetInternalArray(), 2, 0);
            GL.DrawArrays(BeginMode.LineStrip, 0, nelements);
        }

        public void DrawTrianglesWithVertexBuffer(CoordList2f linesBuffer, int nelements, LayoutFarm.Drawing.Color color)
        {
            u_useAggColor.SetValue(0);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(linesBuffer.GetInternalArray(), 2, 0);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }

        public unsafe void DrawLineLoopWithVertexBuffer(float* polygon2dVertices, int nelements, LayoutFarm.Drawing.Color color)
        {
            u_useAggColor.SetValue(0);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(polygon2dVertices, 2, 0);
            GL.DrawArrays(BeginMode.LineLoop, 0, nelements);
        }
        public void DrawLine(float x1, float y1, float x2, float y2, LayoutFarm.Drawing.Color color)
        {
            u_useAggColor.SetValue(0);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);

            unsafe
            {
                float* vtx = stackalloc float[4];
                vtx[0] = x1; vtx[1] = y1;
                vtx[2] = x2; vtx[3] = y2;
                a_position.LoadV2f(vtx, 2, 0);
            }
            GL.DrawArrays(BeginMode.Lines, 0, 2);
        }
        public unsafe void FillTriangleFan(float* polygon2dVertices, int nelements, LayoutFarm.Drawing.Color color)
        {
            u_useAggColor.SetValue(0);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(polygon2dVertices, 2, 0);
            GL.DrawArrays(BeginMode.TriangleFan, 0, nelements);
        }
        public unsafe void FillTriangles(float* polygon2dVertices, int nelements, LayoutFarm.Drawing.Color color)
        {
            u_useAggColor.SetValue(0);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(polygon2dVertices, 2, 0);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);

        }
    }

}