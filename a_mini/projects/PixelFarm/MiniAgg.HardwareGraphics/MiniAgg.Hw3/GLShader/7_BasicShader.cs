﻿//MIT 2014-2016, WinterDev

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class BasicShader
    {
        bool isInited;
        MiniShaderProgram shaderProgram;
        ShaderVtxAttrib2f a_position;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar4 u_solidColor;
        MyMat4 mymat4;
        public BasicShader()
        {
            shaderProgram = new MiniShaderProgram();
            InitShader();
        }
        public void UnloadShader()
        {
            shaderProgram.DeleteMe();
            shaderProgram = null;
        }
        void InitShader()
        {
            if (isInited) { return; }
            //----------------

            //vertex shader source
            string vs = @"        
            attribute vec3 a_position;  
            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;    
            varying vec4 v_color;              
            void main()
            {
                float a= a_position[2]; //before matrix op
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                v_color= u_solidColor;   
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color;                   
                void main()
                {       
                    gl_FragColor= v_color;
                }
            ";
            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }

            a_position = shaderProgram.GetAttrV2f("a_position");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
            shaderProgram.UseProgram();
            isInited = true;
        }
        public MyMat4 ViewMatrix
        {
            get { return this.mymat4; }
            set
            {
                this.mymat4 = value;
                u_matrix.SetData(this.ViewMatrix.data);
            }
        }
        public void DrawLine(float x1, float y1, float x2, float y2, PixelFarm.Drawing.Color color)
        {
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            unsafe
            {
                float* vtx = stackalloc float[4];
                vtx[0] = x1; vtx[1] = y1;
                vtx[2] = x2; vtx[3] = y2;
                a_position.UnsafeLoadPureV2f(vtx);
            }
            GL.DrawArrays(BeginMode.Lines, 0, 2);
        }
    }
}