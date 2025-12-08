using TiledCS;

namespace ReforgedEngine.Core.Tiled
{
    public static class PropertyAggregator
    {
        public static Dictionary<string, string> CollectProperties(
            Dictionary<string, string> tsxProps,
            TiledLayer layer,
            TiledObject obj,
            TiledGroup group = null)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (group?.properties != null)
                foreach (var p in group.properties)
                    result[p.name] = p.value;

            if (layer?.properties != null)
                foreach (var p in layer.properties)
                    result[p.name] = p.value;

            if (tsxProps != null)
                foreach (var kv in tsxProps)
                    result[kv.Key] = kv.Value;

            if (obj?.properties != null)
                foreach (var p in obj.properties)
                    result[p.name] = p.value;

            return result;
        }
    }
}