using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TiledCS; // TiledCS!
using ReforgedEngine.Collision;

namespace ReforgedEngine.Map
{
    public class TileResolverFromTmx : ITileResolver
    {
        private readonly ContentManager _content;
        private readonly string _tilesetFolder;
        private readonly Dictionary<int, Texture2D> _textures = new();
        private readonly Dictionary<int, Rectangle> _sources = new();
        private readonly Dictionary<int, Vector2> _origins = new();
        private readonly Dictionary<int, List<CollisionShape>> _colliders = new();
        private readonly Dictionary<int, Dictionary<string, string>> _properties = new();

        public TileResolverFromTmx(ContentManager content, string folder)
        {
            _content = content;
            _tilesetFolder = folder;
        }

        public void RegisterTileset(int firstGid, TiledTileset tileset, string tsxPath) // TiledTileset!
        {
            string tsxDir = Path.GetDirectoryName(tsxPath)?.Replace("\\", "/") ?? "";

            if (tileset.Tiles != null)
            {
                foreach (var t in tileset.Tiles)
                {
                    if (t.image == null || string.IsNullOrEmpty(t.image.source)) continue;

                    int gid = firstGid + t.id;
                    string fileName = Path.GetFileNameWithoutExtension(t.image.source);
                    string contentPath = Path.Combine(tsxDir, fileName).Replace("Content/", "").Replace("\\", "/");

                    Texture2D tex = _content.Load<Texture2D>(contentPath);
                    _textures[gid] = tex;
                    _sources[gid] = new Rectangle(0, 0, tex.Width, tex.Height);
                    _origins[gid] = Vector2.Zero;
                }
            }
        }

        public void RegisterTileProperties(int gid, Dictionary<string, string> props)
        {
            _properties[gid] = props;
        }

        // TryGet... igual anterior
        public bool TryGetTileData(int gid, out Texture2D texture, out Rectangle src, out Vector2 origin)
        {
            texture = null;
            src = Rectangle.Empty;
            origin = Vector2.Zero;

            if (!_textures.TryGetValue(gid, out texture)) return false;
            if (!_sources.TryGetValue(gid, out src)) return false;
            _origins.TryGetValue(gid, out origin);
            return true;
        }

        public bool TryGetCollisionData(int gid, out List<CollisionShape> shapes)
        {
            return _colliders.TryGetValue(gid, out shapes);
        }

        public bool TryGetTileProperties(int gid, out Dictionary<string, string> props)
        {
            return _properties.TryGetValue(gid, out props);
        }

        // Adicione ao final:
        public bool TryResolveRawTileProperties(int gid, out Dictionary<string, string> props)
        {
            return TryGetTileProperties(gid, out props);
        }
    }
}