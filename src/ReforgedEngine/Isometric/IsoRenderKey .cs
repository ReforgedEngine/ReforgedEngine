// IsoRenderKey.cs – NOVA VERSÃO OFICIAL 2025

using ReforgedEngine.Core.ECS.Components;

namespace ReforgedEngine.Core.Rendering
{
    public readonly struct IsoRenderKey : IComparable<IsoRenderKey>
    {
        public readonly ulong Raw;

        public IsoRenderKey(ulong raw) => Raw = raw;

        public int CompareTo(IsoRenderKey other) => Raw.CompareTo(other.Raw);

        public static IsoRenderKey From(Position pos, Renderable render)
        {
            // ---- HIERARQUIA DE BITS ----
            // [   12 bits   ] RenderLayer (0–4095)
            // [    8 bits   ] Floor       (0–255)
            // [   16 bits   ] ZBase*100   (0–65535)
            // [   24 bits   ] Depth Y     (clamped)
            // [    4 bits   ] MicroBias   (origin.y mod 16)

            ulong rl = ((ulong)render.RenderLayer & 0xFFF) << 52;     // 12 bits
            ulong f = ((ulong)pos.Floor & 0xFF) << 44;               // 8 bits
            ulong zb = ((ulong)(pos.ZBase * 100) & 0xFFFF) << 28;     // 16 bits

            // Depth baseado no FeetIso.Y (ajuste isométrico)
            long dy = (long)pos.FeetIso.Y + 32768;
            if (dy < 0) dy = 0;
            if (dy > 0xFFFFFF) dy = 0xFFFFFF;

            ulong depth = ((ulong)dy & 0xFFFFFF) << 4;                // 24 bits

            ulong micro = (ulong)render.Origin.Y & 0xF;           // 4 bits

            ulong raw = rl | f | zb | depth | micro;

            return new IsoRenderKey(raw);
        }
    }
}
