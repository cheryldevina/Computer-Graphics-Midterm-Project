﻿using OpenTK.Mathematics;

namespace Render3D
{
    class Animator
    {
        public class Ticker
        {
            public float TPS;
            public float DurationInTicks;
            public float CurrentTick;
            public Timeline Timeline;

            public static List<Ticker> Tickers = new List<Ticker>();

            public Ticker(float TPS, float DurationInTicks, Timeline Timeline)
            {
                this.DurationInTicks = DurationInTicks;
                this.Timeline = Timeline;
                this.TPS = TPS;
                CurrentTick = 0;
                Ticker.Tickers.Add(this);
            }

            public static void Update(float delta)
            {
                foreach (var Ticker in Ticker.Tickers)
                {
                    Ticker.CurrentTick += Ticker.TPS * delta;
                    Ticker.CurrentTick %= Ticker.DurationInTicks;
                    Ticker.Timeline.Update(Ticker.CurrentTick);
                }

            }
        }
    }

    class Timeline
    {

        public List<float> ScaleKeys;
        public List<float> RotationKeys;
        public List<float> PositionKeys;

        public List<Vector3> ScaleValues;
        public List<Quaternion> RotationValues;
        public List<Vector3> PositionValues;

        public Node Node;

        public Timeline(Node Node, float TPS, float DurationInTicks, List<Assimp.VectorKey> SK, List<Assimp.QuaternionKey> RK, List<Assimp.VectorKey> PK)
        {
            this.Node = Node;

            ScaleKeys = new List<float>();
            RotationKeys = new List<float>();
            PositionKeys = new List<float>();

            ScaleValues = new List<Vector3>();
            RotationValues = new List<Quaternion>();
            PositionValues = new List<Vector3>();

            foreach (var ScaleKey in SK)
            {
                ScaleKeys.Add((float)ScaleKey.Time);
                ScaleValues.Add(new Vector3(ScaleKey.Value.X, ScaleKey.Value.Y, ScaleKey.Value.Z));
            }

            foreach (var RotationKey in RK)
            {
                RotationKeys.Add((float)RotationKey.Time);
                RotationValues.Add(new Quaternion(RotationKey.Value.X, RotationKey.Value.Y, RotationKey.Value.Z, RotationKey.Value.W));
            }

            foreach (var PositionKey in PK)
            {
                PositionKeys.Add((float)PositionKey.Time);
                PositionValues.Add(new Vector3(PositionKey.Value.X, PositionKey.Value.Y, PositionKey.Value.Z));
            }

            new Animator.Ticker(TPS, DurationInTicks, this);
        }

        public void Update(float CurrentTick)
        {
            Node.RefreshTransformMatrix();

            Vector3 CurrentScale;
            Quaternion CurrentRotation;
            Vector3 CurrentPosition;

            if (ScaleKeys.Count != 0)
                CurrentScale = ScaleValues[0];
            else
                CurrentScale = Node.ScaleVector;

            if (RotationKeys.Count != 0)
                CurrentRotation = RotationValues[0];
            else
                CurrentRotation = Node.RotationQuatertion;

            if (PositionKeys.Count != 0)
                CurrentPosition = PositionValues[0];
            else
                CurrentPosition = Node.PositionVector;

            for (int i = ScaleKeys.Count - 1; i >= 0; i--)
            {
                if (ScaleKeys[i] < CurrentTick)
                {
                    if (i == ScaleKeys.Count - 1)
                    {
                        CurrentScale = ScaleValues[i];
                    }
                    else
                    {
                        CurrentScale = ScaleValues[i] + (ScaleValues[i + 1] - ScaleValues[i]) * ((CurrentTick - ScaleKeys[i]) / (ScaleKeys[i + 1] - ScaleKeys[i]));
                    }
                    break;
                }
            }

            for (int i =RotationKeys.Count - 1; i >= 0; i--)
            {
                if (RotationKeys[i] < CurrentTick)
                {
                    if (i == RotationKeys.Count - 1)
                    {
                        CurrentRotation =RotationValues[i];
                    }
                    else
                    {
                        CurrentRotation = RotationValues[i] + (RotationValues[i + 1] - RotationValues[i]) * ((CurrentTick - RotationKeys[i]) / (RotationKeys[i + 1] - RotationKeys[i]));
                    }
                    break;
                }
            }

            for (int i = PositionKeys.Count - 1; i >= 0; i--)
            {
                if (PositionKeys[i] < CurrentTick)
                {
                    if (i == PositionKeys.Count - 1)
                    {
                        CurrentPosition = PositionValues[i];
                    }
                    else
                    {
                        CurrentPosition = PositionValues[i] + (PositionValues[i + 1] - PositionValues[i]) * ((CurrentTick - PositionKeys[i]) / (PositionKeys[i + 1] - PositionKeys[i]));
                    }
                    break;
                }
            }

            Node.PositionVector = CurrentPosition;
            Node.RotationQuatertion = CurrentRotation;
            Node.ScaleVector = CurrentScale;
        }
    }
}
