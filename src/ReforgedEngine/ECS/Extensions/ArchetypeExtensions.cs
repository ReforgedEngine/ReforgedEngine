using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Core.ECS.Extensions
{
    public static class ArchetypeExtensions
    {
        public static bool Has<T>(this Archetype arch)
            where T : IComponent
        {
            return arch.Mask.Contains<T>();
        }

        public static bool Has(this Archetype arch, Type componentType)
        {
            return arch.Mask.Contains(componentType);
        }
    }
}
