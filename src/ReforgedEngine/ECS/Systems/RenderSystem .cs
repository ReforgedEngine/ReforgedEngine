using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.Camera;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    public sealed class RenderSystem : SystemBase
    {
        public SpriteBatch SpriteBatch { get; set; }
        public Camera2D Camera { get; }

        public RenderSystem(Camera2D camera)
        : base(ComponentMask.Empty
            .With<Position>()
            .With<Renderable>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            if (SpriteBatch == null || Camera == null) return;

            var entities = archetype.Entities;
            var gameTime = (GameTime)ctx;

            // Coletar e ordenar
            var drawList = new System.Collections.Generic.List<(Position pos, Renderable ren)>();

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var pos = entity.Get<Position>();
                var ren = entity.Get<Renderable>();

                if (ren.Visible && ren.Texture != null)
                {
                    drawList.Add((pos, ren));
                }
            }

            // Ordenar por SortKey
            drawList.Sort((a, b) => a.ren.SortKey.CompareTo(b.ren.SortKey));

            // Desenhar
            SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null,
                Camera.Transform);

            foreach (var item in drawList)
            {
                if (!item.pos.IsTile && item.pos.Floor == 3)
                {
                    Console.WriteLine($"Name:{item.ren.RenderLayer}Z:{item.pos.Z}|X:{item.pos.FeetWorld.X}:Y{item.pos.FeetWorld.Y}|Draw:{item.pos.DrawPosition}|Sort:{item.ren.SortKey}|Origin:{item.pos.Origin}");
                }

                SpriteBatch.Draw(
                    item.ren.Texture,
                    item.pos.DrawPosition,
                    item.ren.SourceRect,
                    item.ren.Tint * item.ren.Fade,
                    0f,
                    item.ren.Origin,
                    1f,
                    SpriteEffects.None,
                    0f);
            }

            SpriteBatch.End();
        }

        public void DrawFrame(SpriteBatch spriteBatch, World world)
        {
            SpriteBatch = spriteBatch;
            Update(world, null);
        }
    }
}