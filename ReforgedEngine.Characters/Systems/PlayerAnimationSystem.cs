using Microsoft.Xna.Framework;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.ECS.Systems;

namespace ReforgedEngine.Characters.Systems
{
    public sealed class PlayerAnimationSystem : SystemBase
    {
        public PlayerAnimationSystem()
            : base(ComponentGroups.Renderables
                .With<PlayerTag>()
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

                // Determine animation based on state
                string targetAnimation = GetAnimationForState(state, mv);

                // Change animation if needed
                if (anim.CurrentAnimation != targetAnimation)
                {
                    anim.CurrentAnimation = targetAnimation;
                    anim.CurrentFrame = 0;
                    anim.FrameTime = 0f;
                }

                // Update animation timing
                anim.FrameTime += dt;
                if (anim.FrameTime >= anim.FrameDuration)
                {
                    anim.FrameTime = 0f;
                    anim.CurrentFrame = (anim.CurrentFrame + 1) % GetFrameCountForAnimation(targetAnimation);
                }
            }
        }

        private string GetAnimationForState(PlayerStateComponent state, Movement mv)
        {
            switch (state.CurrentState)
            {
                case PlayerState.Running:
                    return "Run";
                case PlayerState.Walking:
                    return "Walk";
                case PlayerState.Interacting:
                    return "Interact";
                case PlayerState.Combat:
                    return "1H_AttackSideSlash";
                case PlayerState.Dead:
                    return "Dead";
                default: // Idle
                    // Cycle through idle animations based on time
                    if (state.StateTime > 5.0f) return "Idle3";
                    if (state.StateTime > 3.0f) return "Idle2";
                    return "Idle1";
            }
        }

        private int GetFrameCountForAnimation(string animation)
        {
            // Based on PVGames documentation
            return animation switch
            {
                "Walk" or "Run" => 8, // 8 frames per direction
                "Idle1" or "Idle2" or "Idle3" => 12, // 12 frames per direction
                "Interact" => 12,
                "1H_AttackSideSlash" => 12,
                "Dead" => 6,
                _ => 1
            };
        }
    }
}