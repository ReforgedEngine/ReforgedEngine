// World.cs
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS
{
    public class World
    {
        private int _nextEntityId = 1;
        private readonly List<Entity> _entities = new();
        private readonly Dictionary<ComponentMask, Archetype> _archetypes = new();
        private readonly List<Archetype> _archetypeList = new(); // Para IReadOnlyList
        private readonly Dictionary<string, SystemBase> _systems = new();

        public IReadOnlyList<Archetype> Archetypes => _archetypeList;
        public Dictionary<string, SystemBase> Systems => _systems;
        public Vector2 PlayerFeetIso { get; set; } = Vector2.Zero;

        // Adicionar referência ao World no Archetype
        public World()
        {
            // Adicionar archetype vazio inicial
            var emptyArchetype = new Archetype(this, ComponentMask.Empty);
            _archetypes[ComponentMask.Empty] = emptyArchetype;
            _archetypeList.Add(emptyArchetype);
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(_nextEntityId++);
            _entities.Add(entity);

            // Inicialmente colocado no archetype vazio
            var emptyArchetype = GetOrCreateArchetype(ComponentMask.Empty);
            emptyArchetype.AddEntity(entity);

            return entity;
        }

        public void AddEntity(Entity entity)
        {
            if (_entities.Contains(entity))
                return;

            _entities.Add(entity);
            var archetype = GetOrCreateArchetype(entity.Mask);
            archetype.AddEntity(entity);
        }

        public void DestroyEntity(Entity entity)
        {
            entity.CurrentArchetype?.RemoveEntity(entity);
            _entities.Remove(entity);
        }

        internal void EntityMaskChanged(Entity entity, ComponentMask oldMask)
        {
            // Remove do archetype antigo
            if (_archetypes.TryGetValue(oldMask, out var oldArchetype))
            {
                oldArchetype.RemoveEntity(entity);
            }

            // Adiciona ao novo archetype
            var newArchetype = GetOrCreateArchetype(entity.Mask);
            newArchetype.AddEntity(entity);
        }

        private Archetype GetOrCreateArchetype(ComponentMask mask)
        {
            if (!_archetypes.TryGetValue(mask, out var archetype))
            {
                archetype = new Archetype(this, mask);
                _archetypes[mask] = archetype;
                _archetypeList.Add(archetype);
            }
            return archetype;
        }

        public void Update(object context)
        {
            foreach (var system in _systems.Values)
            {
                system.Update(this, context);
            }
        }
    }
}