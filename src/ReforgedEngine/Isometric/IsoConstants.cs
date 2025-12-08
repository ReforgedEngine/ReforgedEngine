// IsoConstants.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReforgedEngine.Isometric
{
    public static class IsoConstants
    {
        // Origem padrão para sprites isométricos: centro na base
        public static Vector2 DefaultOrigin(Texture2D texture)
        {
            return new Vector2(texture.Width * 0.5f, texture.Height);
        }

        public static Vector2 DefaultOrigin(int width, int height)
        {
            return new Vector2(width * 0.5f, height);
        }

        // Origem para tiles (opcional - pode ser diferente)
        public static Vector2 TileOrigin(int tileWidth, int tileHeight)
        {
            // Para tiles, a origem geralmente é no centro
            return new Vector2(tileWidth * 0.5f, tileHeight * 0.5f);
        }

        // Fator de escala para floor height
        public const float FloorHeightMultiplier = 32f;
    }
}