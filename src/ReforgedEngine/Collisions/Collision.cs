namespace ReforgedEngine.Core.Collisions
{
    public static class Collision
    {
        public static class PixelMask
        {
            public static bool Check(int x, int y, byte[] mask, int width, int height)
            {
                if (x < 0 || y < 0 || x >= width || y >= height)
                    return false;

                int index = y * width + x;
                return index < mask.Length && mask[index] > 0;
            }
        }
    }
}