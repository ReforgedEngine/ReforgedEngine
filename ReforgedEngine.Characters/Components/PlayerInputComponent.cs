using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Characters.Components
{
    public struct PlayerInputComponent : IComponent
    {
        public Keys UpKey;
        public Keys DownKey;
        public Keys LeftKey;
        public Keys RightKey;
        public Keys RunKey;
        public Keys InteractKey;

        // Estado atual do input
        public Vector2 MovementDirection;
        public bool IsRunning;
        public bool IsInteracting;
        public bool JustPressedInteract;

        // Configuração padrão (WASD)
        public static PlayerInputComponent Default => new PlayerInputComponent
        {
            UpKey = Keys.W,
            DownKey = Keys.S,
            LeftKey = Keys.A,
            RightKey = Keys.D,
            RunKey = Keys.LeftShift,
            InteractKey = Keys.E,
            MovementDirection = Vector2.Zero,
            IsRunning = false,
            IsInteracting = false,
            JustPressedInteract = false
        };
    }
}