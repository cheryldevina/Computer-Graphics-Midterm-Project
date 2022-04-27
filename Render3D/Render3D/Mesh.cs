using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Render3D
{  
    struct BufferObject
    {
        public int VertexArray;
        public int Element;
        public int Vertex;
        public int TexCoord;
    }

    class Mesh
    {
        public int MaterialIndex;
        public List<uint> Indices;
        public List<Vector3> Positions;
        public List<Vector2> TexCoords;
        public BufferObject Buffers;
        public int[] _pascal = { };
        public Mesh()
        {
            Indices = new List<uint>();
            Positions = new List<Vector3>();
            TexCoords = new List<Vector2>();
            MaterialIndex = 0;

            InitMesh();
        }

        public Mesh(Assimp.Mesh mesh)
        {
            Indices = new List<uint>();
            Positions = new List<Vector3>();
            TexCoords = new List<Vector2>();
            MaterialIndex = 0;

            foreach (var face in mesh.Faces)
                foreach (var index in face.Indices)
                    Indices.Add((uint)index);

            for (int i = 0; i < mesh.Vertices.Count(); i++)
            {
                Positions.Add(new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z));
                if (mesh.HasTextureCoords(0))
                    TexCoords.Add(new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y));
                else
                    TexCoords.Add(new Vector2(0, 0));
            }

            MaterialIndex = mesh.MaterialIndex;

            InitMesh();
        }

        public void ClearMesh() {
            Indices.Clear();
            Positions.Clear();
            TexCoords.Clear();
        }

        public Mesh GenerateCube()
        {
            ClearMesh();

            Positions.Add(new Vector3(0.5f, 0.5f, 0.5f));
            Positions.Add(new Vector3(0.5f, 0.5f, -0.5f));
            Positions.Add(new Vector3(0.5f, -0.5f, 0.5f));
            Positions.Add(new Vector3(0.5f, -0.5f, -0.5f));
            Positions.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            Positions.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            Positions.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            Positions.Add(new Vector3(-0.5f, -0.5f, 0.5f));
            Positions.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            Positions.Add(new Vector3(0.5f, 0.5f, -0.5f));
            Positions.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            Positions.Add(new Vector3(0.5f, 0.5f, 0.5f));
            Positions.Add(new Vector3(-0.5f, -0.5f, 0.5f));
            Positions.Add(new Vector3(0.5f, -0.5f, 0.5f));
            Positions.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            Positions.Add(new Vector3(0.5f, -0.5f, -0.5f));
            Positions.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            Positions.Add(new Vector3(0.5f, 0.5f, 0.5f));
            Positions.Add(new Vector3(-0.5f, -0.5f, 0.5f));
            Positions.Add(new Vector3(0.5f, -0.5f, 0.5f));
            Positions.Add(new Vector3(0.5f, 0.5f, -0.5f));
            Positions.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            Positions.Add(new Vector3(0.5f, -0.5f, -0.5f));
            Positions.Add(new Vector3(-0.5f, -0.5f, -0.5f));

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0,0));
            }

            Indices = new List<uint>(new uint[] {
                0,2,1,
                2,3,1,
                4,6,5,
                6,7,5,
                8,10,9,
                10,11,9,
                12,14,13,
                14,15,13,
                16,18,17,
                18,19,17,
                20,22,21,
                22,23,21
            });

            InitMesh();
            return this;
        }

        public Mesh GenerateBall(float Percent = 1)
        {
            ClearMesh();

            int sharpness = Constants.ROUND_OBJECT_DETAIL_LEVEL;
            int hCount = sharpness;
            int vCount = sharpness;
            float radius = 0.5f;
            float divide = 1 / Percent;
            float PI = (float)Math.PI;
            float hStep = 2 * PI / hCount;
            float vStep = PI / vCount;
            float hAngle, vAngle;

            for (int i = 0; i <= vCount; i++)
            {
                vAngle = PI / 2 - i * vStep;

                for (int j = 0; j <= hCount / divide; j++)
                {
                    hAngle = j * hStep;

                    float x = radius * (float)Math.Cos(vAngle) * (float)Math.Cos(hAngle);
                    float y = radius * (float)Math.Cos(vAngle) * (float)Math.Sin(hAngle);
                    float z = radius * (float)Math.Sin(vAngle);

                    Positions.Add(new Vector3(x, -z, y));
                }
            }
            Positions.Add(new Vector3(0, 0, 0));

            uint k1, k2;
            for (int i = 0; i < vCount; i++)
            {
                k1 = (uint)i * ((uint)(hCount / divide) + 1);
                k2 = (uint)k1 + (uint)(hCount / divide) + 1;

                for (int j = 0; j < hCount / divide; j++, k1++, k2++)
                {
                    if (i != 0)
                    {
                        Indices.Add(k1);
                        Indices.Add(k2);
                        Indices.Add(k1 + 1);
                    }

                    if (i != (vCount - 1))
                    {
                        Indices.Add(k1 + 1);
                        Indices.Add(k2);
                        Indices.Add(k2 + 1);
                    }

                    if (Percent < 1.0f)
                    {
                        Indices.Add(k1);
                        Indices.Add((uint)(Positions.Count - 1));
                        Indices.Add(k1 + 1);

                        Indices.Add(k1 + 1);
                        Indices.Add((uint)(Positions.Count - 1));
                        Indices.Add(k2 + 1);
                    }
                }
            }

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0, 0));
            }

            InitMesh();

            return this;
        }

        public Mesh GenerateCylinder(float Percent = 1, float TopDiameter = 1)
        {
            ClearMesh();

            float length = 1;
            float botdia = 1;
            int sharpness = Constants.ROUND_OBJECT_DETAIL_LEVEL;
            int hCount = sharpness;
            int vCount = 1;
            float divide = 1 / Percent;
            float PI = (float)Math.PI;
            float hStep = 2 * PI / hCount;
            float vStep = length / vCount;
            float hAngle, vAngle;

            for (int i = 0; i <= vCount; i++)
            {
                vAngle = i * vStep;

                for (int j = 0; j <= hCount / divide; j++)
                {
                    hAngle = j * hStep;
                    float x = 0;
                    float y = 0;
                    if (i == 0)
                    {
                        x = TopDiameter / 2 * (float)Math.Cos(hAngle);
                        y = TopDiameter / 2 * (float)Math.Sin(hAngle);
                    }
                    else if (i == 1)
                    {
                        x = botdia / 2 * (float)Math.Cos(hAngle);
                        y = botdia / 2 * (float)Math.Sin(hAngle);
                    }

                    float z = vAngle - length / 2;

                    Positions.Add(new Vector3(x, -z, y));
                }
            }

            Positions.Add(new Vector3(0, length / 2, 0));
            Positions.Add(new Vector3(0, -length / 2, 0));

            uint k1, k2;
            for (int i = 0; i < vCount; i++)
            {
                k1 = (uint)i * ((uint)(hCount / divide) + 1);
                k2 = (uint)k1 + (uint)(hCount / divide) + 1;

                for (int j = 0; j < hCount / divide; j++, k1++, k2++)
                {
                    Indices.Add(k1);
                    Indices.Add(k2);
                    Indices.Add(k1 + 1);

                    Indices.Add(k1 + 1);
                    Indices.Add(k2);
                    Indices.Add(k2 + 1);

                    if (i == 0)
                    {
                        Indices.Add(k1);
                        Indices.Add((uint)Positions.Count - 2);
                        Indices.Add(k1 + 1);

                    }

                    Indices.Add(k2);
                    Indices.Add((uint)Positions.Count - 1);
                    Indices.Add(k2 + 1);
                }
            }

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0, 0));
            }

            InitMesh();
            return this;
        }

        public Mesh GenerateCone(float Percent = 1)
        {
            return GenerateCylinder(Percent, 0);
        }

        public Mesh GenerateTorus(float Percent = 1, float TubeDiameter = 1, int sharpnessV = 0)
        {
            ClearMesh();

            float torusdia = 1f;
            TubeDiameter = 0.25f * TubeDiameter;
            int sharpness;
            if (sharpnessV == 0)
            {
                sharpness = Constants.ROUND_OBJECT_DETAIL_LEVEL;
            }
            else
            {
                sharpness = sharpnessV;
            }
            int hCount = sharpness;
            int vCount = sharpness;
            float divide = 1 / Percent;
            float PI = (float)Math.PI;
            float hStep = 2 * PI / hCount;
            float vStep = 2 * PI / vCount;
            float hAngle, vAngle;

            for (int i = 0; i <= vCount; i++)
            {
                vAngle = i * vStep;

                for (int j = 0; j <= hCount / divide; j++)
                {
                    hAngle = j * hStep;
                    float x = 0;
                    float y = 0;

                    x = (torusdia / 2 + TubeDiameter / 2 * (float)Math.Cos(vAngle)) * (float)Math.Cos(hAngle);
                    y = (torusdia / 2 + TubeDiameter / 2 * (float)Math.Cos(vAngle)) * (float)Math.Sin(hAngle);

                    float z = TubeDiameter / 2 * (float)Math.Sin(vAngle);

                    Positions.Add(new Vector3(x, -z, y));
                }
            }

            uint k1, k2;
            for (int i = 0; i < vCount; i++)
            {
                k1 = (uint)i * ((uint)(hCount / divide) + 1);
                k2 = (uint)k1 + (uint)(hCount / divide) + 1;
                for (int j = 0; j < hCount / divide; j++, k1++, k2++)
                {
                    Indices.Add(k1);
                    Indices.Add(k2);
                    Indices.Add(k1 + 1);

                    Indices.Add(k1 + 1);
                    Indices.Add(k2);
                    Indices.Add(k2 + 1);
                }
            }

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0, 0));
            }

            InitMesh();

            return this;
        }

        public Mesh GeneratePlane()
        {
            ClearMesh();

            Positions.Add(new Vector3(0.5f, 0, 0.5f));
            Positions.Add(new Vector3(0.5f, 0, -0.5f));
            Positions.Add(new Vector3(-0.5f, 0, 0.5f));
            Positions.Add(new Vector3(-0.5f, 0, -0.5f));
            //Positions.Add(new Vector3(0.5f, 0.005f, 0.5f));
            //Positions.Add(new Vector3(0.5f, 0.005f, -0.5f));
            //Positions.Add(new Vector3(-0.5f, 0.005f, 0.5f));
            //Positions.Add(new Vector3(-0.5f, 0.005f, -0.5f));

            Indices = new List<uint>(new uint[] {
                0, 1, 3,
                0, 3, 2,
                //4, 5, 7,
                //4, 7, 6
            });

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0, 0));
            }

            InitMesh();
            return this;
        }

        public Mesh GenerateTriangle(bool Right = false)
        {
            ClearMesh();

            if (!Right)
            {
                Positions.Add(new Vector3(0.5f, -0.005f, -0.5f));
                Positions.Add(new Vector3(0.5f, -0.005f, 0.5f));
                Positions.Add(new Vector3(-0.5f, -0.005f, 0));

                Positions.Add(new Vector3(0.5f, 0.005f, -0.5f));
                Positions.Add(new Vector3(0.5f, 0.005f, 0.5f));
                Positions.Add(new Vector3(-0.5f, 0.005f, 0));
            }
            else
            {
                Positions.Add(new Vector3(0.5f, -0.005f, -0.5f));
                Positions.Add(new Vector3(0.5f, -0.005f, 0.5f));
                Positions.Add(new Vector3(-0.5f, -0.005f, 0.5f));

                Positions.Add(new Vector3(0.5f, 0.005f, -0.5f));
                Positions.Add(new Vector3(0.5f, 0.005f, 0.5f));
                Positions.Add(new Vector3(-0.5f, 0.005f, 0.5f));
            }

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0, 0));
            }

            Indices = new List<uint>(new uint[] {
               0,1,2,
               3,4,5
            });

            InitMesh();
            return this;
        }

        public Mesh GenerateCircle(float Percent = 1)
        {
            ClearMesh();

            int sharpness = Constants.ROUND_OBJECT_DETAIL_LEVEL;
            int hCount = sharpness;
            float divide = 1 / Percent;
            float PI = (float)Math.PI;
            float hStep = 2 * PI / hCount;
            float hAngle;


            for (int j = 0; j <= hCount / divide; j++)
            {
                hAngle = j * hStep;
                float x = 0;
                float y = 0;

                x = (float)0.5 * (float)Math.Cos(hAngle);
                y = (float)0.5 * (float)Math.Sin(hAngle);

                float z = -0.005f;

                Positions.Add(new Vector3(x, -z, y));
            }

            Positions.Add(new Vector3(0, 0, 0));

            for (int j = 0; j <= hCount / divide; j++)
            {
                hAngle = j * hStep;
                float x = 0;
                float y = 0;

                x = (float)0.5 * (float)Math.Cos(hAngle);
                y = (float)0.5 * (float)Math.Sin(hAngle);

                float z = 0.005f;

                Positions.Add(new Vector3(x, -z, y));
            }

            Positions.Add(new Vector3(0, 0, 0));

            uint k1 = 0, k2 = 1;
            for (int j = 0; j < hCount / divide; j++, k1++, k2++)
            {
                Indices.Add(k1);
                Indices.Add((uint)(Positions.Count / 2.0f - 1));
                Indices.Add(k2);
            }

            k1 = (uint)(Positions.Count / 2.0f);
            k2 = (uint)(Positions.Count / 2.0f);
            for (int j = 0; j < hCount / divide; j++, k1++, k2++)
            {
                Indices.Add(k1);
                Indices.Add((uint)(Positions.Count - 1));
                Indices.Add(k2);
            }

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0, 0));
            }

            InitMesh();

            return this;
        }

        public Mesh GenerateCurvedCylinder(List<Vector2> Coords, float Percent = 1, float TopDiameter = 0.2f)
        {
            ClearMesh();

            int sharpness = Constants.ROUND_OBJECT_DETAIL_LEVEL * 10;
            int hCount = sharpness;
            float divide = 1 / Percent;
            float PI = (float)Math.PI;
            float hStep = 2 * PI / hCount;
            float hAngle, lastAngle = 0;

            Coords = generateCurve(Coords, sharpness);
            int vCount = Coords.Count;

            for (int i = 0; i < vCount; i++)
            {
                for (int j = 0; j <= hCount / divide; j++)
                {
                    hAngle = j * hStep;
                    float x = 0;
                    float y = 0;

                    x = 0 + TopDiameter / 2 * (float)Math.Cos(hAngle);
                    y = Coords[i].X + TopDiameter / 2 * (float)Math.Sin(hAngle);

                    float z = Coords[i].Y;
                    if (i != vCount - 1)
                    {
                        lastAngle = (float)Math.Atan2((Coords[i + 1].X - Coords[i].X), (Coords[i + 1].Y - Coords[i].Y));
                    }

                    Vector3 temp = new Vector3(x, y, z);
                    Vector3 clone = new Vector3(temp);
                    temp += new Vector3(0, -Coords[i].X, -clone.Z);
                    temp = new Vector3(new Vector4(temp, 1f) * Matrix4.CreateRotationX(-lastAngle));
                    temp += new Vector3(0, Coords[i].X, clone.Z);

                    Positions.Add(temp);
                }
            }
            Positions.Add(new Vector3(0, Coords[0].X, Coords[0].Y));
            Positions.Add(new Vector3(0, Coords.Last().X, Coords.Last().Y));

            for (int i = 0; i < Positions.Count; i++)
            {
                TexCoords.Add(new Vector2(0, 0));
            }

            uint k1, k2;
            for (int i = 0; i < vCount; i++)
            {
                k1 = (uint)i * ((uint)(hCount / divide) + 1);
                k2 = (uint)k1 + (uint)(hCount / divide) + 1;

                for (int j = 0; j < hCount / divide; j++, k1++, k2++)
                {
                    if (i != vCount - 1)
                    {
                        Indices.Add(k1);
                        Indices.Add(k2);
                        Indices.Add(k1 + 1);

                        Indices.Add(k1 + 1);
                        Indices.Add(k2);
                        Indices.Add(k2 + 1);
                    }
                    if (i == 0)
                    {
                        Indices.Add(k1);
                        Indices.Add((uint)Positions.Count - 2);
                        Indices.Add(k1 + 1);

                    }
                    else if (i == vCount - 1)
                    {
                        Indices.Add(k1);
                        Indices.Add((uint)Positions.Count - 1);
                        Indices.Add(k1 + 1);
                    }
                }
            }

            InitMesh();
            return this;
        }

        private static List<Vector2> generateCurve(List<Vector2> Coords, float detail = 10)
        {
            List<Vector2> p = new List<Vector2>();

            for (float t = 0.0f; t <= 1.0f; t += 1 / detail)
            {
                p.Add(setCurve(Coords, t));
            }

            return p;
        }


        private static Vector2 setCurve(List<Vector2> parr, float t)
        {
            Vector2 p = new Vector2(0, 0);

            for (int i = 0; i < parr.Count; i++)
            {
                p.X += getP(parr.Count - 1, i) * (float)Math.Pow((1 - t), parr.Count - 1 - i) * (float)Math.Pow(t, i) * parr[i].X;
                p.Y += getP(parr.Count - 1, i) * (float)Math.Pow((1 - t), parr.Count - 1 - i) * (float)Math.Pow(t, i) * parr[i].Y;
            }

            return p;
        }

        private static int getP(int n, int k)
        {
            int results = 1;

            if (k > n - k) {
                k = n - k;
            }

            for (int i = 0; i < k; ++i)
            {
                results *= (n - i);
                results /= (i + 1);
            }
            return results;
        }

        private void InitMesh()
        {
            Buffers.Vertex = GL.GenBuffer();
            Buffers.TexCoord = GL.GenBuffer();
            Buffers.Element = GL.GenBuffer();
            Buffers.VertexArray = GL.GenVertexArray();

            GL.BindVertexArray(Buffers.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffers.Vertex);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                Positions.Count * Vector3.SizeInBytes,
                Positions.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffers.TexCoord);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                TexCoords.Count * Vector2.SizeInBytes,
                TexCoords.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffers.Vertex);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffers.TexCoord);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Buffers.Element);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                Indices.Count * sizeof(uint),
                Indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

    }
}
