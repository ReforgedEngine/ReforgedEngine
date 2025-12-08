using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    public sealed class PixelCollisionSystem : SystemBase
    {
        public PixelCollisionSystem()
        {
            _mask = ComponentMask.Empty
                .With<Position>()
                .With<Collider>()
                .With<Renderable>();
        }

        protected override void ProcessArchetype(World world, Archetype arch, object context)
        {
            // Em breve: pixel-perfect mask vs player rectangle
        }
    }
}
