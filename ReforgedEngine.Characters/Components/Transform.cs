using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Characters.Components
{
    public struct Transform : IComponent
    {
        public Vector2 Position;
        public Vector2 FeetPosition;
        public float Rotation;
        public float Scale;
        public int Floor;
    }

}
