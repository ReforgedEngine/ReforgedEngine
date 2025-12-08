using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.Rendering;
using ReforgedEngine.Core.Tiled;
using ReforgedEngine.Core.Tiles;
using ReforgedEngine.Isometric;
using TiledCS;

namespace ReforgedEngine.Core.Map
{
    /// <summary>
    /// Loads TMX infinite maps + TSX tilesets into ECS entities.
    /// Based on the old working version, adapted for ECS.
    /// </summary>
    public sealed class MapLoader
    {
        private readonly GraphicsDevice _graphics;
        private readonly ContentManager _content;
        private readonly ITileResolver _resolver;

        public TiledMap Map { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public Vector2 MapOffset { get; private set; }

        public MapLoader(GraphicsDevice graphics, ContentManager content, ITileResolver resolver)
        {
            _graphics = graphics;
            _content = content;
            _resolver = resolver;
        }

        public void Load(World world, string relativePath)
        {
            string fullPath = Path.Combine(_content.RootDirectory, relativePath).Replace("\\", "/");

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("TMX not found", fullPath);

            Map = new TiledMap(fullPath);

            TileWidth = Map.TileWidth;
            TileHeight = Map.TileHeight;

            CalculateMapOffset(TileWidth, TileHeight);
            LoadTilesets(fullPath);
            ProcessRootLayers(world, fullPath);
            ProcessRootGroups(world, fullPath);
        }

        private void CalculateMapOffset(float tileWidth, float tileHeight)
        {
            // ✅ LÓGICA CORRETA para 64x64
            Vector2 isoZero = IsoMath.WorldToIso(Vector2.Zero, tileWidth, tileHeight);
            Vector2 screenCenter = new Vector2(_graphics.Viewport.Width / 2f, _graphics.Viewport.Height / 2f);
            MapOffset = screenCenter - isoZero;

            System.Diagnostics.Debug.WriteLine($"[MAPOFFSET] Calculated: {MapOffset} for tiles {tileWidth}x{tileHeight}");
        }

        private void LoadTilesets(string tmxPath)
        {
            if (Map.Tilesets == null) return;

            string baseDir = Path.GetDirectoryName(tmxPath) ?? "";

            foreach (var ts in Map.Tilesets)
            {
                string tsx = ts.source.Replace("../", "/");
                string fullTsxPath = Path.Combine(baseDir, tsx).Replace("\\", "/");

                if (!File.Exists(fullTsxPath))
                {
                    // Tentar caminho alternativo
                    string altPath = Path.Combine(_content.RootDirectory, tsx.TrimStart('/')).Replace("\\", "/");
                    if (File.Exists(altPath))
                        fullTsxPath = altPath;
                    else
                        continue;
                }

                var tileset = new TiledTileset(fullTsxPath);
                _resolver.RegisterTileset(ts.firstgid, tileset, fullTsxPath);

                // Registrar propriedades dos tiles
                if (tileset.Tiles != null)
                {
                    foreach (var tile in tileset.Tiles)
                    {
                        int gid = ts.firstgid + tile.id;
                        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                        if (tile.properties != null)
                        {
                            foreach (var p in tile.properties)
                                dict[p.name] = p.value;
                        }

                        _resolver.RegisterTileProperties(gid, dict);
                    }
                }
            }
        }

        private void ProcessRootLayers(World world, string tmxPath)
        {
            if (Map.Layers == null) return;

            foreach (var layer in Map.Layers)
            {
                ProcessLayer(world, layer, null, layer.name, 0, tmxPath);
            }
        }

        private void ProcessRootGroups(World world, string tmxPath)
        {
            if (Map.Groups == null) return;

            foreach (var group in Map.Groups)
            {
                // Processar layers dentro do grupo
                if (group.layers != null)
                {
                    foreach (var layer in group.layers)
                    {
                        ProcessLayer(world, layer, group, layer.name, 0, tmxPath);
                    }
                }

                // Processar sub-grupos recursivamente
                if (group.groups != null)
                {
                    ProcessSubGroups(world, group, tmxPath);
                }
            }
        }

        private void ProcessSubGroups(World world, TiledGroup parentGroup, string tmxPath)
        {
            if (parentGroup.groups == null) return;

            foreach (var group in parentGroup.groups)
            {
                var props = ConvertProps(group.properties);
                int groupFloor = ResolveFloor(props);

                if (group.layers != null)
                {
                    foreach (var layer in group.layers)
                    {
                        ProcessLayer(world, layer, group, layer.name, groupFloor, tmxPath);
                    }
                }

                // Continuar recursão
                if (group.groups != null)
                {
                    ProcessSubGroups(world, group, tmxPath);
                }
            }
        }

        private void ProcessGroupRecursive(World world, TiledGroup group, string name, int inheritedFloor, string tmxPath)
        {
            var props = ConvertProps(group.properties);
            int groupFloor = ResolveFloor(props);
            int floor = groupFloor >= 0 ? groupFloor : inheritedFloor;

            if (group.layers != null)
            {
                foreach (var layer in group.layers)
                {
                    ProcessLayer(world, layer, group, layer.name, floor, tmxPath);
                }
            }

            if (group.groups != null)
            {
                foreach (var subGroup in group.groups)
                {
                    ProcessGroupRecursive(world, subGroup, subGroup.name, floor, tmxPath);
                }
            }
        }

        private void ProcessLayer(World world, TiledLayer layer, TiledGroup group, string name, int inheritedFloor, string tmxPath)
        {
            if (layer == null) return;

            // Usar o enum TiledLayerType
            switch (layer.type)
            {
                case TiledLayerType.TileLayer:
                    ProcessTileLayer(world, layer, group, name, inheritedFloor, tmxPath);
                    break;

                case TiledLayerType.ObjectLayer:
                    ProcessObjectLayer(world, layer, group, name, inheritedFloor, tmxPath);
                    break;
            }
        }

        private void ProcessTileLayer(World world, TiledLayer layer, TiledGroup group, string name, int inheritedFloor, string tmxPath)
        {
            // Mapas infinitos (com chunks)
            if (layer.chunks != null && layer.chunks.Length > 0)
            {
                foreach (var chunk in layer.chunks)
                {
                    ProcessChunk(world, chunk, layer, group, name, inheritedFloor, tmxPath);
                }
            }
            else
            {
                if (layer.data == null) return;
                if (layer.objects.Length != 0) return;

                // Mapas regulares
                ProcessTileData(world, layer.data, 0, 0, layer.width, layer.height, layer, group, inheritedFloor, tmxPath);
            }
        }

        private void ProcessChunk(World world, TiledChunk chunk, TiledLayer layer, TiledGroup group, string name, int inheritedFloor, string tmxPath)
        {
            ProcessTileData(world, chunk.data, chunk.x, chunk.y, chunk.width, chunk.height, layer, group, inheritedFloor, tmxPath);
        }

        private void ProcessTileData(World world, int[] data, int startX, int startY, int width, int height,
                                     TiledLayer layer, TiledGroup group, int inheritedFloor, string tmxPath)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int gid = data[y * width + x];
                    if (gid == 0) continue;

                    int tileX = startX + x;
                    int tileY = startY + y;

                    ProcessSingleTile(world, gid, tileX, tileY, layer, group, inheritedFloor, tmxPath);
                }
            }
        }

        private void ProcessSingleTile(World world, int gid, int tileX, int tileY,
                                       TiledLayer layer, TiledGroup group, int inheritedFloor, string tmxPath)
        {
            // Obter propriedades combinadas
            var tsxProps = GetCombinedTileProperties(gid, tmxPath);
            var mergedProps = PropertyAggregator.CollectProperties(tsxProps, layer, null, group);

            int floor = ResolveFloor(mergedProps);
            if (floor == 0) floor = inheritedFloor;

            float zBase = ResolveZBase(mergedProps, floor);
            RenderLayer renderLayer = ResolveRenderLayer(mergedProps);

            // ✅ DEBUG CRÍTICO: Verificar muros vs terreno
            DebugTileProperties("TILE", gid, tileX, tileY, mergedProps, floor, zBase, renderLayer);

            // Criar entidade ECS
            var entity = CreateTileEntity(world, gid, tileX, tileY, mergedProps, floor, zBase, renderLayer, layer);
            if (entity != null)
            {
                world.AddEntity(entity);
            }
        }

        private void ProcessObjectLayer(World world, TiledLayer layer, TiledGroup group, string name, int inheritedFloor, string tmxPath)
        {
            if (layer.objects == null) return;

            foreach (var tiledObj in layer.objects)
            {
                if (tiledObj.gid == 0) continue;


                var tsxProps = GetCombinedTileProperties(tiledObj.gid, tmxPath);
                var mergedProps = PropertyAggregator.CollectProperties(tsxProps, layer, tiledObj, group);

                int floor = ResolveFloor(mergedProps);
                if (floor == 0) floor = inheritedFloor;

                float zBase = ResolveZBase(mergedProps, floor);
                RenderLayer renderLayer = ResolveRenderLayer(mergedProps);

                // ✅ DEBUG CRÍTICO
                DebugTileProperties("OBJECT", tiledObj.gid, (int)tiledObj.x, (int)tiledObj.y,
                                   mergedProps, floor, zBase, renderLayer);

                var entity = CreateObjectEntity(world, tiledObj, mergedProps, floor, zBase, renderLayer, layer);
                if (entity != null)
                {
                    world.AddEntity(entity);
                }
            }
        }

        // ------------------------------------------------------------------
        // ENTITY CREATION METHODS
        // ------------------------------------------------------------------

        private Entity CreateTileEntity(World world, int gid, int tileX, int tileY,
                                      Dictionary<string, string> props, int floor, float zBase,
                                      RenderLayer renderLayer, TiledLayer layer)
        {
            if (!_resolver.TryGetTileData(gid, out Texture2D texture, out Rectangle src, out Vector2 origin))
                return null;

            var entity = world.CreateEntity();

            // ✅ CALCULAR Z CORRETAMENTE: floor * 32 + z_base DO TMX
            float finalZ = CalculateFinalZ(floor, zBase);

            origin = IsoConstants.DefaultOrigin(src.Width, src.Height);

            var position = new Position
            {
                Floor = floor,
                ZBase = zBase,           // VALOR DO TMX
                Z = finalZ,              // Z calculado correto
                Origin = origin,
                WorldPos = new Vector2(tileX * TileWidth, tileY * TileHeight),
                FeetWorld = new Vector2(tileX * TileWidth, tileY * TileHeight),
                IsTile = true
            };

            position.UpdateIso(MapOffset, TileWidth, TileHeight);


            entity.Set(position);

            // Renderable component
            var renderable = new Core.ECS.Components.Renderable
            {
                Texture = texture,
                SourceRect = src,
                Origin = origin,
                RenderLayer = renderLayer, // Já calculado
                Tint = Color.White,
                Fade = 1f,
                Visible = true,
                OcclusionFadeEnabled = props.TryGetValue("occlusion_fade", out _),
            };
            renderable.SortKey = IsoRenderKey.From(position, renderable).Raw;

            entity.Set(renderable);

            // ✅ Adicionar componente de propriedades para debug
            entity.Set(new TilePropertiesComponent(props, layer?.name, null));

            // Adicionar Collider se necessário
            if (props.TryGetValue("collision", out string collisionType) && collisionType == "true")
            {
                var collider = CreateColliderFromProperties(props, src);
                entity.Set(collider);
            }

            return entity;
        }

        private Entity CreateObjectEntity(World world, TiledObject obj,
                                         Dictionary<string, string> props, int floor, float zBase,
                                         RenderLayer renderLayer, TiledLayer layer)
        {
            if (!_resolver.TryGetTileData(obj.gid, out Texture2D texture, out Rectangle src, out Vector2 origin))
                return null;


            Vector2 correctedOrigin = origin;

            var entity = world.CreateEntity();


            // ✅ CALCULAR Z CORRETAMENTE: floor * 32 + z_base DO TMX
            float finalZ = CalculateFinalZ(floor, zBase);

            var position = new Position
            {
                Floor = floor,
                ZBase = zBase,           // VALOR DO TMX
                Z = finalZ,              // Z calculado correto
                Origin = correctedOrigin,
                WorldPos = new Vector2(obj.x, obj.y),
                FeetWorld = new Vector2(obj.x, obj.y),
                IsTile = false
            };

            position.UpdateIso(MapOffset, TileWidth, TileHeight);

            entity.Set(position);

            // Renderable component
            var renderable = new Core.ECS.Components.Renderable
            {
                Texture = texture,
                SourceRect = src,
                Origin = origin,
                RenderLayer = renderLayer, // Já calculado
                Tint = Color.White,
                Fade = 1f,
                Visible = true,
                OcclusionFadeEnabled = props.TryGetValue("occlusion_fade", out _)
            };

            renderable.SortKey = IsoRenderKey.From(position, renderable).Raw;

            entity.Set(renderable);

            // ✅ Adicionar componente de propriedades para debug
            entity.Set(new TilePropertiesComponent(props, layer?.name, null));

            // Adicionar Collider se for um objeto de colisão
            if (obj.type == "collision" || props.TryGetValue("collision", out string collision) && collision == "true")
            {
                var collider = new Core.ECS.Components.Collider
                {
                    Bounds = new Rectangle(0, 0, src.Width, src.Height),
                    IsSolid = true,
                    PixelPerfect = props.TryGetValue("pixel_perfect", out string pixelPerfect) && pixelPerfect == "true"
                };
                entity.Set(collider);
            }

            return entity;
        }

        private float CalculateFinalZ(int floor, float zBase)
        {
            const float FloorHeight = 32f;
            float finalZ = (floor * FloorHeight) + zBase;

            // ✅ DEBUG: Verificar cálculos importantes
            if (Math.Abs(zBase - (-0.8f)) < 0.01f) // TerrainBase
            {
                System.Diagnostics.Debug.WriteLine($"[Z_CALC] TerrainBase: floor={floor}, zBase={zBase}, finalZ={finalZ}");
            }
            else if (Math.Abs(zBase - 1.5f) < 0.01f ||  // WallsNW
                     Math.Abs(zBase - 1.6f) < 0.01f ||  // WallsNE
                     Math.Abs(zBase - 3.4f) < 0.01f ||  // WallsSE
                     Math.Abs(zBase - 4.5f) < 0.01f)    // WallsSW
            {
                System.Diagnostics.Debug.WriteLine($"[Z_CALC] Wall: floor={floor}, zBase={zBase}, finalZ={finalZ}");
            }

            return finalZ;
        }

        private float ResolveZBase(Dictionary<string, string> props, int floor)
        {
            float zBase = 0f;

            if (props != null)
            {
                // 1) z_base explícito
                if (props.TryGetValue("z_base", out var zBaseStr))
                {
                    zBaseStr = zBaseStr.Trim().Replace(",", ".");
                    if (float.TryParse(
                        zBaseStr,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out float value))
                    {
                        zBase = value;
                    }
                }
                // 2) z_range
                else if (props.TryGetValue("z_range", out var zRangeStr))
                {
                    if (TryParseZRange(zRangeStr, out float rangeZ))
                        zBase = rangeZ;
                }
                // 3) fallback simples e seguro
                else if (props.TryGetValue("RenderLayer", out var rl))
                {
                    string key = rl.Replace("_", "").ToLower();

                    // Apenas valores básicos
                    zBase = key switch
                    {
                        "floor" => 0f,
                        "stairs1" => 2.35f,
                        "stairs2" => 2.45f,
                        "stairs3" => 2.55f,
                        _ => 0f
                    };
                }
            }

            // ======================================================
            // ❗ SOMA DO ANDAR
            // ======================================================
            const float Z_STEP = 4f; // Ou leia do TMX
            float zFinal = zBase + floor * Z_STEP;

            return zFinal;
        }


        private bool TryParseZRange(string zRange, out float result)
        {
            result = 0f;

            if (string.IsNullOrWhiteSpace(zRange))
                return false;

            // Limpar e substituir vírgulas
            zRange = zRange.Trim().Replace(",", ".");

            // Formato esperado: "1.0-3.0" ou "1.0~3.0"
            string[] parts = zRange.Split('-', '~');

            if (parts.Length == 2)
            {
                if (float.TryParse(parts[0].Trim(),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out float min) &&
                    float.TryParse(parts[1].Trim(),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out float max))
                {
                    // Retorna o valor médio como z_base
                    result = (min + max) / 2f;
                    return true;
                }
            }

            return false;
        }

        private RenderLayer ResolveRenderLayer(Dictionary<string, string> props)
        {
            if (props == null)
                return Core.Rendering.RenderLayer.Floor;

            string renderLayerProp = ResolveRenderLayerString(props);


            if (string.IsNullOrEmpty(renderLayerProp))
            {
                System.Diagnostics.Debug.WriteLine($"[RENDERLAYER_ERROR] No RenderLayer property found!");
                System.Diagnostics.Debug.WriteLine($"  Available keys: {string.Join(", ", props.Keys)}");
            }
            else if (renderLayerProp.Contains("Wall") || renderLayerProp.Contains("Corner"))
            {
                System.Diagnostics.Debug.WriteLine($"[RENDERLAYER_FOUND] '{renderLayerProp}'");
            }

            return Core.Tiled.RenderLayerResolver.Resolve(
                renderLayerProp);
        }

        private string ResolveRenderLayerString(Dictionary<string, string> props)
        {
            if (props == null) return null;

            if (props.TryGetValue("RenderLayer", out var layer))
                return layer;

            if (props.TryGetValue("renderlayer", out layer))
                return layer;

            if (props.TryGetValue("layer", out layer))
                return layer;

            return null;
        }

        private void DebugTileProperties(string type, int gid, int x, int y,
                                       Dictionary<string, string> props, int floor,
                                       float zBase, RenderLayer renderLayer)
        {
            // Log apenas para elementos críticos
            bool isWall = renderLayer.ToString().Contains("Wall");
            bool isTerrain = renderLayer == RenderLayer.TerrainBase;

            if (isWall || isTerrain)
            {
                float finalZ = CalculateFinalZ(floor, zBase);

                System.Diagnostics.Debug.WriteLine($"[TMX_DEBUG] ========= {type} =========");
                System.Diagnostics.Debug.WriteLine($"  GID: {gid}, Pos: ({x}, {y})");
                System.Diagnostics.Debug.WriteLine($"  RenderLayer: {renderLayer} ({(int)renderLayer})");
                System.Diagnostics.Debug.WriteLine($"  Floor: {floor}, z_base from TMX: {zBase}");
                System.Diagnostics.Debug.WriteLine($"  Final Z: {finalZ} (floor*32 + z_base)");

                if (props.TryGetValue("RenderLayer", out string tmxLayer))
                    System.Diagnostics.Debug.WriteLine($"  TMX RenderLayer: '{tmxLayer}'");
                if (props.TryGetValue("wall_dir", out string wallDir))
                    System.Diagnostics.Debug.WriteLine($"  wall_dir: '{wallDir}'");

                // Comparação crítica
                if (isWall && isTerrain)
                {
                    System.Diagnostics.Debug.WriteLine($"  ⚠️  WALL vs TERRAIN COMPARISON:");
                    System.Diagnostics.Debug.WriteLine($"      Wall Z ({finalZ}) should be > Terrain Z");
                }

                System.Diagnostics.Debug.WriteLine("");
            }
        }

        // ------------------------------------------------------------------
        // HELPER METHODS (copiados do sistema antigo)
        // ------------------------------------------------------------------

        private Dictionary<string, string> GetCombinedTileProperties(int gid, string tmxPath)
        {
            var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Propriedades do TSX
            if (_resolver.TryGetTileProperties(gid, out var tsxProps))
            {
                foreach (var kv in tsxProps)
                {
                    props[kv.Key] = kv.Value;
                }
            }

            return props;
        }

        private int ResolveFloor(Dictionary<string, string> props)
        {
            if (props.TryGetValue("floor", out var s) && int.TryParse(s, out int floor))
                return floor;
            return 0;
        }

        private Core.ECS.Components.Collider CreateColliderFromProperties(Dictionary<string, string> props, Rectangle srcRect)
        {
            var collider = new Core.ECS.Components.Collider
            {
                IsSolid = true,
                PixelPerfect = props.TryGetValue("pixel_perfect", out var pp) && pp == "true",
                Bounds = new Rectangle(0, 0, srcRect.Width, srcRect.Height)
            };

            // Ajustar bounds se especificado
            if (props.TryGetValue("collision_bounds", out var boundsStr))
            {
                var parts = boundsStr.Split(',');
                if (parts.Length == 4 &&
                    int.TryParse(parts[0], out int x) &&
                    int.TryParse(parts[1], out int y) &&
                    int.TryParse(parts[2], out int w) &&
                    int.TryParse(parts[3], out int h))
                {
                    collider.Bounds = new Rectangle(x, y, w, h);
                }
            }

            return collider;
        }

        private Dictionary<string, string> ConvertProps(TiledProperty[] props)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (props != null)
            {
                foreach (var p in props)
                    dict[p.name] = p.value;
            }
            return dict;
        }
    }
}