using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    /// <summary>
    /// ECS Collision Pipeline (Alpha 0.01)
    /// -----------------------------------
    /// - Itera sobre pares de entidades com Position + Collider
    /// - AABB check
    /// - Pixel-perfect opcional
    /// - Resolve deslocamento mínimo (slide)
    /// 
    /// Suporta:
    ///   Player vs Mundo
    ///   Objetos sólidos do mapa
    /// </summary>
    public sealed class CollisionSystem : SystemBase
    {
        public CollisionSystem()
        : base(ComponentMask.Empty
            .With<Position>()
            .With<Collider>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype arch, object ctx)
        {
            var entities = arch.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var a = entities[i];
                var posA = a.Get<Position>();
                var colA = a.Get<Collider>();

                if (!colA.IsSolid)
                    continue;

                Rectangle boundsA = ComputeWorldBounds(posA, colA);

                // Compare with all others
                for (int j = 0; j < entities.Count; j++)
                {
                    if (i == j) continue;

                    var b = entities[j];
                    var posB = b.Get<Position>();
                    var colB = b.Get<Collider>();

                    if (!colB.IsSolid)
                        continue;

                    Rectangle boundsB = ComputeWorldBounds(posB, colB);

                    if (!boundsA.Intersects(boundsB))
                        continue;

                    // Optional pixel perfect
                    if (colA.PixelPerfect || colB.PixelPerfect)
                    {
                        if (!PixelCollisionCheck(posA, colA, posB, colB))
                            continue;
                    }

                    ResolveOverlap(ref posA, boundsA, boundsB);
                    a.Set(posA); // write back
                }
            }
        }

        private Rectangle ComputeWorldBounds(Position pos, Collider col)
        {
            // FeetWorld is bottom-left corner of sprite
            return new Rectangle(
                (int)(pos.FeetWorld.X + col.Bounds.X),
                (int)(pos.FeetWorld.Y + col.Bounds.Y),
                col.Bounds.Width,
                col.Bounds.Height);
        }

        private bool PixelCollisionCheck(
            Position posA, Collider colA,
            Position posB, Collider colB)
        {
            // Using bounding-box intersections first
            Rectangle A = new Rectangle(
                (int)(posA.FeetWorld.X + colA.Bounds.X),
                (int)(posA.FeetWorld.Y + colA.Bounds.Y),
                colA.Bounds.Width,
                colA.Bounds.Height);

            Rectangle B = new Rectangle(
                (int)(posB.FeetWorld.X + colB.Bounds.X),
                (int)(posB.FeetWorld.Y + colB.Bounds.Y),
                colB.Bounds.Width,
                colB.Bounds.Height);

            Rectangle inter = Rectangle.Intersect(A, B);
            if (inter.IsEmpty)
                return false;

            int ax = inter.X - A.X;
            int ay = inter.Y - A.Y;

            int bx = inter.X - B.X;
            int by = inter.Y - B.Y;

            for (int y = 0; y < inter.Height; y++)
            {
                for (int x = 0; x < inter.Width; x++)
                {
                    bool hitA = colA.PixelPerfect &&
                        Collision.PixelMask.Check(ax + x, ay + y, colA.PixelMask, colA.MaskWidth, colA.MaskHeight);

                    bool hitB = colB.PixelPerfect &&
                        Collision.PixelMask.Check(bx + x, by + y, colB.PixelMask, colB.MaskWidth, colB.MaskHeight);

                    if ((hitA && hitB) || (hitA && !colB.PixelPerfect) || (!colA.PixelPerfect && hitB))
                        return true;
                }
            }

            return false;
        }

        private void ResolveOverlap(ref Position posA, Rectangle A, Rectangle B)
        {
            // minimal translation vector
            int moveLeft = A.Right - B.Left;
            int moveRight = B.Right - A.Left;
            int moveUp = A.Bottom - B.Top;
            int moveDown = B.Bottom - A.Top;

            int smallest = Math.Abs(moveLeft);
            float dx = -moveLeft;
            float dy = 0;

            if (Math.Abs(moveRight) < smallest)
            {
                smallest = Math.Abs(moveRight);
                dx = moveRight;
                dy = 0;
            }

            if (Math.Abs(moveUp) < smallest)
            {
                smallest = Math.Abs(moveUp);
                dx = 0;
                dy = -moveUp;
            }

            if (Math.Abs(moveDown) < smallest)
            {
                dx = 0;
                dy = moveDown;
            }

            posA.WorldPos += new Vector2(dx, dy);
            posA.FeetWorld = posA.WorldPos;

            // Recompute ISO - precisa do MapOffset, vamos usar Vector2.Zero por enquanto
            posA.UpdateIso(Vector2.Zero,64,64);
        }
    }
}