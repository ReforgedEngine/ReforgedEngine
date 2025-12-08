using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Characters.Animation.PVGames;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.ECS.Systems;

namespace ReforgedEngine.Characters.Systems
{
    public sealed class PlayerRendererSystem : SystemBase
    {
        private readonly PVGSpritesheetInfo _spriteSheets;

        public PlayerRendererSystem(PVGSpritesheetInfo spriteSheets)
            : base(ComponentGroups.Renderables
                .With<PlayerTag>()
                .With<AnimationState>()
                .With<Renderable>())
        {
            _spriteSheets = spriteSheets;
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            var gameTime = (GameTime)ctx;

            foreach (var e in archetype.Entities)
            {
                ref AnimationState anim = ref e.GetRef<AnimationState>();
                ref Renderable renderable = ref e.GetRef<Renderable>();

                // Get current frame rectangle from PVGames spritesheet
                var frameRect = GetAnimationFrame(anim.CurrentAnimation, anim.Direction, anim.CurrentFrame);

                // Update renderable
                renderable.SourceRect = frameRect;
                renderable.Texture = _spriteSheets.GetSheet(GetSheetForAnimation(anim.CurrentAnimation));

                // Update origin (center bottom for characters)
                renderable.Origin = new Vector2(frameRect.Width / 2f, frameRect.Height);
            }
        }

        private Rectangle GetAnimationFrame(string animation, int direction, int frame)
        {
            // Calculate frame position based on PVGames layout
            int framesPerDirection = GetFrameCountForAnimation(animation);
            int frameIndex = (direction * framesPerDirection) + frame;

            string sheet = GetSheetForAnimation(animation);

            return _spriteSheets.GetFrame(sheet, frameIndex);
        }

        private string GetSheetForAnimation(string animation)
        {
            // Based on PVGames documentation
            if (animation.StartsWith("1H_") || animation.StartsWith("2H_") ||
                animation == "AttackPolearm" || animation == "IdleDualWield" ||
                animation == "AttackDualWield")
                return "Sprite2";

            if (animation == "AttackPistol" || animation == "IdleMG" ||
                animation == "AttackMG" || animation == "IdleRifle")
                return "Sprite3";

            if (animation == "Sneak" || animation == "Jump" ||
                animation == "Woozy" || animation == "Attack2HSwing")
                return "Sprite4";

            return "Sprite1"; // Default
        }

        private int GetFrameCountForAnimation(string animation)
        {
            // Based on PVGames documentation
            return animation switch
            {
                "Walk" or "Run" or "Sneak" => 8,
                "Idle1" or "Idle2" or "Idle3" or "Idle4" or "Idle5" or "Idle6" => 12,
                "Interact" or "Sleep" or "Sitting" => 12,
                "1H_AttackOverhead" or "1H_AttackSideSlash" or "1H_AttackStab" => 12,
                "2H_AttackSlash" or "AttackPolearm" => 12,
                _ => 1
            };
        }
    }
}