// Atualizar Position.cs
using Microsoft.Xna.Framework;
using ReforgedEngine.Isometric;
using ReforgedEngine.Tools;

namespace ReforgedEngine.Core.ECS.Components
{
    public struct Position : IComponent
    {
        public Vector2 WorldPos;
        public Vector2 FeetWorld;
        public Vector2 FeetIso;
        public Vector2 DrawPosition;
        public float ZBase;
        public int Floor;
        public float Z;
        public Vector2 Origin;
        public bool IsTile;

        // Propriedade para compatibilidade
        public Vector2 Iso => FeetIso;

        public void UpdateIso(Vector2 mapOffset, float tileW, float tileH)
        {
            FeetIso = IsoMath.WorldToIso(FeetWorld, tileW, tileH);

            if (!IsTile)
            {
                FeetIso += IsoDirections.GetTileOffset(IsoDirections.Direction.NW, 0, 64);
            }
            DrawPosition = FeetIso + mapOffset;
        }
    }

}