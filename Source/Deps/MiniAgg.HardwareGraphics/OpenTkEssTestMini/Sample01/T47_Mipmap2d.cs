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
    [Info(OrderCode = "047")]
    [Info("T47_TextureWrap")]
    public class T47_Mipmap2d : PrebuiltGLControlDemoBase
    {

        bool isGLInit;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {

            string vs = @"
                 uniform float u_offset;
                 attribute vec4 a_position;
                 attribute vec2 a_texCoord;
                 varying vec2 v_texCoord; 
                 void main()
                 {
                        gl_Position = a_position;
                        gl_Position.x += u_offset;
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
                throw new NotSupportedException();
            }

            //// Get the attribute locations
            //mPositionLoc = glGetAttribLocation(mProgram, "a_position");
            mPositionLoc = GL.GetAttribLocation(mProgram, "a_position");
            //mTexCoordLoc = glGetAttribLocation(mProgram, "a_texCoord");
            mTexCoordLoc = GL.GetAttribLocation(mProgram, "a_texCoord");

            //// Get the sampler location
            //mSamplerLoc = glGetUniformLocation(mProgram, "s_texture");
            mSamplerLoc = GL.GetUniformLocation(mProgram, "s_texture");
            //// Get the offset location
            //mOffsetLoc = glGetUniformLocation(mProgram, "u_offset");
            mOffsetLoc = GL.GetUniformLocation(mProgram, "u_offset");
            //// Load the texture
            //mTextureID = CreateMipMappedTexture2D();
            mTextureID = ES2Utils2.CreateMipMappedTexture2D();

            //// Check Anisotropy limits
            //glGetFloatv(GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, &mMaxAnisotropy);
            GL.GetFloat((GetPName)(ExtTextureFilterAnisotropic.MAX_TEXTURE_MAX_ANISOTROPY), out mMaxAnisotropy);
            //glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.ClearColor(0, 0, 0, 0);
            isGLInit = true;
            this.EnableAnimationTimer = true;
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

            float[] vertices = new float[]        {
            -0.25f,  0.5f, 0.0f, 5.0f, // Position 0
             0.0f,  0.0f,              // TexCoord 0
            -0.25f, -0.5f, 0.0f, 1.0f, // Position 1
             0.0f,  1.0f,              // TexCoord 1
             0.25f, -0.5f, 0.0f, 1.0f, // Position 2
             1.0f,  1.0f,              // TexCoord 2
             0.25f,  0.5f, 0.0f, 5.0f, // Position 3
             1.0f,  0.0f               // TexCoord 3
            };
            ushort[] indices = new ushort[] { 0, 1, 2, 0, 2, 3 };

            // Set the viewport
            //glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Viewport(0, 0, this.Width, this.Height);
            // Clear the color buffer
            //glClear(GL_COLOR_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Use the program object
            //glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            // Load the vertex position
            //glVertexAttribPointer(mPositionLoc, 4, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), vertices);
            GL.VertexAttribPointer(mPositionLoc, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), vertices);
            // Load the texture coordinate
            //glVertexAttribPointer(mTexCoordLoc, 2, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), vertices + 4);
            unsafe
            {
                fixed (float* h = &vertices[0])
                {
                    GL.VertexAttribPointer(mTexCoordLoc, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), (IntPtr)(h + 4));
                }
            }
            //glEnableVertexAttribArray(mPositionLoc);
            GL.EnableVertexAttribArray(mPositionLoc);
            //glEnableVertexAttribArray(mTexCoordLoc);            
            GL.EnableVertexAttribArray(mTexCoordLoc);
            // Bind the texture
            //glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);
            //glBindTexture(GL_TEXTURE_2D, mTextureID);
            GL.BindTexture(TextureTarget.Texture2D, mTextureID);
            // Set the sampler texture unit to 0
            //glUniform1i(mSamplerLoc, 0);
            GL.Uniform1(mSamplerLoc, 0);
            // Draw quad with nearest sampling
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);

            //glUniform1f(mOffsetLoc, -0.6f);
            GL.Uniform1(mOffsetLoc, -0.6f);
            //glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, indices);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, indices);
            // Draw quad with trilinear filtering
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            //glUniform1f(mOffsetLoc, 0.0f);
            GL.Uniform1(mOffsetLoc, 0.0f);
            //glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, indices);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, indices);
            // Draw quad with anisotropic filtering
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            //glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAX_ANISOTROPY_EXT, mMaxAnisotropy);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)(ExtTextureFilterAnisotropic.TEXTURE_MAX_ANISOTROPY), mMaxAnisotropy);

            //glUniform1f(mOffsetLoc, 0.6f);
            GL.Uniform1(mOffsetLoc, 0.6f);
            //glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, indices);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, indices);
            //glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAX_ANISOTROPY_EXT, 1.0f);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)(ExtTextureFilterAnisotropic.TEXTURE_MAX_ANISOTROPY), 1f);

            this.miniGLControl.SwapBuffers();

        }
        protected override void DemoClosing()
        {
            GL.DeleteProgram(mProgram);
            GL.DeleteTexture(mTextureID);
            mProgram = mTextureID = 0;
        }
        int mProgram;

        // Attribute locations
        int mPositionLoc;
        int mTexCoordLoc;

        // Sampler location
        int mSamplerLoc;

        // Offset location
        int mOffsetLoc;


        // Texture handle
        int mTextureID;

        float mMaxAnisotropy;
    }
}