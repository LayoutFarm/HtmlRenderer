//
// Copyright (c) 2014 The ANGLE Project Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;


namespace OpenTK.Graphics.ES20
{
    public static class ES2Utils2
    {
        public static int CreateSimpleTexture2D()
        {
            // Use tightly packed data
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            //glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

            // Generate a texture object
            //GLuint texture;
            int texture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texture);

            // Bind the texture object
            // glBindTexture(GL_TEXTURE_2D, texture);

            // Load the texture: 2x2 Image, 3 bytes per pixel (R, G, B)
            const int width = 2;
            const int height = 2;
            //        GLubyte pixels[width * height * 3] =
            //{
            //    255,   0,   0, // Red
            //      0, 255,   0, // Green
            //      0,   0, 255, // Blue
            //    255, 255,   0, // Yellow
            //};
            byte[] pixels = new byte[width * height * 3]
            {
                  255,   0,   0, // Red
                  0, 255,   0, // Green
                  0,   0, 255, // Blue
                255, 255,   0, // Yellow
            };

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, pixels);

            // Set the filtering mode

            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            return texture;
        }

        public static int CreateMipMappedTexture2D()
        {
            int width = 256;
            int height = 256;
            byte[] pixels = new byte[width * height * 3];
            int checkerSize = 8;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    byte rColor = 0;
                    byte bColor = 0;
                    if ((x / checkerSize) % 2 == 0)
                    {
                        rColor = (byte)(255 * ((y / checkerSize) % 2));
                        bColor = (byte)(255 * (1 - ((y / checkerSize) % 2)));
                    }
                    else
                    {

                        bColor = (byte)(255 * ((y / checkerSize) % 2));
                        rColor = (byte)(255 * (1 - ((y / checkerSize) % 2)));
                    }

                    pixels[(y * height + x) * 3] = rColor;
                    pixels[(y * height + x) * 3 + 1] = 0;
                    pixels[(y * height + x) * 3 + 2] = bColor;
                }
            }
            // Generate a texture object
            int texture;
            //    glGenTextures(1, &texture);
            GL.GenTextures(1, out texture);
            // Bind the texture object
            //glBindTexture(GL_TEXTURE_2D, texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            // Load mipmap level 0
            // glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, pixels.data());
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);
            //    // Generate mipmaps
            //    glGenerateMipmap(GL_TEXTURE_2D);
            GL.GenerateMipmap(TextureTarget.Texture2D);

            //    // Set the filtering mode
            //    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_NEAREST);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            //    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            return texture;
        }
    }
}