using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReforgedEngine.Core.Rendering
{
    public static class SpriteBatchExtensions
    {
        private static Texture2D _pixelTexture;

        private static void EnsurePixelTexture(SpriteBatch spriteBatch)
        {
            if (_pixelTexture == null || _pixelTexture.IsDisposed)
            {
                _pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixelTexture.SetData(new[] { Color.White });
            }
        }

        /// <summary>
        /// Desenha um ponto (círculo pequeno) na posição especificada.
        /// </summary>
        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, int size)
        {
            EnsurePixelTexture(spriteBatch);

            // Desenha um quadrado como ponto
            var rect = new Rectangle(
                (int)position.X - size / 2,
                (int)position.Y - size / 2,
                size,
                size);

            spriteBatch.Draw(_pixelTexture, rect, color);
        }

        /// <summary>
        /// Desenha um retângulo vazio (somente bordas).
        /// </summary>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, int borderThickness = 1)
        {
            EnsurePixelTexture(spriteBatch);

            // Desenha as 4 bordas do retângulo
            var left = new Rectangle(rectangle.Left, rectangle.Top, borderThickness, rectangle.Height);
            var right = new Rectangle(rectangle.Right - borderThickness, rectangle.Top, borderThickness, rectangle.Height);
            var top = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, borderThickness);
            var bottom = new Rectangle(rectangle.Left, rectangle.Bottom - borderThickness, rectangle.Width, borderThickness);

            spriteBatch.Draw(_pixelTexture, left, color);
            spriteBatch.Draw(_pixelTexture, right, color);
            spriteBatch.Draw(_pixelTexture, top, color);
            spriteBatch.Draw(_pixelTexture, bottom, color);
        }

        /// <summary>
        /// Desenha um retângulo preenchido.
        /// </summary>
        public static void DrawFilledRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            EnsurePixelTexture(spriteBatch);
            spriteBatch.Draw(_pixelTexture, rectangle, color);
        }

        /// <summary>
        /// Desenha uma linha entre dois pontos.
        /// </summary>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, int thickness = 1)
        {
            EnsurePixelTexture(spriteBatch);

            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            spriteBatch.Draw(
                _pixelTexture,
                point1,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(distance, thickness),
                SpriteEffects.None,
                0);
        }

        /// <summary>
        /// Desenha um círculo.
        /// </summary>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int segments = 16, int thickness = 1)
        {
            EnsurePixelTexture(spriteBatch);

            var lastPoint = center + new Vector2(radius, 0);

            for (int i = 1; i <= segments; i++)
            {
                var angle = MathHelper.TwoPi * i / segments;
                var nextPoint = center + new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius);

                DrawLine(spriteBatch, lastPoint, nextPoint, color, thickness);
                lastPoint = nextPoint;
            }
        }
    }
}