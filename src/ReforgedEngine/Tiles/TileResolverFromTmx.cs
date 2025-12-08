using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.Tiled;
using ReforgedEngine.Isometric;
using TiledCS;

namespace ReforgedEngine.Core.Tiles
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
        private readonly Dictionary<int, TiledObjectAlignment> _alignments = new();

        public TileResolverFromTmx(ContentManager content, string folder = "Tilesets/")
        {
            _content = content;
            _tilesetFolder = folder;
        }

        public void Reset()
        {
            _textures.Clear();
            _sources.Clear();
            _origins.Clear();
            _colliders.Clear();
            _properties.Clear();
            _alignments.Clear();
        }

        public void RegisterTileset(int firstGid, TiledTileset tileset, string tsxPath)
        {
            System.Diagnostics.Debug.WriteLine($"[TileResolver] Registering tileset: firstGid={firstGid}");

            // Verificar se é spritesheet (uma imagem grande) ou multi-image (imagens separadas)
            if (tileset.Image != null && !string.IsNullOrEmpty(tileset.Image.source))
            {
                // É uma spritesheet
                RegisterSpritesheet(firstGid, tileset, Path.GetDirectoryName(tsxPath));
            }
            else if (tileset.Tiles != null && tileset.Tiles.Any(t => t.image != null))
            {
                // São múltiplas imagens separadas
                RegisterMultiImage(firstGid, tileset, tsxPath);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[TileResolver] Warning: Tileset has no image data");
            }

            // ✅ REGISTRAR TILE PROPERTIES GLOBAIS DO TILESET
            if (tileset.Properties != null)
            {
                foreach (var tileProps in tileset.Tiles)
                {
                    int gid = firstGid + tileProps.id;

                    if (!_properties.ContainsKey(gid))
                        _properties[gid] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var prop in tileProps.properties)
                    {
                        _properties[gid][prop.name] = prop.value;

                        // ✅ CAPTURAR ALINHAMENTOS DAS PROPRIEDADES GLOBAIS
                        if (prop.name.Equals("objectalignment", StringComparison.OrdinalIgnoreCase))
                        {
                            var alignment = TiledAlignmentHelper.ParseAlignment(prop.value);
                            _alignments[gid] = alignment;

                            // Atualizar origem se já temos a textura carregada
                            if (_sources.TryGetValue(gid, out var src))
                            {
                                _origins[gid] = TiledAlignmentHelper.CalculateOrigin(alignment, src.Width, src.Height);
                            }
                        }
                    }
                }
            }
        }

        // Adicionar estes métodos à classe TileResolverFromTmx:

        /// <summary>
        /// Obtém o alinhamento de um tile específico.
        /// </summary>
        public bool TryGetTileAlignment(int gid, out TiledObjectAlignment alignment)
        {
            // ✅ CORREÇÃO: Atribuir valor ao parâmetro 'out' antes de sair do método
            if (_alignments.TryGetValue(gid, out var foundAlignment))
            {
                alignment = foundAlignment;
                return true;
            }

            // ✅ SE NÃO ENCONTRAR: atribuir valor padrão e retornar false
            alignment = TiledObjectAlignment.Unspecified;
            return false;
        }

        private void RegisterMultiImage(int firstGid, TiledTileset tileset, string tsxPath)
        {
            if (tileset.Tiles == null)
                return;

            string tsxDir = Path.GetDirectoryName(tsxPath).Replace("\\", "/");

            foreach (var tile in tileset.Tiles)
            {
                if (tile.image == null || string.IsNullOrEmpty(tile.image.source))
                    continue;

                int gid = firstGid + tile.id;

                string fileName = Path.GetFileNameWithoutExtension(tile.image.source);
                string contentPath = Path.Combine(tsxDir, fileName)
                    .Replace("\\", "/")
                    .Replace("Content/", "");

                try
                {
                    Texture2D tex = _content.Load<Texture2D>(contentPath);
                    int w = tex.Width;
                    int h = tex.Height;

                    _textures[gid] = tex;
                    _sources[gid] = new Rectangle(0, 0, w, h);

                    // ✅ ORIGEM PADRÃO INICIAL (será ajustada depois conforme alinhamento)
                    _origins[gid] = IsoConstants.DefaultOrigin(w, h);

                    // ✅ LER E ARMAZENAR PROPRIEDADES DO TILE
                    var tileProps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    // Ler propriedades do tile
                    if (tile.properties != null)
                    {
                        foreach (var prop in tile.properties)
                        {
                            tileProps[prop.name] = prop.value;

                            // ✅ CAPTURAR OBJECTALIGNMENT SE EXISTIR
                            if (prop.name.Equals("objectalignment", StringComparison.OrdinalIgnoreCase))
                            {
                                var alignment = TiledAlignmentHelper.ParseAlignment(prop.value);
                                _alignments[gid] = alignment;

                                // ✅ AJUSTAR ORIGEM BASEADA NO ALINHAMENTO
                                _origins[gid] = TiledAlignmentHelper.CalculateOrigin(alignment, w, h);

                                System.Diagnostics.Debug.WriteLine($"[TileResolver] Tile {gid}: " +
                                                                  $"alignment={alignment}, " +
                                                                  $"origin={_origins[gid]}");
                            }

                            // ✅ CAPTURAR TILEALIGNMENT TAMBÉM (para tiles normais)
                            if (prop.name.Equals("tilealignment", StringComparison.OrdinalIgnoreCase))
                            {
                                var alignment = TiledAlignmentHelper.ParseAlignment(prop.value);
                                _alignments[gid] = alignment;

                                // Para tiles, também ajustamos a origem
                                _origins[gid] = TiledAlignmentHelper.CalculateOrigin(alignment, w, h);
                            }
                        }
                    }

                    // ✅ SE NÃO TEM ALINHAMENTO ESPECÍFICO, TENTAR DETERMINAR POR TIPO
                    if (!_alignments.ContainsKey(gid))
                    {
                        // Tentar inferir pelo nome do arquivo ou outras propriedades
                        if (tileProps.ContainsKey("type"))
                        {
                            string type = tileProps["type"].ToLower();
                            if (type.Contains("character") || type.Contains("npc") ||
                                type.Contains("object") || type.Contains("prop"))
                            {
                                // Objetos/Personagens: bottom (centro na base)
                                _alignments[gid] = TiledObjectAlignment.Bottom;
                                _origins[gid] = TiledAlignmentHelper.CalculateOrigin(
                                    TiledObjectAlignment.Bottom, w, h);
                            }
                            else if (type.Contains("tile") || type.Contains("floor") ||
                                     type.Contains("wall") || type.Contains("terrain"))
                            {
                                // Tiles/terreno: center
                                _alignments[gid] = TiledObjectAlignment.Center;
                                _origins[gid] = TiledAlignmentHelper.CalculateOrigin(
                                    TiledObjectAlignment.Center, w, h);
                            }
                        }
                    }

                    // ✅ ARMAZENAR PROPRIEDADES COMPLETAS
                    _properties[gid] = tileProps;

                    // ✅ DEBUG: Log para tiles com alinhamento específico
                    if (_alignments.ContainsKey(gid))
                    {
                        System.Diagnostics.Debug.WriteLine($"[TileResolver] Registered tile {gid}: " +
                                                          $"{fileName} ({w}x{h}), " +
                                                          $"Alignment: {_alignments[gid]}, " +
                                                          $"Origin: {_origins[gid]}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load texture {contentPath}: {ex.Message}");

                    // Mesmo se falhar o load, registrar propriedades se existirem
                    if (tile.properties != null)
                    {
                        var tileProps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var prop in tile.properties)
                        {
                            tileProps[prop.name] = prop.value;

                            if (prop.name.Equals("objectalignment", StringComparison.OrdinalIgnoreCase))
                            {
                                _alignments[gid] = TiledAlignmentHelper.ParseAlignment(prop.value);
                            }
                        }
                        _properties[gid] = tileProps;
                    }
                }
            }

            // ✅ LOG RESUMO DOS ALINHAMENTOS
            int alignedCount = _alignments.Count;
            int totalCount = tileset.Tiles.Count(t => t.image != null && !string.IsNullOrEmpty(t.image.source));

            System.Diagnostics.Debug.WriteLine($"[TileResolver] Summary: " +
                                              $"{alignedCount}/{totalCount} tiles have alignment info");

            // ✅ LISTAR ALINHAMENTOS ENCONTRADOS (para debug)
            foreach (var alignment in _alignments.GroupBy(kvp => kvp.Value))
            {
                System.Diagnostics.Debug.WriteLine($"  {alignment.Key}: {alignment.Count()} tiles");
            }
        }

        private void RegisterSpritesheet(int firstGid, TiledTileset tsx, string tsxDir)
        {
            if (tsx.Image == null || string.IsNullOrEmpty(tsx.Image.source))
                return;

            string pngPath = Path.Combine(tsxDir, tsx.Image.source)
                .Replace("\\", "/")
                .Replace("Content/", "");

            string assetName = Path.GetFileNameWithoutExtension(pngPath);

            try
            {
                Texture2D tex = _content.Load<Texture2D>(assetName);
                int tileW = tsx.TileWidth;
                int tileH = tsx.TileHeight;
                int columns = tsx.Columns;
                int tileCount = tsx.TileCount;

                for (int i = 0; i < tileCount; i++)
                {
                    int gid = firstGid + i;
                    int sx = (i % columns) * tileW;
                    int sy = (i / columns) * tileH;

                    _textures[gid] = tex;
                    _sources[gid] = new Rectangle(sx, sy, tileW, tileH);

                    // ✅ ORIGEM PADRÃO INICIAL
                    _origins[gid] = new Vector2(tileW * 0.5f, tileH);

                    // ✅ LER PROPRIEDADES ESPECÍFICAS DO TILE (se existirem)
                    if (tsx.Tiles != null)
                    {
                        var specificTile = tsx.Tiles.FirstOrDefault(t => t.id == i);
                        if (specificTile?.properties != null)
                        {
                            var tileProps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                            foreach (var prop in specificTile.properties)
                            {
                                tileProps[prop.name] = prop.value;

                                // ✅ CAPTURAR ALINHAMENTOS
                                if (prop.name.Equals("objectalignment", StringComparison.OrdinalIgnoreCase))
                                {
                                    var alignment = TiledAlignmentHelper.ParseAlignment(prop.value);
                                    _alignments[gid] = alignment;
                                    _origins[gid] = TiledAlignmentHelper.CalculateOrigin(alignment, tileW, tileH);
                                }

                                if (prop.name.Equals("tilealignment", StringComparison.OrdinalIgnoreCase))
                                {
                                    var alignment = TiledAlignmentHelper.ParseAlignment(prop.value);
                                    _alignments[gid] = alignment;
                                    _origins[gid] = TiledAlignmentHelper.CalculateOrigin(alignment, tileW, tileH);
                                }
                            }

                            _properties[gid] = tileProps;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[TileResolver] Registered spritesheet: " +
                                                  $"{assetName}, {tileCount} tiles ({tileW}x{tileH})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load spritesheet {assetName}: {ex.Message}");
            }
        }

        public void RegisterTileProperties(int gid, Dictionary<string, string> props)
        {
            _properties[gid] = props ?? new Dictionary<string, string>();

            // Extrair e armazenar alinhamento
            if (props.TryGetValue("objectalignment", out string alignmentStr))
            {
                _alignments[gid] = TiledAlignmentHelper.ParseAlignment(alignmentStr);
            }
        }

        public bool TryGetTileData(int gid, out Texture2D texture, out Rectangle src, out Vector2 origin)
        {
            texture = null;
            src = Rectangle.Empty;
            origin = Vector2.Zero;

            if (!_textures.TryGetValue(gid, out texture))
                return false;

            if (!_sources.TryGetValue(gid, out src))
                return false;

            if (!_origins.TryGetValue(gid, out origin))
                origin = IsoConstants.DefaultOrigin(src.Width, src.Height);

            return true;
        }


        public bool TryGetTileProperties(int gid, out Dictionary<string, string> props)
        {
            if (_properties.TryGetValue(gid, out var foundProps))
            {
                props = foundProps;
                return true;
            }

            props = null;
            return false;
        }

        public bool TryGetCollisionData(int gid, out List<CollisionShape> shapes)
        {
            // ✅ CORREÇÃO: Atribuir valor ao parâmetro 'out'
            if (_colliders.TryGetValue(gid, out var foundShapes))
            {
                shapes = foundShapes;
                return true;
            }

            // ✅ SE NÃO ENCONTRAR: atribuir null e retornar false
            shapes = null;
            return false;
        }

        // Método adicional para compatibilidade
        public bool TryResolveRawTileProperties(int gid, out Dictionary<string, string> props)
        {
            return TryGetTileProperties(gid, out props);
        }

        /// <summary>
        /// Força um alinhamento específico para um tile.
        /// </summary>
        public void SetTileAlignment(int gid, TiledObjectAlignment alignment)
        {
            _alignments[gid] = alignment;

            // Atualizar origem automaticamente se temos os dados da textura
            if (_sources.TryGetValue(gid, out var src))
            {
                _origins[gid] = TiledAlignmentHelper.CalculateOrigin(alignment, src.Width, src.Height);
            }
        }

        /// <summary>
        /// Verifica se um tile tem propriedades de alinhamento.
        /// </summary>
        public bool HasAlignmentInfo(int gid)
        {
            return _alignments.ContainsKey(gid);
        }

        /// <summary>
        /// Lista todos os tiles com um alinhamento específico.
        /// </summary>
        public IEnumerable<int> GetTilesWithAlignment(TiledObjectAlignment alignment)
        {
            foreach (var kvp in _alignments)
            {
                if (kvp.Value == alignment)
                    yield return kvp.Key;
            }
        }

        /// <summary>
        /// Exporta informações de alinhamento para debug.
        /// </summary>
        public string ExportAlignmentReport()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("=== TILE ALIGNMENT REPORT ===");
            report.AppendLine($"Total tiles loaded: {_textures.Count}");
            report.AppendLine($"Tiles with alignment info: {_alignments.Count}");
            report.AppendLine();

            var byAlignment = _alignments.GroupBy(kvp => kvp.Value)
                                         .OrderByDescending(g => g.Count());

            foreach (var group in byAlignment)
            {
                report.AppendLine($"{group.Key}: {group.Count()} tiles");
                foreach (var tile in group.Take(10)) // Limita a 10 por grupo
                {
                    if (_sources.TryGetValue(tile.Key, out var src))
                    {
                        report.AppendLine($"  GID {tile.Key}: {src.Width}x{src.Height}");
                    }
                }
                if (group.Count() > 10)
                    report.AppendLine($"  ... and {group.Count() - 10} more");
                report.AppendLine();
            }

            report.AppendLine("Tiles without alignment info:");
            int noAlignmentCount = 0;
            foreach (var gid in _textures.Keys)
            {
                if (!_alignments.ContainsKey(gid))
                {
                    noAlignmentCount++;
                    if (noAlignmentCount <= 20) // Limita a 20
                    {
                        report.Append($"GID {gid}, ");
                    }
                }
            }
            if (noAlignmentCount > 20)
                report.Append($"... and {noAlignmentCount - 20} more");

            return report.ToString();
        }
    }
}