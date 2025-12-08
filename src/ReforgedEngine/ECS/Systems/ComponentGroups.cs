using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    public static class ComponentGroups
    {
        public static readonly ComponentMask Renderables =
            ComponentMask.Empty.With<Position>().With<Renderable>();

        public static readonly ComponentMask Animated =
            ComponentMask.Empty.With<Renderable>().With<Animation>();

        public static readonly ComponentMask Collidable =
            ComponentMask.Empty.With<Position>().With<Collider>();
    }
}
