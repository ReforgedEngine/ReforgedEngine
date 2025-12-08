// Archetype.cs
namespace ReforgedEngine.Core.ECS.Entities
{
    public sealed class Archetype
    {
        public readonly ComponentMask Mask;
        public readonly World World;
        public readonly List<Entity> Entities = new List<Entity>(256);

        public Archetype(World world, ComponentMask mask)
        {
            World = world;
            Mask = mask;
        }

        internal void AddEntity(Entity e)
        {
            Entities.Add(e);
            e.CurrentArchetype = this;
        }

        internal void RemoveEntity(Entity e)
        {
            Entities.Remove(e);
            e.CurrentArchetype = null;
        }
    }
}