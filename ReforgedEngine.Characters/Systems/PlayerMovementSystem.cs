using Microsoft.Xna.Framework;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.ECS.Systems;

namespace ReforgedEngine.Characters.Systems
{
    public sealed class PlayerMovementSystem : SystemBase
    {
        public PlayerMovementSystem()
            : base(ComponentGroups.Renderables
                .With<PlayerTag>()
                .With<Movement>()
                .With<Transform>()
                .With<Position>()
                .With<PlayerStateComponent>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            var gameTime = (GameTime)ctx;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var e in archetype.Entities)
            {
                ref Movement mv = ref e.GetRef<Movement>();
                ref Transform tf = ref e.GetRef<Transform>();
                ref Position pos = ref e.GetRef<Position>();
                ref PlayerStateComponent state = ref e.GetRef<PlayerStateComponent>();

                // Skip if interacting or dead
                if (state.CurrentState == PlayerState.Interacting ||
                    state.CurrentState == PlayerState.Dead)
                {
                    continue;
                }

                // Apply movement
                if (mv.IsMoving)
                {
                    Vector2 proposedMovement = mv.Velocity * dt;

                    // Check collision if collider exists
                    if (e.Has<Collider>())
                    {
                        var collider = e.Get<Collider>();
                        if (collider.IsSolid)
                        {
                            proposedMovement = HandleCollision(world, e, tf.Position, proposedMovement);
                        }
                    }

                    tf.Position += proposedMovement;
                    tf.FeetPosition = tf.Position;
                }

                // Update position component
                pos.WorldPos = tf.Position;
                pos.FeetWorld = tf.Position;

                // ISO update will be handled by SortKeyUpdateSystem

                // Update world player position
                world.PlayerFeetIso = pos.FeetIso;

                // Update state time
                state.StateTime += dt;
            }
        }

        private Vector2 HandleCollision(World world, Entity player, Vector2 currentPos, Vector2 proposedMove)
        {
            // Collision is handled by the Engine's CollisionSystem
            // This system just applies movement
            return proposedMove;
        }
    }
}