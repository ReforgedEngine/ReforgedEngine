// PVGFrameResolver.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Characters.Components;
using System.Collections.Generic;

namespace ReforgedEngine.Characters.Animation.PVGames
{
    public static class PVGFrameResolver
    {
        public static int FrameWidth = 160;
        public static int FrameHeight = 160;

        // Mapeamento de animações
        private static readonly Dictionary<string, AnimationData> _animations = new();
        private static PVGSpritesheetInfo _spriteSheets;

        public static void Initialize(PVGSpritesheetInfo spriteSheets)
        {
            _spriteSheets = spriteSheets;
            RegisterAnimations();
        }

        private static void RegisterAnimations()
        {
            // Sprite 1
            RegisterAnimation("Walk", "Sprite1", 64, 0, 31, 32, 63);
            RegisterAnimation("Run", "Sprite1", 64, 64, 95, 96, 127);
            RegisterAnimation("Idle1", "Sprite1", 24, 128, 139, 140, 151);
            RegisterAnimation("Idle2", "Sprite1", 24, 152, 163, 164, 175);
            RegisterAnimation("Idle3", "Sprite1", 24, 176, 187, 188, 199);
            RegisterAnimation("Idle4", "Sprite1", 24, 200, 211, 212, 223);
            RegisterAnimation("Idle5", "Sprite1", 24, 224, 235, 236, 247);
            RegisterAnimation("Idle6", "Sprite1", 24, 248, 259, 260, 271);
            RegisterAnimation("Interact", "Sprite1", 24, 296, 307, 308, 319);
            // ... adicione outras conforme necessário
        }

        public static void RegisterAnimation(string name, string spriteSheet, int totalFrames,
                                           int cardStart, int cardEnd, int diagStart, int diagEnd)
        {
            _animations[name] = new AnimationData(spriteSheet, totalFrames,
                                                 cardStart, cardEnd, diagStart, diagEnd);
        }

        public static Rectangle GetFrameRectangle(string animationName, int direction, int frame)
        {
            if (!_animations.TryGetValue(animationName, out var data) || _spriteSheets == null)
                return new Rectangle(0, 0, FrameWidth, FrameHeight);

            // Determinar se é direção diagonal (ímpares)
            bool isDiagonal = direction % 2 == 1;
            int baseFrame = isDiagonal ? data.DiagonalStart : data.CardinalStart;
            int framesPerDirection = data.TotalFrames / 8;

            // Limitar frame
            if (frame >= framesPerDirection)
                frame = framesPerDirection - 1;

            // Calcular frame absoluto
            int absoluteFrame = baseFrame + (direction / 2 * framesPerDirection) + frame;

            return _spriteSheets.GetFrame(data.SpriteSheet, absoluteFrame);
        }

        public static int NextFrame(string animationName, int direction, int currentFrame)
        {
            if (!_animations.TryGetValue(animationName, out var data))
                return 0;

            int framesPerDirection = data.TotalFrames / 8;
            int nextFrame = currentFrame + 1;

            return nextFrame >= framesPerDirection ? 0 : nextFrame;
        }

        public static Texture2D GetTexture(string animationName)
        {
            if (_animations.TryGetValue(animationName, out var data) && _spriteSheets != null)
                return _spriteSheets.GetSheet(data.SpriteSheet);

            return _spriteSheets?.GetSheet("Sprite1");
        }

        private class AnimationData
        {
            public string SpriteSheet { get; }
            public int TotalFrames { get; }
            public int CardinalStart { get; }
            public int CardinalEnd { get; }
            public int DiagonalStart { get; }
            public int DiagonalEnd { get; }

            public AnimationData(string spriteSheet, int totalFrames,
                                int cardStart, int cardEnd, int diagStart, int diagEnd)
            {
                SpriteSheet = spriteSheet;
                TotalFrames = totalFrames;
                CardinalStart = cardStart;
                CardinalEnd = cardEnd;
                DiagonalStart = diagStart;
                DiagonalEnd = diagEnd;
            }
        }
    }
}