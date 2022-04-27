using Assimp;
using OpenTK.Mathematics;

namespace Render3D
{
    class Model
    {
        public List<Mesh> Meshes;
        public List<Material> Materials;
        public Dictionary<string, Texture> Textures;
        public Dictionary<string, Node> Nodes;
        public Node ParentNode;
        public List<Timeline> Timelines;

        public Model(string path)
        {
            Assimp.Scene loader;
            AssimpContext importer = new AssimpContext();
            loader = importer.ImportFile(path, PostProcessSteps.Triangulate);
            String directory = Path.GetDirectoryName(path);
            directory += "\\";

            Meshes = new List<Mesh>();
            Materials = new List<Material>();
            Textures = new Dictionary<string, Texture>();
            Nodes = new Dictionary<string, Node>();
            Timelines = new List<Timeline>();

            Textures.Add("BLACK_DEFAULT_MAP", new Texture(new List<byte>(4 * 1 * 1) { 0, 0, 0, 1 }.ToArray(), 1, 1));

            Console.WriteLine("\n[Meshes]");

            foreach (var mesh in loader.Meshes)
            {
                Console.WriteLine("Loading Mesh: " + mesh.Name);
                Meshes.Add(new Mesh(mesh));
            }


            Console.WriteLine("\n[Materials & Textures]");

            foreach (var material in loader.Materials)
            {

                Console.WriteLine("Loading Material: " + material.Name);
                Materials.Add(new Material(material));

                Console.WriteLine("Loading Texture: " + material.TextureDiffuse.FilePath);
                if (material.HasTextureDiffuse)
                    if (material.TextureDiffuse.FilePath != "")
                        if (!Textures.ContainsKey(material.TextureDiffuse.FilePath))
                        {
                            EmbeddedTexture EmbeddedTexture = loader.GetEmbeddedTexture(material.TextureDiffuse.FilePath);
                            if (EmbeddedTexture != null)
                            {
                                if (EmbeddedTexture.IsCompressed)
                                    Textures.Add(material.TextureDiffuse.FilePath, new Texture(EmbeddedTexture.CompressedData));
                            }
                            else
                                Textures.Add(material.TextureDiffuse.FilePath, new Texture(directory + material.TextureDiffuse.FilePath));
                        }
            }


            Console.WriteLine("\n[Nodes]");
            ParentNode = new Node(loader.RootNode, this);

            float MaxAnimationTime = 0;

            Console.WriteLine("\n[Animations]\n");

            foreach (var animation in loader.Animations)
                if (animation.DurationInTicks / animation.TicksPerSecond > MaxAnimationTime)
                    MaxAnimationTime = (float)animation.DurationInTicks / (float)animation.TicksPerSecond;

            foreach (var animation in loader.Animations)
                foreach (var node in animation.NodeAnimationChannels)
                {
                    Console.WriteLine("Loading Animation: " + animation.Name + " -> " + node.NodeName);
                    Timelines.Add(new Timeline(Nodes[node.NodeName], (float)animation.TicksPerSecond, MaxAnimationTime * (float)animation.TicksPerSecond, node.ScalingKeys, node.RotationKeys, node.PositionKeys));
                }

            Console.WriteLine("\nLoading Complete!\n");
        }

        public Model()
        {
            Meshes = new List<Mesh>();
            Materials = new List<Material>();
            Textures = new Dictionary<string, Texture>();
            Nodes = new Dictionary<string, Node>();
            Timelines = new List<Timeline>();

            Textures.Add("BLACK_DEFAULT_MAP", new Texture(new List<byte>(4 * 1 * 1) { 0, 0, 0, 1 }.ToArray(), 1, 1));

            ParentNode = new Node(this, "ROOT");
        }

        public int AddColorMaterial(int R = 255, int G = 0, int B = 0, int A = 255) {
            Material NewMaterial = new Material(R, G, B, A);
            Materials.Add(NewMaterial);
            return Materials.Count - 1;
        }

        public void Render()
        {
            ParentNode.Render();
        }
    }
}



