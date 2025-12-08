namespace ReforgedEngine.Core.ECS.Components
{
    /// <summary>
    /// Optional additional sort modifier.
    /// IsoRenderKey computes final key using Position + Renderable.
    /// </summary>
    public sealed class SortKeyComponent : IComponent
    {
        public long OverrideSortKey = -1;
    }
}
