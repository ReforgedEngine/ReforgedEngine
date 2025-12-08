using TiledCS;

namespace ReforgedEngine.Core.Tiled
{
    /// <summary>
    /// Centralized property resolver for Tiled (.tmx + .tsx) using TiledCS.
    /// 
    /// Responsibilities:
    ///   - Load external TSX
    ///   - Retrieve tile-level properties
    ///   - Provide normalized dictionary<string,string>
    /// 
    /// This replaces all legacy GetTiledTileProperties-like calls.
    /// </summary>
    public static class TilePropertyResolver
    {
        public static Dictionary<string, string> GetTileProperties(TiledMap map, int gid, string tmxDir)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var mapTileset = map.GetTiledMapTileset(gid);
            if (mapTileset == null)
                return result;

            // Resolve TSX from TMX directory
            string tsxFullPath = Path.Combine(tmxDir, mapTileset.source);
            if (!File.Exists(tsxFullPath))
                return result;

            var tileset = new TiledTileset(tsxFullPath);
            var tile = map.GetTiledTile(mapTileset, tileset, gid);

            if (tile?.properties != null)
            {
                foreach (var p in tile.properties)
                    result[p.name] = p.value;
            }

            return result;
        }
    }

}