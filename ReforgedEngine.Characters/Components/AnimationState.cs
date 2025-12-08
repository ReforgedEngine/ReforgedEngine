// AnimationState.cs
using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Characters.Components
{
    public struct AnimationState : IComponent
    {
        public string CurrentAnimation;
        public int CurrentFrame;
        public float FrameTime;
        public float FrameDuration;
        public int Direction;
        public string TextureKey; // Adicionar esta propriedade
    }
}