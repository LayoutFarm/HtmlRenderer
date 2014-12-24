//
// Copyright (c) 2014 The ANGLE Project Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//

//            Based on Simple_VertexShader.c from
// Book:      OpenGL(R) ES 2.0 Programming Guide
// Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
// ISBN-10:   0321502795
// ISBN-13:   9780321502797
// Publisher: Addison-Wesley Professional
// URLs:      http://safari.informit.com/9780321563835
//            http://www.opengles-book.com



#region Using Directives

using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics; 
using OpenTK.Graphics.ES20;
using Examples.Tutorial;
using Mini;

#endregion

namespace OpenTkEssTest
{
    [Info(OrderCode = "048")]
    [Info("T48_MultiTexture")]
    public class T48_MultiTexture : PrebuiltGLControlDemoBase
    {

        bool isGLInit;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {

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
                 uniform sampler2D s_baseMap;
                 uniform sampler2D s_lightMap;
                 void main()
                 {
                        vec4 baseColor;
                        vec4 lightColor;
                        baseColor = texture2D(s_baseMap, v_texCoord);
                        lightColor = texture2D(s_lightMap, v_texCoord);
                        gl_FragColor = baseColor * (lightColor + 0.25);
                 }
            ";

            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                throw new NotSupportedException();
            }

            // Get the attribute locations
            mPositionLoc = GL.GetAttribLocation(mProgram, "a_position");
            mTexCoordLoc = GL.GetAttribLocation(mProgram, "a_texCoord");

            // Get the sampler location
            mBaseMapLoc = GL.GetUniformLocation(mProgram, "s_baseMap");
            mLightMapLoc = GL.GetUniformLocation(mProgram, "s_lightMap");

            // Load the textures
            mBaseMapTexID = LoadTexture(@"..\..\SampleImages\basemap01.png");
            mLightMapTexID = LoadTexture(@"..\..\SampleImages\lightmap01.png");
            if (mBaseMapTexID == 0 || mLightMapTexID == 0)
            {
                throw new NotSupportedException();
            }


            isGLInit = true;
            this.EnableAnimationTimer = true;
        }

        int LoadTexture(string imgFileName)
        {
            Bitmap bmp = new Bitmap(imgFileName);


            int texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

            //     glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, static_cast<GLsizei>(image.width), static_cast<GLsizei>(image.height), 0,
            //             GL_RGBA, GL_UNSIGNED_BYTE, image.data.data());
            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bmpdata.Scan0);
            bmp.UnlockBits(bmpdata);

            //glGenerateMipmap(GL_TEXTURE_2D);
            GL.GenerateMipmap(TextureTarget.Texture2D);
            return texture;
        }
        enum ExtTextureFilterAnisotropic
        {
            TEXTURE_MAX_ANISOTROPY = 0x84FE,
            MAX_TEXTURE_MAX_ANISOTROPY = 0x84FF
        }

        protected override void OnGLRender(object sender, EventArgs args)
        {

            if (!isGLInit)
            {
                return;
            }
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
            //GLushort indices[] = { 0, 1, 2, 0, 2, 3 };
            float[] vertices = new float[]{ 
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

            //// Set the viewport
            //glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Viewport(0, 0, this.Width, this.Height);
            //// Clear the color buffer
            //glClear(GL_COLOR_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //// Use the program object
            //glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            //// Load the vertex position
            //glVertexAttribPointer(mPositionLoc, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), vertices);
            GL.VertexAttribPointer(mPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), vertices);
            //// Load the texture coordinate
            //glVertexAttribPointer(mTexCoordLoc, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), vertices + 3);
            unsafe
            {
                fixed (float* h = &vertices[0])
                {
                    GL.VertexAttribPointer(mTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)(h + 3));
                }
            }
            //glEnableVertexAttribArray(mPositionLoc);
            GL.EnableVertexAttribArray(mPositionLoc);
            //glEnableVertexAttribArray(mTexCoordLoc);
            GL.EnableVertexAttribArray(mTexCoordLoc);
            //// Bind the base map
            //glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);
            //glBindTexture(GL_TEXTURE_2D, mBaseMapTexID);
            GL.BindTexture(TextureTarget.Texture2D, mBaseMapTexID);
            //// Set the base map sampler to texture unit to 0
            //glUniform1i(mBaseMapLoc, 0);
            GL.Uniform1(mBaseMapLoc, 0);
            //// Bind the light map
            //glActiveTexture(GL_TEXTURE1);
            GL.ActiveTexture(TextureUnit.Texture1);
            //glBindTexture(GL_TEXTURE_2D, mLightMapTexID);
            GL.BindTexture(TextureTarget.Texture2D, mLightMapTexID);
            //// Set the light map sampler to texture unit 1
            //glUniform1i(mLightMapLoc, 1);
            GL.Uniform1(mLightMapLoc, 1);
            //glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, indices);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, indices);
            this.miniGLControl.SwapBuffers();
        }

        protected override void DemoClosing()
        {
            GL.DeleteProgram(mProgram);
            GL.DeleteTexture(mBaseMapTexID);
            GL.DeleteTexture(mLightMapTexID);
            mProgram = mBaseMapTexID = mLightMapTexID = 0;
        }
        // Handle to a program object
        int mProgram;

        // Attribute locations
        int mPositionLoc;
        int mTexCoordLoc;

        // Sampler locations
        int mBaseMapLoc;
        int mLightMapLoc;

        // Texture handle
        int mBaseMapTexID;
        int mLightMapTexID;
    }
}