using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.Rendering;

namespace ReforgedEngine.Isometric
{
    /// <summary>
    /// Debug helpers for outlining tile positions,
    /// draw order, feet coordinates, and origins.
    /// </summary>
    public static class IsoDebug
    {
        public static void DrawFeet(SpriteBatch batch, Vector2 feetIso, Color color)
        {
            // Desenha um ponto vermelho nos pés
            batch.DrawPoint(feetIso, color, 4);
        }

        public static void DrawOrigin(SpriteBatch batch, Position pos)
        {
            // Desenha um ponto azul na origem
            batch.DrawPoint(pos.DrawPosition + pos.Origin, Color.CornflowerBlue, 4);
        }

        public static void DrawBox(SpriteBatch batch, Rectangle rect, Color color)
        {
            // Desenha um retângulo (bordas apenas)
            batch.DrawRectangle(rect, color);
        }

        public static void DrawTileBounds(SpriteBatch batch, Vector2 tilePosition, int tileWidth, int tileHeight, Color color)
        {
            // Converte para isométrico e desenha o bounding box do tile
            var worldPos = tilePosition;
            var isoPos = IsoMath.WorldToIso(worldPos, tileWidth, tileHeight);

            // Pontos do tile em coordenadas mundiais
            var points = new Vector2[]
            {
                worldPos,
                worldPos + new Vector2(tileWidth, 0),
                worldPos + new Vector2(tileWidth, tileHeight),
                worldPos + new Vector2(0, tileHeight)
            };

            // Converte para isométrico
            var isoPoints = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                isoPoints[i] = IsoMath.WorldToIso(points[i], tileWidth, tileHeight);
            }

            // Desenha linhas conectando os pontos
            for (int i = 0; i < 4; i++)
            {
                var next = (i + 1) % 4;
                batch.DrawLine(isoPoints[i], isoPoints[next], color, 1);
            }
        }

        public static void DrawEntityBounds(SpriteBatch batch, Position pos, Texture2D texture, Color color)
        {
            if (texture == null) return;

            // Calcula os cantos da textura em espaço de tela
            var topLeft = pos.DrawPosition;
            var topRight = pos.DrawPosition + new Vector2(texture.Width, 0);
            var bottomRight = pos.DrawPosition + new Vector2(texture.Width, texture.Height);
            var bottomLeft = pos.DrawPosition + new Vector2(0, texture.Height);

            // Desenha o bounding box
            batch.DrawLine(topLeft, topRight, color, 1);
            batch.DrawLine(topRight, bottomRight, color, 1);
            batch.DrawLine(bottomRight, bottomLeft, color, 1);
            batch.DrawLine(bottomLeft, topLeft, color, 1);
        }

        public static void DrawCollider(SpriteBatch batch, Position pos, Collider collider, Color color)
        {
            if (collider.Bounds.Width == 0 || collider.Bounds.Height == 0) return;

            // Calcula a posição mundial do collider
            var worldBounds = new Rectangle(
                (int)(pos.FeetWorld.X + collider.Bounds.X),
                (int)(pos.FeetWorld.Y + collider.Bounds.Y),
                collider.Bounds.Width,
                collider.Bounds.Height);

            // Converte para isométrico
            var isoTopLeft = IsoMath.WorldToIso(new Vector2(worldBounds.Left, worldBounds.Top), 64, 32);
            var isoTopRight = IsoMath.WorldToIso(new Vector2(worldBounds.Right, worldBounds.Top), 64, 32);
            var isoBottomRight = IsoMath.WorldToIso(new Vector2(worldBounds.Right, worldBounds.Bottom), 64, 32);
            var isoBottomLeft = IsoMath.WorldToIso(new Vector2(worldBounds.Left, worldBounds.Bottom), 64, 32);

            // Desenha o collider
            batch.DrawLine(isoTopLeft, isoTopRight, color, 2);
            batch.DrawLine(isoTopRight, isoBottomRight, color, 2);
            batch.DrawLine(isoBottomRight, isoBottomLeft, color, 2);
            batch.DrawLine(isoBottomLeft, isoTopLeft, color, 2);
        }

        // No IsoDebug.cs, adicionar:
        public static void DrawTileGrid(SpriteBatch batch, Vector2 mapOffset,
            int gridWidth, int gridHeight, float tileWidth, float tileHeight, Color color)
        {
            // Desenha grid de tiles para debug
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    var isoPos = IsoMath.TileToScreen(x, y, tileWidth, tileHeight) + mapOffset;

                    // Desenha ponto no centro do tile
                    batch.DrawPoint(isoPos, color, 2);

                    // Desenha número do tile
                    // (precisa de SpriteFont - comentado por enquanto)
                    // batch.DrawString(font, $"{x},{y}", isoPos + new Vector2(-10, -10), color);

                    // Desenha borda do tile
                    var corners = IsoMath.GetTileCorners(x, y, tileWidth, tileHeight);
                    for (int i = 0; i < 4; i++)
                    {
                        var next = (i + 1) % 4;
                        batch.DrawLine(
                            corners[i] + mapOffset,
                            corners[next] + mapOffset,
                            color, 1);
                    }
                }
            }
        }

        // Em IsoDebug.cs, adicionar:
        public static void DrawTiledAlignment(SpriteBatch batch, Vector2 tiledObjectPos,
            Texture2D texture, Vector2 origin, Vector2 mapOffset,
            float tileWidth, float tileHeight, Color color)
        {
            // 1. Mostra a posição do objeto no Tiled (base)
            var tiledIso = IsoMath.WorldToIso(tiledObjectPos, tileWidth, tileHeight) + mapOffset;
            batch.DrawPoint(tiledIso, Color.Red, 8);

            // 2. Mostra onde o sprite será desenhado com a origem atual
            var drawPos = tiledObjectPos - origin;
            var drawIso = IsoMath.WorldToIso(drawPos, tileWidth, tileHeight) + mapOffset;
            batch.DrawPoint(drawIso, Color.Green, 6);

            // 3. Linha conectando os dois pontos
            batch.DrawLine(tiledIso, drawIso, Color.Yellow, 2);

            // 4. Bounding box do sprite
            var boundsRect = new Rectangle(
                (int)drawPos.X, (int)drawPos.Y,
                texture.Width, texture.Height);

            var corners = new Vector2[4];
            corners[0] = new Vector2(boundsRect.Left, boundsRect.Top);
            corners[1] = new Vector2(boundsRect.Right, boundsRect.Top);
            corners[2] = new Vector2(boundsRect.Right, boundsRect.Bottom);
            corners[3] = new Vector2(boundsRect.Left, boundsRect.Bottom);

            for (int i = 0; i < 4; i++)
            {
                var cornerIso = IsoMath.WorldToIso(corners[i], tileWidth, tileHeight) + mapOffset;
                var nextCornerIso = IsoMath.WorldToIso(corners[(i + 1) % 4], tileWidth, tileHeight) + mapOffset;
                batch.DrawLine(cornerIso, nextCornerIso, color * 0.5f, 1);
            }

            // 5. Origem correta esperada (centro na base)
            var correctOrigin = new Vector2(texture.Width * 0.5f, texture.Height);
            var correctDrawPos = tiledObjectPos - correctOrigin;
            var correctDrawIso = IsoMath.WorldToIso(correctDrawPos, tileWidth, tileHeight) + mapOffset;
            batch.DrawPoint(correctDrawIso, Color.Blue, 4);

            // 6. Se a origem atual não for a correta, mostra linha de correção
            if (Vector2.DistanceSquared(origin, correctOrigin) > 1f)
            {
                batch.DrawLine(drawIso, correctDrawIso, Color.Magenta, 1);
            }
        }
    }
}