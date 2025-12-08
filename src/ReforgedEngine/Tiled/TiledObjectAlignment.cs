// TiledAlignment.cs
using Microsoft.Xna.Framework;

namespace ReforgedEngine.Core.Tiled
{
    public enum TiledObjectAlignment
    {
        Unspecified,    // Não definido
        TopLeft,        // topleft
        Top,            // top
        TopRight,       // topright
        Left,           // left
        Center,         // center
        Right,          // right
        BottomLeft,     // bottomleft
        Bottom,         // bottom (O SEU CASO)
        BottomRight,    // bottomright
        Custom          // Valores customizados
    }

    public static class TiledAlignmentHelper
    {
        public static TiledObjectAlignment ParseAlignment(string alignment)
        {
            if (string.IsNullOrEmpty(alignment))
                return TiledObjectAlignment.Unspecified;

            return alignment.ToLower() switch
            {
                "topleft" => TiledObjectAlignment.TopLeft,
                "top" => TiledObjectAlignment.Top,
                "topright" => TiledObjectAlignment.TopRight,
                "left" => TiledObjectAlignment.Left,
                "center" => TiledObjectAlignment.Center,
                "right" => TiledObjectAlignment.Right,
                "bottomleft" => TiledObjectAlignment.BottomLeft,
                "bottom" => TiledObjectAlignment.Bottom,
                "bottomright" => TiledObjectAlignment.BottomRight,
                _ => TiledObjectAlignment.Unspecified
            };
        }

        public static Vector2 CalculateOrigin(TiledObjectAlignment alignment, int width, int height)
        {
            return alignment switch
            {
                TiledObjectAlignment.TopLeft => new Vector2(0, 0),
                TiledObjectAlignment.Top => new Vector2(width * 0.5f, 0),
                TiledObjectAlignment.TopRight => new Vector2(width, 0),
                TiledObjectAlignment.Left => new Vector2(0, height * 0.5f),
                TiledObjectAlignment.Center => new Vector2(width * 0.5f, height * 0.5f),
                TiledObjectAlignment.Right => new Vector2(width, height * 0.5f),
                TiledObjectAlignment.BottomLeft => new Vector2(0, height),
                TiledObjectAlignment.Bottom => new Vector2(width * 0.5f, height), //PVGAMES
                TiledObjectAlignment.BottomRight => new Vector2(width, height),
                TiledObjectAlignment.Unspecified => new Vector2(width * 0.5f, height), // Padrão: bottom-center
                _ => new Vector2(width * 0.5f, height)
            };
        }

        public static Vector2 CalculateOrigin(string alignment, int width, int height)
        {
            var align = ParseAlignment(alignment);
            return CalculateOrigin(align, width, height);
        }
    }
}