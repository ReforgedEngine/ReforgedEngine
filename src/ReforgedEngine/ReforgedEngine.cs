// ReforgedEngine.cs (principal)
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Content;
using ReforgedEngine.Core;
using ReforgedEngine.Core.Camera;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.ECS.Systems;
using ReforgedEngine.Core.Map;
using ReforgedEngine.Core.Tiles;

namespace ReforgedEngine
{
    public sealed class ReforgedEngine
    {
        public World World { get; } = new World();
        public MapLoader MapLoader { get; }
        public Camera2D Camera { get; }
        

        private readonly RenderSystem _renderSystem;
        private readonly FrustumCullingSystem _frustumCullingSystem;
        private readonly SortKeyUpdateSystem _sortKeySystem;
        private readonly AsyncContentManager _asyncContent;

        public Entity PlayerEntity { get; private set; }

        public ReforgedEngine(GraphicsDevice graphicsDevice, ContentManager content,
                             Viewport viewport, SpriteBatch spriteBatch)
        {
            LicenseManager.Validate();

            _asyncContent = new AsyncContentManager(content, graphicsDevice);

            // Create camera
            Camera = new Camera2D(viewport);

            // Create map loader
            var tileResolver = new TileResolverFromTmx(content);
            MapLoader = new MapLoader(graphicsDevice, content, tileResolver);

            // Create systems
            _renderSystem = new RenderSystem(Camera)
            {
                SpriteBatch = spriteBatch
            };

            _frustumCullingSystem = new FrustumCullingSystem(Camera);
            _sortKeySystem = new SortKeyUpdateSystem(Vector2.Zero);

            // Register Engine systems
            World.Systems["Render"] = _renderSystem;
            World.Systems["FrustumCulling"] = _frustumCullingSystem;
            World.Systems["SortKeyUpdate"] = _sortKeySystem;
            World.Systems["Collision"] = new CollisionSystem();
            World.Systems["Animation"] = new AnimationSystem();
            World.Systems["OcclusionFade"] = new OcclusionFadeSystem();

            // Register Player/Character systems (will be added after player creation)
            // They need PlayerSpriteSheets to be fully loaded

#if DEBUG
            World.Systems["AutoOrigin"] = new AutoOriginCorrectionSystem();
            World.Systems["Validation"] = new MapValidationSystem();
#endif
        }
      
        public async Task LoadMapAsync(string tmxPath)
        {
            await Task.Run(() => MapLoader.Load(World, tmxPath));

            // Update sort system with new map offset
            _sortKeySystem.MapOffset = MapLoader.MapOffset;
        }

        public void LoadMap(string tmxPath)
        {
            MapLoader.Load(World, tmxPath);
            _sortKeySystem.MapOffset = MapLoader.MapOffset;
        }

        public void Update(GameTime gameTime)
        {  
            // Update all systems
            World.Update(gameTime);
        }

        public void Draw()
        {
            // The RenderSystem handles its own Begin/End
            // We just need to ensure SpriteBatch is set
            if (_renderSystem.SpriteBatch != null)
            {
                // Update with null context (RenderSystem will use its own logic)
                _renderSystem.Update(World, null);
            }
        }

        public void SetSpriteBatch(SpriteBatch spriteBatch)
        {
            if (_renderSystem != null)
                _renderSystem.SpriteBatch = spriteBatch;
        }
    }
}