using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Characters.Entities.Player;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.ECS.Systems;
using System;

namespace ReforgedEngine.Characters.Systems
{
    public sealed class PlayerInputSystem : SystemBase
    {
        private KeyboardState _prevKeyboardState;

        public PlayerInputSystem()
            : base(ComponentGroups.Renderables // Usar o mesmo padrão da Engine
                .With<PlayerTag>()
                .With<PlayerInputComponent>()
                .With<Movement>()
                .With<Transform>()
                .With<AnimationState>()
                .With<PlayerStateComponent>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            var gameTime = (GameTime)ctx;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var currentKeyboard = Keyboard.GetState();

            foreach (var e in archetype.Entities)
            {
                ref PlayerInputComponent input = ref e.GetRef<PlayerInputComponent>();
                ref Movement mv = ref e.GetRef<Movement>();
                ref Transform tf = ref e.GetRef<Transform>();
                ref AnimationState anim = ref e.GetRef<AnimationState>();
                ref PlayerStateComponent state = ref e.GetRef<PlayerStateComponent>();

                // Reset input state
                input.MovementDirection = Vector2.Zero;
                input.IsRunning = false;
                input.JustPressedInteract = false;

                // Capture movement
                if (currentKeyboard.IsKeyDown(input.UpKey)) input.MovementDirection.Y -= 1;
                if (currentKeyboard.IsKeyDown(input.DownKey)) input.MovementDirection.Y += 1;
                if (currentKeyboard.IsKeyDown(input.LeftKey)) input.MovementDirection.X -= 1;
                if (currentKeyboard.IsKeyDown(input.RightKey)) input.MovementDirection.X += 1;

                // Running
                input.IsRunning = currentKeyboard.IsKeyDown(input.RunKey);

                // Interaction
                bool interactPressed = currentKeyboard.IsKeyDown(input.InteractKey);
                bool prevInteractPressed = _prevKeyboardState.IsKeyDown(input.InteractKey);
                input.JustPressedInteract = interactPressed && !prevInteractPressed;

                // Apply to movement component
                mv.IsMoving = input.MovementDirection != Vector2.Zero;

                if (mv.IsMoving)
                {
                    if (input.MovementDirection.LengthSquared() > 1f)
                        input.MovementDirection.Normalize();

                    float currentSpeed = input.IsRunning ? mv.Speed * 1.5f : mv.Speed;
                    mv.Velocity = input.MovementDirection * currentSpeed;

                    mv.Direction = ResolveDirection(input.MovementDirection);
                    anim.Direction = (int)mv.Direction;

                    // Update player state
                    if (state.CurrentState != PlayerState.Interacting &&
                        state.CurrentState != PlayerState.Combat)
                    {
                        state.PreviousState = state.CurrentState;
                        state.CurrentState = input.IsRunning ? PlayerState.Running : PlayerState.Walking;
                        state.StateTime = 0f;
                    }
                }
                else
                {
                    mv.Velocity = Vector2.Zero;

                    if (state.CurrentState != PlayerState.Interacting &&
                        state.CurrentState != PlayerState.Combat &&
                        state.CurrentState != PlayerState.Idle)
                    {
                        state.PreviousState = state.CurrentState;
                        state.CurrentState = PlayerState.Idle;
                        state.StateTime = 0f;
                    }
                }
            }

            _prevKeyboardState = currentKeyboard;
        }

        private MoveDirection ResolveDirection(Vector2 dir)
        {
            float angle = MathHelper.ToDegrees((float)Math.Atan2(dir.Y, dir.X));
            if (angle < 0) angle += 360;

            angle += 45; // Adjust for isometric
            if (angle >= 360) angle -= 360;

            float sector = angle / 45f;
            int sectorIndex = (int)Math.Floor(sector) % 8;

            return (MoveDirection)sectorIndex;
        }
    }
}