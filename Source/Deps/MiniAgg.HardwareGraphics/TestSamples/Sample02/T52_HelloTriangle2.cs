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
    [Info(OrderCode = "052")]
    [Info("T52_HelloTriangle2")]
    public class T52_HelloTriangle2 : SampleBase
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //----------------
            //vertex shader source
            string vs = @"      
             
            attribute vec2 a_position;
            attribute vec4 a_color;
            
            varying vec4 v_color;
 
            void main()
            {   
                
                gl_Position = vec4(a_position[0],a_position[1],0,1);  
                v_color = a_color;
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
            a_color = shaderProgram.GetAttrV3f("a_color");
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.ClearColor(0, 0, 0, 0);
            GL.ClearColor(1, 1, 1, 1);
        }
        protected override void DemoClosing()
        {
            shaderProgram.DeleteMe();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //------------------------------------------------------------------------------------------------
            int width = Width;
            int height = Height;
            float[] vertices =
            {
                     0.0f,  0.5f, //2d coord
                     1, 0, 0, 0.1f,//r
                    -0.5f, -0.5f,  //2d coord
                     0,1,0,0.1f,//g
                     0.5f, -0.5f,  //2d corrd
                     0,0,1,0.1f, //b
            };
            GL.Viewport(0, 0, width, height);
            // Set the viewport 
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shaderProgram.UseProgram();
            // Load the vertex data 
            unsafe
            {
                fixed (float* head = &vertices[0])
                {
                    a_position.UnsafeLoadMixedV2f(head, 5);
                    a_color.UnsafeLoadMixedV3f(head + 2, 5);
                }
            }

            GL.DrawArrays(BeginMode.Triangles, 0, 3);
            SwapBuffer();
        }
        //-------------------------------
        ShaderVtxAttrib2f a_position;
        ShaderVtxAttrib3f a_color;
    }
}
