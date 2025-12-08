using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Characters.Animation
{
    public struct CharacterAnimationComponent : IComponent
    {
        public string CurrentAnimation;  // "Idle", "Walk", "Attack"
        public int CurrentFrame;
        public float FrameTime;
        public float FrameDuration;
    }
}
