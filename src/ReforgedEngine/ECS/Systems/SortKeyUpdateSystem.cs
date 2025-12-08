// SortKeyUpdateSystem.cs - VERSÃO CORRIGIDA
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.Rendering;

namespace ReforgedEngine.Core.ECS.Systems
{
    public sealed class SortKeyUpdateSystem : SystemBase
    {
        public Vector2 MapOffset { get; set; }

        public SortKeyUpdateSystem(Vector2 mapOffset)
        : base(ComponentMask.Empty
            .With<Position>()
            .With<Renderable>())
        {
            MapOffset = mapOffset;
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            var gameTime = (GameTime)ctx;
            var entities = archetype.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var pos = entity.Get<Position>();
                var ren = entity.Get<Renderable>();

                ren.SortKey = IsoRenderKey.From(pos, ren).Raw;
                entity.Set(ren);
            }
        }
    }
}