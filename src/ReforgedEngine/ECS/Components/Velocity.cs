using Microsoft.Xna.Framework;

namespace ReforgedEngine.Core.ECS.Components
{
    /// <summary>
    /// Simple movement component used by PlayerMovementSystem.
    /// </summary>
    public sealed class Velocity : IComponent
    {
        public Vector2 Value;
    }
}
