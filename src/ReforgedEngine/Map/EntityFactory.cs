using Microsoft.Xna.Framework;
using ReforgedEngine.Core;
using ReforgedEngine.Core.Components;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;
using ReforgedEngine.Core.Rendering;
using ReforgedEngine.Core.Tiled;
using ReforgedEngine.Core.Tiles;
using ReforgedEngine.Isometric;
using System.Collections.Generic;
using TiledCS;

namespace ReforgedEngine.Map
{
    public static class EntityFactory
    {
        public static Entity FromTile(int gid, int tileX, int tileY, TiledMap map, ITileResolver resolver, Dictionary<string, string> props, int floor, float zBase, World world, Vector2 mapOffset)
        {
            if (!resolver.TryGetTileData(gid, out var tex, out var src, out var origin)) return null;

            var e = world.Create();
            var pos = new Position
            {
                WorldPos = new Vector2(tileX * map.TileWidth, tileY * map.TileHeight),
                FeetWorld = new Vector2(tileX * map.TileWidth, tileY * map.TileHeight),
                Z = floor * 32f + zBase,
                ZBase = zBase,
                Floor = floor,
                Origin = origin // Add Origin to Position if needed
            };
            pos.UpdateIso(mapOffset);
            e.Add(pos);

            var rend = new Renderable
            {
                Texture = tex,
                SourceRect = src,
                Origin = origin,
                Layer = ResolveRenderLayer(props),
                SortKey = IsoRenderKey.FromPosition(pos),
                Tint = Color.White
            };
            e.Add(rend);

            var meta = new Meta
            {
                Properties = props,
                WallType = TilePropertyResolver.ResolveWallType(props),
                CornerType = TilePropertyResolver.ResolveCornerType(props),
                RoofType = TilePropertyResolver.ResolveRoofType(props),
                CollisionType = TilePropertyResolver.ResolveCollisionType(props),
                Flags = TilePropertyResolver.ResolveFlags(props)
            };
            e.Add(meta);

            return e;
        }

        // FromObject: similar, WorldPos = new(tiledObj.x, tiledObj.y)

        // FromObject similar (tiledObj.x/y)
        public static Entity FromObject(TiledObject obj, Tileset map, ITileResolver resolver, Dictionary<string, string> props, int floor, float zBase, World world, Vector2 mapOffset)
        {
            if (!resolver.TryGetTileData(obj.gid, out var tex, out var src, out var origin)) return null;

            var e = world.Create();
            e.Add(new Position
            {
                WorldPos = new Vector2(obj.x, obj.y),
                FeetWorld = new Vector2(obj.x, obj.y),
                Z = floor * 32f + zBase,
                ZBase = zBase,
                Floor = floor
            });
            e.Add(new Renderable
            {
                Texture = tex,
                SourceRect = src,
                Origin = origin,
                Layer = ResolveRenderLayer(props),
                SortKey = IsoRenderKey.FromPosition(e.Get<Position>())
            });
            var meta = new Meta
            {
                Properties = props,
                WallType = TilePropertyResolver.ResolveWallType(props),
                CornerType = TilePropertyResolver.ResolveCornerType(props),
                RoofType = TilePropertyResolver.ResolveRoofType(props),
                Flags = TilePropertyResolver.ResolveFlags(props)
            };
            e.Add(meta);
            e.Add(meta);
            e.Get<Position>().UpdateIso(mapOffset);
            return e;
        }

        private static RenderLayer ResolveRenderLayer(Dictionary<string, string> props)
        {
            // Seu fallback completo (wall_dir, roof_dir, etc.)
            if (props.TryGetValue("RenderLayer", out var layerStr) && Enum.TryParse<RenderLayer>(layerStr, out var layer)) return layer;
            // Defaults...
            return RenderLayer.Default;
        }
    }
}