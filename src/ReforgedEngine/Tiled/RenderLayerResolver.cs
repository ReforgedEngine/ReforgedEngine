using ReforgedEngine.Core.Rendering;

namespace ReforgedEngine.Core.Tiled
{
    public static class RenderLayerResolver
    {
        /// <summary>
        /// Resolve o RenderLayer pelo nome vindo do TMX.
        /// O TMX SEMPRE vem em string, pode conter maiúsculas/minúsculas,
        /// underline ou hifens. Normalizamos antes de comparar.
        /// </summary>
        public static RenderLayer Resolve(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return RenderLayer.Default;

            string key = raw.Trim().Replace(" ", "").Replace("-", "").ToLower();

            return key switch
            {
                // ========== DEEP TERRAIN ==========
                "abyss1" => RenderLayer.Abyss1,
                "abyss2" => RenderLayer.Abyss2,
                "abyss3" => RenderLayer.Abyss3,
                "abyss4" => RenderLayer.Abyss4,
                "abyss5" => RenderLayer.Abyss5,
                "abyss6" => RenderLayer.Abyss6,
                "abyss7" => RenderLayer.Abyss7,
                "abyss8" => RenderLayer.Abyss8,

                "cloudsback" => RenderLayer.CloudsBack,
                "fogback" => RenderLayer.FogBack,
                "fogmid" => RenderLayer.FogMid,

                // ========== TERRAIN ==========
                "heightmap" => RenderLayer.Heightmap,
                "rocklayer" => RenderLayer.RockLayer,
                "soildepth" => RenderLayer.SoilDepth,
                "waterdeep" => RenderLayer.WaterDeep,
                "watersurface" => RenderLayer.WaterSurface,

                "terrainbase" => RenderLayer.TerrainBase,
                "terraindetail" => RenderLayer.TerrainDetail,
                "pathstrails" => RenderLayer.PathsTrails,
                "surfacerocks" => RenderLayer.SurfaceRocks,

                "resourceore" => RenderLayer.ResourceOre,
                "resourcefishing" => RenderLayer.ResourceFishing,
                "resourceherbs" => RenderLayer.ResourceHerbs,
                "resourcerocks" => RenderLayer.ResourceRocks,
                "resourcetrees" => RenderLayer.ResourceTrees,

                // ========== FLOOR (0 e 1) ==========
                "floor" => RenderLayer.Floor,
                "decals" => RenderLayer.Decals,
                "cornersimple" => RenderLayer.CornerSimple,

                // Walls
                "wallsnw" => RenderLayer.WallsNW,
                "wallsne" => RenderLayer.WallsNE,
                "wallsse" => RenderLayer.WallsSE,
                "wallssw" => RenderLayer.WallsSW,

                // Objects
                "objectslow" => RenderLayer.ObjectsLow,
                "objectsmedium" => RenderLayer.ObjectsMedium,
                "objectshigh" => RenderLayer.ObjectsHigh,

                // Doors
                "doors" => RenderLayer.Doors,

                // Corners
                "cornerne" => RenderLayer.CornerNE,
                "cornerse" => RenderLayer.CornerSE,
                "cornersw" => RenderLayer.CornerSW,
                "cornernw" => RenderLayer.CornerNW,

                // ========== STACKS (estrutura) ==========
                "structure" => RenderLayer.Structure,

                // ========== STAIRS (OFICIAL DO SEU TMX) ==========
                "stairs" => RenderLayer.Stairs1, // base das escadas
                "stairs1" => RenderLayer.Stairs1,
                "stairs2" => RenderLayer.Stairs2,
                "stairs3" => RenderLayer.Stairs3,

                // ========== ROOF ==========
                "rooffaces" => RenderLayer.RoofFaces,

                // Roof stacks
                "roofstack_ne_1" => RenderLayer.RoofStack_NE_1,
                "roofstack_ne_2" => RenderLayer.RoofStack_NE_2,
                "roofstack_ne_3" => RenderLayer.RoofStack_NE_3,

                "roofstack_se_1" => RenderLayer.RoofStack_SE_1,
                "roofstack_se_2" => RenderLayer.RoofStack_SE_2,
                "roofstack_se_3" => RenderLayer.RoofStack_SE_3,

                "roofstack_sw_1" => RenderLayer.RoofStack_SW_1,
                "roofstack_sw_2" => RenderLayer.RoofStack_SW_2,
                "roofstack_sw_3" => RenderLayer.RoofStack_SW_3,

                "roofstack_nw_1" => RenderLayer.RoofStack_NW_1,
                "roofstack_nw_2" => RenderLayer.RoofStack_NW_2,
                "roofstack_nw_3" => RenderLayer.RoofStack_NW_3,

                // Roof Corner stacks
                "roofcornerstack_n_1" => RenderLayer.RoofCornerStack_N_1,
                "roofcornerstack_n_2" => RenderLayer.RoofCornerStack_N_2,
                "roofcornerstack_n_3" => RenderLayer.RoofCornerStack_N_3,

                "roofcornerstack_e_1" => RenderLayer.RoofCornerStack_E_1,
                "roofcornerstack_e_2" => RenderLayer.RoofCornerStack_E_2,
                "roofcornerstack_e_3" => RenderLayer.RoofCornerStack_E_3,

                "roofcornerstack_s_1" => RenderLayer.RoofCornerStack_S_1,
                "roofcornerstack_s_2" => RenderLayer.RoofCornerStack_S_2,
                "roofcornerstack_s_3" => RenderLayer.RoofCornerStack_S_3,

                "roofcornerstack_w_1" => RenderLayer.RoofCornerStack_W_1,
                "roofcornerstack_w_2" => RenderLayer.RoofCornerStack_W_2,
                "roofcornerstack_w_3" => RenderLayer.RoofCornerStack_W_3,

                // Roof extras
                "roofinvertedcorners" => RenderLayer.RoofInvertedCorners,
                "rooftopper" => RenderLayer.RoofTopper,

                // Fallback
                _ => RenderLayer.Default
            };
        }
    }
}
