// SystemBase.cs
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Core.ECS.Entities
{
    public abstract class SystemBase
    {
        protected ComponentMask _mask;

        // Construtor com mask (para sistemas que precisam de máscara específica)
        protected SystemBase(ComponentMask mask)
        {
            _mask = mask;
        }

        // Construtor vazio (para sistemas sem máscara específica ou que definem depois)
        protected SystemBase()
        {
            _mask = ComponentMask.Empty;
        }

        // Método para definir máscara (se usar construtor vazio)
        protected void SetMask(ComponentMask mask)
        {
            _mask = mask;
        }

        public void Update(World world, object ctx)
        {
            foreach (var archetype in world.Archetypes)
            {
                if (archetype.Mask.Contains(_mask))
                    ProcessArchetype(world, archetype, ctx);
            }
        }

        protected abstract void ProcessArchetype(World world, Archetype archetype, object ctx);
    }
}