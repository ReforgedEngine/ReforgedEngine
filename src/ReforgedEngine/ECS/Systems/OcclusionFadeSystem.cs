using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    public sealed class OcclusionFadeSystem : SystemBase
    {
        public OcclusionFadeSystem()
        : base(ComponentMask.Empty
            .With<Position>()
            .With<Renderable>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype arch, object ctx)
        {
            foreach (var e in arch.Entities)
            {
                var pos = e.Get<Position>();
                var ren = e.Get<Renderable>();

                if (!ren.OcclusionFadeEnabled)
                    continue;

                // Usar FeetIso.Y diretamente
                float d = pos.FeetIso.Y - world.PlayerFeetIso.Y;

                ren.Fade = d < -20 ? 0.35f : 1f;

                e.Set(ren);
            }
        }
    }
}