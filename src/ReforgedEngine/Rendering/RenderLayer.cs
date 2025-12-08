namespace ReforgedEngine.Core.Rendering
{
    public enum RenderLayer : ushort
    {
        // ============================================================
        // ====================== 0 – DEEP TERRAIN =====================
        // ============================================================

        Abyss8 = 10,
        Abyss7 = 11,
        Abyss6 = 12,
        Abyss5 = 13,
        Abyss4 = 14,
        Abyss3 = 15,
        Abyss2 = 16,
        Abyss1 = 17,

        CloudsBack = 30,
        FogBack = 31,
        FogMid = 32,


        // ============================================================
        // ======================= 100 – TERRAIN =======================
        // ============================================================

        Heightmap = 100,
        RockLayer = 101,
        SoilDepth = 102,

        WaterDeep = 110,
        WaterSurface = 111,

        TerrainBase = 120,
        TerrainDetail = 121,
        PathsTrails = 122,
        SurfaceRocks = 123,

        ResourceOre = 130,
        ResourceFishing = 131,
        ResourceHerbs = 132,
        ResourceRocks = 133,
        ResourceTrees = 134,


        // ============================================================
        // ============== 200 – LEVEL ELEMENTS / FLOOR 0 ===============
        // ============================================================

        Floor = 200,
        Decals = 201,
        CornerSimple = 210,

        // Walls
        WallsNW = 220,
        WallsNE = 221,
        WallsSE = 222,
        WallsSW = 223,

        // Objects
        ObjectsLow = 230,
        ObjectsMedium = 231,
        ObjectsHigh = 232,

        // Doors
        Doors = 240,

        // Corners
        CornerNE = 250,
        CornerSE = 251,
        CornerSW = 252,
        CornerNW = 253,

        // Structural stacks
        Structure = 260,


        // ============================================================
        // ================== 300 – STAIRS (NEW!!) =====================
        // ============================================================

        // General stairs (support walls)
        StairsWallSupportNW = 300,
        StairsWallSupportNE = 301,
        StairsWallSupportSE = 302,
        StairsWallSupportSW = 303,

        // Stairs object stacks (ascending visuals)
        Stairs1 = 310,     // base of stair
        Stairs2 = 311,     // mid
        Stairs3 = 312,     // top

        // Stairs corners (if needed)
        StairsCorner1 = 320,
        StairsCorner2 = 321,
        StairsCorner3 = 322,


        // ============================================================
        // ======================== 400 – ROOF =========================
        // ============================================================

        RoofFaces = 400,

        // Roof stacks – NE
        RoofStack_NE_1 = 410,
        RoofStack_NE_2 = 411,
        RoofStack_NE_3 = 412,

        // Roof stacks – SE
        RoofStack_SE_1 = 420,
        RoofStack_SE_2 = 421,
        RoofStack_SE_3 = 422,

        // Roof stacks – SW
        RoofStack_SW_1 = 430,
        RoofStack_SW_2 = 431,
        RoofStack_SW_3 = 432,

        // Roof stacks – NW
        RoofStack_NW_1 = 440,
        RoofStack_NW_2 = 441,
        RoofStack_NW_3 = 442,


        // ============================================================
        // ================== 450 – ROOF CORNER STACKS =================
        // ============================================================

        // North
        RoofCornerStack_N_1 = 450,
        RoofCornerStack_N_2 = 451,
        RoofCornerStack_N_3 = 452,

        // East
        RoofCornerStack_E_1 = 460,
        RoofCornerStack_E_2 = 461,
        RoofCornerStack_E_3 = 462,

        // South
        RoofCornerStack_S_1 = 470,
        RoofCornerStack_S_2 = 471,
        RoofCornerStack_S_3 = 472,

        // West
        RoofCornerStack_W_1 = 480,
        RoofCornerStack_W_2 = 481,
        RoofCornerStack_W_3 = 482,


        // ============================================================
        // ==================== 490 – SPECIAL ROOF =====================
        // ============================================================

        RoofInvertedCorners = 490,
        RoofTopper = 500,

        // FALLBACK
        Default = 65000
    }
}
