using Microsoft.Xna.Framework.Input;

namespace ReforgedEngine.Core.ECS.Components
{
    /// <summary>
    /// Captures per-frame keyboard state for systems that need it.
    /// </summary>
    public sealed class InputState : IComponent
    {
        public KeyboardState Keyboard;
    }
}
