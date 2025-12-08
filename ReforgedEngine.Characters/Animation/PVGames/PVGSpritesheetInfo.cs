using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ReforgedEngine.Characters.Animation.PVGames
{
    public class PVGSpritesheetInfo
    {
        public const int FrameWidth = 160;
        public const int FrameHeight = 160;
        public const int FramesPerRow = 24;
        public const int TotalRows = 25;

        private readonly Dictionary<string, Texture2D> _sheets = new();
        private readonly Dictionary<string, Rectangle> _frameCache = new();

        public void RegisterSheet(string name, Texture2D texture)
        {
            _sheets[name] = texture;
            CacheFrames(name, texture);
        }

        private void CacheFrames(string sheetName, Texture2D texture)
        {
            int totalFrames = FramesPerRow * TotalRows;

            for (int frame = 0; frame < totalFrames; frame++)
            {
                int row = frame / FramesPerRow;
                int col = frame % FramesPerRow;

                var rect = new Rectangle(
                    col * FrameWidth,
                    row * FrameHeight,
                    FrameWidth,
                    FrameHeight
                );

                _frameCache[$"{sheetName}_{frame}"] = rect;
            }
        }

        public Texture2D GetSheet(string name)
        {
            return _sheets.TryGetValue(name, out var texture) ? texture : null;
        }

        public Rectangle GetFrame(string sheetName, int frameNumber)
        {
            string key = $"{sheetName}_{frameNumber}";
            return _frameCache.TryGetValue(key, out var rect)
                ? rect
                : new Rectangle(0, 0, FrameWidth, FrameHeight);
        }

        // Métodos de conveniência (adicionados da PVGamesSpriteSheets)
        public (Texture2D texture, Rectangle rect) GetWalkFrame(int direction, int frame)
        {
            // Direções: 0=S, 1=SW, 2=W, 3=NW, 4=N, 5=NE, 6=E, 7=SE
            int baseFrame = direction * 8; // 8 frames por direção para caminhada

            if (frame >= 8) frame = 7;

            return (GetSheet("Sprite1"), GetFrame("Sprite1", baseFrame + frame));
        }

        public (Texture2D texture, Rectangle rect) GetIdleFrame(int direction, int frame = 0)
        {
            // Para idle, usamos frames da linha 128-151 (Idle 1)
            int baseFrame = 128 + (direction * 2); // 2 frames por direção para idle
            return (GetSheet("Sprite1"), GetFrame("Sprite1", baseFrame + frame));
        }
    }
}