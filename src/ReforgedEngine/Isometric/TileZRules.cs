namespace ReforgedEngine.Isometric
{
    /// <summary>
    /// Utility class responsible for converting TMX z_base + floor
    /// into actual world Z values used by Position and IsoRenderKey.
    /// </summary>
    public static class TileZRules
    {
        public static float ComputeZ(int floor, float zBase)
        {
            return floor * 32f + zBase;
        }

        public static float SafeParseZ(Dictionary<string, string> props, string name, float fallback)
        {
            if (props != null && props.TryGetValue(name, out string s)
                && float.TryParse(s, out float v))
                return v;

            return fallback;
        }
    }
}
