using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    public sealed class AnimationSystem : SystemBase
    {
        public AnimationSystem()
        {
            _mask = ComponentMask.Empty
                .With<Renderable>()
                .With<Animation>();
        }

        protected override void ProcessArchetype(World world, Archetype arch, object ctx)
        {
            var gameTime = (GameTime)ctx;

            foreach (var e in arch.Entities)
            {
                var anim = e.Get<Animation>();
                var ren = e.Get<Renderable>();

                anim.Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (anim.Timer >= anim.FrameTime)
                {
                    anim.Timer = 0;
                    anim.FrameIndex = (anim.FrameIndex + 1) % anim.Frames;

                    // Atualizar SourceRect com base no frame
                    if (anim.Frames > 0)
                    {
                        int frameWidth = ren.SourceRect.Width / anim.Frames;
                        ren.SourceRect = new Rectangle(
                            anim.FrameIndex * frameWidth,
                            0,
                            frameWidth,
                            ren.SourceRect.Height);

                        e.Set(ren);
                    }
                }

                e.Set(anim);
            }
        }
    }
}