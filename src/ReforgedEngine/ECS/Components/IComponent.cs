namespace ReforgedEngine.Core.ECS.Components
{
    /// <summary>
    /// Marker interface for ECS components.
    /// 
    /// This is intentionally EMPTY.
    /// Components must NEVER inherit IDisposable, ISite,
    /// or anything do System.ComponentModel.
    /// 
    /// Pure ECS components are plain data containers (POCOs).
    /// </summary>
    public interface IComponent
    {
    }
}
