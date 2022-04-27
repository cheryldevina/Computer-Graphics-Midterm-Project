using OpenTK.Mathematics;

namespace Render3D
{
    class Camera
    {
        public Vector3 Position;
        public Vector3 Up;
        public Vector3 Front;
        public float AspectRatio;
        public float FOV;
        public float NearPlane;
        public float FarPlane;
        public float Pitch;
        public float Yaw;
        public float Sensitivity;
        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;

        public Camera(float fov, float aspect, float near, float far)
        {
            NearPlane = near;
            FarPlane = far;
            FOV = fov;
            AspectRatio = aspect;
            Position = new Vector3(0, 0, 0);
            Up = new Vector3(0, 1, 0);
            Front = new Vector3(0.0f, 0.0f, -1.0f);
            Pitch = 0;
            Yaw = 0;
            Sensitivity = 1;

            RefreshProjectionMatrix();
            RefreshLookAt();
        }

        public void LookAt(Vector3 To)
        {
            Front = To;
            ViewMatrix = Matrix4.LookAt(Position, To, Up);
        }

        public void RefreshLookAt()
        {
            ViewMatrix = Matrix4.LookAt(Position, Position+Front, Up);
        }

        public void RefreshProjectionMatrix()
        {
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV.Rad(), AspectRatio, NearPlane, FarPlane);
        }
    }
}
