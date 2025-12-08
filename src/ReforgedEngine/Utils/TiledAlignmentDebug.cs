// TiledAlignmentDebug.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.Rendering;
using ReforgedEngine.Isometric;

namespace ReforgedEngine.Tools
{
    public class TiledAlignmentDebug
    {
        private struct AlignmentMarker
        {
            public Vector2 Position;
            public Color Color;
            public string Label;
        }

        private List<AlignmentMarker> _markers = new List<AlignmentMarker>();
        private SpriteFont _font;
        private Texture2D _pixel;

        public void Initialize(GraphicsDevice graphics, SpriteFont font)
        {
            _font = font;
            _pixel = new Texture2D(graphics, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void AddMarker(Vector2 worldPos, Color color, string label)
        {
            _markers.Add(new AlignmentMarker
            {
                Position = worldPos,
                Color = color,
                Label = label
            });
        }

        public void ClearMarkers()
        {
            _markers.Clear();
        }

        public void DrawAlignmentGrid(SpriteBatch spriteBatch, Vector2 mapOffset,
            int gridSizeX, int gridSizeY, float tileWidth, float tileHeight)
        {
            spriteBatch.Begin();

            // Desenha grid de alinhamento
            for (int y = 0; y <= gridSizeY; y++)
            {
                for (int x = 0; x <= gridSizeX; x++)
                {
                    Vector2 worldPos = new Vector2(x * tileWidth, y * tileHeight);
                    Vector2 isoPos = IsoMath.WorldToIso(worldPos, tileWidth, tileHeight) + mapOffset;

                    // Linha vertical (em coordenadas mundo)
                    if (x < gridSizeX)
                    {
                        Vector2 nextWorld = new Vector2((x + 1) * tileWidth, y * tileHeight);
                        Vector2 nextIso = IsoMath.WorldToIso(nextWorld, tileWidth, tileHeight) + mapOffset;
                        spriteBatch.DrawLine(isoPos, nextIso, Color.Gray * 0.3f, 1);
                    }

                    // Linha horizontal (em coordenadas mundo)
                    if (y < gridSizeY)
                    {
                        Vector2 nextWorld = new Vector2(x * tileWidth, (y + 1) * tileHeight);
                        Vector2 nextIso = IsoMath.WorldToIso(nextWorld, tileWidth, tileHeight) + mapOffset;
                        spriteBatch.DrawLine(isoPos, nextIso, Color.Gray * 0.3f, 1);
                    }

                    // Ponto no centro do tile
                    spriteBatch.DrawPoint(isoPos, Color.DarkGray, 2);

                    // Coordenadas do tile
                    if (_font != null)
                    {
                        string coordText = $"{x},{y}";
                        Vector2 textSize = _font.MeasureString(coordText);
                        spriteBatch.DrawString(_font, coordText,
                            isoPos - textSize * 0.5f,
                            Color.DarkGray * 0.8f);
                    }
                }
            }

            // Desenha marcadores
            foreach (var marker in _markers)
            {
                Vector2 isoPos = IsoMath.WorldToIso(marker.Position, tileWidth, tileHeight) + mapOffset;

                // Ponto colorido
                spriteBatch.DrawPoint(isoPos, marker.Color, 8);

                // Label
                if (_font != null && !string.IsNullOrEmpty(marker.Label))
                {
                    Vector2 textSize = _font.MeasureString(marker.Label);
                    spriteBatch.DrawString(_font, marker.Label,
                        isoPos + new Vector2(10, -textSize.Y * 0.5f),
                        marker.Color);
                }
            }

            spriteBatch.End();
        }

        public void DrawAlignmentInfo(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (_font == null) return;

            spriteBatch.Begin();

            string info = "TILED ALIGNMENT DEBUG\n" +
                         "Red: Object base position (Tiled pivot)\n" +
                         "Green: Sprite origin (should match red)\n" +
                         "Blue: Sprite bounds center\n" +
                         "Yellow: Tile center";

            var bgRect = new Rectangle(
                (int)screenPos.X, (int)screenPos.Y, 350, 120);
            spriteBatch.Draw(_pixel, bgRect, Color.Black * 0.7f);

            spriteBatch.DrawString(_font, info, screenPos + new Vector2(10, 10), Color.White);

            spriteBatch.End();
        }

        // Método para testar alinhamento de um objeto específico
        public void TestObjectAlignment(Vector2 tiledObjectPos, Texture2D texture,
            Vector2 currentOrigin, Vector2 mapOffset, float tileWidth, float tileHeight)
        {
            ClearMarkers();

            // 1. Posição do objeto no Tiled (base)
            AddMarker(tiledObjectPos, Color.Red, "Tiled Base");

            // 2. Posição com origem atual
            Vector2 currentDrawPos = tiledObjectPos - currentOrigin;
            AddMarker(currentDrawPos, Color.Green, "Current Origin");

            // 3. Posição com origem correta (centro na base)
            Vector2 correctOrigin = new Vector2(texture.Width * 0.5f, texture.Height);
            Vector2 correctDrawPos = tiledObjectPos - correctOrigin;
            AddMarker(correctDrawPos, Color.Blue, "Correct Origin");

            // 4. Centro do tile mais próximo
            int tileX = (int)(tiledObjectPos.X / tileWidth);
            int tileY = (int)(tiledObjectPos.Y / tileHeight);
            Vector2 tileCenter = new Vector2(
                (tileX + 0.5f) * tileWidth,
                (tileY + 0.5f) * tileHeight);
            AddMarker(tileCenter, Color.Yellow, "Tile Center");

            // Log para debug
            System.Diagnostics.Debug.WriteLine($"=== Tiled Alignment Test ===");
            System.Diagnostics.Debug.WriteLine($"Tiled Object Position: {tiledObjectPos}");
            System.Diagnostics.Debug.WriteLine($"Texture Size: {texture.Width}x{texture.Height}");
            System.Diagnostics.Debug.WriteLine($"Current Origin: {currentOrigin}");
            System.Diagnostics.Debug.WriteLine($"Correct Origin: {correctOrigin}");
            System.Diagnostics.Debug.WriteLine($"Offset needed: {correctOrigin - currentOrigin}");
        }
    }
}