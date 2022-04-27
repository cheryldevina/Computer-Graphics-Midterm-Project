using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Render3D
{
    class Node
    {
        public String Name;
        public Node? Parent;
        public List<Node> Children;
        public Vector3 PositionVector;
        public Quaternion RotationQuatertion;
        public Vector3 ScaleVector;
        public Matrix4 TransformMatrix;
        public Matrix4 ParentInverseMatrix;
        public List<int> MeshIndices;
        public Model ParentModel;
        public Matrix4 OriginMatrix;


        public Node(Model ParentModel, String Name)
        {
            this.Name = Name;
            this.ParentModel = ParentModel;
            MeshIndices = new List<int>();
            Children = new List<Node>();
            ParentModel.Nodes.Add(Name, this);
            TransformMatrix = Matrix4.Identity;
            OriginMatrix = Matrix4.Identity;
            ParentInverseMatrix = Matrix4.Identity;
            DecomposeTransformMatrix();
        }

        public Node AddChild(Node Child)
        {
            Child.Parent = this;
            RefreshTransformMatrix();
            Child.ParentInverseMatrix = this.TransformMatrix.Inverted();
            Children.Add(Child);
            return this;
        }

        public Node AddMesh(Mesh Generator, int MaterialIndex = 0)
        {
            Mesh NewMesh = Generator;
            NewMesh.MaterialIndex = MaterialIndex;
            MeshIndices.Add(ParentModel.Meshes.Count);
            ParentModel.Meshes.Add(NewMesh);
            return this;
        }

        public Node(Assimp.Node Node, Model ParentModel)
        {
            OriginMatrix = Matrix4.Identity;

            Name = Node.Name;

            ParentModel.Nodes.Add(Name, this);

            this.ParentModel = ParentModel;

            MeshIndices = new List<int>();
            foreach (var MeshIndex in Node.MeshIndices)
            {
                MeshIndices.Add(MeshIndex);
            }

            Assimp.Matrix4x4 ps = Node.Transform;

            TransformMatrix = new Matrix4(new Vector4(ps.A1, ps.A2, ps.A3, ps.A4), new Vector4(ps.B1, ps.B2, ps.B3, ps.B4), new Vector4(ps.C1, ps.C2, ps.C3, ps.C4), new Vector4(ps.D1, ps.D2, ps.D3, ps.D4));

            TransformMatrix.Transpose();

            DecomposeTransformMatrix();

            TransformMatrix = Matrix4.Identity;

            RefreshTransformMatrix();

            Children = new List<Node>();
            foreach (var child in Node.Children)
            {
                Node NewChild = new Node(child, ParentModel);
                NewChild.Parent = this;

                Console.WriteLine($"Loading Node: {Name} -> {child.Name}");

                Children.Add(NewChild);
            }


        }

        public void DecomposeTransformMatrix()
        {
            PositionVector = TransformMatrix.ExtractTranslation();
            ScaleVector = TransformMatrix.ExtractScale();
            RotationQuatertion = TransformMatrix.ExtractRotation();
        }

        public Node Scale(float X, float Y, float Z)
        {
            TransformMatrix *= Matrix4.CreateScale(new Vector3(X, Z, Y));
            DecomposeTransformMatrix();
            return this;
        }

        public Node Rotate(float X, float Y, float Z)
        {
            Vector3 TempPos = PositionVector;
            TransformMatrix *= Matrix4.CreateRotationX(X);
            TransformMatrix *= Matrix4.CreateRotationY(Z);
            TransformMatrix *= Matrix4.CreateRotationZ(-Y);
            DecomposeTransformMatrix();
            PositionVector = TempPos;
            RefreshTransformMatrix();
            return this; ;
        }

        public Node Translate(float X, float Y, float Z)
        {
            TransformMatrix *= Matrix4.CreateTranslation(new Vector3(X / 2, Z / 2, -Y / 2));
            DecomposeTransformMatrix();
            return this;
        }

        public Node SetScale(float X, float Y, float Z)
        {
            ScaleVector.X = X;
            ScaleVector.Y = Z;
            ScaleVector.Z = Y;
            RefreshTransformMatrix();
            return this;
        }

        public Node SetRotation(float X, float Y, float Z, float W)
        {
            this.RotationQuatertion.X = X;
            this.RotationQuatertion.Y = Z;
            this.RotationQuatertion.Z = -Y;
            this.RotationQuatertion.W = W;
            RefreshTransformMatrix();
            return this;
        }

        public Node SetPosition(float X, float Y, float Z)
        {
            this.PositionVector.X = X / 2;
            this.PositionVector.Y = Z / 2;
            this.PositionVector.Z = -Y / 2;
            RefreshTransformMatrix();
            return this;
        }

        public void RefreshTransformMatrix()
        {
            TransformMatrix = Matrix4.Identity;
            TransformMatrix *= Matrix4.CreateScale(ScaleVector);
            TransformMatrix *= Matrix4.CreateFromQuaternion(RotationQuatertion);
            TransformMatrix *= Matrix4.CreateTranslation(PositionVector);
        }
        public void Render(Matrix4? ParentTransform = null)
        {

            Matrix4 ParentScale;
            if (ParentTransform == null)
                ParentTransform = Matrix4.Identity;



            RefreshTransformMatrix();
            Matrix4 AppliedTransform = (Matrix4)(OriginMatrix.Inverted() * TransformMatrix * ParentInverseMatrix * OriginMatrix * ParentTransform);

            foreach (var MeshIndex in MeshIndices)
            {
                Mesh mesh = ParentModel.Meshes[MeshIndex];
                Material material = ParentModel.Materials[mesh.MaterialIndex];
                //Console.WriteLine(mesh.Buffers.VertexArray);
                Window.shader.SetInt("diffMap", 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, ParentModel.Textures[material.DiffuseMap].TextureHandle);
                Window.shader.SetMatrix4("model", AppliedTransform);
                Window.shader.SetVector4("material.ambient", material.Ambient);
                Window.shader.SetVector4("material.diffuse", material.Diffuse);
                GL.BindVertexArray(mesh.Buffers.VertexArray);
                GL.DrawElements(PrimitiveType.Triangles, mesh.Indices.Count, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            foreach (var Child in Children)
            {
                Child.Render(AppliedTransform);
            }
        }
        public void SetOrigin(float x, float y, float z)
        {
            OriginMatrix = Matrix4.CreateTranslation(x / 2, z / 2, -y / 2);
        }
    }
}