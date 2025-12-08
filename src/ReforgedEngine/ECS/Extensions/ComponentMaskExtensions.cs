using ReforgedEngine.Core.ECS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ReforgedEngine.Core.ECS.Extensions
{
    public static class ComponentMaskExtensions
    {
        public static ComponentMask With<T>(this ComponentMask mask) where T : IComponent
        {
            
            return mask.With(typeof(T));
        }
    }
}
