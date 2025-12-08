using ReforgedEngine.Core.ECS.Components;
using Microsoft.Xna.Framework;

namespace ReforgedEngine.Characters.Components
{
    public struct Movement : IComponent
    {
        public Vector2 Velocity;
        public float Speed;
        public bool IsMoving;
        public MoveDirection Direction;
    }

}
