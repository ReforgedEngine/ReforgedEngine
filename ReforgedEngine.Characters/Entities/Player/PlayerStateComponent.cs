using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Characters.Components
{
    public struct PlayerStateComponent : IComponent
    {
        public PlayerState CurrentState;
        public PlayerState PreviousState;
        public float StateTime;

        // Estados específicos
        public bool IsInBuilding;
        public bool IsInCombat;
        public bool IsInteracting;
    }

    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Interacting,
        Combat,
        Dead
    }
}