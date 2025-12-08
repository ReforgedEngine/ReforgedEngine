// PlayerPrefab.cs (atualizado)
using Microsoft.Xna.Framework;
using ReforgedEngine.Characters.Animation.PVGames;
using ReforgedEngine.Characters.Components;
using ReforgedEngine.Characters.Entities.Player;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.Rendering;

namespace ReforgedEngine.Characters.Prefabs
{
    public static class PlayerPrefab
    {
        public static Entity Create(World world, PVGSpritesheetInfo spriteSheets, // Agora funciona
                                    Vector2 startPos, Vector2 mapOffset,
                                    float tileWidth, float tileHeight)
        {
            var e = world.CreateEntity();

            // Inicializar o PVGFrameResolver
            PVGFrameResolver.Initialize(spriteSheets);

            // 1. POSITION (Componente da Engine)
            var position = new Position
            {
                WorldPos = startPos,
                FeetWorld = startPos,
                Floor = 0,
                ZBase = 3f,
                Z = 3f,
                Origin = new Vector2(PVGFrameResolver.FrameWidth / 2f,
                                   PVGFrameResolver.FrameHeight),
                IsTile = false
            };

            position.UpdateIso(mapOffset, tileWidth, tileHeight);
            e.Set(position);

            // 2. TRANSFORM (Componente do Characters)
            e.Set(new Transform
            {
                Position = startPos,
                FeetPosition = startPos,
                Scale = 1f,
                Rotation = 0f,
                Floor = 0
            });

            // 3. MOVEMENT
            e.Set(new Movement
            {
                Speed = 250f,
                Velocity = Vector2.Zero,
                IsMoving = false,
                Direction = MoveDirection.South
            });

            // 4. PLAYER INPUT
            e.Set(PlayerInputComponent.Default);

            // 5. ANIMATION STATE
            e.Set(new AnimationState
            {
                CurrentAnimation = "Idle1",
                CurrentFrame = 0,
                FrameTime = 0f,
                FrameDuration = 0.15f,
                Direction = (int)MoveDirection.South,
                TextureKey = "Sprite1"
            });

            // 6. PLAYER STATE
            e.Set(new PlayerStateComponent
            {
                CurrentState = PlayerState.Idle,
                PreviousState = PlayerState.Idle,
                StateTime = 0f,
                IsInBuilding = false,
                IsInCombat = false,
                IsInteracting = false
            });

            // 7. PLAYER TAG
            e.Set(new PlayerTag());

            // 8. RENDERABLE (Componente da Engine)
            var idleFrame = PVGFrameResolver.GetFrameRectangle("Idle1", (int)MoveDirection.South, 0);

            var renderable = new Renderable
            {
                Texture = spriteSheets.GetSheet("Sprite1"),
                SourceRect = idleFrame,
                Origin = new Vector2(PVGFrameResolver.FrameWidth / 2f,
                                   PVGFrameResolver.FrameHeight),
                Tint = Color.White,
                Fade = 1f,
                Visible = true,
                RenderLayer = RenderLayer.ObjectsHigh,
                OcclusionFadeEnabled = true
            };

            renderable.SortKey = IsoRenderKey.From(position, renderable).Raw;
            e.Set(renderable);

            // 9. COLLIDER (Componente da Engine)
            var collider = new Core.ECS.Components.Collider
            {
                Bounds = new Rectangle(-40, -80, 80, 80),
                IsSolid = true,
                PixelPerfect = false
            };
            e.Set(collider);

            return e;
        }
    }
}