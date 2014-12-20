﻿//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;

using OpenTK.Graphics.ES20;


namespace Mini
{
    public struct ShaderVtxAttrib
    {
        internal readonly int location;

        public ShaderVtxAttrib(int location)
        {
            this.location = location;
        }



        /// <summary>
        /// load and enable
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="fieldCount"></param>
        /// <param name="startOffset"></param>
        public void LoadV3f(float[] vertices, int fieldCount, int startOffset)
        {
            BindV3f(vertices, fieldCount, startOffset);
            Enable();
        }
        public void LoadV4f(float[] vertices, int fieldCount, int startOffset)
        {
            BindV4f(vertices, fieldCount, startOffset);
            Enable();
        }
        /// <summary>
        /// load and enable
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="fieldCount"></param>
        /// <param name="startOffset"></param>
        public void LoadV2f(float[] vertices, int fieldCount, int startOffset)
        {
            BindV2f(vertices, fieldCount, startOffset);
            Enable();
        }

        public void BindV3f(float[] vertices, int fieldCount, int startOffset)
        {
            if (startOffset == 0)
            {
                GL.VertexAttribPointer(location,
                    3, //float3
                    VertexAttribPointerType.Float,
                    false,
                    fieldCount * sizeof(float), //total size
                    vertices);
            }
            else
            {
                unsafe
                {
                    fixed (float* h = &vertices[0])
                    {
                        GL.VertexAttribPointer(location,
                            3, //float3
                            VertexAttribPointerType.Float,
                            false,
                            fieldCount * sizeof(float), //total size
                            (IntPtr)(h + startOffset));
                    }
                }
            }
        }
        public void BindV2f(float[] vertices, int fieldCount, int startOffset)
        {
            if (startOffset == 0)
            {
                GL.VertexAttribPointer(location,
                    2, //float2
                    VertexAttribPointerType.Float,
                    false,
                    fieldCount * sizeof(float), //total size
                    vertices);
            }
            else
            {
                unsafe
                {
                    fixed (float* h = &vertices[0])
                    {
                        GL.VertexAttribPointer(location,
                            2, //float3
                            VertexAttribPointerType.Float,
                            false,
                            fieldCount * sizeof(float), //total size
                            (IntPtr)(h + startOffset));
                    }
                }
            }
        }
        public void BindV4f(float[] vertices, int fieldCount, int startOffset)
        {
            if (startOffset == 0)
            {
                GL.VertexAttribPointer(location,
                    4, //float4
                    VertexAttribPointerType.Float,
                    false,
                    fieldCount * sizeof(float), //total size
                    vertices);
            }
            else
            {
                unsafe
                {
                    fixed (float* h = &vertices[0])
                    {
                        GL.VertexAttribPointer(location,
                            4, //float3
                            VertexAttribPointerType.Float,
                            false,
                            fieldCount * sizeof(float), //total size
                            (IntPtr)(h + startOffset));
                    }
                }
            }
        }
        public void Enable()
        {
            GL.EnableVertexAttribArray(this.location);
        }
    }

    public struct ShaderUniformMatrix4
    {
        readonly int location;
        public ShaderUniformMatrix4(int location)
        {
            this.location = location;
        }
        public void SetData(int count, bool transpose, float[] mat)
        {
            GL.UniformMatrix4(this.location, count, transpose, mat);
        }
        public void SetData(float[] mat)
        {
            GL.UniformMatrix4(this.location, 1, false, mat);
        }


    }
    public struct ShaderUniformVar1
    {
        readonly int location;
        public ShaderUniformVar1(int location)
        {
            this.location = location;
        }
        public void SetValue(float value)
        {
            GL.Uniform1(this.location, value);
        }
        public void SetValue(int value)
        {
            GL.Uniform1(this.location, value);
        }


    }
    public struct ShaderUniformVar2
    {
        internal readonly int location;
        public ShaderUniformVar2(int location)
        {
            this.location = location;
        }

    }
    public struct ShaderUniformVar3
    {
        internal readonly int location;
        public ShaderUniformVar3(int location)
        {
            this.location = location;
        }

    }
    public struct ShaderUniformVar4
    {
        internal readonly int location;
        public ShaderUniformVar4(int location)
        {
            this.location = location;
        }
        public void SetValue(int a, int b, int c, int d)
        {
            GL.Uniform4(this.location, a, b, c, d);
        }
        public void SetValue(float a, float b, float c, float d)
        {
            GL.Uniform4(this.location, a, b, c, d);
        }

    }


    public class MiniShaderProgram
    {
        int mProgram;

        string vs;
        string fs;
        public void LoadVertexShaderSource(string vs)
        {
            this.vs = vs;
        }
        public void LoadFragmentShaderSource(string fs)
        {
            this.fs = fs;
        }
        public void DeleteMe()
        {
            GL.DeleteProgram(mProgram);
            this.mProgram = 0;
        }
        public bool Build()
        {

            mProgram = OpenTkEssTest.ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                return false;
            }
            return true;
        }
        public bool Build(string vs, string fs)
        {
            LoadVertexShaderSource(vs);
            LoadFragmentShaderSource(fs);
            mProgram = OpenTkEssTest.ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                return false;
            }
            return true;
        }
        public ShaderVtxAttrib GetVtxAttrib(string attrName)
        {
            return new ShaderVtxAttrib(GL.GetAttribLocation(mProgram, attrName));
        }

        public ShaderUniformVar1 GetUniform1(string uniformVarName)
        {
            return new ShaderUniformVar1(GL.GetUniformLocation(this.mProgram, uniformVarName));
        }
        public ShaderUniformVar2 GetUniform2(string uniformVarName)
        {
            return new ShaderUniformVar2(GL.GetUniformLocation(this.mProgram, uniformVarName));
        }
        public ShaderUniformVar3 GetUniform3(string uniformVarName)
        {
            return new ShaderUniformVar3(GL.GetUniformLocation(this.mProgram, uniformVarName));
        }
        public ShaderUniformVar4 GetUniform4(string uniformVarName)
        {
            return new ShaderUniformVar4(GL.GetUniformLocation(this.mProgram, uniformVarName));
        }
        public ShaderUniformMatrix4 GetUniformMat4(string uniformVarName)
        {
            return new ShaderUniformMatrix4(GL.GetUniformLocation(this.mProgram, uniformVarName));
        }

        public void UseProgram()
        {
            GL.UseProgram(mProgram);
        }
        public int ProgramId
        {
            get { return this.mProgram; }
        }
    }
}