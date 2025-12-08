using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReforgedEngine.Core.Collision
{
    /// <summary>
    /// Builds 1-bit mask for pixel-perfect collision.
    /// Transparent = 0
    /// Opaque = 1
    /// </summary>
    public static class PixelMask
    {
        public static void Build(Texture2D tex, out byte[] mask, out int w, out int h)
        {
            w = tex.Width;
            h = tex.Height;
            mask = new byte[w * h];

            Color[] data = new Color[w * h];
            tex.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                mask[i] = data[i].A > 40 ? (byte)1 : (byte)0; // threshold
            }
        }

        public static bool Check(
            int localX,
            int localY,
            byte[] mask,
            int maskW,
            int maskH)
        {
            if (localX < 0 || localY < 0 || localX >= maskW || localY >= maskH)
                return false;

            return mask[localY * maskW + localX] == 1;
        }
    }
}
