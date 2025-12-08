using Microsoft.Xna.Framework;
using ReforgedEngine.Core.Camera;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.Tools
{
    /// <summary>
    /// Utilitário oficial para mover a câmera como se fosse um player.
    /// Não possui sprite, collider, nem animação.
    /// Apenas gera uma Entity válida com Position e Renderable 
    /// para permitir testes de fade/hide/oclusão.
    /// </summary>
    public sealed class DebugPlayerController
    {
        private readonly World _world;
        private readonly Camera2D _camera;
        private Entity _entity;

        public Entity Entity => _entity;

        public DebugPlayerController(World world, Camera2D camera)
        {
            _world = world;
            _camera = camera;
        }

        // =====================================================================
        // SPAWN
        // =====================================================================

        public void Spawn(Vector2 isoFeet, int floor = 0)
        {
            _entity = _world.CreateEntity();

            var pos = new Position
            {
                FeetIso = isoFeet,
                Floor = floor,
                ZBase = floor * 4f, // compatível com z_step_per_floor
                Origin = Vector2.Zero
            };

            var render = new Renderable
            {
                Visible = false,
                //Alpha = 1f,
                RenderLayer = Rendering.RenderLayer.ObjectsLow,
                Origin = Vector2.Zero
            };

            _entity.Set(pos);
            _entity.Set(render);

            // A câmera segue este "player ghost"
            _camera.LookAt(isoFeet);

            // Atualiza valor global usado pelo OcclusionFadeSystem
            _world.PlayerFeetIso = isoFeet;
        }

        // =====================================================================
        // MOVIMENTAÇÃO 
        // =====================================================================

        public void Move(Vector2 delta)
        {
            if (_entity == null) return;

            ref var pos = ref _entity.GetRef<Position>();
            pos.FeetIso += delta;

            // Atualiza world PlayerFeetIso (fade/hide depende disso!)
            _world.PlayerFeetIso = pos.FeetIso;

            // Move câmera junto
            _camera.LookAt(pos.FeetIso);
        }

        // =====================================================================
        // TROCA DE ANDAR
        // =====================================================================

        public void ChangeFloor(int delta)
        {
            if (_entity == null) return;

            ref var pos = ref _entity.GetRef<Position>();

            pos.Floor = Math.Max(0, pos.Floor + delta);
            pos.ZBase = pos.Floor * 4f;  // compatível com template TMX

            // Fade/hide depende do floor correto
        }
    }
}
