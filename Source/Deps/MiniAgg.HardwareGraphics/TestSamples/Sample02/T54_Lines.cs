//MIT, 2014-2016,WinterDev
//
// Copyright (c) 2014 The ANGLE Project Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//

//            Based on Hello_Triangle.c from
// Book:      OpenGL(R) ES 2.0 Programming Guide
// Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
// ISBN-10:   0321502795
// ISBN-13:   9780321502797
// Publisher: Addison-Wesley Professional
// URLs:      http://safari.informit.com/9780321563835
//            http://www.opengles-book.com

using System;
using OpenTK.Graphics.ES20;
using Mini;
namespace OpenTkEssTest
{
    [Info(OrderCode = "054")]
    [Info("T54_Lines")]
    public class T54_Lines : SampleBase
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib2f a_position;
        ShaderVtxAttrib4f a_color;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 u_useSolidColor;
        ShaderUniformVar4 u_solidColor;
        MyMat4 orthoView;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position;
            attribute vec4 a_color; 

            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
            uniform int u_useSolidColor;              

            varying vec4 v_color;
 
            void main()
            {
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                if(u_useSolidColor !=0)
                {
                    v_color= u_solidColor;
                }
                else
                {
                    v_color = a_color;
                }
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
            a_color = shaderProgram.GetAttrV4f("a_color");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int max = Math.Max(this.Width, this.Height);
            //square viewport
            GL.Viewport(0, 0, max, max);
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            //--------------------------------------------------------------------------------

            //load image


        }
        protected override void DemoClosing()
        {
            shaderProgram.DeleteMe();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //------------------------------------------------------------------------------------------------


            GL.Clear(ClearBufferMask.ColorBufferBit);
            shaderProgram.UseProgram();
            //---------------------------------------------------------  
            u_matrix.SetData(orthoView.data);
            //---------------------------------------------------------  

            DrawLines(0, 0, 300, 150);
            float[] rect = CreateRectCoords2(100, 100, 50, 50);
            FillPolygonWithSolidColor(rect, rect.Length / 2, PixelFarm.Drawing.Color.Black);
            //---------------------------------------------------------

            SwapBuffer();
        }

        //-------------------------------
        void FillPolygonWithSolidColor(float[] onlyCoords,
               int numVertices, PixelFarm.Drawing.Color c)
        {
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)c.R / 255f, (float)c.G / 255f, (float)c.B / 255f, (float)c.A / 255f);//use solid color 
            a_position.LoadPureV2f(onlyCoords);
            GL.DrawArrays(BeginMode.Triangles, 0, numVertices);
        }
        static float[] CreateRectCoords2(float x, float y, float w, float h)
        {
            float x0 = x;
            float y0 = y + h;
            float x1 = x;
            float y1 = y;
            float x2 = x + w;
            float y2 = y + h;
            float x3 = x + w;
            float y3 = y;
            float[] vertices = new float[]{
               x0,y0,
               x1,y1,
               x2,y2,
               x1,y1,
               x3,y3,
               x2,y2
            };
            return vertices;
        }
        //------------------------------- 
        void DrawLines(float x1, float y1, float x2, float y2)
        {
            float[] vtxs = new float[] { x1, y1, x2, y2 };
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue(0f, 0f, 0f, 1f);//use solid color 
            a_position.LoadPureV2f(vtxs);
            GL.DrawArrays(BeginMode.Lines, 0, 2);
        }
        void DrawImage(float x, float y)
        {
        }
    }
}
