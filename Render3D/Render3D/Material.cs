using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render3D
{
    class Material
    {
        public Vector4 Ambient;
        public Vector4 Diffuse;
        public string DiffuseMap;

        public Material(int R = 255, int G = 0, int B = 0, int A = 255)
        {
            Ambient = new Vector4();
            Diffuse = new Vector4();
            DiffuseMap = "BLACK_DEFAULT_MAP";

            Ambient.X = 1;
            Ambient.Y = 1;
            Ambient.Z = 1;
            Ambient.W = 1;

            Diffuse.X = (float)R/255.0f;
            Diffuse.Y = (float)G/255.0f;
            Diffuse.Z = (float)B/255.0f;
            Diffuse.W = (float)A/255.0f;
        }

        public Material(Assimp.Material material)
        {

            Ambient = new Vector4();
            Diffuse = new Vector4();

            if (material.HasColorAmbient)
            {
                Ambient.X = material.ColorAmbient.R;
                Ambient.Y = material.ColorAmbient.G;
                Ambient.Z = material.ColorAmbient.B;
                Ambient.W = material.ColorAmbient.A;
            }
            else
            {
                Ambient.X = 1;
                Ambient.Y = 1;
                Ambient.Z = 1;
                Ambient.W = 1;
            }

            if (material.HasColorDiffuse)
            {
                Diffuse.X = material.ColorDiffuse.R;
                Diffuse.Y = material.ColorDiffuse.G;
                Diffuse.Z = material.ColorDiffuse.B;
                Diffuse.W = material.ColorDiffuse.A;
            }
            else
            {
                Diffuse.X = 1;
                Diffuse.Y = 0;
                Diffuse.Z = 0;
                Diffuse.W = 1;
            }

            DiffuseMap = "BLACK_DEFAULT_MAP";
            ClearDiffuseMap();

            if (material.HasTextureDiffuse)
            {
                LoadDiffuse(material.TextureDiffuse.FilePath);
            }
        }

        public void SetColor(int R = 255, int G = 0, int B = 0, int A = 255)
        {
            Ambient = new Vector4();
            Diffuse = new Vector4();
            DiffuseMap = "BLACK_DEFAULT_MAP";

            Ambient.X = 1;
            Ambient.Y = 1;
            Ambient.Z = 1;
            Ambient.W = 1;

            Diffuse.X = (float)R / 255.0f;
            Diffuse.Y = (float)G / 255.0f;
            Diffuse.Z = (float)B / 255.0f;
            Diffuse.W = (float)A / 255.0f;
        }

        public void ClearDiffuseMap()
        {
            DiffuseMap = "BLACK_DEFAULT_MAP";
            Diffuse.X = 1;
            Diffuse.Y = 0;
            Diffuse.Z = 0;
            Diffuse.W = 1;
        }

        public void LoadDiffuse(String diffusePath = "")
        {
            if (diffusePath == "")
                ClearDiffuseMap();
            else
            {
                DiffuseMap = diffusePath;
                Diffuse.X = 0;
                Diffuse.Y = 0;
                Diffuse.Z = 0;
                Diffuse.W = 1;
            }
        }
    }




}
