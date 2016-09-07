#region License
// Copyright (c) 2013 Antonie Blom
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
#endregion


using System;
namespace Pencil.Gaming
{
    public sealed class MouseState
    {
        public bool LeftButton { get; private set; }
        public bool MiddleButton { get; private set; }
        public bool RightButton { get; private set; }
        public int ScrollWheel { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }

        private MouseState()
        {
        }

        public static MouseState GetMouseState(GlfwWindowPtr window)
        {
            MouseState result = new MouseState();
            result.LeftButton = Glfw.GetMouseButton(window, MouseButton.LeftButton);
            result.MiddleButton = Glfw.GetMouseButton(window, MouseButton.MiddleButton);
            result.RightButton = Glfw.GetMouseButton(window, MouseButton.RightButton);
            double x, y;
            Glfw.GetCursorPos(window, out x, out y);
            result.X = x;
            result.Y = y;
            return result;
        }
    }
}

