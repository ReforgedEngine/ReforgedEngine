namespace ReforgedEngine.Core.ECS.Components
{
    /// <summary>
    /// Marks an entity as fadeable when blocking player view.
    /// Renderable.Fade is modified by OcclusionFadeSystem.
    /// </summary>
    public sealed class OcclusionFade : IComponent
    {
        public float TargetFade = 1f;
        public float Speed = 4f;
    }
}
