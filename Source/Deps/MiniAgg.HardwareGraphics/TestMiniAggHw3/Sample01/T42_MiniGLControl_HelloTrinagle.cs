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


#region Using Directives

using System;
using OpenTK.Graphics.ES20;
using Mini;
#endregion




namespace OpenTkEssTest
{
    [Info(OrderCode = "042")]
    [Info("T42_HelloTriangle")]
    public class T42_ES2HelloTriangleDemo : PrebuiltGLControlDemoBase
    {
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec4 vPosition;
            void main()
            {
                gl_Position = vPosition;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                void main()
                {
                    gl_FragColor = vec4(1.0,0.0, 0.0, 1.0);
                }
            ";
            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                //return false
            }
            GL.ClearColor(0, 0, 0, 0);
        }
        protected override void DemoClosing()
        {
            GL.DeleteProgram(mProgram);
            mProgram = 0;
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //------------------------------------------------------------------------------------------------
            int width = miniGLControl.Width;
            int height = miniGLControl.Height;
            float[] vertices =
                {
                     0.0f,  0.5f, 0.0f,
                    -0.5f, -0.5f, 0.0f,
                     0.5f, -0.5f, 0.0f,
                };
            GL.Viewport(0, 0, width, height);
            // Set the viewport
            //glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Clear the color buffer
            // glClear(GL_COLOR_BUFFER_BIT); 
            // Use the program object
            //glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            // Load the vertex data
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, vertices);
            GL.EnableVertexAttribArray(0);
            //glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, vertices);
            //glEnableVertexAttribArray(0);
            GL.DrawArrays(BeginMode.Triangles, 0, 3);
            //glDrawArrays(GL_TRIANGLES, 0, 3); 
            miniGLControl.SwapBuffers();
        }
        //-------------------------------
        int mProgram;
    }
}
