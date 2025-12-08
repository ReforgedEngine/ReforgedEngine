// Characters/Systems/CharacterAnimationSystem.cs (unificado)
using Microsoft.Xna.Framework;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Characters.Systems
{
    public sealed class CharacterAnimationSystem : SystemBase
    {
        public CharacterAnimationSystem()
            : base(ComponentMask.Empty
                .With<AnimationState>()
                .With<PlayerStateComponent>()
                .With<Movement>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            var gameTime = (GameTime)ctx;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var e in archetype.Entities)
            {
                ref AnimationState anim = ref e.GetRef<AnimationState>();
                ref PlayerStateComponent state = ref e.GetRef<PlayerStateComponent>();
                ref Movement mv = ref e.GetRef<Movement>();

                // Determinar animação baseada no estado
                string targetAnimation = GetAnimationForState(state, mv);

                // Mudar animação se necessário
                if (anim.CurrentAnimation != targetAnimation)
                {
                    anim.CurrentAnimation = targetAnimation;
                    anim.CurrentFrame = 0;
                    anim.FrameTime = 0f;
                }

                // Atualizar frame
                anim.FrameTime += dt;
                if (anim.FrameTime >= anim.FrameDuration)
                {
                    anim.FrameTime = 0f;
                    anim.CurrentFrame = (anim.CurrentFrame + 1) % GetFrameCount(targetAnimation);
                }
            }
        }

        private string GetAnimationForState(PlayerStateComponent state, Movement mv)
        {
            if (state.CurrentState == PlayerState.Dead) return "Dead";
            if (state.CurrentState == PlayerState.Combat) return "1H_AttackSideSlash";
            if (state.CurrentState == PlayerState.Interacting) return "Interact";

            if (mv.IsMoving)
            {
                return state.CurrentState == PlayerState.Running ? "Run" : "Walk";
            }

            // Idle animations cycle based on time
            if (state.StateTime > 5.0f) return "Idle3";
            if (state.StateTime > 3.0f) return "Idle2";
            return "Idle1";
        }

        private int GetFrameCount(string animation)
        {
            return animation switch
            {
                "Walk" or "Run" => 8,
                "Idle1" or "Idle2" or "Idle3" => 12,
                "Interact" => 12,
                "1H_AttackSideSlash" => 12,
                "Dead" => 6,
                _ => 1
            };
        }
    }
}