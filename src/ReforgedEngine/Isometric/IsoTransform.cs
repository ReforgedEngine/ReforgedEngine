using Microsoft.Xna.Framework;

namespace ReforgedEngine.Isometric
{
    /// <summary>
    /// Holds computed isometric projection coordinates.
    /// 
    /// FeetIso:     projected feet position
    /// DrawPosition:final draw location after origin offset
    /// </summary>
    public struct IsoTransform
    {
        public Vector2 FeetIso;
        public Vector2 DrawPosition;
    }
}
