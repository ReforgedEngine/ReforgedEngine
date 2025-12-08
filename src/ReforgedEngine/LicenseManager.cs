namespace ReforgedEngine.Core
{
    public static class LicenseManager
    {
        public static bool IsPro { get; private set; }

        public static void Validate(string keyPath = "license.pro")
        {
#if FREE_BUILD
            IsPro = false;
#else
            IsPro = File.Exists(keyPath);
#endif
        }

        public static bool UseFeature(Feature feature)
        {
            return IsPro || feature == Feature.BasicRender;
        }
    }

    public enum Feature
    {
        BasicRender, PixelCollision, Lighting, FOV, Multiplayer
    }
}