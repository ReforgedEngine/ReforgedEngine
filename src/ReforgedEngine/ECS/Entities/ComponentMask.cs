// ComponentMask.cs
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Core.ECS.Entities
{
    public struct ComponentMask : IEquatable<ComponentMask>
    {
        private ulong _mask;

        public static ComponentMask Empty => default;

        public ComponentMask With<T>() where T : IComponent
        {
            var mask = this;
            mask._mask |= 1UL << TypeIndex<T>.Value;
            return mask;
        }

        // Método não genérico para uso via reflexão
        public ComponentMask With(Type componentType)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
                throw new ArgumentException($"Type must implement IComponent: {componentType}");

            var mask = this;
            mask._mask |= 1UL << ComponentRegistry.GetIndex(componentType);
            return mask;
        }

        public ComponentMask Without<T>() where T : IComponent
        {
            var mask = this;
            mask._mask &= ~(1UL << TypeIndex<T>.Value);
            return mask;
        }

        public bool Contains<T>() where T : IComponent
        {
            return (_mask & (1UL << TypeIndex<T>.Value)) != 0;
        }

        public bool Contains(Type componentType)
        {
            return (_mask & (1UL << ComponentRegistry.GetIndex(componentType))) != 0;
        }

        public bool Contains(ComponentMask other)
        {
            return (_mask & other._mask) == other._mask;
        }

        public bool Equals(ComponentMask other) => _mask == other._mask;
        public override bool Equals(object obj) => obj is ComponentMask other && Equals(other);
        public override int GetHashCode() => _mask.GetHashCode();

        public static bool operator ==(ComponentMask left, ComponentMask right) => left.Equals(right);
        public static bool operator !=(ComponentMask left, ComponentMask right) => !left.Equals(right);

        private static class TypeIndex<T> where T : IComponent
        {
            public static readonly int Value = ComponentRegistry.RegisterType(typeof(T));
        }
    }

    internal static class ComponentRegistry
    {
        private static readonly Dictionary<Type, int> _typeToIndex = new();
        private static int _nextIndex = 0;

        public static int RegisterType(Type type)
        {
            if (!_typeToIndex.TryGetValue(type, out int index))
            {
                index = _nextIndex++;
                _typeToIndex[type] = index;
            }
            return index;
        }

        public static int GetIndex(Type type)
        {
            return _typeToIndex.TryGetValue(type, out int index) ? index : -1;
        }
    }
}