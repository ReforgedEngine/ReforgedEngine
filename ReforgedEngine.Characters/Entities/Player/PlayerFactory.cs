using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Characters.Animation;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Characters.Entities.Player
{
    public static class PlayerFactory
    {
        public static Entity Create(World world, Texture2D spriteSheet, Vector2 startPos)
        {
            Entity e = world.CreateEntity();

            e.Set(new Position { WorldPos = startPos, Floor = 0 });
            e.Set(new Renderable { Texture = spriteSheet });
            e.Set(new PlayerMovementComponent { Speed = 180f });
            e.Set(new PlayerStateComponent());
            e.Set(new CharacterAnimationComponent
            {
                CurrentAnimation = "Walk",
                CurrentFrame = 0,
                FrameTime = 0,
                FrameDuration = 0.1f
            });

            world.AddEntity(e);
            return e;
        }
    }
}
