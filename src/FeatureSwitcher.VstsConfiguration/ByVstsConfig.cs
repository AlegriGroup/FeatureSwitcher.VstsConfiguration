using FeatureSwitcher.Configuration;

namespace FeatureSwitcher.VstsConfiguration
{
    public static class ByVstsConfig
    {
        public static IConfigureVsts VstsConfig(this IConfigureBehavior control)
        {
            return new ConfigureVsts(control);
        }
    }
}
