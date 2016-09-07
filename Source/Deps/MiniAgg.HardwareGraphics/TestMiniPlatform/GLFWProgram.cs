//MIT, 2016, WinterDev
using System;
using OpenTK.Graphics.ES20;
using Pencil.Gaming;

namespace TestGlfw
{
    class GLFWProgram
    {

        public static void Start()
        {

            if (!Glfw.Init())
            {
                Console.WriteLine("can't init glfw");
                return;
            }
            //---------------------------------------------------
            //specific OpenGLES
            Glfw.WindowHint(WindowHint.GLFW_CLIENT_API, (int)OpenGLAPI.OpenGLESAPI);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_CREATION_API, (int)OpenGLContextCreationAPI.GLFW_EGL_CONTEXT_API);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MAJOR, 2);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MINOR, 0);
            //---------------------------------------------------
            GlfwMonitorPtr monitor = new GlfwMonitorPtr();
            GlfwWindowPtr winPtr = new GlfwWindowPtr();
            GlfwWindowPtr glWindow = Glfw.CreateWindow(800, 600, "Test Glfw", monitor, winPtr);

            /* Make the window's context current */
            Glfw.MakeContextCurrent(glWindow);
            Glfw.SwapInterval(1);
            GlfwWindowPtr currentContext = Glfw.GetCurrentContext();



            var contextHandler = new OpenTK.ContextHandle(currentContext.inner_ptr);
            var context = OpenTK.Graphics.GraphicsContext.CreateDummyContext(contextHandler);
            bool isCurrent = context.IsCurrent;
            PixelFarm.GlfwWinInfo winInfo = new PixelFarm.GlfwWinInfo(glWindow.inner_ptr);
            context.MakeCurrent(winInfo);
            //-------------------------------------- 
            //bind open gl funcs here..
            new OpenTK.Graphics.ES20.GL().LoadEntryPoints();
            //-------------------------------------- 
            //create shader program
            var shaderProgram = new MiniShaderProgram();
            //-------------------------------------- 

            ShaderVtxAttrib3f a_position;
            ShaderVtxAttrib4f a_color;
            ShaderUniformMatrix4 u_matrix;
            ShaderUniformVar1 u_useSolidColor;
            ShaderUniformVar4 u_solidColor;

            MyMat4 orthoView;

            string vs = @"        
                attribute vec3 a_position;
                attribute vec4 a_color;  

                uniform mat4 u_mvpMatrix;
                uniform vec4 u_solidColor;
                uniform int u_useSolidColor;              

                varying vec4 v_color;
                varying vec4 a_position_output;
                void main()
                {

                    a_position_output =  u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                    gl_Position = a_position_output;
                    v_color=  vec4(1,0,0,1); 
                }
                ";
            //fragment source
            //            string fs = @"void main()
            //                {
            //                    gl_FragColor = vec4(0.0, 1.0, 0.0, 1.0);
            //                }
            //            ";
            string fs = @"
                    precision mediump float;
                    varying vec4 v_color;  
                    varying vec4 a_position_output;
                    void main()
                    {
                        if(a_position_output[1]>0.5){
                            gl_FragColor = vec4(0,1,1,1);
                        }else{
                            gl_FragColor= vec4(0,1,0,1); 
                        }
                    }
                ";
            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }


            a_position = shaderProgram.GetAttrV3f("a_position");
            a_color = shaderProgram.GetAttrV4f("a_color");

            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");

            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int ww_w = 800;
            int ww_h = 600;
            int max = Math.Max(ww_w, ww_h);
            //square viewport
            GL.Viewport(0, 0, max, max);
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            //--------------------------------------------------------------------------------

            //load image

            /* Loop until the user closes the window */
            int width, height;
            Glfw.GetFramebufferSize(glWindow, out width, out height);
            float ratio = (float)width / (float)height;

            GL.Viewport(0, 0, 800, 600);
            shaderProgram.UseProgram();


            while (!Glfw.WindowShouldClose(glWindow))
            {



                //set clear color to white
                GL.ClearColor(1f, 1, 1, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                //--------------------------------- 
                //---------------------------------------------------------  
                u_matrix.SetData(orthoView.data);
                //---------------------------------------------------------  

                //DrawLines(0, 0, 300, 300);
                float x1 = 50, y1 = 20,
                     x2 = 300, y2 = 20;


                float[] vtxs = new float[] {
                        x1, y1, 1,
                        x2, y2, 1,
                        50, 300, 1
                    };

                u_useSolidColor.SetValue(1);
                u_solidColor.SetValue(1f, 0f, 0f, 1f);//use solid color  
                a_position.LoadPureV3f(vtxs);
                GL.DrawArrays(BeginMode.Triangles, 0, 3);

                //--------------------------------------------------------- 
                //GL.MatrixMode(MatrixMode.Modelview);
                //GL.LoadIdentity(); 
                //GL.Begin(BeginMode.Triangles); 
                //GL.Color3(1f, 0, 0);
                //GL.Vertex3(-0.6f, -0.4f, 0f); 
                //GL.Color3(0f, 1f, 0);
                //GL.Vertex3(0.6f, -0.4f, 0f); 
                //GL.Color3(0f, 0, 1f);
                //GL.Vertex3(0.0f, 0.6f, 0f); 
                //GL.End();

                //---------------------------------
                /* Render here */
                /* Swap front and back buffers */
                Glfw.SwapBuffers(glWindow);
                /* Poll for and process events */
                Glfw.PollEvents();
            }
            Glfw.Terminate();
        }
    }
}