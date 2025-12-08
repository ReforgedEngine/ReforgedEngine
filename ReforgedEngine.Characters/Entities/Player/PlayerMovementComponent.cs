using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Characters.Entities.Player
{
    public struct PlayerMovementComponent : IComponent
    {
        public float Speed;
        public Vector2 DesiredMove;
    }
}
