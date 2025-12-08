using Microsoft.Xna.Framework;

namespace ReforgedEngine.Tools
{
    public static class IsoDirections
    {
        public static Vector2 GetTileOffset(Direction dir, int tileWidth = 64, int tileHeight = 64)
        {
            return dir switch
            {
                Direction.NW => new Vector2(-tileWidth, -tileHeight),
                Direction.NE => new Vector2(tileWidth, -tileHeight),
                Direction.SW => new Vector2(-tileWidth, tileHeight),
                Direction.SE => new Vector2(tileWidth, tileHeight),
                _ => Vector2.Zero
            };
        }

        public enum Direction { NW, NE, SW, SE }
    }
}
