// Collider.cs - mudar para struct
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.Tiles;

namespace ReforgedEngine.Core.ECS.Components
{
    public struct Collider : IComponent
    {
        public Rectangle Bounds;
        public bool PixelPerfect;
        public byte[] PixelMask;
        public int MaskWidth;
        public int MaskHeight;
        public bool IsSolid;
        public List<CollisionShape> Shapes;
    }
}