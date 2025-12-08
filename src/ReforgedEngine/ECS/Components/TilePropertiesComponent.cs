namespace ReforgedEngine.Core.ECS.Components
{
    public struct TilePropertiesComponent : IComponent
    {
        public Dictionary<string, string> Properties;
        public string LayerName;
        public string GroupName;

        public TilePropertiesComponent(Dictionary<string, string> props, string layerName, string groupName)
        {
            Properties = props ?? new Dictionary<string, string>();
            LayerName = layerName;
            GroupName = groupName;
        }
    }
}
