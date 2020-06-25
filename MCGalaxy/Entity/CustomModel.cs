using System;
using MCGalaxy.Maths;

namespace MCGalaxy {

    public class CustomModel {
        public string name;
        // humanoid defaults
        public float nameY = 32.5f / 16.0f;
        public float eyeY = 26.0f / 16.0f;
        public Vec3F32 collisionBounds = new Vec3F32 {
            X = (8.6f) / 16.0f,
            Y = (28.1f) / 16.0f,
            Z = (8.6f) / 16.0f
        };
        public Vec3F32 pickingBoundsMin = new Vec3F32 {
            X = (-8) / 16.0f,
            Y = (0) / 16.0f,
            Z = (-4) / 16.0f
        };
        public Vec3F32 pickingBoundsMax = new Vec3F32 {
            X = (8) / 16.0f,
            Y = (32) / 16.0f,
            Z = (4) / 16.0f
        };
        public bool bobbing = true;
        public bool pushes = true;
        // if true, uses skin from your account
        public bool usesHumanSkin = true;
        public bool calcHumanAnims = true;
        public UInt16 uScale = 64;
        public UInt16 vScale = 64;
        public byte partCount;
    }

    public class CustomModelPart {
        /* min and max vec3 points */
        public Vec3F32 min;
        public Vec3F32 max;

        /* uv coords in order: top, bottom, front, back, left, right */
        public UInt16[] u1;
        public UInt16[] v1;
        public UInt16[] u2;
        public UInt16[] v2;
        /* rotation origin point */
        public Vec3F32 rotationOrigin;

        public Vec3F32 rotation = new Vec3F32 {
            X = 0.0f,
            Y = 0.0f,
            Z = 0.0f,
        };
        public CustomModelAnim anim = CustomModelAnim.None;
        public float animModifier = 1.0f;
        public bool fullbright = false;
        public bool firstPersonArm = false;
    }

    public enum CustomModelAnim {
        None = 0,
        Head = 1,
        LeftLeg = 2,
        RightLeg = 3,
        LeftArm = 4,
        RightArm = 5,
        SpinX = 6,
        SpinY = 7,
        SpinZ = 8,
        SpinXVelocity = 9,
        SpinYVelocity = 10,
        SpinZVelocity = 11,
    }
}
