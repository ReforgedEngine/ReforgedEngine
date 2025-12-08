using Microsoft.Xna.Framework;

namespace ReforgedEngine.Core.Tiles
{
    public struct CollisionShape
    {
        public ShapeType Type;
        public Rectangle Bounds;
        public Vector2[] Vertices; // Para polígonos

        public enum ShapeType
        {
            Rectangle,
            Ellipse,
            Polygon
        }
    }
}