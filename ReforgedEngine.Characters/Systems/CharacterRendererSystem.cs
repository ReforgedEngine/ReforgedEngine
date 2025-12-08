using Microsoft.Xna.Framework;
using ReforgedEngine.Characters.Animation.PVGames;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Characters.Systems
{
    public sealed class CharacterRenderSystem : SystemBase
    {
        private readonly PVGSpritesheetInfo _spriteSheets;

        public CharacterRenderSystem(PVGSpritesheetInfo spriteSheets)
            : base(ComponentMask.Empty
                .With<AnimationState>()
                .With<Renderable>())
        {
            _spriteSheets = spriteSheets;
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            foreach (var e in archetype.Entities)
            {
                ref AnimationState anim = ref e.GetRef<AnimationState>();
                ref Renderable renderable = ref e.GetRef<Renderable>();

                // Usar PVGFrameResolver
                var frameRect = PVGFrameResolver.GetFrameRectangle(
                    anim.CurrentAnimation,
                    anim.Direction,
                    anim.CurrentFrame
                );

                var texture = PVGFrameResolver.GetTexture(anim.CurrentAnimation);

                if (texture != null)
                {
                    renderable.Texture = texture;
                    renderable.SourceRect = frameRect;
                    renderable.Origin = new Vector2(frameRect.Width / 2f, frameRect.Height);
                }
            }
        }
    }
}