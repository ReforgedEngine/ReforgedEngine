using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.Rendering;

namespace ReforgedEngine.Core.ECS.Components
{
    public struct Renderable : IComponent
    {
        public Texture2D Texture;
        public Rectangle SourceRect;
        public Vector2 Origin;
        public Color Tint;
        public float Fade;
        public bool Visible;
        public bool OcclusionFadeEnabled;
        public RenderLayer RenderLayer;
        public ulong SortKey;
    }
}