// Entity.cs
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Core.ECS.Entities
{
    public sealed class Entity
    {
        public int Id { get; }
        internal Archetype CurrentArchetype { get; set; }

        private readonly Dictionary<Type, object> _components = new();
        private ComponentMask _mask = ComponentMask.Empty;

        public ComponentMask Mask => _mask;

        internal Entity(int id)
        {
            Id = id;
        }

        public T Get<T>() where T : struct, IComponent
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var boxed))
            {
                return ((Box<T>)boxed).Value;
            }
            return default;
        }

        public ref T GetRef<T>() where T : struct, IComponent
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var boxed))
            {
                return ref ((Box<T>)boxed).Value;
            }
            throw new KeyNotFoundException($"Component {typeof(T)} not found");
        }

        public void Set<T>(T component) where T : struct, IComponent
        {
            var type = typeof(T);
            var oldMask = _mask;

            // Criar uma nova cópia da struct
            _components[type] = new Box<T>(component);

            _mask = _mask.With<T>();

            // Notificar mudança de archetype se necessário
            if (CurrentArchetype != null && !oldMask.Equals(_mask))
            {
                CurrentArchetype.World?.EntityMaskChanged(this, oldMask);
            }
        }

        public void Remove<T>() where T : struct, IComponent
        {
            var type = typeof(T);
            var oldMask = _mask;

            if (_components.Remove(type))
            {
                _mask = _mask.Without<T>();

                // Notificar mudança de archetype se necessário
                if (CurrentArchetype != null && !oldMask.Equals(_mask))
                {
                    CurrentArchetype.World?.EntityMaskChanged(this, oldMask);
                }
            }
        }

        public bool Has<T>() where T : struct, IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        // Wrapper para structs (boxing controlado)
        private class Box<T> where T : struct
        {
            public T Value;
            public Box(T value) => Value = value;
        }
    }
}