using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.Tiled;
using TiledCS;

namespace ReforgedEngine.Core.Tiles
{
    /// <summary>
    /// Unified tile resolver interface for TMX/Tileset lookups.
    /// Provides:
    ///   - Texture
    ///   - SourceRect
    ///   - Origin
    ///   - Collision shapes
    ///   - Raw TSX properties
    /// 
    /// Implementations are responsible for loading everything
    /// from TSX/TMX.
    /// </summary>
    public interface ITileResolver
    {
        /// <summary>
        /// Clears all cached tile data.
        /// </summary>
        void Reset();

        /// <summary>
        /// Registers the TSX tileset.
        /// </summary>
        void RegisterTileset(
            int firstGid,
            TiledTileset tileset,
            string tsxPath);

        /// <summary>
        /// Stores tile properties (from TSX).
        /// </summary>
        void RegisterTileProperties(
            int gid,
            Dictionary<string, string> properties);

        /// <summary>
        /// Retrieves texture, source rect and origin.
        /// </summary>
        bool TryGetTileData(
            int gid,
            out Texture2D texture,
            out Rectangle src,
            out Vector2 origin);

        /// <summary>
        /// Retrieves TSX tile properties.
        /// </summary>
        bool TryGetTileProperties(
            int gid,
            out Dictionary<string, string> props);

        /// <summary>
        /// Retrieves collision shapes.
        /// </summary>
        bool TryGetCollisionData(
            int gid,
            out List<CollisionShape> shapes);

        bool TryGetTileAlignment(int gid, out TiledObjectAlignment alignment);

    }
}
