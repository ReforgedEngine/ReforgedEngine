// FrustumCullingSystem.cs
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.Camera;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    public sealed class FrustumCullingSystem : SystemBase
    {
        private Camera2D _camera;
        private Rectangle _frustumBounds;

        public FrustumCullingSystem(Camera2D camera)
        : base(ComponentMask.Empty
            .With<Position>()
            .With<Renderable>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            UpdateFrustumBounds();

            var entities = archetype.Entities;
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var pos = entity.Get<Position>();
                var ren = entity.Get<Renderable>();

                // Calcular bounds da entidade em tela
                Rectangle entityBounds = CalculateScreenBounds(pos, ren);

                // Culling simples: se não intersecta frustum, esconder
                ren.Visible = _frustumBounds.Intersects(entityBounds);

                entity.Set(ren);
            }
        }

        private void UpdateFrustumBounds()
        {
            // Converter viewport da câmera para retângulo mundial
            Vector2 topLeft = Vector2.Transform(
                new Vector2(0, 0),
                Matrix.Invert(_camera.Transform));

            Vector2 bottomRight = Vector2.Transform(
                new Vector2(_camera.ViewWidth, _camera.ViewHeight),
                Matrix.Invert(_camera.Transform));

            _frustumBounds = new Rectangle(
                (int)topLeft.X - 100,  // Padding
                (int)topLeft.Y - 100,
                (int)(bottomRight.X - topLeft.X) + 200,
                (int)(bottomRight.Y - topLeft.Y) + 200);
        }

        private Rectangle CalculateScreenBounds(Position pos, Renderable ren)
        {
            if (ren.Texture == null) return Rectangle.Empty;

            return new Rectangle(
                (int)(pos.DrawPosition.X - ren.Origin.X),
                (int)(pos.DrawPosition.Y - ren.Origin.Y),
                ren.Texture.Width,
                ren.Texture.Height);
        }
    }
}