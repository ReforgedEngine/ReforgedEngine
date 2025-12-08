using Microsoft.Xna.Framework;

namespace ReforgedEngine.Isometric
{
    /// <summary>
    /// Static math helpers for World → Iso conversion.
    /// Supports any tile size (64x64, 64x32, etc.)
    /// </summary>
    public static class IsoMath
    {
        // Valores padrão para compatibilidade
        private const float DefaultTileWidth = 64f;
        private const float DefaultTileHeight = 64f; // 64x64 no seu caso

        /// <summary>
        /// Converte coordenadas mundiais para isométricas.
        /// </summary>
        /// <param name="world">Posição em pixels (coordenadas mundo)</param>
        /// <param name="tileWidth">Largura do tile em pixels</param>
        /// <param name="tileHeight">Altura do tile em pixels</param>
        public static Vector2 WorldToIso(Vector2 world, float tileWidth = 64f, float tileHeight = 64f)
        {
            float tx = world.X / tileWidth;
            float ty = world.Y / tileHeight;

            return new Vector2(
                (tx - ty) * (tileWidth * 0.5f),
                (tx + ty) * (tileHeight * 0.5f)
            );
        }

        /// <summary>
        /// Método para compatibilidade com código antigo.
        /// </summary>
        public static Vector2 ProjectFromPixels(Vector2 pixel)
        {
            return WorldToIso(pixel, DefaultTileWidth, DefaultTileHeight);
        }

        /// <summary>
        /// Calcula a posição de desenho para um tile.
        /// </summary>
        public static Vector2 TileToScreen(int tileX, int tileY, float tileWidth = 64f, float tileHeight = 64f)
        {
            return WorldToIso(new Vector2(tileX * tileWidth, tileY * tileHeight), tileWidth, tileHeight);
        }

        /// <summary>
        /// Obtém os quatro cantos de um tile em coordenadas isométricas.
        /// </summary>
        public static Vector2[] GetTileCorners(int tileX, int tileY, float tileWidth = 64f, float tileHeight = 64f)
        {
            var corners = new Vector2[4];
            var center = TileToScreen(tileX, tileY, tileWidth, tileHeight);

            corners[0] = center + new Vector2(-tileWidth * 0.5f, 0);          // Esquerda
            corners[1] = center + new Vector2(0, -tileHeight * 0.5f);         // Topo
            corners[2] = center + new Vector2(tileWidth * 0.5f, 0);           // Direita
            corners[3] = center + new Vector2(0, tileHeight * 0.5f);          // Base

            return corners;
        }

        /// <summary>
        /// Verifica se um ponto está dentro de um tile.
        /// </summary>
        public static bool PointInTile(Vector2 screenPoint, int tileX, int tileY,
            Vector2 mapOffset, float tileWidth = 64f, float tileHeight = 64f)
        {
            var tileScreenPos = TileToScreen(tileX, tileY, tileWidth, tileHeight) + mapOffset;
            var corners = GetTileCorners(0, 0, tileWidth, tileHeight); // Corners relativos

            // Transforma corners para posição absoluta
            for (int i = 0; i < 4; i++)
            {
                corners[i] += tileScreenPos;
            }

            // Teste de ponto em polígono (losango)
            return IsPointInDiamond(screenPoint, corners);
        }

        private static bool IsPointInDiamond(Vector2 point, Vector2[] corners)
        {
            // Algoritmo simples para losango (assumindo corners[0]=esquerda, [1]=topo, [2]=direita, [3]=base)
            Vector2 left = corners[0];
            Vector2 top = corners[1];
            Vector2 right = corners[2];
            Vector2 bottom = corners[3];

            // Calcula as áreas dos 4 triângulos
            float area1 = TriangleArea(point, left, top);
            float area2 = TriangleArea(point, top, right);
            float area3 = TriangleArea(point, right, bottom);
            float area4 = TriangleArea(point, bottom, left);

            float totalArea = TriangleArea(left, top, right) + TriangleArea(left, right, bottom);
            float sumArea = area1 + area2 + area3 + area4;

            return Math.Abs(totalArea - sumArea) < 0.1f;
        }

        private static float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
        {
            return Math.Abs((a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) * 0.5f);
        }
    }
}