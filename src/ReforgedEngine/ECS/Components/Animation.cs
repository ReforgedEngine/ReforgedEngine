// Animation.cs - corrigir propriedades

// Animation.cs - corrigir propriedades
namespace ReforgedEngine.Core.ECS.Components
{
    public struct Animation : IComponent
    {
        public string Name;
        public int Frames;
        public float FrameTime;
        public bool Loop;
        public float Timer;
        public int FrameIndex;
        public int FrameCount => Frames;
        public float FrameDuration => FrameTime;

        // Método auxiliar
        public Microsoft.Xna.Framework.Rectangle GetFrameRectangle(int frameWidth, int frameHeight)
        {
            return new Microsoft.Xna.Framework.Rectangle(
                FrameIndex * frameWidth,
                0,
                frameWidth,
                frameHeight);
        }
    }
}