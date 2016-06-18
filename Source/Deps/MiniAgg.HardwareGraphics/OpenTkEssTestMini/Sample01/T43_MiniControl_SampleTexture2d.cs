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
    [Info(OrderCode = "043")]
    [Info("T43_HelloTriangle")]
    public class T43_MiniControl_SampleTexture2dDemo : PrebuiltGLControlDemoBase
    {
        protected override void OnInitGLProgram(object sender, EventArgs handler)
        {
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = a_position;
                    v_texCoord = a_texCoord;
                 }	 
                ";
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      void main()
                      {
                         gl_FragColor = texture2D(s_texture, v_texCoord);
                      }
                ";
            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                //return false
            }

            // Get the attribute locations
            mPositionLoc = GL.GetAttribLocation(mProgram, "a_position");
            mTexCoordLoc = GL.GetAttribLocation(mProgram, "a_texCoord");
            // Get the sampler location
            mSamplerLoc = GL.GetUniformLocation(mProgram, "s_texture");
            //// Load the texture
            mTexture = ES2Utils2.CreateSimpleTexture2D();
            GL.ClearColor(0, 0, 0, 0);
            //================================================================================

        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //    GLfloat vertices[] =
            //{
            //    -0.5f,  0.5f, 0.0f,  // Position 0
            //     0.0f,  0.0f,        // TexCoord 0
            //    -0.5f, -0.5f, 0.0f,  // Position 1
            //     0.0f,  1.0f,        // TexCoord 1
            //     0.5f, -0.5f, 0.0f,  // Position 2
            //     1.0f,  1.0f,        // TexCoord 2
            //     0.5f,  0.5f, 0.0f,  // Position 3
            //     1.0f,  0.0f         // TexCoord 3
            //};
            //    GLushort indices[] = { 0, 1, 2, 0, 2, 3 };

            float[] vertices = new float[] {
                    -0.5f,  0.5f, 0.0f,  // Position 0
                     0.0f,  0.0f,        // TexCoord 0
                    -0.5f, -0.5f, 0.0f,  // Position 1
                     0.0f,  1.0f,        // TexCoord 1
                     0.5f, -0.5f, 0.0f,  // Position 2
                     1.0f,  1.0f,        // TexCoord 2
                     0.5f,  0.5f, 0.0f,  // Position 3
                     1.0f,  0.0f         // TexCoord 3
                     };
            ushort[] indices = new ushort[] { 0, 1, 2, 0, 2, 3 };
            //    // Set the viewport
            //    glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Viewport(0, 0, 800, 600);
            //    // Clear the color buffer
            //    glClear(GL_COLOR_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //    // Use the program object
            //    glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            //    // Load the vertex position
            //    glVertexAttribPointer(mPositionLoc, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), vertices);
            GL.VertexAttribPointer(mPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), vertices);
            //    // Load the texture coordinate
            //    glVertexAttribPointer(mTexCoordLoc, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), vertices + 3);
            unsafe
            {
                fixed (float* v_3 = &vertices[3])
                {
                    GL.VertexAttribPointer(mTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)v_3);
                }
            }
            //    glEnableVertexAttribArray(mPositionLoc);
            //    glEnableVertexAttribArray(mTexCoordLoc);
            GL.EnableVertexAttribArray(mPositionLoc);
            GL.EnableVertexAttribArray(mTexCoordLoc);
            //    // Bind the texture
            //    glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);
            //    glBindTexture(GL_TEXTURE_2D, mTexture);
            GL.BindTexture(TextureTarget.Texture2D, mTexture);
            //    // Set the texture sampler to texture unit to 0
            //    glUniform1i(mSamplerLoc, 0);
            GL.Uniform1(mSamplerLoc, 0);
            //    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, indices);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, indices);
            miniGLControl.SwapBuffers();
        }
        protected override void DemoClosing()
        {
            GL.DeleteProgram(mProgram);
            GL.DeleteTexture(mTexture);
            mProgram = mTexture = 0;
        }
        int mProgram;
        // Attribute locations
        int mPositionLoc;
        int mTexCoordLoc;
        // Sampler location
        int mSamplerLoc;
        // Texture handle
        int mTexture;
    }
}
