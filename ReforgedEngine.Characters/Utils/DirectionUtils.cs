using Microsoft.Xna.Framework;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Characters.Entities.Player;

namespace ReforgedEngine.Characters.Utils
{
    public static class DirectionUtils
    {
        public static MoveDirection ResolveDirection(Vector2 dir)
        {
            if (dir.X == 0 && dir.Y == 0)
                return MoveDirection.South;

            float angle = MathF.Atan2(dir.Y, dir.X);

            if (angle >= -0.39f && angle < 0.39f) return MoveDirection.East;
            if (angle >= 0.39f && angle < 1.17f) return MoveDirection.SouthEast;
            if (angle >= 1.17f && angle < 1.96f) return MoveDirection.South;
            if (angle >= 1.96f && angle < 2.74f) return MoveDirection.SouthWest;
            if (angle >= -1.17f && angle < -0.39f) return MoveDirection.NorthEast;
            if (angle >= -1.96f && angle < -1.17f) return MoveDirection.North;
            if (angle >= -2.74f && angle < -1.96f) return MoveDirection.NorthWest;

            return MoveDirection.West;
        }
    }
}
