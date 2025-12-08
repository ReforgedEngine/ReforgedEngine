
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.Rendering;

namespace ReforgedEngine.Core.ECS.Systems
{
    public sealed class MapValidationSystem : SystemBase
    {
        private int _frameCount = 0;

        public MapValidationSystem()
        {
            _mask = ComponentMask.Empty
                .With<Position>()
                .With<Renderable>()
                .With<TilePropertiesComponent>();
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            _frameCount++;
            if (_frameCount % 300 != 0) return; // A cada 5 segundos

            var gameTime = (GameTime)ctx;
            var entities = archetype.Entities;

            System.Diagnostics.Debug.WriteLine($"[VALIDATION] ===== FRAME {_frameCount} =====");

            // Coletar estatísticas
            var terrainEntities = new System.Collections.Generic.List<Entity>();
            var wallEntities = new System.Collections.Generic.List<Entity>();

            foreach (var entity in entities)
            {
                var render = entity.Get<Renderable>();

                if (render.RenderLayer == RenderLayer.TerrainBase)
                    terrainEntities.Add(entity);
                else if (render.RenderLayer.ToString().Contains("Wall"))
                    wallEntities.Add(entity);
            }

            System.Diagnostics.Debug.WriteLine($"[VALIDATION] TerrainBase: {terrainEntities.Count} entities");
            System.Diagnostics.Debug.WriteLine($"[VALIDATION] Walls: {wallEntities.Count} entities");

            // Comparar Z médio
            if (terrainEntities.Any() && wallEntities.Any())
            {
                float avgTerrainZ = terrainEntities.Average(e => e.Get<Position>().Z);
                float avgWallZ = wallEntities.Average(e => e.Get<Position>().Z);

                System.Diagnostics.Debug.WriteLine($"[VALIDATION] Avg Terrain Z: {avgTerrainZ:F2}");
                System.Diagnostics.Debug.WriteLine($"[VALIDATION] Avg Wall Z: {avgWallZ:F2}");
                System.Diagnostics.Debug.WriteLine($"[VALIDATION] Difference (Wall - Terrain): {avgWallZ - avgTerrainZ:F2}");

                if (avgWallZ <= avgTerrainZ)
                {
                    System.Diagnostics.Debug.WriteLine($"[VALIDATION] ⚠️⚠️⚠️ ERRO CRÍTICO: Walls estão abaixo do Terrain!");

                    // Mostrar exemplo específico
                    var exampleWall = wallEntities.First();
                    var exampleTerrain = terrainEntities.First();

                    var wallPos = exampleWall.Get<Position>();
                    var terrainPos = exampleTerrain.Get<Position>();
                    var wallProps = exampleWall.Get<TilePropertiesComponent>();
                    var terrainProps = exampleTerrain.Get<TilePropertiesComponent>();

                    System.Diagnostics.Debug.WriteLine($"[VALIDATION] Example Wall:");
                    System.Diagnostics.Debug.WriteLine($"  Z: {wallPos.Z}, ZBase: {wallPos.ZBase}, Floor: {wallPos.Floor}");
                    System.Diagnostics.Debug.WriteLine($"  TMX z_base: {wallProps.Properties.GetValueOrDefault("z_base", "NOT FOUND")}");

                    System.Diagnostics.Debug.WriteLine($"[VALIDATION] Example Terrain:");
                    System.Diagnostics.Debug.WriteLine($"  Z: {terrainPos.Z}, ZBase: {terrainPos.ZBase}, Floor: {terrainPos.Floor}");
                    System.Diagnostics.Debug.WriteLine($"  TMX z_base: {terrainProps.Properties.GetValueOrDefault("z_base", "NOT FOUND")}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[VALIDATION] ✅ CORRETO: Walls estão acima do Terrain!");
                }
            }

            System.Diagnostics.Debug.WriteLine("");
        }
    }
}