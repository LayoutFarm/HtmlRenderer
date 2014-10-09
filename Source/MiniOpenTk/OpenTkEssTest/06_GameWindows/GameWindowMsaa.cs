// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Examples
{
    //[Example("GameWindow MSAA", ExampleCategory.OpenTK, "GameWindow", 2, Documentation = "GameWindowMsaa")]
    public class FullscreenAntialias : GameWindow
    {
        public FullscreenAntialias()
            : base(800, 600, new GraphicsMode(32, 0, 0, 4))
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
        }

        #region Keyboard_KeyDown

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="key">The key that was pressed.</param>
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Exit();

            if (e.Key == Key.F11)
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        #endregion

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {

            GL.ClearColor(Color.White);
            //GL.Enable(EnableCap.PolygonSmooth);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlphaSaturate, BlendingFactorDest.One);
            //GL.Hint(HintTarget.PolygonSmoothHint, HintMode.DontCare);

            //glCullFace(GL_BACK);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlphaSaturate, BlendingFactorDest.One);
            //glEnable(GL_CULL_FACE);
            //glBlendFunc(GL_SRC_ALPHA_SATURATE, GL_ONE);
            //glClearColor(0.0, 0.0, 0.0, 0.0);

            //glEnable(GL_LINE_SMOOTH);
            //glEnable(GL_BLEND);
            //glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            //glHint(GL_LINE_SMOOTH_HINT, GL_DONT_CARE);
        }

        #endregion

        #region OnResize

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

            base.OnResize(e);
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Nothing to do!
        }

        #endregion



        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            //GL.Enable(EnableCap.Blend);
            //GL.Enable(EnableCap.PolygonSmooth);
            //GL.Disable(EnableCap.DepthTest);
            //glEnable(GL_BLEND);
            //glEnable(GL_POLYGON_SMOOTH);
            //glDisable(GL_DEPTH_TEST);


            GL.Begin(BeginMode.Triangles);

            GL.Color3(Color.MidnightBlue);
            GL.Vertex2(-1.0f, 1.0f);
            GL.Color3(Color.SpringGreen);
            GL.Vertex2(0.0f, -1.0f);
            GL.Color3(Color.Ivory);
            GL.Vertex2(1.0f, 1.0f);

            GL.End();

            this.SwapBuffers();
        }
    }
}
